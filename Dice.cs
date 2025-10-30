using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Xna.Framework.Graphics;


namespace DiceMG
{
    public enum DieState
    {
        free,
        held,
        played,
        rolling,
    }

    public class Dice : GameObject
    {

        public int _sides = 6;
        public int[] _range = [1, 2, 3, 4, 5, 6];
        public DieState State = DieState.free; 
        private int _side_up;
        public int Value { get { return _side_up; } }
        
        // Constructor - use 'base' to call parent constructor
        public Dice(Vector2 position) 
            : base(50, 50, position, ObjType.Dice)
        {
            // Any Dice-specific initialization goes here
        }
        
        
        public Dice(float width, float height, Vector2 position) 
            : base(width, height, position, ObjType.Dice)
        {
            // Any Dice-specific initialization goes here
        }

        public void Roll()
        {
            _side_up = Global.SoRandom.Next(1, _sides + 1);
            State = DieState.rolling; 
            // Give it some random initial velocity
            Velocity = new Vector2(
                (float)(Global.SoRandom.NextDouble() - 0.5) * 500, // random X velocity
                (float)(Global.SoRandom.NextDouble() - 0.5) * 500  // random Y velocity
            );
        }





        public void startMove(Vector2 start, Vector2 target)
        {
            PositionF = start;
            Target = target;
            Vector2 direction = Target - PositionF;
            direction.Normalize();
            // Higher initial velocity for throwing effect
            Velocity = direction * (100 + Random.Shared.Next(100, 400));
            isMoving = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!isMoving) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update position
            PositionF += Velocity * dt;
            if ((PositionF.Y < Global.Tray.TrayHeight() + Global.Tray.PositionF.Y-200) && (PositionF.X < Global.Tray.TrayWidth() + Global.Tray.PositionF.X))
            {
                EnteredTray = true; 
            }

            if (PositionF.Y > Global.Tray.TrayHeight() + Global.Tray.PositionF.Y)
            {
                EnteredTray = false;
            }
            // Apply friction and deceleration
            if (Velocity.Length() > 0)
            {
                // Apply friction (multiplicative)
                Velocity *= friction;
                
                // Apply deceleration (subtractive)
                Vector2 decel = Vector2.Normalize(Velocity) * deceleration * dt;
                if (decel.Length() > Velocity.Length())
                    Velocity = Vector2.Zero;
                else
                    Velocity -= decel;
            }

            // Stop if velocity is very low or close to target
            if (Velocity.Length() < 20f || Vector2.Distance(PositionF, Target) < 15f)
            {
                Velocity = Vector2.Zero;
                isMoving = false;

            }

