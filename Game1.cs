using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace DiceMG
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D pixelTexture;
        private Texture2D diceSheet;
        private DiceManager _diceManager;

        private int number_of_dice = 6; 


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            diceSheet = Content.Load<Texture2D>("diceRed_border");

            _diceManager = new DiceManager(number_of_dice);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            _diceManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Pink);

            _spriteBatch.Begin();

            foreach (var dice in _diceManager.DiceList)
            {
                DrawDiceSprite(dice);
            }
            

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawDiceSprite(Dice dice)
        {
            var spriteValue = dice.spriteCoord(); 
            _spriteBatch.Draw(
                diceSheet,
                new Vector2(dice._position.X, dice._position.Y),  // Convert manually
                spriteValue,
                Color.White
        );
        }

    }

}
