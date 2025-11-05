using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using DiceMG.Input;


namespace DiceMG
{

    public class DiceyDivas : Core
    {
        private ObjectManager _objectManager = new ObjectManager();
        private Texture2D pixelTexture;
        private RollingTray _rollingTray;
        
        private int number_of_dice = 6; 

        public DiceyDivas() : base("Dicey Divas", 1280, 720, false)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;
            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
        }

        protected override void LoadContent()
        {
            _objectManager.LoadGameContent(Content);
            base.LoadContent();
            
            // Tray dimensions and position
            float trayw = 200f;
            float trayh = 2 * trayw;  // 400
            float trayx = 200f;
            float trayy = 100f;
            
            Debug.WriteLine($"Tray: x={trayx}, y={trayy}, w={trayw}, h={trayh}");
            Debug.WriteLine($"Tray bounds: ({trayx}, {trayy}) to ({trayx + trayw}, {trayy + trayh})");
            
            // Dice dimensions
            float dw = 35;
            float dh = 35;

            float TRAY_PADDING = Global.TRAY_PADDING;
            
            // Calculate spawn bounds with padding
            int DICE_SPAWN_X_LOWERBOUND = (int)(trayx + TRAY_PADDING);
            int DICE_SPAWN_X_UPPERBOUND = (int)(trayx + trayw - dw - TRAY_PADDING);
            int DICE_SPAWN_Y_LOWERBOUND = (int)(trayy + trayh*0.5 + TRAY_PADDING);
            int DICE_SPAWN_Y_UPPERBOUND = (int)(trayy + trayh - dh - TRAY_PADDING);
            
            Debug.WriteLine($"Dice spawn bounds: X=[{DICE_SPAWN_X_LOWERBOUND}, {DICE_SPAWN_X_UPPERBOUND}] Y=[{DICE_SPAWN_Y_LOWERBOUND}, {DICE_SPAWN_Y_UPPERBOUND}]");

            // Load tray sprite
            _objectManager.BirthObject(new RollingTray(trayw, trayh, new Vector2(trayx, trayy)));
            

            
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

            CheckMouse(); 
            
            base.Update(gameTime);
        }

        private void CheckMouse()
        {
            if (Input.Mouse.ButtonPressed(MouseButton.Left))
            {
                Debug.WriteLine("left button clicked");
                var mousePos = Input.Mouse.Position;
                foreach (Dice die in _objectManager.ObjList.OfType<Dice>())
                {
                    if (die.Box.Contains(mousePos))
                    {
                        Debug.WriteLine("Holding");
                        die.Hold();
                    }
                }

            }
        }

        private bool IsMouseOverDice(Dice die)
        {
            if (die.Box.Contains(Input.Mouse.Position))
                return true;
            
            return false; 
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Pink);

            SpriteBatch.Begin();

            // Draw all objects
            foreach (RollingTray tray in _objectManager.ObjList.OfType<RollingTray>())
            {
                SpriteManager.Draw(SpriteBatch, tray.TextureKey, tray, Color.White);
            }
            foreach (Dice die in _objectManager.ObjList.OfType<Dice>()) 
            {
                SpriteManager.Draw(SpriteBatch, die.TextureKey, die, 0f);
                if (IsMouseOverDice(die))
                    SpriteManager.DrawDiceOutline(SpriteBatch, die.Box, Color.White, 2);
            }

            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}