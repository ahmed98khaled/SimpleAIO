using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace Warwick{
    internal class Program{
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void Main(string[] args){
            GameEvent.OnGameLoad += OnGameLoad;
        }
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Warwick") return;
            var Rrang = GameObjects.Player.MoveSpeed * 250/100 ;
            Q = new Spell(SpellSlot.Q, 350f);
            W = new Spell(SpellSlot.W, 4000f);
            E = new Spell(SpellSlot.E, 375f);
            R = new Spell(SpellSlot.R, Rrang);
            Q.SetTargetted(0,float.MaxValue);
            R.SetSkillshot(.1f,10f,float.MaxValue,false,SpellType.Line);
            
            mainMenu = new Menu("Warwick","Warwick",true);
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
           
   
           
        }
        
        
        private static void ComboLogic()
        {
            var targetW = W.GetTarget();
            var targetQ = Q.GetTarget();
            var targetE = E.GetTarget();
            var targetR = R.GetTarget();
            var inputR = R.GetPrediction(targetR);
            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled && Q.IsReady() && targetQ.IsValidTarget() && Q.IsInRange(targetQ))  Q.Cast(targetQ);
            if(mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled && E.IsReady() && targetE.IsValidTarget())  E.Cast(targetE);
            if(mainMenu["Combo"].GetValue<MenuBool>("Ruse").Enabled && R.IsReady() && targetR.IsValidTarget()&& inputR.Hitchance >= HitChance.High && R.IsInRange(inputR.CastPosition) && R.GetDamage(targetR) > targetR.Health)  R.Cast(inputR.CastPosition);
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

      
        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
                case OrbwalkerMode.Combo:
                    ComboLogic();
                    break;
                    case OrbwalkerMode.Harass:
                    //HarassLogic();
                    break;
                case OrbwalkerMode.LaneClear:
                   
                    break;
            }
        }
        private static void OnDraw(EventArgs args){
           var PlayerPos = GameObjects.Player.Position;
            if(mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled){
                if(mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled) if(Q.IsReady()) Render.Circle.DrawCircle(PlayerPos, Q.Range,System.Drawing.Color.Cyan,1);
                if(mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled) if(W.IsReady()) Render.Circle.DrawCircle(PlayerPos, W.Range,System.Drawing.Color.Silver,1);
                if(mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled) if(E.IsReady())Render.Circle.DrawCircle(PlayerPos, E.Range,System.Drawing.Color.Yellow,1);
                if(mainMenu["Draw"].GetValue<MenuBool>("RRange").Enabled) if(R.IsReady())Render.Circle.DrawCircle(PlayerPos, R.Range,System.Drawing.Color.DeepSkyBlue,1);
            }
        }
    }
}
