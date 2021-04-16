using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using Color = System.Drawing.Color;

namespace TAHMKENCH
{
    internal class Program{
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void Main(string[] args){
            GameEvent.OnGameLoad += OnGameLoad;
        }
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "TahmKench") return;
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 250);
            E = new Spell(SpellSlot.E, 900);
            R = new Spell(SpellSlot.R);
            W.SetTargetted(0.25f,1600f);
            Q.SetSkillshot(0.25f, 120f,2000f,true,SpellType.Line);
           // E.SetSkillshot(0.25f, 200f, 3000f, false,SpellType.Line);

            
            /*Q = new Spell(SpellSlot.Q,825f);
            W = new Spell(SpellSlot.W,1000f);
            E = new Spell(SpellSlot.E,360f);
            R = new Spell(SpellSlot.R,900f);
            R.SetSkillshot(0.25f,250f,1200f,false,SpellType.Line);*/

            mainMenu = new Menu("TAHM KENCH","TAHM KENCH",true);
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
        private static bool eatable()
        {   var target = TargetSelector.GetTarget(Q.Range);
            var staks =target.GetBuffCount("tahmkenchpdebuffcounter");
            if(staks ==3)
            {
                return true;
            }
            return false;
        }
        
        private static void ComboLogic()
        {
            var target = TargetSelector.GetTarget(Q.Range);
            
            var input = Q.GetPrediction(target);
           
            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled)
            {
                if(Q.IsReady() && target.IsValidTarget()&& input.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(input.CastPosition);
                }
            }
               

            
            if(mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled)
            {
              if(W.IsReady() && target.IsValidTarget()&& eatable()&& W.IsInRange(target))
                {   
                    W.Cast(target);
                }
            }
            
            /*if(mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled)
            {
               if(E.IsReady() && target.IsValidTarget() && inputE.Hitchance >= HitChance.VeryHigh)
                {
                        E.Cast(inputE.UnitPosition);
                }
            }*/
            
        
        }
        private static void HarassLogic()
        {
            var target = TargetSelector.GetTarget(E.Range);
            var input = Q.GetPrediction(target);
            
            var inputE = E.GetPrediction(target);
            if(mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent){
                if(mainMenu["Harass"].GetValue<MenuBool>("Quse").Enabled){
                   if(Q.IsReady() && target.IsValidTarget() && input.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(input.UnitPosition);
                    }
                }
            }
            /*if(mainMenu["Harass"].GetValue<MenuSlider>("mana%").Value <= GameObjects.Player.ManaPercent){
                if(mainMenu["Harass"].GetValue<MenuBool>("Euse").Enabled){
                    if(E.IsReady() && target.IsValidTarget() && inputE.Hitchance >= HitChance.VeryHigh)
                {
                        E.Cast(inputE.UnitPosition);
                }
                }
            }*/
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
            var target0= TargetSelector.GetTarget(1000f);
            Vector2 pos = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y));
            var buffs =target0.Buffs ;
            if(buffs.Any()){
                Drawing.DrawText(pos,Color.White,"Buffs: ");
            }
            for(var i = 0;i<buffs.Count()*10;i+=10){
                Vector2 pos2 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y+120+i));
                Drawing.DrawText(pos2,Color.Black ,buffs[i/10].Count+"X "+buffs[i/10].Name);}


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
