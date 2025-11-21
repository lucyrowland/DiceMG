using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using DiceMG.Input;
using Apos.Shapes; 
using DiceMG.Scenes;

namespace DiceMG
{

    public class DiceyDivas : Core
    {
        private Texture2D pixelTexture;
        private RollingTray _rollingTray;
        private ScoringSystem ScoringSystem;
        public SpriteFont _windowsXPFont; 
        public SpriteFont _font;
        public SpriteFont _font5x;
        public int _tempScore = 0;
        public List<int> _heldDice = new List<int>();
        public static GameManager GM = new GameManager();
        
        public int number_of_dice = 6; 

        public DiceyDivas() : base("Dicey Divas", 1280, 720, false)
        {

        }


        protected override void Initialize()
        {
            base.Initialize(); 
            Global.ScreenHeight = GraphicsDevice.Viewport.Height;
            Global.ScreenWidth = GraphicsDevice.Viewport.Width;
            
            ChangeScene(new TitleScene());
        }

        protected override void LoadContent()
        {
            
            
            base.LoadContent();
            _font = Content.Load<SpriteFont>("Fonts/File");
            _font5x = Content.Load<SpriteFont>("Fonts/5x");
            /*
            GameObjManager.LoadGameContent(Content);
            //Font initialization
            _windowsXPFont = Content.Load<SpriteFont>("Fonts/File");
            
            // Tray dimensions and position
            float trayw = 200f;
            float trayh = 2 * trayw;  // 400
            float trayx = 200f;
            float trayy = 100f;
            
            
            // Dice dimensions
            float dw = 35;
            float dh = 35;

            float TRAY_PADDING = Global.TRAY_PADDING;
            
            // Calculate spawn bounds with padding
            int DICE_SPAWN_X_LOWERBOUND = (int)(trayx + TRAY_PADDING);
            int DICE_SPAWN_X_UPPERBOUND = (int)(trayx + trayw - dw - TRAY_PADDING);
            int DICE_SPAWN_Y_LOWERBOUND = (int)(trayy + trayh*0.5 + TRAY_PADDING);
            int DICE_SPAWN_Y_UPPERBOUND = (int)(trayy + trayh - dh - TRAY_PADDING);
            

            // Load tray sprite and buttons
            RollingTray P1Tray = new RollingTray(trayw, trayh, new Vector2(trayx, trayy));
            GameObject LockButton = new GameObject(500,100,new Vector2(450,420), ObjType.Button);
            GameObject PassButton = new GameObject(500,100,new Vector2(450,550), ObjType.Button);
            GameObjManager.BirthObject(P1Tray);
            GameObjManager.DictAdd("P1Tray", P1Tray ); 
            GameObjManager.BirthObject(LockButton);
            GameObjManager.DictAdd("LockButton", LockButton );
            GameObjManager.BirthObject(PassButton);
            GameObjManager.DictAdd("PassButton", PassButton );
            */

            

        }

        protected override void Update(GameTime gameTime)
        {
            /*
            GameObjManager.Update(gameTime);

            foreach (Dice die in GameObjManager.ObjList.OfType<Dice>()) 
                ClickedDice(die);
            CheckForRoll(gameTime);
            CheckForLockButtonClick();
            
            _heldDice = GameObjManager.ObjList.OfType<Dice>().Where(d => d.State == DieState.held).Select(d => d.Value).ToList();
            _tempScore = ScoringSystem.ShowScore(_heldDice);
            
            if (Input.Keyboard.KeyPressed(Keys.N))
                GameObjManager.ResetRound(number_of_dice);  
            */
            base.Update(gameTime);
            
            
        }

        private void CheckForPassButtonClick()
        {
            GameObject passbutton = GameObjManager.DictGet("PassButton");
            if (Input.Mouse.ButtonPressed(MouseButton.Left) && passbutton.Box.Contains(Input.Mouse.Position))
                ClickPassButton();
        }

        private void CheckForLockButtonClick()
        {
            GameObject lockbutton = GameObjManager.DictGet("LockButton");
            if (Input.Mouse.ButtonPressed(MouseButton.Left) && lockbutton.Box.Contains(Input.Mouse.Position))
                ClickLockButton();
        }

        private void CheckForRoll(GameTime dt)
        {
            if (Input.Keyboard.IsKeyDown(Keys.Space))
            {
                float rand_vol = (float)Random.Shared.NextDouble();
                float rand_pitch = (float)Random.Shared.NextDouble();
                GameObjManager.RollDice();
                //_sound.Play(rand_vol, rand_pitch, 0f);
                Debug.WriteLine("Space");
            
            }
        }

        private void ClickedDice(Dice die)
        {
            if (Input.Mouse.ButtonPressed(MouseButton.Left))
            {
                
                if (die.Box.Contains(Input.Mouse.Position))
                {
                    if (die.State != DieState.held)
                        die.State = DieState.held;
                    else
                        die.State = DieState.free;
                }
                Debug.WriteLine($"Die Value: {die.Value}, Die State: {die.State}");
                Debug.WriteLine($"Score: {ScoringSystem.ShowScore(_heldDice)}");
            }
        }

