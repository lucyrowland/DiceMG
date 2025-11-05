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
        public DieState State = DieState.free;
        private int _side_up;

        public int Value
        {
            get => _side_up;
            private set
            {
                _side_up = value;
                UpdateTexture();
            } 
        }
        
        public Dice(float width, float height, Vector2 position)
            : base(width, height, position, ObjType.Dice)
        {
            // Any Dice-specific initialization goes here
            TextureKey = $"DPurp1";
        }

        private void UpdateTexture()
        {
            TextureKey = $"DPurp{Value}";
        }

        public void Roll()
        {
            _side_up = Global.SoRandom.Next(1, _sides + 1);
            UpdateTexture();
            State = DieState.rolling;
            // Give it some random initial velocity
            Velocity = new Vector2(
                (float)(Global.SoRandom.NextDouble() - 0.5) * 500, // random X velocity
                (float)(Global.SoRandom.NextDouble() - 0.5) * 500 // random Y velocity
            );
        }

        public void Hold()
        {
            TransformS(new Vector2(1.2f,1.2f));
        }
    }
}

        
    