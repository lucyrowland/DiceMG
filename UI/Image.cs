using Apos.Shapes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using DiceMG;
using Microsoft.Xna.Framework.Content;
using RenderingLibrary.Graphics;

namespace DiceMG.UI;

public class Image : UIElement
{
    public Texture2D Texture { get; set; }
    public Color TintColour { get; set; } = Color.White;

    public override void Draw(ShapeBatch sb, SpriteBatch spriteBatch, Rectangle screenBounds)
    {
        if (!IsVisible || Texture == null) return; 
        
        var bounds = GetBounds(screenBounds);
        spriteBatch.Draw(Texture, bounds, TintColour);
        
        base.Draw(sb, spriteBatch, screenBounds); 
    }
}