        private bool IsMouseOverDice(Dice die)
        {
            if (die.Box.Contains(Input.Mouse.Position))
                return true;
            
            return false; 
        }
        // Method to get rainbow color
        private Color GetRainbowColor(float time, float speed = 2f)
        {
            
            float r = (MathF.Sin(speed * time) + 1f) / 2f;
            float g = (MathF.Sin(speed * time + 2f) + 1f) / 2f;
            float b = (MathF.Sin(speed * time + 4f) + 1f) / 2f;
    
            return new Color(r, g, b);
        }
        public void DrawDiceOutline(ShapeBatch shapeBatch, Dice die)
        {
            if (die.State == DieState.held)
            {
                DrawOutline(die, Color.Gold, 2f, 2f, 5f);
                    
            }
            if (IsMouseOverDice(die) && die.State != DieState.held)
            {
                DrawOutline(die, Color.White, 2f, 2f, 5f);

            }

            void DrawOutline(Dice die, Color colour, float thickness, float offset, float rounded)
            {
                Vector2 pos = die.Position - new Vector2(offset, offset);
                Vector2 size = die.Size + new Vector2(offset, offset);
                shapeBatch.BorderRectangle(pos, size, colour, thickness, rounded);
            }
            

        }

        protected override void Draw(GameTime gameTime)
        {
            /*
            GraphicsDevice.Clear(Color.Pink);

            SpriteBatch.Begin();
            
            // Draw all objects
            foreach (RollingTray tray in GameObjManager.ObjList.OfType<RollingTray>())
            {
                SpriteManager.Draw(SpriteBatch, tray.TextureKey, tray, Color.White);
            }
            foreach (Dice die in GameObjManager.ObjList.OfType<Dice>()) 
            {
                if (die.Visible)
                    SpriteManager.Draw(SpriteBatch, die.TextureKey, die, 0f);
            }
            
            string held_dice_display = String.Join(',', _heldDice);

            SpriteBatch.DrawString(_windowsXPFont, "Press Space to Roll", new Vector2(10, 10), Color.White);
            SpriteBatch.DrawString(_windowsXPFont, $"Held Dice: {held_dice_display}" , new Vector2(500, 250), Color.White);
            SpriteBatch.DrawString(_windowsXPFont, "Held Score: " + _tempScore, new Vector2(500, 300), Color.White);
            SpriteBatch.DrawString(_windowsXPFont, "Total Score: " + ScoringSystem.GetTotalScore(), new Vector2(500, 350), Color.White);
            
            SpriteBatch.End();
            
            // Draw shapes after sprites
            ShapeBatch.Begin();
            
            foreach (Dice die in GameObjManager.ObjList.OfType<Dice>()) 
                DrawDiceOutline(ShapeBatch, die);

            LockButton(ShapeBatch);
            PassButton(ShapeBatch);
            
            ShapeBatch.End(); 
            
            SpriteBatch.Begin();
            SpriteBatch.DrawString(_windowsXPFont, "Lock and Roll Again", new Vector2(490,440), Color.White);
            SpriteBatch.DrawString(_windowsXPFont, "Lock and Pass Round", new Vector2(490,570), Color.White);
            SpriteBatch.End();
            
            GameOverScreen(SpriteBatch, ShapeBatch);
            */
            base.Draw(gameTime);
            
        }

        private void LockButton(ShapeBatch _sb)
        {
            GameObject lockbutton = GameObjManager.DictGet("LockButton");
            if (lockbutton.Box.Contains(Input.Mouse.Position)) 
                _sb.DrawRectangle(lockbutton.Position, lockbutton.Size, Color.Gold, Color.White, 4f, 2f);
            else
                _sb.DrawRectangle(lockbutton.Position, lockbutton.Size, ColourInventory.DicePurple, Color.White, 4f, 2f);
        }
        private void PassButton(ShapeBatch _sb)
        {
            GameObject passbutton = GameObjManager.DictGet("PassButton");
            if (passbutton.Box.Contains(Input.Mouse.Position)) 
                _sb.DrawRectangle(passbutton.Position, passbutton.Size, Color.Gold, Color.White, 4f, 2f);
            else
                _sb.DrawRectangle(passbutton.Position, passbutton.Size, ColourInventory.DicePurple, Color.White, 4f, 2f);
        }

        private void ClickLockButton()
        {
            
            if (ScoringSystem.GetTempScore() > 0)
            {
                GM.NextRound();
                GameObjManager.RemoveLockedInDice();
                GameObjManager.RollDice();
            }
                
        }

        private void ClickPassButton()
        {
            GameObjManager.ResetRound(number_of_dice);
            GM.SkipTurn(); 
        }

        private void GameOverScreen(SpriteBatch spriteBatch, ShapeBatch sb)
        {
            if (!GameObjManager.ObjList.OfType<Dice>().Any(d => d.State == DieState.rolling))
                if (!ScoringSystem.PossibleScore())
                {
                    sb.Begin(); 
                    sb.DrawRectangle(new Vector2(100, 100), new Vector2(1000, 600), Color.Black, Color.White, 4f, 2f);
                    sb.End(); 
                    
                    spriteBatch.Begin();
                    spriteBatch.DrawString(_windowsXPFont, "FLOP", new Vector2(500, 300), Color.White);
                    SpriteBatch.DrawString(_windowsXPFont, $"Rolled a dead hand: {String.Join(",",GameObjManager.ActiveDiceValues)}", new Vector2(400, 425), Color.White);
                    spriteBatch.DrawString(_windowsXPFont, "Press N to go to next round", new Vector2(450, 550), Color.White);
                    spriteBatch.End();

                    GM.FlopTurn(); 
                }
        }
    }
    
}