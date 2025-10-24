using System;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;


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

        public Dice(int sides, Point position)
        {
            _position = position;
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
            public void Roll()
            {
                _side_up = random.Next(1, _sides + 1);

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
                _new_dice.Roll(); 
                DiceList.Add(_new_dice);
            }
        }
        private Point diceLocation(int index)
        {
            int x_val = 100;
            x_val = x_val * (index + 1);
            return new Point(x_val, 200);
        }
    }
}