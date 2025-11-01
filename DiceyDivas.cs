using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;


namespace DiceMG
{

    public class DiceyDivas : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteManager _spriteManager;
        private ObjectManager _objectManager = new ObjectManager();

        private Texture2D pixelTexture;
        private RollingTray _rollingTray;
        
        private int number_of_dice = 6; 

        public DiceyDivas()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            // Set your desired resolution here
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            // Make sure to call ApplyChanges() for the changes to take effect
            _graphics.ApplyChanges();
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
            _spriteManager = new SpriteManager(GraphicsDevice);
            
            // Tray dimensions and position
            float trayw = 200f;
            float trayh = 2 * trayw;  // 400
            float trayx = 200f;
            float trayy = 100f;
            
            Debug.WriteLine($"Tray: x={trayx}, y={trayy}, w={trayw}, h={trayh}");
            Debug.WriteLine($"Tray bounds: ({trayx}, {trayy}) to ({trayx + trayw}, {trayy + trayh})");
            
            // Dice dimensions
            float dw = 30f;
            float dh = 30f; 
            
            // Calculate spawn bounds with padding
            int DICE_SPAWN_X_LOWERBOUND = (int)(trayx + 5);
            int DICE_SPAWN_X_UPPERBOUND = (int)(trayx + trayw - dw - 5);
            int DICE_SPAWN_Y_LOWERBOUND = (int)(trayy + 5);
            int DICE_SPAWN_Y_UPPERBOUND = (int)(trayy + trayh - dh - 5);
            
            Debug.WriteLine($"Dice spawn bounds: X=[{DICE_SPAWN_X_LOWERBOUND}, {DICE_SPAWN_X_UPPERBOUND}] Y=[{DICE_SPAWN_Y_LOWERBOUND}, {DICE_SPAWN_Y_UPPERBOUND}]");

            // Load tray sprite
            _spriteManager.LoadTexture("tray", Content.Load<Texture2D>("tray_sprite"));
            _objectManager.BirthObject(new RollingTray(trayw, trayh, new Vector2(trayx, trayy)));
            
            // Load all dice face textures (1-6)
            for (int i = 1; i <= 6; i++)
            {
                _spriteManager.LoadTexture("DPurp" + i, Content.Load<Texture2D>("DPurp" + i));
            }
            
            // Create dice and spawn them within the tray
            for (int i = 0; i < number_of_dice; i++)
            {
                int dx = Random.Shared.Next(DICE_SPAWN_X_LOWERBOUND, DICE_SPAWN_X_UPPERBOUND);
                int dy = Random.Shared.Next(DICE_SPAWN_Y_LOWERBOUND, DICE_SPAWN_Y_UPPERBOUND);
                Debug.WriteLine($"Spawning die {i + 1} at ({dx}, {dy})");
                _objectManager.BirthObject(new Dice(dw, dh, new Vector2(dx, dy)));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _objectManager.Update(gameTime);
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Pink);

            _spriteBatch.Begin();

            // Draw all objects
            foreach (RollingTray tray in _objectManager.ObjList.OfType<RollingTray>())
            {
                _spriteManager.Draw(_spriteBatch, tray.TextureKey, tray, Color.White);
            }
            foreach (Dice die in _objectManager.ObjList.OfType<Dice>()) 
            {
                _spriteManager.Draw(_spriteBatch, die.TextureKey, die, 0f);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}