﻿using Fargowiltas;
using Fargowiltas.Items.Misc;
using Fargowiltas.UI;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoMemeChinese.Patch.Fargowiltas
{
    public static class StatSheetTranslate
    {
        private static MethodBase _drawChildren;
        private static MethodBase _rebuildStatList;
        public static void Load()
        {
            On.Terraria.Main.DrawInterface_33_MouseText += Main_DrawInterface_33_MouseText;

            _rebuildStatList = typeof(StatSheetUI).GetMethod("RebuildStatList");
            if (_rebuildStatList is not null)
                HookEndpointManager.Add(_rebuildStatList, RebuildStatList);

            _drawChildren = typeof(UISearchBar).GetMethod("DrawChildren", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_drawChildren is not null)
                HookEndpointManager.Modify(_drawChildren, ILModifyHintText);
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawInterface_33_MouseText -= Main_DrawInterface_33_MouseText;

            if (_rebuildStatList is not null)
                HookEndpointManager.Remove(_rebuildStatList, RebuildStatList);
            _rebuildStatList = null;

            if (_drawChildren is not null)
                HookEndpointManager.Unmodify(_drawChildren, ILModifyHintText);
            _drawChildren = null;
        }

        private static void Main_DrawInterface_33_MouseText(On.Terraria.Main.orig_DrawInterface_33_MouseText orig, Main self)
        {
            if (Main.hoverItemName == "Stat Sheet")
                Main.hoverItemName = "属性统计表";
            orig.Invoke(self);
        }
        private static void RebuildStatList(StatSheetUI orig)
        {
            Player player = Main.LocalPlayer;
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();

            orig.InnerPanel.RemoveAllChildren();
            orig.ColumnCounter = orig.LineCounter = 0;

            double Damage(DamageClass damageClass) => Math.Round(player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
            int Crit(DamageClass damageClass) => (int)player.GetTotalCritChance(damageClass);

            orig.AddStat($"近战伤害：{Damage(DamageClass.Melee)}%", ItemID.CopperBroadsword);
            orig.AddStat($"近战暴击率：{Crit(DamageClass.Melee)}%", ItemID.CopperBroadsword);
            orig.AddStat($"近战速度：{(int)(1f / player.GetAttackSpeed(DamageClass.Melee) * 100)}%", ItemID.CopperBroadsword);
            orig.AddStat($"远程伤害：{Damage(DamageClass.Ranged)}%", ItemID.CopperBow);
            orig.AddStat($"远程暴击率：{Crit(DamageClass.Ranged)}%", ItemID.CopperBow);
            orig.AddStat($"魔法伤害：{Damage(DamageClass.Magic)}%", ItemID.WandofSparking);
            orig.AddStat($"魔法暴击率：{Crit(DamageClass.Magic)}%", ItemID.WandofSparking);
            orig.AddStat($"魔力消耗降低：{Math.Round((1.0 - player.manaCost) * 100)}%", ItemID.WandofSparking);
            orig.AddStat($"召唤伤害：{Damage(DamageClass.Summon)}%", ItemID.SlimeStaff);
            if (ModLoader.TryGetMod("FargowiltasSouls", out _))
                orig.AddStat($"召唤暴击率：{(int)ModLoader.GetMod("FargowiltasSouls").Call("GetSummonCrit")}%", ItemID.SlimeStaff);
            else
                orig.AddStat("");
            orig.AddStat($"仆从数量上限：{player.maxMinions}", ItemID.SlimeStaff);
            orig.AddStat($"哨兵数量上限：{player.maxTurrets}", ItemID.SlimeStaff);

            orig.AddStat($"盔甲穿透：{player.GetArmorPenetration(DamageClass.Generic)}", ItemID.SharkToothNecklace);
            orig.AddStat($"仇恨：{player.aggro}", ItemID.FleshKnuckles);


            orig.AddStat($"生命值上限：{player.statLifeMax2}", ItemID.LifeCrystal);
            orig.AddStat($"生命再生：每秒{player.lifeRegen / 2}", ItemID.BandofRegeneration);
            orig.AddStat($"魔力值上限：{player.statManaMax2}", ItemID.ManaCrystal);
            orig.AddStat($"魔力再生：每秒{player.manaRegen / 2}", ItemID.ManaCrystal);
            orig.AddStat($"防御力：{player.statDefense}", ItemID.CobaltShield);
            orig.AddStat($"伤害减免：{Math.Round(player.endurance * 100)}%", ItemID.WormScarf);
            orig.AddStat($"运气：{Math.Round(player.luck, 2)}", ItemID.Torch);
            orig.AddStat($"已完成钓鱼任务：{player.anglerQuestsFinished}", ItemID.AnglerEarring);

            var BattleCry = modPlayer.GetType().GetField("BattleCry", BindingFlags.Instance | BindingFlags.NonPublic);
            bool battleCry = (bool)BattleCry.GetValue(modPlayer);
            var CalmingCry = modPlayer.GetType().GetField("CalmingCry", BindingFlags.Instance | BindingFlags.NonPublic);
            bool calmingCry = (bool)CalmingCry.GetValue(modPlayer);
            orig.AddStat($"战争号角效果：{(battleCry ? "[c/ff0000:战争]" : calmingCry ? "[c/00ffff:镇静]" : "无")}", ModContent.ItemType<BattleCry>());

            orig.AddStat($"最大速度：{(int)((player.accRunSpeed + player.maxRunSpeed) / 2f * player.moveSpeed * 6)}英里每小时", ItemID.HermesBoots);

            string RenderWingStat(double stat) => stat <= 0 ? "？？？" : stat.ToString();
            orig.AddStat(player.wingTimeMax / 60 > 60 || player.empressBrooch ? "飞行时间：接近无限" : $"飞行时间：{RenderWingStat(Math.Round(player.wingTimeMax / 60.0, 2))}秒", ItemID.AngelWings);
            orig.AddStat($"最大飞行速度：{RenderWingStat(Math.Round(modPlayer.StatSheetWingSpeed * 32 / 6.25))}英里每小时", ItemID.AngelWings);
            orig.AddStat($"翅膀上升速度：{RenderWingStat(Math.Round(modPlayer.StatSheetMaxAscentMultiplier * 100))}%", ItemID.AngelWings);
            orig.AddStat($"翅膀是否可水平悬停：{(modPlayer.CanHover == null ? "无翅膀" : (bool)modPlayer.CanHover ? "是" : "否")}", ItemID.AngelWings);
        }
        private static void ILModifyHintText(ILContext il) => il.ILTranslate("Search...", "搜索……");
    }
}