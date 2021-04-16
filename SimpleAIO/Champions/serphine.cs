using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace SERAPHINE{
    internal class Program{
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void Main(string[] args){
            GameEvent.OnGameLoad += OnGameLoad;
        }
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Seraphine") return;
            Q = new Spell(SpellSlot.Q, 900f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 1300f);
            R = new Spell(SpellSlot.R, 1200f);
            Q.SetSkillshot(.25f,350f,1200f,false,SpellType.Circle);
            E.SetSkillshot(0.25f,140f,1200f,false,SpellType.Line);
            R.SetSkillshot(.5f,320f,1600f,false,SpellType.Line);
            //E.SetTargetted(0,float.MaxValue);
            //W.SetSkillshot(0.5f, 40f, float.MaxValue, false,SpellType.Line);
          
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
            Harass.Add(new MenuBool("Euse","Use E",true));
            Harass.Add(new MenuBool("Ruse","Use R",true));
            Harass.Add(new MenuSlider("mana%","Mana porcent",50,0,100));
            mainMenu.Add(Harass);
        
            var Draw = new Menu("Draw","Draw Settings");
            Draw.Add(new MenuBool("qRange","Draw Q range",true));
            Draw.Add(new MenuBool("wRange","Draw W range",true));
            Draw.Add(new MenuBool("eRange","Draw E range",true));
            Draw.Add(new MenuBool("RRange","Draw R range",true));

            Draw.Add(new MenuBool("lista","Draw only if spell is ready",true));
            mainMenu.Add(Draw);

            mainMenu.Attach();
            GameEvent.OnGameTick += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
           
   
           
        }
        
        
        private static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(E.Range);

            var targetQ = TargetSelector.GetTarget(Q.Range);
            var inputQ = Q.GetPrediction(targetQ);

            var targetW = TargetSelector.GetTarget(W.Range);
            var inputW = W.GetPrediction(targetW);
           
            var targetE = TargetSelector.GetTarget(E.Range);
            var inputE = E.GetPrediction(targetE);

            var targetR = TargetSelector.GetTarget(R.Range);
            var inputR = E.GetPrediction(targetR);

            
            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled && Q.IsReady() && target.IsValidTarget()&& inputQ.Hitchance >= HitChance.VeryHigh )
            {
                
               Q.Cast(inputQ.CastPosition);
               
            }
            
            if(mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled && W.IsReady() && target.IsValidTarget()&& inputW.Hitchance >= HitChance.VeryHigh )
            {
                
               W.Cast(inputW.CastPosition);
               
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled && E.IsReady() && target.IsValidTarget()&& inputE.Hitchance >= HitChance.VeryHigh )
            {
                
               E.Cast(inputE.CastPosition);
               
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Ruse").Enabled && R.IsReady() && target.IsValidTarget()&& inputR.Hitchance >= HitChance.VeryHigh )
            {
                
               R.Cast(inputR.CastPosition);
               
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

        public static float LastQ = 0f;
        public static void OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
              if(Q.Cast()){
              LastQ = Game.Time + .5f;
                           }
        }
             
      

        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
                case OrbwalkerMode.Combo:
                    ComboLogic();
                    break;
                    case OrbwalkerMode.Harass:
                    ComboLogic();
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
                if(mainMenu["Draw"].GetValue<MenuBool>("RRange").Enabled){
                    if(R.IsReady()){
                        Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range,System.Drawing.Color.DarkGoldenrod ,1);
                    }

                }
            }
        }
    }
}
