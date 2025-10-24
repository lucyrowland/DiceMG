using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;


namespace DiceMG
{
    public class Dice
    {
        Random random = new Random();

        public Point _position { get; set; }
        public int width { get; } = 50;
        public int _sides { get; init; }
        private int _side_up { get; set; }
        private List<int> _range;
        public Rectangle gameObj { get; set; } = Rectangle.Empty;
        public DieState _state { get; set; } = DieState.free;
        private int spriteSize = 68;
        public Rectangle Sprite;

        public Vector2 PositionF;
        public Vector2 Velocity;
        public Vector2 Target;
        private float springK = 20f;
        private float damping = 8f;
        private float stopThreshold = 5f;
        public bool isMoving = false; 

        public Dice(int sides, Point position)
        {
            _position = position;
            PositionF = new Vector2(position.X, position.Y);
            _sides = sides;
            _state = DieState.free;
            _range = Enumerable.Range(1, _sides).ToList();
        }
        public Rectangle spriteCoord()
        {
            Sprite = _side_up switch
            {
                1 => new Rectangle(0, 136, spriteSize, spriteSize),
                4 => new Rectangle(68, 136, spriteSize, spriteSize),
                5 => new Rectangle(0, 68, spriteSize, spriteSize),
                2 => new Rectangle(68, 68, spriteSize, spriteSize),
                6 => new Rectangle(0, 0, spriteSize, spriteSize),
                3 => new Rectangle(68, 0, spriteSize, spriteSize),
                _ => new Rectangle(68, 0, spriteSize, spriteSize)
            };
            return Sprite;
        }

        public enum DieState
        {
            free, held, played
        }
        public void Roll(Vector2 start, Vector2 target)
        {
            _side_up = random.Next(1, _sides + 1);
            startMove(start, target);
        }
        public Point Size()
        {
            return new Point(width);  //dice is a square so width is the same as height 
        }
        public Point Location()
        {
            return _position;
        }

        public int Value
        {
            get { return _side_up;  }
        }
        public void startMove(Vector2 start, Vector2 target)
        {
            PositionF = start; 
            Target = target;
            Vector2 direction = Target - PositionF; 
            direction.Normalize();
            Velocity = direction * (30 + Random.Shared.Next(100, 200));
            isMoving = true;
        }
        public void Update(GameTime gameTime)
        {
            if (!isMoving) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // spring-damped motion
            Vector2 displacement = Target - PositionF;
            Vector2 springForce = displacement * springK;
            Vector2 dampingForce = -Velocity * damping;
            Vector2 acceleration = springForce + dampingForce;

            Velocity += acceleration * dt;
            PositionF += Velocity * dt;

            // stop if nearly still
            if (displacement.Length() < stopThreshold && Velocity.Length() < 10f)
            {
                Velocity = Vector2.Zero;
                PositionF = Target;
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
        public DiceManager(int n_dice)
        {
            _n_dice = n_dice;
            createDiceList();
        }
        private void createDiceList()
        {
            for (int i = 0; i < _n_dice; i++)
            {
                Point diceXY = diceLocation(i);
                Dice _new_dice = new Dice(6, diceXY);
                
                
                Vector2 start = new Vector2(-100 - i * 50, Random.Shared.Next(100, 400));  // offscreen left
                Vector2 target = new Vector2(Random.Shared.Next(300, 700), Random.Shared.Next(200, 400));
                _new_dice.Roll(start, target);

                DiceList.Add(_new_dice);
            }
        }
        private Point diceLocation(int index)
        {
            int x_val = 100;
            x_val = x_val * (index + 1);
            return new Point(x_val, 200);
        }
        public void Update(GameTime gameTime)
        {
            foreach (var dice in DiceList)
                dice.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                int i = 0; 
                foreach (var dice in DiceList)
                {
                    Vector2 start = new Vector2(-100 - i * 50, Random.Shared.Next(100, 400));  // offscreen left
                    Vector2 target = new Vector2(Random.Shared.Next(300, 700), Random.Shared.Next(200, 400));
                    dice.Roll(start, target);
                    i++; 
                }
            }
        }
    }
}