using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Chat;
using Terraria.Localization;

namespace FargoMemeChinese
{
    public class FCPlayer : ModPlayer
    {
        public override void OnEnterWorld(Player player)
        {
            Main.NewText("注意！你现在使用的是Fargo梗体汉化补丁，如果你是第一次游玩Fargo，推荐使用Fargo突变&魂汉化补丁以获得最好的游戏体验！", Color.LightGreen);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("注意，现在的Fargo汉化是客户端模组，不会参与服务器的自动同步，若需要开启汉化需要在模组列表自行开启！"), Color.LightGreen);
            }
        }
    }
}
