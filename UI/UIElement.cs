using Apos.Shapes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DiceMG.UI;

public enum Anchor
{
    TopLeft, TopCentre, TopRight, 
    MiddleLeft, MiddleCentre, MiddleRight, 
    BottomLeft, BottomCentre, BottomRight
}

public class UIElement
{
    public Anchor Anchor { get; set; } = Anchor.TopLeft; 
    public Vector2 Offset { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = new Vector2(100, 100);
    public bool IsVisible { get; set; } = true;
    
    public UIElement Parent { get; set; }
    public List<UIElement> Children { get; } = new();

    public void AddChild(UIElement child)
    {
        child.Parent = this; 
        Children.Add(child);
    }

    public Rectangle GetBounds(Rectangle screenBounds)
    {
        Rectangle parentBounds = Parent?.GetBounds(screenBounds) ?? screenBounds;
        Vector2 anchorPos = GetAnchorPoint(parentBounds);
        Vector2 origin = GetOrigin(); 
        Vector2 finalPos = anchorPos + Offset - origin;
        
        return new Rectangle((int)finalPos.X, (int)finalPos.Y, (int)Size.X, (int)Size.Y);
    }

    private Vector2 GetAnchorPoint(Rectangle parent)
    {
        float x = Anchor switch
        {
            Anchor.TopLeft or Anchor.MiddleLeft or Anchor.BottomLeft => parent.Left,
            Anchor.TopCentre or Anchor.MiddleCentre or Anchor.BottomCentre => parent.Center.X,
            _ => parent.Right
        };
        float y = Anchor switch
        {
            Anchor.TopLeft or Anchor.TopCentre or Anchor.TopRight => parent.Top,
            Anchor.MiddleLeft or Anchor.MiddleCentre or Anchor.MiddleRight => parent.Center.Y,
            _ => parent.Bottom
        };
        return new Vector2(x, y);
    }

    private Vector2 GetOrigin()
    {
        float x = Anchor switch
        {
            Anchor.TopLeft or Anchor.MiddleLeft or Anchor.BottomLeft => 0,
            Anchor.TopCentre or Anchor.MiddleCentre or Anchor.BottomCentre => Size.X / 2,
            _ => Size.X
        };
        float y = Anchor switch
        {
            Anchor.TopLeft or Anchor.TopCentre or Anchor.TopRight => 0,
            Anchor.MiddleLeft or Anchor.MiddleCentre or Anchor.MiddleRight => Size.Y / 2,
            _ => Size.Y
        };
        
        return new Vector2(x, y);
    }

    public virtual void Draw(ShapeBatch sb, SpriteBatch spriteBatch, Rectangle screenBounds)
    {
        if (IsVisible)
        {
            foreach (var child in Children) child.Draw(sb, spriteBatch, screenBounds);
        }

        return; 
    }
}