using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;     
using System.Collections.Generic;
using System.Linq;

namespace DiceMG
{
    public class RollingTray
    {
        public Vector2 PositionF = new Vector2(0,0);
        public Rectangle Sprite;

        public RollingMaterial Material = RollingMaterial.Wood;

        public RollingTray(Vector2 position, RollingMaterial material)
        {
            PositionF = position;
            Material = material;
            
        }
        public Rectangle SpriteCoord()
        {
            Sprite = Material switch
            {
                RollingMaterial.Steel => new Rectangle(85, 50, 325, 220), //(85,50) -> (410,270) 
                RollingMaterial.Ice => new Rectangle(420, 45, 450, 558), //(420,45) -> (870,267)
                RollingMaterial.Felt => new Rectangle(85, 280, 325, 220), //(85,280) -> (410,500)
                RollingMaterial.Wood => new Rectangle(425, 280, 335, 220), //(425,280) -> (750,500)
                _ => new Rectangle(0, 0, 0, 0)
            };
            return Sprite; 
        }

        public int TrayWidth()
        {
            return SpriteCoord().Width; 
        }
        public int TrayHeight()
        {
            return SpriteCoord().Height; 
        }
        public enum RollingMaterial
        {
            Steel,
            Ice, 
            Felt, 
            Wood
        }
    }
}
