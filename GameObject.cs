using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System; 
using System.Collections.Generic;
using System.Linq;
using DiceMG.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using DiceMG.Input;


namespace DiceMG;

public enum ObjType
{
    Dice, 
    Tray,
    Button, 
    Other
}

public class GameObject
{
    
    public float Width { get; set; }
    public float Height { get; set; }
    public Vector2 Position = Vector2.Zero;
    public Rectangle Box => new ((int)Position.X, (int)Position.Y, (int)Width, (int)Height);
    public ObjType Type { get; set; }
    public float Mass = 1f;
    public Vector2 Velocity = Vector2.Zero;
    public string TextureKey { get; set; }
    public float Rotation { get; set; } = 0f; 
    public Vector2 Acceleration = Vector2.Zero;
    
    public bool Visible { get; set; } = true;

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
    public Vector2 Size => new Vector2(Width, Height);

    public void TransformS(Vector2 scale)
    {
        Width *= scale.X;
        Height *= scale.Y;
    }



    public void Move(float _acceleration, GameTime dt)
    {

        if (Velocity.LengthSquared() < 0.01f)
            return;
        
        // 1. Apply acceleration to velocity
        Velocity.X += _acceleration * (float)dt.ElapsedGameTime.TotalSeconds;
        Velocity.Y += _acceleration * (float)dt.ElapsedGameTime.TotalSeconds;

        // 2. Apply velocity to position
        Position += Velocity * (float)dt.ElapsedGameTime.TotalSeconds;
        
    }
    
    
}

public class ObjPhysics
{
    public void BoxCollision(Dice obj1, Dice obj2)
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
                float separation = overlapX;
                
                obj1.Position.X += direction * -separation;
                obj2.Position.X += direction * separation;
                
                // Bounce velocities with restitution
                float relativeVelocity = obj1.Velocity.X - obj2.Velocity.X;
                float restitution = 0.9f; // bounciness (0 = no bounce, 1 = perfect bounce)
                
                obj1.Velocity.X = ((obj1.Mass - restitution * obj2.Mass) * obj1.Velocity.X + 
                                   (1 + restitution) * obj2.Mass * obj2.Velocity.X) / (obj1.Mass + obj2.Mass);
                obj2.Velocity.X = ((obj2.Mass - restitution * obj1.Mass) * obj2.Velocity.X + 
                                   (1 + restitution) * obj1.Mass * obj1.Velocity.X) / (obj1.Mass + obj2.Mass);
            }
            else
            {
                // Vertical collision - separate on Y axis
                float direction = (obj1.Position.Y < obj2.Position.Y) ? -1 : 1;
                float separation = overlapY;
                
                obj1.Position.Y += direction * -separation;
                obj2.Position.Y += direction * separation;
                
                // Bounce velocities with restitution
                float relativeVelocity = obj1.Velocity.Y - obj2.Velocity.Y;
                float restitution = 0.9f;
                
                obj1.Velocity.Y = ((obj1.Mass - restitution * obj2.Mass) * obj1.Velocity.Y + 
                                   (1 + restitution) * obj2.Mass * obj2.Velocity.Y) / (obj1.Mass + obj2.Mass);
                obj2.Velocity.Y = ((obj2.Mass - restitution * obj1.Mass) * obj2.Velocity.Y + 
                                   (1 + restitution) * obj1.Mass * obj1.Velocity.Y) / (obj1.Mass + obj2.Mass);
            }
        }
    }

    public void CheckTrayBounds(Dice obj, RollingTray tray)
    {
        float restitution = 0.95f; // Bounciness (0 = no bounce, 1 = perfect bounce)
        float tray_padding = Global.TRAY_PADDING;
        
        var leftbound = tray.Position.X+tray_padding;
        var rightbound = tray.Position.X + tray.Width-tray_padding;
        var topbound = tray.Position.Y+tray_padding;
        var bottombound = tray.Position.Y + tray.Height-tray_padding;
        
        // Left edge
        if (obj.Position.X < leftbound)
        {
            obj.Position.X = leftbound + tray_padding;
            obj.Velocity.X = -obj.Velocity.X * restitution;
        }
        
        // Right edge
        if (obj.Position.X + obj.Width > rightbound)
        {
            obj.Position.X = rightbound - obj.Width;
            obj.Velocity.X = -obj.Velocity.X * restitution;
        }
        
        // Top edge
        if (obj.Position.Y < topbound)
        {
            obj.Position.Y = topbound + tray_padding;
            obj.Velocity.Y = -obj.Velocity.Y * restitution;
        }
        
        // Bottom edge
        if (obj.Position.Y + obj.Height > bottombound)
        {
            obj.Position.Y = bottombound - obj.Height;
            obj.Velocity.Y = -obj.Velocity.Y * restitution;
        }
    }
    public void CheckScreenBounds(GameObject obj, int screenWidth, int screenHeight)
    {
        float restitution = 0.7f; // Bounciness (0 = no bounce, 1 = perfect bounce)
        
        // Left edge
        if (obj.Position.X < 0)
        {
            obj.Position.X = 0;
            obj.Velocity.X = -obj.Velocity.X * restitution;
        }
        
        // Right edge
        if (obj.Position.X + obj.Width > screenWidth)
        {
            obj.Position.X = screenWidth - obj.Width;
            obj.Velocity.X = -obj.Velocity.X * restitution;
        }
        
        // Top edge
        if (obj.Position.Y < 0)
        {
            obj.Position.Y = 0;
            obj.Velocity.Y = -obj.Velocity.Y * restitution;
        }
        
        // Bottom edge
        if (obj.Position.Y + obj.Height > screenHeight)
        {
            obj.Position.Y = screenHeight - obj.Height;
            obj.Velocity.Y = -obj.Velocity.Y * restitution;
        }
    }
}

