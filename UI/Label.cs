using Apos.Shapes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using DiceMG;
using Microsoft.Xna.Framework.Content;
using RenderingLibrary.Graphics;

namespace DiceMG.UI;

/// <summary>
/// For Text rendering of UI Elements
/// </summary>
public class Label : UIElement
{
    public string Text { get; set; } = "";
    public SpriteFont Font { get; set; } = Core.Content.Load<SpriteFont>("Fonts/File"); //basic font for game
    public Color TextColour { get; set; } = Color.White;
    public bool CentreText { get; set; } = true;

    public override void Draw(ShapeBatch sb, SpriteBatch spriteBatch, Rectangle screenBounds)
    {
        if (!IsVisible || Font == null || spriteBatch == null) return;

        var bounds = GetBounds(screenBounds);
        Vector2 pos;

        if (CentreText)
        {
            var textSize = Font.MeasureString(Text); 
            pos = new Vector2(
                bounds.Center.X - textSize.X / 2, 
                bounds.Center.Y - textSize.Y / 2);
        }
        else
        {
            pos = new Vector2(bounds.X, bounds.Y);
        }
    
        spriteBatch.DrawString(Font, Text, pos, TextColour);
        base.Draw(sb, spriteBatch, screenBounds);
    }
}