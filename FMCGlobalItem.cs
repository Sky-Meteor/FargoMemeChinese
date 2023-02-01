using FargoMemeChinese.UnmanagedTranslations;
using FargowiltasSouls.Items.Summons;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace FargoMemeChinese
{
    public class FMCGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<MutantsCurse>() && Main.LocalPlayer.name.Contains("清州可奈"))
            {
                tooltips.Find(l => l.Mod == "Terraria" && l.Name == "ItemName").Text = "清州可奈快乐器";
            }
        }
    }
}