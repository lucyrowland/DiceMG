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
    public void RemoveChild(UIElement child) => Children.Remove(child);
    public void ClearChildren() => Children.Clear();

    
    public void SetParent(UIElement parent) => Parent = parent;
    public void MatchParentSize() => Size = Parent.Size;

    public void MatchParentPosition()
    {
        if (Parent.Anchor == this.Anchor)
        {
            Offset = Parent.Offset;
        }
        else
        {
            Offset = Parent.Offset - GetOriginOffset(Parent.Anchor, Parent.Size);
        }
    }
    public void MatchPosition(UIElement elem)
    {
        if (elem.Anchor == this.Anchor)
        {
            Offset = elem.Offset;
        }
        else
        {
            Offset = elem.Offset - GetOriginOffset(elem.Anchor, elem.Size);
        }
    }
    
    public void AddToOffset(Vector2 offset) => Offset += offset;
    public void MatchParentWidth() => Size = new Vector2(Parent.Size.X, Size.Y);
    public void MatchParentHeight() => Size = new Vector2(Size.X, Parent.Size.Y);
    public void MatchX(UIElement elem) => Offset = new Vector2(elem.Offset.X, Offset.Y);
    public void MatchY(UIElement elem) => Offset = new Vector2(Offset.X, elem.Offset.Y);
    public void MatchSize(UIElement elem) => Size = elem.Size;
    public void MatchBottom(UIElement elem) => Offset = new Vector2(Offset.X, elem.Offset.Y - Size.Y);
    public void MatchWidth(UIElement elem) => Size = new Vector2(elem.Size.X, Size.Y);
    public void MatchHeight(UIElement elem) => Size = new Vector2(Size.X, elem.Size.Y);

    // Get the actual screen bounds for this element
    public Rectangle GetBounds(Rectangle screenBounds)
    {
        Rectangle parentBounds = Parent?.GetBounds(screenBounds) ?? screenBounds;
        Vector2 anchorPos = GetAnchorPoint(parentBounds, Anchor);
        Vector2 origin = GetOriginOffset(Anchor, Size);
        Vector2 finalPos = anchorPos + Offset - origin;
        
        return new Rectangle((int)finalPos.X, (int)finalPos.Y, (int)Size.X, (int)Size.Y);
    }

    public static void SpaceApart(List<UIElement> elements, Vector2 spacing)
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

    //static helper can be used to get the anchor point for a given rectangle
    public static Vector2 GetAnchorPoint(Rectangle bounds, Anchor anchor)
    {
        float x = anchor switch
        {
            Anchor.TopLeft or Anchor.MiddleLeft or Anchor.BottomLeft => bounds.Left,
            Anchor.TopCentre or Anchor.MiddleCentre or Anchor.BottomCentre => bounds.Center.X,
            _ => bounds.Right
        };
        float y = anchor switch
        {
            Anchor.TopLeft or Anchor.TopCentre or Anchor.TopRight => bounds.Top,
            Anchor.MiddleLeft or Anchor.MiddleCentre or Anchor.MiddleRight => bounds.Center.Y,
            _ => bounds.Bottom
        };
        return new Vector2(x, y);
    }
    // Static helper - returns the offset from anchor point based on size
    public static Vector2 GetOriginOffset(Anchor anchor, Vector2 size)
    {
        float x = anchor switch
        {
            Anchor.TopLeft or Anchor.MiddleLeft or Anchor.BottomLeft => 0,
            Anchor.TopCentre or Anchor.MiddleCentre or Anchor.BottomCentre => size.X / 2,
            _ => size.X
        };
        float y = anchor switch
        {
            Anchor.TopLeft or Anchor.TopCentre or Anchor.TopRight => 0,
            Anchor.MiddleLeft or Anchor.MiddleCentre or Anchor.MiddleRight => size.Y / 2,
            _ => size.Y
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