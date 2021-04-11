using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace Riven{
    internal class Program{
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void Main(string[] args){
            GameEvent.OnGameLoad += OnGameLoad;
        }
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Riven") return;
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E, 650f);
            R = new Spell(SpellSlot.R, 450f);
            E.SetTargetted(0,float.MaxValue);
            W.SetSkillshot(0.5f, 40f, float.MaxValue, false,SpellType.Line);
          
            mainMenu = new Menu("XinZhao","SimpleZhao",true);
            var Combo = new Menu("Combo","Combo Settings");
            Combo.Add(new MenuBool("Quse","Use Q",true));
            Combo.Add(new MenuBool("Wuse","Use W",true));
            Combo.Add(new MenuBool("Euse","Use E",true));
            Combo.Add(new MenuBool("Ruse","Use R",true));
            mainMenu.Add(Combo);
            var Harass = new Menu("Harass","Harass Settings");
            Harass.Add(new MenuBool("Quse","Use Q",true));
            Harass.Add(new MenuBool("Wuse","Use W",true));
            Harass.Add(new MenuSlider("mana%","Mana porcent",50,0,100));
            mainMenu.Add(Harass);
        
            var Draw = new Menu("Draw","Draw Settings");
            Draw.Add(new MenuBool("qRange","Draw Q range",true));
            Draw.Add(new MenuBool("wRange","Draw W range",true));
            Draw.Add(new MenuBool("eRange","Draw E range",true));
            Draw.Add(new MenuBool("lista","Draw only if spell is ready",true));
            mainMenu.Add(Draw);

            mainMenu.Attach();
            GameEvent.OnGameTick += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnAfterAttack += OnAfterAttack;
   
           
        }
        
        
        private static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(E.Range);
            var targetQ = TargetSelector.GetTarget(175);
            var inputW = W.GetPrediction(target);

            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled)
            {
                if(Q.IsReady() && targetQ.IsValidTarget())
                {
                    Q.Cast();
                }
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled)
            {
               if(E.IsReady() && target.IsValidTarget())
                {
                        E.Cast(target);
                       
                }
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled)
            {
               if(W.IsReady() && target.IsValidTarget()&& inputW.Hitchance >= HitChance.VeryHigh)
                {   
                        W.Cast(inputW.CastPosition);
                }
            }
        }


        private static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(W.Range);
            var inputW = W.GetPrediction(target);
            
           if(mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent)
           {
                 if(mainMenu["Harass"].GetValue<MenuBool>("Wuse").Enabled)
                 {
                     if(W.IsReady() && target.IsValidTarget()&& inputW.Hitchance >= HitChance.VeryHigh)
                     {
                        W.Cast(inputW.CastPosition);
                     }
                 }
           }
        }

        
       private static void OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
            if (args.Target == null || !args.Target.IsValidTarget())
            {
                return;
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && args.Target is AIHeroClient &&
                mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled)
            {
                if (W.Cast() || Q.Cast())
                {
                    Orbwalker.ResetAutoAttackTimer();
                }
            }

            

            
        }
             
      

        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
                case OrbwalkerMode.Combo:
                    //ComboLogic();
                    break;
                    case OrbwalkerMode.Harass:
                   // HarassLogic();
                    break;
                case OrbwalkerMode.LaneClear:
                   
                    break;
            }
        }
        private static void OnDraw(EventArgs args){
            if(mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled){
                if(mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled){
                    if(Q.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range,System.Drawing.Color.Cyan,1);
                    }
                }
                if(mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled){
                    if(W.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range,System.Drawing.Color.Silver,1);
                    }
                }
                if(mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled){
                    if(E.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range,System.Drawing.Color.Yellow,1);
                    }

                }
            }
        }
    }
}
