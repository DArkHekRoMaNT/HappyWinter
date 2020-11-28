using System;
using Vintagestory.API.Common;

namespace HappyWinter
{
    public class HappyWinter : ModSystem
    {
        public static string MOD_ID = "happywinter";
        public static string MOD_SPACE = "HappyWinter";
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterItemClass("ItemSnowball", Type.GetType(MOD_SPACE + ".ItemSnowball"));
            api.RegisterEntity("EntityThrownSnowball", Type.GetType(MOD_SPACE + ".EntityThrownSnowball"));
            api.RegisterBlockBehaviorClass("CollectSnowball", Type.GetType(MOD_SPACE + ".BlockBehaviorCollectSnowball"));
        }

    }
}