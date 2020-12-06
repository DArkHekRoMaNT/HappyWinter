using Vintagestory.API.Common;

namespace HappyWinter
{
    public class BlockBehaviorCollectSnowball : BlockBehavior
    {
        ICoreAPI api;
        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            this.api = api;
        }
        public BlockBehaviorCollectSnowball(Block block) : base(block) { }
        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {
            if (byPlayer.Entity.Controls.Sneak &&
                byPlayer.Entity.Controls.RightMouseDown &&
                byPlayer.Entity.LeftHandItemSlot.Empty)
            {
                handling = EnumHandling.PreventDefault;
                ItemStack snowball = new ItemStack(byPlayer.Entity.World.GetItem(
                    new AssetLocation(HappyWinter.MOD_ID, "snowball-normal")));

                ItemSlot rslot = byPlayer.Entity.RightHandItemSlot;
                if (rslot.Empty)
                {
                    rslot.Itemstack = snowball;
                }
                else if (rslot.Itemstack.Collectible.Code == snowball.Collectible.Code &&
                    rslot.Itemstack.StackSize < rslot.Itemstack.Collectible.MaxStackSize)
                {
                    rslot.Itemstack.StackSize++;
                }
                else return false;
                rslot.MarkDirty();

                Block snowlayer = api.World.BlockAccessor.GetBlock(blockSel.Position);
                if (snowlayer.LastCodePart() == "1")
                {
                    api.World.BlockAccessor.BreakBlock(blockSel.Position, byPlayer, 0);
                }
                else
                {
                    int height = int.Parse(snowlayer.LastCodePart()) - 1;
                    AssetLocation newBlockLoc = snowlayer.CodeWithVariant("height", height.ToString());
                    Block newBlock = api.World.GetBlock(newBlockLoc);
                    api.World.BlockAccessor.SetBlock(newBlock.Id, blockSel.Position);
                }
                return true;
            }
            return base.OnBlockInteractStart(world, byPlayer, blockSel, ref handling);
        }
    }
}