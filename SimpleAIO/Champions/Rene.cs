using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using Color = System.Drawing.Color;

namespace SimpleAIO.Champions
{
    internal class Renekton
    {
        
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Renekton") return;
            Q = new Spell(SpellSlot.Q, 325f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 450f){ AddHitBox = true };
            R = new Spell(SpellSlot.R);
            //E.SetTargetted(0,float.MaxValue);
            E.SetSkillshot(.5f, 10f, float.MaxValue, false,SpellType.Line);
          
            mainMenu = new Menu("Rene","Rene",true);
            var Combo = new Menu("Combo","Combo Settings");
            Combo.Add(new MenuBool("Quse","Use Q",true));
            Combo.Add(new MenuBool("Wuse","Use W",true));
            Combo.Add(new MenuBool("Euse","Use E",true));
            //Combo.Add(new MenuBool("Ruse","Use R",true));
            mainMenu.Add(Combo);
            var Harass = new Menu("Harass","Harass Settings");
            Harass.Add(new MenuBool("Quse","Use Q",true));
            //Harass.Add(new MenuBool("Wuse","Use W",true));
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
            var startPos = GameObjects.Player.Position;
            var player = GameObjects.Player;
            var rage = GameObjects.Player.ManaPercent;
            var target = Q.GetTarget();
            var targetE = E.GetTarget();
            var inputE =E.GetPrediction(targetE);
           // var targetQ = TargetSelector.GetTarget(175);
            //var inputW = W.GetPrediction(target);

            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled)
            {
                if(Q.IsReady() && Q.IsInRange(target))
                {
                    Q.Cast();
                }
            }

            if(mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled && inputE.Hitchance >= HitChance.High)
            {
               if(E.IsReady() && target.IsValidTarget()&& inputE.Hitchance >= HitChance.High && E.IsInRange(inputE.CastPosition) && !ObjectManager.Player.HasBuff("RenektonSliceAndDiceDelay") )
                {
                        E.Cast(inputE.CastPosition);
                  
                     
               }

               if(!Q.IsReady() && !W.IsReady() && E.IsReady() && target.IsValidTarget()&& inputE.Hitchance >= HitChance.High && E.IsInRange(inputE.CastPosition) && rage < 50 && ObjectManager.Player.HasBuff("RenektonSliceAndDiceDelay") )
                {
                        E.Cast(inputE.CastPosition);
                  
                     
               }
              
            }
           

            /*if(mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled)
            {
               if(W.IsReady() && target.IsValidTarget()&& inputW.Hitchance >= HitChance.VeryHigh)
                {   
                        W.Cast(inputW.CastPosition);
                }
            }*/
        }


        private static void HarassLogic()
        {
            var target = Q.GetTarget();
           
            if(mainMenu["Harass"].GetValue<MenuBool>("Quse").Enabled)
            {
                if(Q.IsReady() && Q.IsInRange(target))
                {
                    Q.Cast();
                }
            }

        }

        public static void OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
               if (args.Target == null || !args.Target.IsValidTarget()) return;
                

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && args.Target is AIHeroClient &&
                mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled && GameObjects.Player.ManaPercent>=50)
            {
                if (W.Cast()) Orbwalker.ResetAutoAttackTimer();
                
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
            
            /* Vector2 pos = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y));
            var buffs = GameObjects.Player.Buffs;
            if(buffs.Any()){
                Drawing.DrawText(pos,Color.White,"Buffs: ");
            }
            for(var i = 0;i<buffs.Count()*10;i+=10){
                Vector2 pos2 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y+120+i));
                Drawing.DrawText(pos2,Color.Cyan,buffs[i/10].Count+"X "+buffs[i/10].Name);
            }*/

            var PlayerPos = GameObjects.Player.Position;

            if(mainMenu["Draw"].GetValue<MenuBool>("lista").Enabled){
                if(mainMenu["Draw"].GetValue<MenuBool>("qRange").Enabled){
                    if(Q.IsReady()){
                        Render.Circle.DrawCircle(PlayerPos, Q.Range,System.Drawing.Color.Cyan,1);
                    }
                }
                if(mainMenu["Draw"].GetValue<MenuBool>("wRange").Enabled){
                    if(W.IsReady()){
                        Render.Circle.DrawCircle(PlayerPos, W.Range,System.Drawing.Color.Silver,1);
                    }
                }
                if(mainMenu["Draw"].GetValue<MenuBool>("eRange").Enabled){
                    if(E.IsReady()){
                        Render.Circle.DrawCircle(PlayerPos, E.Range,System.Drawing.Color.Yellow,1);
                    }

                }
            }
        }
    }
}
