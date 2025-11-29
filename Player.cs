using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Apos.Shapes;
using DiceMG.UI;
using System.Linq;

namespace DiceMG;


public class Player
{
    public string Name;
    public Panel Tray;
    public Dictionary<Dice, string> DiceSet;
    public List<Dice> DiceList;
    public List<Dice> ActiveDice => DiceList.Where(d => d.State != DieState.played).ToList();
    public int Score;

    public float _trayx;
    public float _trayy;
    public Vector2 _traysize; 
    
    public int PNumber;

    public Player()
    {
        DiceSet = new Dictionary<Dice, string>();
        DiceList = new List<Dice>();
    }

    public Player(string name, int p_num)
    {
        Name = name;
        DiceSet = new Dictionary<Dice, string>();
        DiceList = new List<Dice>();
        PNumber = p_num;
    }

    public void AddDice(Dice d, string dNum)
    {
        DiceSet.Add(d, dNum);
        DiceList.Add(d);
    }

    public void RollDice()
    {
        foreach (Dice d in ActiveDice)
        {
            d.Roll();
            // Give each die a random velocity for variety
            int vel_x = Global.SoRandom.Next(-2000, 2000);
            int vel_y = Global.SoRandom.Next(-2000, 2000);
            
            d.Velocity = new Vector2(
                (float)(vel_x), 
                (float)(vel_y) 
            );
        }
    }

    public void Update(GameTime dt)
    {

        // Update all dice movement every frame
        foreach (Dice die in ActiveDice)
        {

            if (die.State == DieState.rolling)
            {
                // Apply friction/drag to slow down
                float friction = 0.99f; // Value between 0 and 1 (lower = more friction)
                float angularFriction = 0.8f; // Friction for rotation
                
                die.Velocity *= friction;
                die.AngularVelocity *= angularFriction;

                // Update position based on velocity
                die.Position += die.Velocity * (float)dt.ElapsedGameTime.TotalSeconds;

                // Update rotation based on angular velocity
                die.Rotation += die.AngularVelocity * (float)dt.ElapsedGameTime.TotalSeconds;
                
                // Keep rotation in 0-2π range
                if (die.Rotation > MathHelper.TwoPi)
                    die.Rotation -= MathHelper.TwoPi;
                else if (die.Rotation < 0)
                    die.Rotation += MathHelper.TwoPi;

                // Check if velocity has stopped (or is very close to zero)
                if (die.Velocity.LengthSquared() < 0.5f) // threshold for "stopped"
                {
                    die.Velocity = Vector2.Zero; // Stop completely
                    die.AngularVelocity = 0f;
                    die.State = DieState.free;
                }
            }
        }
    }

}