            // update integer position for drawing
            _position = new Point((int)PositionF.X, (int)PositionF.Y);
        }
    }

    public class DiceManager
    {
        private int _n_dice { get; }
        public List<Dice> DiceList { get; set; } = new List<Dice>();
        private int _screenwidth = Global.ScreenWidth; 
        private int _screenheight = Global.ScreenHeight;
        private float separationForce = 150f; // Force to push overlapping dice apart

        public DiceManager(int n_dice)
        {
            _n_dice = n_dice;
            CreateDiceList();
        }

        private void CreateDiceList()
        {
            for (int i = 0; i < _n_dice; i++)
            {
                Point diceXY = DiceLocation(i);
                Dice _new_dice = new Dice(6, diceXY);

                float MinTargetX = 200f;
                float MinTargetY = 0;
                
                float randomTargetX = MinTargetX + (float)Global.SoRandom.NextDouble() * Global.Tray.TrayWidth()*1.3f;
                float randomTargetY = MinTargetY + (float)Global.SoRandom.NextDouble() * Global.Tray.TrayHeight()*1.5f;
                
                float maxY = Global.RollingPosition.Y + Global.Tray.TrayHeight()*1.5f;
                float maxX = MinTargetX + Global.Tray.TrayWidth() * 1.5f;
                float startY = maxY - 200; 
                // Start from bottom of screen with slight horizontal variation
                Vector2 start = new Vector2(
                    Global.SoRandom.Next( (int)Global.RollingPosition.X,(int)maxX), 
                    startY
                );
                
                // Target positions spread across the middle of the screen
                Vector2 target = new Vector2(
                    randomTargetX,  // Spread horizontally with variation
                    randomTargetY  // Random vertical position
                );
                
                _new_dice.Roll(start, target);
                DiceList.Add(_new_dice);
            }
        }

        private Point DiceLocation(int index)
        {
            int x_val = 100;
            x_val = x_val * (index + 1);
            return new Point(x_val, 200);
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Update dice positions first
            foreach (var die in DiceList)
            {
                die.Update(gameTime);
            }
            
            // Handle collisions with multiple iterations for stability
            for (int iter = 0; iter < 3; iter++)
            {
                HandleCollisions(dt);
            }
            
            // Keep dice on screen
            foreach (var die in DiceList)
            {
                float radius = die.width / 2f;
                die.PositionF.X = Math.Clamp(die.PositionF.X, radius, Global.ScreenWidth - radius);
                die.PositionF.Y = Math.Clamp(die.PositionF.Y, radius, Global.ScreenHeight - radius);
                die._position = new Point((int)die.PositionF.X, (int)die.PositionF.Y);
            }

            // Re-roll on R key
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                for (int i = 0; i < DiceList.Count; i++)
                {
                    var dice = DiceList[i];
                    float MinTargetX = 250f;
                    float MinTargetY = 0;
                
                    float randomTargetX = MinTargetX + (float)Global.SoRandom.NextDouble() * Global.Tray.TrayWidth()*1.5f;
                    float randomTargetY = MinTargetY + (float)Global.SoRandom.NextDouble() * Global.Tray.TrayHeight()*1.5f;
                
                    float maxY = Global.RollingPosition.Y + Global.Tray.TrayHeight()*1.5f;
                    float maxX = MinTargetX + Global.Tray.TrayWidth() * 1.5f;
                    float startY = maxY - 200; 
                    // Start from bottom of screen with slight horizontal variation
                    Vector2 start = new Vector2(
                        MinTargetX + ((float)Global.SoRandom.NextDouble() * (Global.Tray.TrayWidth()*1.5f)) - 50, 
                        startY-150
                    );
                
                    // Target positions spread across the middle of the screen
                    Vector2 target = new Vector2(
                        randomTargetX,  // Spread horizontally with variation
                        MinTargetY
                    );
                
                    dice.Roll(start, target);
                }
            }
        }

        private bool Overlap(Dice a, Dice b)
        {
            int AminX = (int)a.PositionF.X;
            int AmaxX = (int)(a.PositionF.X + a.width);
            int AminY = (int)a.PositionF.Y;
            int AmaxY = (int)(a.PositionF.Y + a.width);
            int BminX = (int)b.PositionF.X;
            int BmaxX = (int)(b.PositionF.X + b.width);
            int BminY = (int)b.PositionF.Y;
            int BmaxY = (int)(b.PositionF.Y + b.width);
            if (BminX < AmaxX && BminY < AmaxY && BmaxX > AminX && BmaxY > AminY) 
                return true; 
            if (AminX < BmaxX && AminY < BmaxY && AmaxX > BminX && AmaxY > BminY)
                return true;
            return false; 
        }

        private bool HitTrayEdge(Dice dice)
        {
            float minX = dice.PositionF.X;
            float maxX = (dice.PositionF.X + dice.width);
            float minY = dice.PositionF.Y;
            float maxY = (dice.PositionF.Y + dice.width);
            float trayMinX = Global.RollingPosition.X;
            float trayMaxX = (Global.RollingPosition.X + Global.Tray.TrayWidth()*1.5f);
            float trayMinY = Global.RollingPosition.Y;
            float trayMaxY = (Global.RollingPosition.Y + Global.Tray.TrayHeight()*1.5f);


            if (minX < trayMinX)
                return true;
            if (minY < trayMinY)
                return true;
            if (maxX > trayMaxX)
                return true;
            return false;
        }
        private void HandleCollisions(float dt)
        {
            float dampingFactor = 0.5f; // Dice lose 1-dampingFactor of their velocity
            for (int i = 0; i < DiceList.Count; i++)
            {
                var a = DiceList[i];
                var HitEdge = HitTrayEdge(a);
                float minX = a.PositionF.X;
                float maxX = (a.PositionF.X + a.width);
                float minY = a.PositionF.Y;
                float maxY = (a.PositionF.Y + a.width);
                float trayMinX = Global.RollingPosition.X+1;
                float trayMaxX = (Global.RollingPosition.X + Global.Tray.TrayWidth()*1.5f)-10f;
                float trayMinY = Global.RollingPosition.Y;
                float trayMaxY = (Global.RollingPosition.Y + Global.Tray.TrayHeight()*1.5f);


                
                for (int j = i + 1; j < DiceList.Count; j++)
                {


                    var b = DiceList[j];

                    
                    if (Overlap(a,b))
                    {
                        float overlapX = Math.Min(a.PositionF.X + a.width - b.PositionF.X, 
                            b.PositionF.X + b.width - a.PositionF.X);
                        float overlapY = Math.Min(a.PositionF.Y + a.width - b.PositionF.Y, 
                            b.PositionF.Y + b.width - a.PositionF.Y);
                        
                        if (overlapX < overlapY)
                        {
                            // Separate horizontally along X axis
                            float sign = (a.PositionF.X < b.PositionF.X) ? -1 : 1;
                            float pushDistance = (overlapX / 2f) + 1f; // Extra pixel for safety
                    
                            a.PositionF.X += sign * pushDistance;
                            b.PositionF.X -= sign * pushDistance;
                    
                            // Apply velocity response if needed
                            if (a.isMoving || b.isMoving)
                            {
                                // Simple velocity swap with damping
                                
                                float tempVelocityX = a.Velocity.X;
                                a.Velocity.X = b.Velocity.X * dampingFactor; 
                                b.Velocity.X = tempVelocityX * dampingFactor;
                            }
                        }
                        else
                        {
                            // Separate vertically along the Y axis
                            float sign = (a.PositionF.Y < b.PositionF.Y) ? -1 : 1;
                            float pushDistance = (overlapY / 2f) + 1f;
                            
                            a.PositionF.Y += sign * pushDistance;
                            b.PositionF.Y -= sign * pushDistance;
                            
                            if (a.isMoving || b.isMoving)
                            {
                                // Reflect Y velocities
                                float tempVelY = a.Velocity.Y;
                                a.Velocity.Y = b.Velocity.Y * dampingFactor;
                                b.Velocity.Y = tempVelY * dampingFactor;
                            }
                        }
                        
                        
                    }
                    if (a.EnteredTray && HitEdge)
                    {
                        if (minX < trayMinX && a.Velocity.X > 0)
                        {
                            var distX = MathF.Abs(Global.RollingPosition.X + 2 - a.PositionF.X);
                            a.PositionF.X += distX;
                            a.Velocity.X = -a.Velocity.X * 0.95f;
                            a.Velocity.X += distX * separationForce * dt;
                        }

                        if (minY < trayMinY && a.Velocity.X > 0)
                        {
                            var distY = MathF.Abs(Global.RollingPosition.Y + 2 - a.PositionF.Y);
                            a.PositionF.Y += distY;
                            a.Velocity.Y = -a.Velocity.Y * 0.95f;
                            a.Velocity.Y += distY * separationForce * dt;
                        }
                        if (maxX > trayMaxX && a.Velocity.X > 0)
                        {
                            var distX = MathF.Abs(Global.RollingPosition.X + 2 - a.PositionF.X);
                            a.PositionF.X -= distX;
                            a.Velocity.X = -a.Velocity.X * 0.95f;
                            a.Velocity.X -= distX * separationForce * dt;
                        }
                    }
                }
            }
        }
        
 
    }
}