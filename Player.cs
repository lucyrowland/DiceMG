using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Apos.Shapes;
using DiceMG.UI;
using System.Linq;
using DiceMG.Input;
using System;

namespace DiceMG;


public class Player
{
    public string Name;
    public Panel Tray;
    public Dictionary<Dice, string> DiceSet;
    public List<Dice> DiceList;
    public List<Dice> ActiveDice => DiceList.Where(d => d.State != DieState.played).ToList();
    public List<Dice> HeldDice => DiceList.Where(d => d.State == DieState.held).ToList();

    public Score Scores;
    

    public float _trayx;
    public float _trayy;
    public Vector2 _traysize; 
    
    public int PNumber;
    
    //for click and drag func
    private Dice _diceInDrag; 
    private Vector2 _dragOffset = Vector2.Zero;

    public Player()
    {
        DiceSet = new Dictionary<Dice, string>();
        DiceList = new List<Dice>();
        Scores = new Score();
    }

    public int HandScore => Scores.PossibleScore(HeldDice) ? HeldDice.Sum(d => d.Value) : 0;
    public string HandScoreText => HandScore.ToString();
    public int RoundScore => Scores.GetRoundScore(); 
    public string RoundScoreText => RoundScore.ToString();
    public Player(string name, int p_num)
    {
        Name = name;
        DiceSet = new Dictionary<Dice, string>();
        DiceList = new List<Dice>();
        PNumber = p_num;
        Scores = new Score();
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

    public void StopDice()
    {
        foreach (Dice d in ActiveDice)
        {
            d.Velocity = Vector2.Zero;
        }
    }

    public void DeleteDice(Dice d)
    {
        DiceList.Remove(d);
    }

    public void RemoveHeldDice()
    {
        foreach (Dice d in DiceList)
        {
            if (d.State == DieState.held) d.State = DieState.played;
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
        
        HandleDiceClick(ActiveDice);
    }
    public void HandleDiceClick(List<Dice> dice)
    {
        var mousePos = Core.Input.Mouse.Position.ToVector2();
    
        // Start dragging
        if (Core.Input.Mouse.ButtonPressed(MouseButton.Left))
        {
            foreach (var die in dice)
            {
                if (die.Box.Contains(Core.Input.Mouse.Position))
                {
                    _diceInDrag = die;
                    _dragOffset = die.Position - mousePos;
                    break; // Only drag one die at a time
                }
            }
        }
    
        // During drag
        if (_diceInDrag != null && Core.Input.Mouse.IsButtonDown(MouseButton.Left))
        {
            _diceInDrag.Position = mousePos + _dragOffset;
            _diceInDrag.Velocity = Vector2.Zero; // Stop physics
            
            //rotate the die with the scroll wheel 
            if (Core.Input.Mouse.ScrollWheelDelta != 0)
            {
                float rotationSpeed = 0.05f;
                _diceInDrag.Rotation += rotationSpeed * Core.Input.Mouse.ScrollWheelDelta;
            }
            
        }
    
        // End drag
        if (_diceInDrag != null && Core.Input.Mouse.ButtonReleased(MouseButton.Left))
        {
            _diceInDrag = null;
        }
    }


}