using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace HappyWinter
{
    public static class Util
    {
        public static void SendMessage(string msg, ICoreAPI api, IPlayer player, int chatGroup = -1)
        {
            if (chatGroup == -1) chatGroup = GlobalConstants.InfoLogChatGroup;
            if (player == null)
            {
                api.World.Logger.Chat(msg);
            }
            else if (api.Side == EnumAppSide.Server)
            {
                IServerPlayer sp = player as IServerPlayer;
                sp.SendMessage(chatGroup, msg, EnumChatType.Notification);
                api.World.Logger.Chat(msg);
            }
            else
            {
                IClientPlayer cp = player as IClientPlayer;
                cp.ShowChatNotification(msg);
                api.World.Logger.Chat(msg);
            }
        }
        public static void SendMessage(string msg, ICoreAPI api, Entity playerEntity = null, int chatGroup = -1)
        {
            IPlayer player = api.World.PlayerByUid((playerEntity as EntityPlayer)?.PlayerUID);
            SendMessage(msg, api, player, chatGroup);
        }
        public static void SendMessage(string msg, Entity playerEntity, int chatGroup = -1)
        {
            SendMessage(msg, playerEntity.Api, playerEntity, chatGroup);
        }
        public static void SendMessageAll(string msg, ICoreAPI api, int chatGroup = -1)
        {
            IPlayer[] players = api.World.AllPlayers;
            foreach (IPlayer player in players)
            {
                SendMessage(msg, api, player, chatGroup);
            }
        }
    }
}