using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Net.Mime;
using DiceMG.Input;
using Apos.Shapes;
using DiceMG; 
using Microsoft.Xna.Framework.Graphics;

namespace DiceMG.UI;

public class Button : Panel
{
    //Text
    public string Text { get; set; } = "Button";
    public SpriteFont Font { get; set; }
    public Color TextColour { get; set; } = Color.White;
    public float TextScale { get; set; } = 1f;
    
    //Hover Effect
    public Color HoverFillColour { get; set; } = new Color(60, 60, 60);
    public Color HoverBorderColour { get; set; } = Color.White;
    private bool _isHovering;
    
    //Sizing
    private static float _paddingFactor = 8f;
    public Vector2 Padding { get; set; } = new Vector2(2 * _paddingFactor, _paddingFactor); 
    //public bool AutoSize { get; set; } = true;
    public Vector2 ButtonSize => Font.MeasureString(Text) * TextScale + Padding*2; 
    
    //Click Events
    public Action OnClick { get; set; }
    private bool _wasLeftClicked;

    public void AutoSize()
    {
        if (Font != null & !string.IsNullOrEmpty(Text)) Size = Font.MeasureString(Text) * TextScale + Padding*2;
    }
    public static void SpaceApart(List<Button> elements, Vector2 spacing)
    {
        if (elements.Count == 0) return;
        
        // Calculate the center point of all elements
        Vector2 center = Vector2.Zero;
        foreach (var elem in elements)
        {
            center += elem.Offset;
        }
        center /= elements.Count;
        
        // Move each element away from the center
        foreach (var elem in elements)
        {
            Vector2 direction = elem.Offset - center;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                elem.Offset += direction * spacing;
            }
        }
    }

    public void Update(Rectangle screenBounds)
    {

        var bounds = GetBounds(screenBounds);
        
        _isHovering = bounds.Contains(Core.Input.Mouse.Position);
        _wasLeftClicked = Core.Input.Mouse.ButtonPressed(MouseButton.Left);
        
        if(_isHovering && _wasLeftClicked) OnClick?.Invoke();
    }

    public override void Draw(ShapeBatch sb, SpriteBatch spriteBatch, Rectangle screenBounds)
    {
        if (!IsVisible) return;
    
        var bounds = GetBounds(screenBounds);
        var fill = _isHovering ? HoverFillColour : FillColour;
        var border = _isHovering ? HoverBorderColour : BorderColour;
    
        // Only draw the shape if we have a ShapeBatch
        if (sb != null && (HasFill || HasBorder))
        {
            var centre = bounds.Center.ToVector2();
            //Size = new Vector2(bounds.Width, bounds.Height) + Padding*2;
            var pos = centre - Size/2;
            
            if (HasFill && HasBorder)
                sb.DrawRectangle(pos, Size, fill, border, BorderThickness, CornerRadius);
            else if (HasFill)
                sb.FillRectangle(pos, Size, fill, CornerRadius);
            else if (HasBorder)
                sb.BorderRectangle(pos, Size, border, BorderThickness, CornerRadius);
        }
        
        //Draw text if centre if text and font variables exist
        if (spriteBatch != null && Font != null && !string.IsNullOrEmpty(Text))
        {
            var textSize = Font.MeasureString(Text) * TextScale;  
            var textPos = new Vector2(bounds.Center.X - textSize.X / 2, bounds.Center.Y - textSize.Y / 2);
            
            spriteBatch.DrawString(Font, Text, textPos, TextColour, Font.Spacing, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
        }
        
        // Draw children if fany
        foreach (var child in Children)
            child.Draw(sb, spriteBatch, screenBounds);
    } 
    
}