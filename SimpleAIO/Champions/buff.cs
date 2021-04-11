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
            mainMenuB.Add(myBuffs);
        

            mainMenuB.Attach();
            Drawing.OnDraw += OnDraw;
             
        }
        
        private static void OnDraw(EventArgs args){
            if (mainMenuB["my"].GetValue<MenuBool>("showmy").Enabled)
	        {
                Vector2 pos = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y));
                var buffs = GameObjects.Player.Buffs;
                if(buffs.Any())Drawing.DrawText(pos,Color.White,"Buffs: ");
                for(var i = 0;i<buffs.Count()*10;i+=10)
                {
                Vector2 pos2 = new Vector2((Drawing.WorldToScreen(GameObjects.Player.Position).X),(Drawing.WorldToScreen(GameObjects.Player.Position).Y+120+i));
                Drawing.DrawText(pos2,Color.Blue,buffs[i/10].Count+"X "+buffs[i/10].Name);
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
                Drawing.DrawText(pos2,Color.Blue,buffs[i/10].Count+"X "+buffs[i/10].Name);
                }
	        }
        }
    }
}
