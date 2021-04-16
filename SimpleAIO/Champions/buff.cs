using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using Color = System.Drawing.Color;

namespace SimpleAIO.Champions{
    internal class buff{
        private static Menu mainMenuB;
        public static void OnGameLoad(){
            
          
            mainMenuB = new Menu("Buffs","Buffs",true);
            
            var EnemyBuffs = new Menu("enemy","enemy");
            EnemyBuffs.Add(new MenuBool("showenemy","enemy",false));
            mainMenuB.Add(EnemyBuffs);
            var myBuffs = new Menu("my","my");
            myBuffs.Add(new MenuBool("showmy","me",false));
            myBuffs.Add(new MenuBool("showob", "objects", false));
            mainMenuB.Add(myBuffs);
        

            mainMenuB.Attach();
            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += AiMinionClientOnOnCreate;


        }
        private static void AiMinionClientOnOnCreate(GameObject sender, EventArgs args)
        {
            if (GameObjectExtensions.DistanceToPlayer (sender) < 1000f && sender.Team == GameObjects.Player.Team && !sender.Name.ToLower().Contains("turret"))
            {
               buff.MinionName = "Minion Name: " + sender.Name;
               buff.MinionType = "Minion Type: " + sender.Type;
               buff.MinionPosition = "Minion Position: " + sender.Position;
            }
        }

        private static void OnDraw(EventArgs args){
            if (mainMenuB["my"].GetValue<MenuBool>("showob").Enabled)
            {
                 Vector2 posf = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X), (Drawing.WorldToScreen(GameObjects.Player.Position).Y));
            Vector2 poss = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X), (Drawing.WorldToScreen(GameObjects.Player.Position).Y + 120));
            Vector2 poss1 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X), (Drawing.WorldToScreen(GameObjects.Player.Position).Y + 130));
            Vector2 poss2 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X), (Drawing.WorldToScreen(GameObjects.Player.Position).Y + 140));

            Drawing.DrawText(posf, Color.White, "MinionCreate");

            if (!string.IsNullOrWhiteSpace(buff.MinionName))
            {
                Drawing.DrawText(poss, Color.White, buff.MinionName);
            }
            if (!string.IsNullOrWhiteSpace(buff.MinionType))
            {
                Drawing.DrawText(poss1, Color.White, buff.MinionType);
            }
            if (!string.IsNullOrWhiteSpace(buff.MinionPosition))
            {
                Drawing.DrawText(poss2, Color.White, buff.MinionPosition);
            }
            }
           

            if (mainMenuB["my"].GetValue<MenuBool>("showmy").Enabled)
	        {
                Vector2 pos = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y));
                var buffs = GameObjects.Player.Buffs;
                if(buffs.Any())Drawing.DrawText(pos,Color.White,"Buffs: ");
                for(var i = 0;i<buffs.Count()*10;i+=10)
                {
                Vector2 pos2 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y+120+i));
                Drawing.DrawText(pos2,Color.White, buffs[i/10].Count+"X "+buffs[i/10].Name);
                }
	        }
               
            if (mainMenuB["enemy"].GetValue<MenuBool>("showenemy").Enabled)
	        {
                var target0 = TargetSelector.GetTarget(1500);
                Vector2 pos = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y));
                var buffs = target0.Buffs;
                if(buffs.Any())Drawing.DrawText(pos,Color.White,"Buffs: ");
                for(var i = 0;i<buffs.Count()*10;i+=10)
                {
                Vector2 pos2 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y+120+i));
                Drawing.DrawText(pos2,Color.White, buffs[i/10].Count+"X "+buffs[i/10].Name);
                }
	        }
        }
        private static string MinionName;
        private static string MinionType;
        private static string MinionPosition;
    }

}
