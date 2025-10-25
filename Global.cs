using System;
using Microsoft.Xna.Framework;


namespace DiceMG
{
    public class Global
    {
        public static int ScreenWidth; 
        public static int ScreenHeight;
        public static Random SoRandom = new Random();
        
        public static float rollingx = 400f; 
        public static float rollingy = 200f;
        public static Vector2 RollingPosition = new ( rollingx,  rollingy);
        public static RollingTray.RollingMaterial ChosenMaterial = RollingTray.RollingMaterial.Felt;
        public static RollingTray Tray= new RollingTray(RollingPosition, ChosenMaterial);
    }
}
