using System;
using System.Net;
using System.Diagnostics;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SimpleAIO.Champions;
using System.Threading.Tasks;
using EnsoulSharp.SDK.MenuUI;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace SimpleAIO
{
    public class Program
    {
        public static void Main(string[] args)
        {           
            GameEvent.OnGameLoad += OnLoadingComplete;
        }
        private static void OnLoadingComplete()
        {
            if (ObjectManager.Player == null)
                return;
            buff.OnGameLoad();
            try
            {
                switch (GameObjects.Player.CharacterName)
                {
                    case "Jayce":
                        Jayce.OnGameLoad();
                        break;
                    case "Tryndamere":
                        Tryndamere.OnGameLoad();
                        break;
                    case "Garen":
                        Garen.OnGameLoad();
                        break;
                    //case "Gwen":
                       // Gwen.OnGameLoad();
                       // break;
                    case "Illaoi":
                        Illaoi.OnGameLoad();
                        break;
                    case "Mordekaiser":
                        Mordekaiser.OnGameLoad();
                        break;
                    case "Rumble":
                        Rumble.OnGameLoad();
                        break;
                    case "XinZhao":
                        XinZhao.OnGameLoad();
                        break;
                    case "Renekton":
                        Renekton.OnGameLoad();
                        break;
                    default:
                        Game.Print("<font color='#b756c5' size='25'>SimpleAIO Does Not Support :" + ObjectManager.Player.CharacterName+ "</font>");
                        Console.WriteLine("Not Supported " + ObjectManager.Player.CharacterName);
                        break;                   
                }
            }
            catch (Exception ex)
            {
                Game.Print("Error in loading");
                Console.WriteLine("Error in loading :");
                Console.WriteLine(ex);
            }
        }
    }   
}
