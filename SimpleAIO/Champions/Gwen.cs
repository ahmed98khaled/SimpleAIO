#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
#endregion
namespace SimpleAIO.Champions{
    internal class Gwen{
       
        #region declaring
        private static Spell Q,W,E,R;
        private static Menu mainMenu;
        #endregion
        #region OnGameLoad

        public static void OnGameLoad(){
            if(GameObjects.Player.CharacterName != "Gwen") return;
            //Melee
            Q = new Spell(SpellSlot.Q );
            W = new Spell(SpellSlot.W, 370f);
            E = new Spell(SpellSlot.E );  
            R = new Spell(SpellSlot.R);
            //Targeting input
            Q.SetSkillshot(.5f,12f * 2 * (float)Math.PI / 180,false,SpellType.Circle);
                      
            mainMenu = new Menu("Jayce","Jayce",true);
            var Combo = new Menu("Combo","Combo Settings");
            Combo.Add(new MenuBool("Quse","Use Q",true));
            Combo.Add(new MenuBool("Q2use","Use Q Ranged",true));
            Combo.Add(new MenuBool("Q3use","Use EQ ",true));
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
            Draw.Add(new MenuBool("RRange","Draw R range",true));
            Draw.Add(new MenuBool("lista","Draw only if spell is ready",true));
            mainMenu.Add(Draw);

            mainMenu.Attach();
            GameEvent.OnGameTick += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            Orbwalker.OnAfterAttack += OnAfterAttack;
        }
       #endregion
        #region OnAfterAttack
        public static void OnAfterAttack(object sender, AfterAttackEventArgs args)
        {
               if (args.Target == null || !args.Target.IsValidTarget()) return;
                

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && args.Target is AIHeroClient &&
                mainMenu["Combo"].GetValue<MenuBool>("Wuse").Enabled && !Ismelee() )
            {
                if (W.Cast()) Orbwalker.ResetAutoAttackTimer();
            }
        }
        #endregion
        #region Combo
        private static void ComboMelee() 
        {
         var targetQ = Q.GetTarget();
         var targetE = E.GetTarget();

            if(mainMenu["Combo"].GetValue<MenuBool>("Quse").Enabled && Q.IsReady() && targetQ.IsValidTarget())
            {
                    Q.Cast(targetQ);
                    W.Cast();
            }

             if(mainMenu["Combo"].GetValue<MenuBool>("Euse").Enabled && E.IsReady() && targetE.IsValidTarget())
             {
                    E.Cast(targetE);
             }

             
        }
        
        private static void ComboLogic()
        {
            
            var targetQ2 = Q2.GetTarget();
            var targetQ3 = Q3.GetTarget();
            var PlayerPos = GameObjects.Player.Position;

            var inputQ2 = Q2.GetPrediction(targetQ2);
            var inputQ3 = Q3.GetPrediction(targetQ2);


            if(mainMenu["Combo"].GetValue<MenuBool>("Q2use").Enabled && Q.IsReady() && targetQ2.IsValidTarget() && inputQ2.Hitchance >= HitChance.VeryHigh)
            {
                    Q2.Cast(inputQ2.CastPosition);
            }

             if(mainMenu["Combo"].GetValue<MenuBool>("Q3use").Enabled && Q.IsReady() && targetQ3.IsValidTarget() && inputQ3.Hitchance >= HitChance.VeryHigh && E2.IsReady())
             {
                    E2.Cast(PlayerPos);
                    Q3.Cast(inputQ3.CastPosition);
             }
        }

         public static bool Ismelee()
         {
            
             return GameObjects.Player.IsMelee ;
         }
        #endregion
        #region Harass
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
        #endregion
        #region OnGameUpdate

        private static void OnGameUpdate(EventArgs args){
            if(GameObjects.Player.IsDead) return;
            
            switch (Orbwalker.ActiveMode){
                case OrbwalkerMode.Combo:
                   
                    if (!Ismelee())
                        ComboLogic();
                    else
                        ComboMelee();
                    break;

                    case OrbwalkerMode.Harass:
                   // HarassLogic();
                    break;
                case OrbwalkerMode.LaneClear:
                   
                    break;
            }
        }
        #endregion
        #region OnDraw
        private static void OnDraw(EventArgs args){
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
                if(mainMenu["Draw"].GetValue<MenuBool>("RRange").Enabled){
                    if(R.IsReady()){
                        Render.Circle.DrawCircle(PlayerPos, R.Range,System.Drawing.Color.Yellow,1);
                    }

                }
            }
        }
        #endregion
    }
}
