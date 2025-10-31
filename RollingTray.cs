using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;     
using System.Collections.Generic;
using System.Linq;

namespace DiceMG;


public class RollingTray : GameObject

{
    public RollingTray(float width, float height, Vector2 position) : base(width, height, position, ObjType.Tray)
    {
        TextureKey = "tray";
    }



}
