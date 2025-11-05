using System;
using Microsoft.Xna.Framework;
using DiceMG.Input;


namespace DiceMG
{
    public class Global
    {
        public static int ScreenWidth; 
        public static int ScreenHeight;
        public static Random SoRandom = new Random();
        public static float TRAY_PADDING = 10f;
        public InputManager INPUT;
    }
}
