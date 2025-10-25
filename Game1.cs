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
        private Texture2D traySheet;
        private DiceManager _diceManager;
        private RollingTray _rollingTray;
        
        private int number_of_dice = 6; 
        private KeyboardState previousKeyboardState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;
            
            
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            diceSheet = Content.Load<Texture2D>("diceRed_border");
            traySheet = Content.Load<Texture2D>("tray_designs_v1");
            
            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;


            _diceManager = new DiceManager(number_of_dice);
            
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;
            
            _diceManager.Update(gameTime);
            
            previousKeyboardState = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Pink);

            _spriteBatch.Begin();
            DrawRollingTray();

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
            
            // Calculate the center origin for proper rotation if needed
            Vector2 origin = new Vector2(spriteValue.Width / 2f, spriteValue.Height / 2f);
            
            // Draw centered at the dice position
            _spriteBatch.Draw(
                diceSheet,
                dice.PositionF,  // Use the float position for smoother movement
                spriteValue,
                Color.White,
                0f,  // No rotation for now
                origin,  // Center the sprite
                0.77f,  // Scale
                SpriteEffects.None,
                0f
            );
        }
        private void DrawRollingTray()
        {
            var spriteValue = Global.Tray.SpriteCoord();
            
            // Calculate the center origin for proper rotation if needed
            Vector2 origin = new Vector2(spriteValue.Width / 2f, spriteValue.Height / 2f);
            
            // Draw centered at the dice position
            _spriteBatch.Draw(
                traySheet,
                Global.Tray.PositionF,  // Use the float position for smoother movement
                spriteValue,
                Color.White,
                0f,  // No rotation for now
                origin,  // Center the sprite
                1.5f,  // Scale
                SpriteEffects.None,
                0f
            );
        }
    }

}
