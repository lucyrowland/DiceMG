using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using DiceMG.Input;
using Apos.Shapes;
using DiceMG; 
using Microsoft.Xna.Framework.Graphics;

namespace DiceMG.UI;

public class Button : Panel
{
    public Action OnClick { get; set; }
    public Color HoverFillColour { get; set; } = new Color(60, 60, 60);
    public Color HoverBorderColour { get; set; } = Color.White;

    private bool _isHovering;
    private bool _wasLeftClicked;

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
        var center = bounds.Center.ToVector2();
        var size = new Vector2(bounds.Width, bounds.Height);
        
        var fill = _isHovering ? HoverFillColour : FillColour;
        var border = _isHovering ? HoverBorderColour : BorderColour;
        
        sb.DrawRectangle(center, size, fill, border, BorderThickness, CornerRadius);
        
        // Draw children (like labels)
        foreach (var child in Children)
            child.Draw(sb, spriteBatch, screenBounds);
    }    
    
}