public class ObjectManager
{
    private SoundEffect _sound; 
    private ObjPhysics Physics = new ObjPhysics();
    public Dictionary<string, GameObject> ObjDict = new Dictionary<string, GameObject>();
    public List<GameObject> ObjList = new List<GameObject>(); 
    bool _spaceReleased = false;
    private List<Dice> _activeDice => ObjList.OfType<Dice>()
        .Where(d => d.State != DieState.played)
        .ToList();
    public List<int> ActiveDiceValues => _activeDice.Select(d => d.Value).ToList();
    private List<Dice> _heldDice => ObjList.OfType<Dice>().Where(d => d.State == DieState.held).ToList();
    public void BirthObject(GameObject obj) => ObjList.Add(obj);
    public void KillObject(GameObject obj) => ObjList.Remove(obj);
    
    public void DictAdd(string key, GameObject obj) => ObjDict.Add(key, obj);
    public void DictRemove(string key) => ObjDict.Remove(key);
    public GameObject DictGet(string key) => ObjDict[key];
    public void DictClear() => ObjDict.Clear();
    public void DictClear(string key) => ObjDict.Remove(key);
    public void DictClear(List<string> keys)
    {
        foreach (string key in keys)
        {
            ObjDict.Remove(key);
        }
    }
    public RollingTray Tray => ObjList.OfType<RollingTray>().FirstOrDefault();

    public void ResetRound(int number_of_dice = 6)
    {
        //remove dice from screen from previous round 
        foreach (Dice die in ObjList.OfType<Dice>())
            KillObject(die);
        
        // Tray dimensions and position
        float trayw = 200f;
        float trayh = 2 * trayw;  // 400
        float trayx = 200f;
        float trayy = 100f;
        
        // Dice dimensions
        float dw = 35;
        float dh = 35;

        float TRAY_PADDING = Global.TRAY_PADDING;
            
        // Calculate spawn bounds with padding
        int DICE_SPAWN_X_LOWERBOUND = (int)(trayx + TRAY_PADDING);
        int DICE_SPAWN_X_UPPERBOUND = (int)(trayx + trayw - dw - TRAY_PADDING);
        int DICE_SPAWN_Y_LOWERBOUND = (int)(trayy + trayh*0.5 + TRAY_PADDING);
        int DICE_SPAWN_Y_UPPERBOUND = (int)(trayy + trayh - dh - TRAY_PADDING);

        // Create dice and spawn them within the tray
        for (int i = 0; i < number_of_dice; i++)
        {
            int dx = Random.Shared.Next(DICE_SPAWN_X_LOWERBOUND, DICE_SPAWN_X_UPPERBOUND);
            int dy = Random.Shared.Next(DICE_SPAWN_Y_LOWERBOUND, DICE_SPAWN_Y_UPPERBOUND);
            Debug.WriteLine($"Spawning die {i + 1} at ({dx}, {dy})");
            BirthObject(new Dice(dw, dh, new Vector2(dx, dy)));
        }

        RollDice();
        ScoringSystem.Reset(); 
    }
    
    public void RollDice()
    {
        foreach (Dice die in _activeDice)
        {
            die.Roll();
            // Give each die a random velocity for variety
            int vel_x = Random.Shared.Next(-400, 400);
            int vel_y = Random.Shared.Next(-1500, -1000);
            die.Velocity = new Vector2(
                (float)(vel_x), 
                (float)(vel_y) 
            );
        }
    }


    public void RemoveLockedInDice()
    {
        foreach (Dice die in _heldDice)
        {
            die.Visible = false;
            die.State = DieState.played;
            KillObject(die);

        }
             
    }
    public void LoadGameContent(ContentManager content)
    {
        content.RootDirectory = "Content";
        _sound = content.Load<SoundEffect>("Audio/medium_dice_roll");
        
    }
    public void Update(GameTime dt)
    {

        // Update all dice movement every frame
        foreach (Dice die in _activeDice)
        {
            Physics.CheckScreenBounds(die, Global.ScreenWidth, Global.ScreenHeight);
            Physics.CheckTrayBounds(die, Tray);

            if (die.State == DieState.rolling)
            {
                // Apply friction/drag to slow down
                float friction = 0.93f; // Value between 0 and 1 (lower = more friction)
                die.Velocity *= friction;
                
                // Update position based on velocity
                die.Position += die.Velocity * (float)dt.ElapsedGameTime.TotalSeconds;
                
                // Check screen boundaries and bounce

                
                
                // Check if velocity has stopped (or is very close to zero)
                if (die.Velocity.LengthSquared() < 0.5f) // threshold for "stopped"
                {
                    die.Velocity = Vector2.Zero; // Stop completely
                    die.State = DieState.free;
                }
            }
        }
        
        for (int i = 0; i < _activeDice.Count; i++)
        {
            for (int j = i + 1; j < _activeDice.Count; j++)
            {
                Physics.BoxCollision(_activeDice[i], _activeDice[j]);
            }
        }

    }
}
