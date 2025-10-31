using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

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
            float dw = 50f;
            float dh = 50f;
            float x = 100f;
            float y = 100f;

            // Create dice and load all the dice textures into SpriteManager
            for (int i = 1; i <= number_of_dice; i++)
            {
                _spriteManager.LoadTexture("DPurp" + i, Content.Load<Texture2D>("DPurp" + i));
                _objectManager.BirthObject(new Dice(dw, dh, new Vector2(x*i, y)));
            }

        }

        protected override void Update(GameTime gameTime)
        {
            _objectManager.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

                
            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;
            
            
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Pink);

            _spriteBatch.Begin();

            foreach (GameObject obj in _objectManager.ObjList)
            {
                if (obj.Type == ObjType.Dice)
                {
                    _spriteManager.Draw(_spriteBatch, obj.TextureKey, obj, 0);
                }
            }


            _spriteBatch.End();
            base.Draw(gameTime);
        }
        
    }

}
