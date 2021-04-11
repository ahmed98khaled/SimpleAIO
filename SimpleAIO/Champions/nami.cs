using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace Nami{
    internal class Program{
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Nami") return;
            Q = new Spell(SpellSlot.Q, 875);
            W = new Spell(SpellSlot.W, 725);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 1000);
            W.SetTargetted(0.25f,float.MaxValue);
            Q.SetSkillshot(0.25f, 110, float.MaxValue, false,SpellType.Circle);
            R.SetSkillshot(0.5f, 180, 850, false,SpellType.Circle);
            /*Q = new Spell(SpellSlot.Q,825f);
            W = new Spell(SpellSlot.W,1000f);
            E = new Spell(SpellSlot.E,360f);
            R = new Spell(SpellSlot.R,900f);
            R.SetSkillshot(0.25f,250f,1200f,false,SpellType.Line);*/

            mainMenu = new Menu("Sona","BestSona",true);
            var Combo = new Menu("Combo","Combo Settings");
            Combo.Add(new MenuBool("Quse","Use Q",true));
            Combo.Add(new MenuBool("Wuse","Use W",true));
            Combo.Add(new MenuBool("Ruse","Use R",true));
            mainMenu.Add(Combo);
            var Harass = new Menu("Harass","Harass Settings");
            Harass.Add(new MenuBool("Quse","Use Q",true));
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
            Console.Write("Sona loaded");
   
           
        }
        
        
        private static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range);
            var inputR = R.GetPrediction(target);
            var input = Q.GetPrediction(target);
            //var inputW = W.GetPrediction(target);
            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled)
            {
                if(Q.IsReady() && target.IsValidTarget()&& inputR.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(input.UnitPosition);
                }
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled)
            {
               if(W.IsReady() && target.IsValidTarget())
                {
                        W.Cast(target);
                }
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Ruse").Enabled)
            {
               if(R.IsReady() && target.IsValidTarget() && inputR.Hitchance >= HitChance.High)
                {
                        R.Cast(inputR.UnitPosition);
                }
            }
            
        
        }
        private static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range);
            
            if(mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent){
                if(mainMenu["Harass"].GetValue<MenuBool>("Quse").Enabled){
                    if(Q.IsReady() && target.IsValidTarget()){
                        Q.Cast();
                    }
                }
            }
        }
      

        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
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