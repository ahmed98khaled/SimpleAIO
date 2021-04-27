using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace SimpleAIO.Champions{
    internal class XinZhao
    {
        private static Spell Q, W, E, R;
        private static Menu mainMenu;
        private static int Range_E = 650;
        public static void OnGameLoad()
        {
            if (GameObjects.Player.CharacterName != "XinZhao") return;
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900f) { AddHitBox = true };
            E = new Spell(SpellSlot.E, Range_E);
            R = new Spell(SpellSlot.R, 450f);
            E.SetTargetted(0, float.MaxValue);
            W.SetSkillshot(0.5f * 0.75f, 10f, 6250f, false, SpellType.Line);
            mainMenu = new Menu("XinZhao", "SimpleZhao", true);
            var Combo = new Menu("Combo", "Combo Settings");
            Combo.Add(new MenuBool("Quse", "Use Q", true));
            Combo.Add(new MenuBool("Wuse", "Use W", true));
            Combo.Add(new MenuBool("Euse", "Use E", true));
            Combo.Add(new MenuBool("E2use", "Use E at new range (Beta)", false));
            Combo.Add(new MenuBool("Ruse", "Use R", true));
            mainMenu.Add(Combo);
            var Harass = new Menu("Harass", "Harass Settings");
            Harass.Add(new MenuBool("Quse", "Use Q", true));
            Harass.Add(new MenuBool("Wuse", "Use W", true));
            Harass.Add(new MenuSlider("mana%", "Mana porcent", 50, 0, 100));
            mainMenu.Add(Harass);
            var Draw = new Menu("Draw", "Draw Settings");
            Draw.Add(new MenuBool("qRange", "Draw Q range", true));
            Draw.Add(new MenuBool("wRange", "Draw W range", true));
            Draw.Add(new MenuBool("eRange", "Draw E range", true));
            Draw.Add(new MenuBool("lista", "Draw only if spell is ready", true));
            mainMenu.Add(Draw);
            mainMenu.Attach();
            GameEvent.OnGameTick += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnAfterAttack += OnAfterAttack;
        }
        private static void ComboLogic()
        {
            var targetW = W.GetTarget();
            var inputW = W.GetPrediction(targetW);
            var targetE = E.GetTarget();
            if (mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled && E.IsReady() && targetE.IsValidTarget()) E.Cast(targetE);
            if (mainMenu["Combo"].GetValue<MenuBool>("E2use").Enabled && E.IsReady() && targetW.IsValidTarget() && targetW.HasBuff("slow") && !W.IsReady()) E.Cast(targetW);
            if (mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled && W.IsReady() && targetW.IsValidTarget() && inputW.Hitchance >= HitChance.High && W.IsInRange(inputW.CastPosition)) W.Cast(inputW.CastPosition);
            if (mainMenu["Combo"].GetValue<MenuBool>("Ruse").Enabled && R.IsReady() && GameObjects.Player.CountEnemyHeroesInRange(R.Range) > 0)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget() && R.GetDamage(x) > x.Health)) R.Cast(target);
            }
        }
        private static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(W.Range);
            var inputW = W.GetPrediction(target);
            if (mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent && mainMenu["Harass"].GetValue<MenuBool>("Wuse").Enabled && W.IsReady() && target.IsValidTarget() && inputW.Hitchance >= HitChance.VeryHigh) W.Cast(inputW.CastPosition);
        }
        public static void OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
            if (args.Target == null || !args.Target.IsValidTarget()) return;
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && args.Target is AIHeroClient && mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled) if (Q.Cast()) Orbwalker.ResetAutoAttackTimer();
        }
        private static void OnGameUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead) return;

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    ComboLogic();
                    break;
                case OrbwalkerMode.Harass:
                    HarassLogic();
                    break;
                case OrbwalkerMode.LaneClear:

                    break;
            }
        }
        private static void OnDraw(EventArgs args)
        {
            var PlayerPos = GameObjects.Player.Position;
            if (mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled)
            {
                if (mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled) if (Q.IsReady()) Render.Circle.DrawCircle(PlayerPos, Q.Range, System.Drawing.Color.Cyan, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled) if (W.IsReady()) Render.Circle.DrawCircle(PlayerPos, W.Range + 100, System.Drawing.Color.Silver, 1);
                if (mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled) if (E.IsReady()) Render.Circle.DrawCircle(PlayerPos, E.Range, System.Drawing.Color.Yellow, 1);
            }
        }
    }
}