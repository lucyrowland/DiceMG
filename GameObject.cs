using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System; 
using System.Collections.Generic;
using System.Linq;

namespace DiceMG;

public enum ObjType
{
    Dice, 
    Tray,
    Other
}

public class GameObject
{
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 Position = Vector2.Zero;
    public Rectangle Box => new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);
    public ObjType Type { get; set; }
    public float Mass = 1f;
    public Vector2 Velocity = Vector2.Zero;
    public string TextureKey { get; set; }
    public float Rotation { get; set; } = 0f; 

    public GameObject(){}
    
    public GameObject(float width, float height, Vector2 position, ObjType type)
    {
        Width = width;
        Height = height;
        Position = position;
        Type = type; 
    }
    public void TransformP(Vector2 position)
    {
        Position += position;
    }

    public void TransformS(Vector2 scale)
    {
        Width *= scale.X;
        Height *= scale.Y;
    }
    
}

public class ObjPhysics
{
    public void BoxCollision(GameObject obj1, GameObject obj2)
    {
        if (obj1.Box.Intersects(obj2.Box))
        {
                       // Calculate overlap on each axis
            float overlapX = Math.Min(obj1.Box.Right, obj2.Box.Right) - Math.Max(obj1.Box.Left, obj2.Box.Left);
            float overlapY = Math.Min(obj1.Box.Bottom, obj2.Box.Bottom) - Math.Max(obj1.Box.Top, obj2.Box.Top);
            
            // Find the axis with minimum penetration (that's our collision normal)
            if (overlapX < overlapY)
            {
                // Horizontal collision - separate on X axis
                float direction = (obj1.Position.X < obj2.Position.X) ? -1 : 1;
                float separation = overlapX / 2f;
                
                obj1.Position.X += direction * -separation;
                obj2.Position.X += direction * separation;
                
                // Bounce velocities with restitution
                float relativeVelocity = obj1.Velocity.X - obj2.Velocity.X;
                float restitution = 0.5f; // bounciness (0 = no bounce, 1 = perfect bounce)
                
                obj1.Velocity.X = ((obj1.Mass - restitution * obj2.Mass) * obj1.Velocity.X + 
                                   (1 + restitution) * obj2.Mass * obj2.Velocity.X) / (obj1.Mass + obj2.Mass);
                obj2.Velocity.X = ((obj2.Mass - restitution * obj1.Mass) * obj2.Velocity.X + 
                                   (1 + restitution) * obj1.Mass * obj1.Velocity.X) / (obj1.Mass + obj2.Mass);
            }
            else
            {
                // Vertical collision - separate on Y axis
                float direction = (obj1.Position.Y < obj2.Position.Y) ? -1 : 1;
                float separation = overlapY / 2f;
                
                obj1.Position.Y += direction * -separation;
                obj2.Position.Y += direction * separation;
                
                // Bounce velocities with restitution
                float relativeVelocity = obj1.Velocity.Y - obj2.Velocity.Y;
                float restitution = 0.5f;
                
                obj1.Velocity.Y = ((obj1.Mass - restitution * obj2.Mass) * obj1.Velocity.Y + 
                                   (1 + restitution) * obj2.Mass * obj2.Velocity.Y) / (obj1.Mass + obj2.Mass);
                obj2.Velocity.Y = ((obj2.Mass - restitution * obj1.Mass) * obj2.Velocity.Y + 
                                   (1 + restitution) * obj1.Mass * obj1.Velocity.Y) / (obj1.Mass + obj2.Mass);
            }
        }
    }
}

public class ObjectManager
{
    public ObjPhysics Physics = new ObjPhysics();
    public List<GameObject> ObjList = new List<GameObject>(); 
    bool _spaceReleased = false;
    public List<Dice> _activeDice => ObjList.OfType<Dice>()
        .Where(d => d.State != DieState.played)
        .ToList(); 
    
    public void BirthObject(GameObject obj) => ObjList.Add(obj);
    public void KillObject(GameObject obj) => ObjList.Remove(obj);

    
    public void RollDice(bool released)
    {
        foreach (Dice die in _activeDice)
        {
            die.Roll(); 
        }
        
    }
    public void Update()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Space))
        {
            if (!_spaceReleased)
            {
                RollDice(_spaceReleased);
                Debug.WriteLine("Space");
                _spaceReleased = true;
            }
            
        }
        else
        {
            _spaceReleased = false;
        }
        for (int i = 0; i < ObjList.Count; i++)
        {
            for (int j = i + 1; j < ObjList.Count; j++)
            {
                Physics.BoxCollision(ObjList[i], ObjList[j]);
            }
        }
        
       
        foreach (Dice die in _activeDice)
        {
            if (die.State == DieState.rolling)
            {
                // Check if velocity has stopped (or is very close to zero)
                if (die.Velocity.LengthSquared() < 0.01f) // threshold for "stopped"
                {
                    die.State = DieState.free;
                }
            }
        }


    }
}
