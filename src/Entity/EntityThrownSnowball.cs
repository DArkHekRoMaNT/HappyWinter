using System;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace HappyWinter
{
    public class EntityThrownSnowball : Entity
    {
        bool beforeCollided;
        bool stuck;

        long msLaunch;
        Vec3d motionBeforeCollide = new Vec3d();

        //CollisionTester collTester = new CollisionTester();

        public Entity FiredBy;
        internal float Damage;
        public ItemStack ProjectileStack;

        Random random = new Random();

        public override bool IsInteractable
        {
            get { return false; }
        }

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);

            msLaunch = World.ElapsedMilliseconds;

            if (ProjectileStack?.Collectible != null)
            {
                ProjectileStack.ResolveBlockOrItem(World);
            }
        }

        public override void OnGameTick(float dt)
        {
            base.OnGameTick(dt);
            if (ShouldDespawn) return;

            SidedPos.Pitch = (World.ElapsedMilliseconds / 300f) % GameMath.TWOPI;
            SidedPos.Roll = 0;
            SidedPos.Yaw = (World.ElapsedMilliseconds / 400f) % GameMath.TWOPI;

            if (World is IServerWorldAccessor)
            {
                Entity entity = World.GetNearestEntity(ServerPos.XYZ, 5f, 5f, (e) =>
                {
                    if (e.EntityId == this.EntityId || (FiredBy != null && e.EntityId == FiredBy.EntityId && World.ElapsedMilliseconds - msLaunch < 500) || !e.IsInteractable)
                    {
                        return false;
                    }

                    double dist = e.CollisionBox.ToDouble().Translate(e.ServerPos.X, e.ServerPos.Y, e.ServerPos.Z).ShortestDistanceFrom(ServerPos.X, ServerPos.Y, ServerPos.Z);
                    return dist < 0.5f;
                });

                if (entity != null)
                {
                    float kbres = entity.Properties.KnockbackResistance;
                    entity.Properties.KnockbackResistance = -1;
                    bool didDamage = entity.ReceiveDamage(new DamageSource() { Source = EnumDamageSource.Entity, SourceEntity = FiredBy == null ? this : FiredBy, Type = EnumDamageType.BluntAttack }, Damage);
                    entity.Properties.KnockbackResistance = kbres;
                    World.PlaySoundAt(new AssetLocation(HappyWinter.MOD_ID, "sounds/snowballhit"), this, null, false, 32);

                    if (FiredBy is EntityPlayer && didDamage)
                    {
                        //World.PlaySoundFor(new AssetLocation("game", "sounds/player/projectilehit"), (FiredBy as EntityPlayer).Player, false, 24);
                    }

                    Remove();
                    return;
                }
            }

            beforeCollided = false;
            motionBeforeCollide.Set(SidedPos.Motion.X, SidedPos.Motion.Y, SidedPos.Motion.Z);
        }
        public override ItemStack OnCollected(Entity byEntity)
        {
            ProjectileStack.ResolveBlockOrItem(World);
            return ProjectileStack;
        }

        public override void OnCollided()
        {
            if (Api.Side.IsClient()) return;

            BlockPos snowPos = new BlockPos((int)SidedPos.X, (int)SidedPos.Y, (int)SidedPos.Z);
            Block target = Api.World.BlockAccessor.GetBlock(snowPos);
            AssetLocation snowBlockCode = null;

            if (target.Id == 0) snowBlockCode = new AssetLocation("game", "snowlayer-1");
            else if (target.LastCodePart() == "free" && target.CodeWithVariant("cover", "snow").Valid)
            {
                snowBlockCode = target.CodeWithVariant("cover", "snow");
            }
            else if (target.FirstCodePart() == "snowlayer")
            {
                int height = int.Parse(target.LastCodePart());
                int newHeight = height + 1;

                if (newHeight == 8) snowBlockCode = new AssetLocation("game", "snowblock");
                else snowBlockCode = target.CodeWithVariant("height", newHeight.ToString());
            }

            if (snowBlockCode != null)
            {
                int blockId = Api.World.GetBlock(snowBlockCode).Id;
                Api.World.BlockAccessor.SetBlock(blockId, snowPos);
                Api.World.BlockAccessor.MarkBlockDirty(snowPos);
            }

            World.PlaySoundAt(new AssetLocation(HappyWinter.MOD_ID, "sounds/snowballhit"), this, null, false, 32);
            Remove();
        }

        public override void OnCollideWithLiquid()
        {
            Remove();
        }

        public override void ToBytes(BinaryWriter writer, bool forClient)
        {
            base.ToBytes(writer, forClient);
            writer.Write(beforeCollided);
            ProjectileStack.ToBytes(writer);
        }

        public override void FromBytes(BinaryReader reader, bool fromServer)
        {
            base.FromBytes(reader, fromServer);
            beforeCollided = reader.ReadBoolean();

            ProjectileStack = World == null ? new ItemStack(reader) : new ItemStack(reader, World);
        }

        private void Remove()
        {
            try { World.SpawnCubeParticles(SidedPos.XYZ.OffsetCopy(0, 0.2, 0), ProjectileStack, 0.2f, 20); }
            catch (Exception e) { Api.World.Logger.Warning(e.Message); }
            Die();
        }
    }
}

