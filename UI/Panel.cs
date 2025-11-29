using Apos.Shapes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DiceMG.UI;

public class Panel : UIElement
{
    public Color FillColour { get; set; } = new Color(40, 40, 40);
    public Color BorderColour { get; set; } = Color.White;
    public float BorderThickness { get; set; } = 2f;
    public float CornerRadius { get; set; } = 5f;
    public bool HasBorder { get; set; } = true;
    public bool HasFill { get; set; } = true;

    public override void Draw(ShapeBatch sb, SpriteBatch spriteBatch, Rectangle screenBounds)
    {
        if (!IsVisible) return; 
        
        // Only draw shapes if ShapeBatch is provided
        if (sb != null)
        {
            var bounds = GetBounds(screenBounds);
            var centre = bounds.Center.ToVector2();
            var size = new Vector2(bounds.Width, bounds.Height);
            
            var pos = centre - Size/2;
            if (HasFill && HasBorder)
            {
                // Draw filled rectangle with border
                sb.DrawRectangle(pos, Size, FillColour, BorderColour, BorderThickness, CornerRadius);
            }
            else if (HasFill)
            {
                // Fill only
                sb.FillRectangle(pos, Size, FillColour, CornerRadius);
            }
            else if (HasBorder)
            {
                // Border only
                sb.BorderRectangle(pos, Size, BorderColour, BorderThickness, CornerRadius);
            }
        }
        
        base.Draw(sb, spriteBatch, screenBounds);
    }
}