using System; 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DiceMG;
using DiceMG.Input;
using DiceMG.Scenes;
using Apos.Shapes;
using DiceMG.UI; 
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DiceMG.Scenes;

public class GameScene : Scene

{
    
    private List<UIElement> _ui = new();
    private List<Button> _buttons = new();
    private List<Label> _labels = new(); 
    private SpriteBatch _spriteBatch = Core.SpriteBatch;
    private ShapeBatch _shapeBatch = Core.ShapeBatch;
    private ObjectManager ObjManager = new ObjectManager();
    private ObjPhysics Physics = new ObjPhysics();
    public GameManager GM = Core.GM; 
    
    private int numberOfDice = 6;

    public Player PlayerOne;
    public Player PlayerTwo;
    
    public override void Initialize()
    {
        
        base.Initialize();
        Core.ExitOnEscape = true; 
        

        
    }
    

    public override void LoadContent()
    {
        PlayerOne = new Player("Player 1",1);
        PlayerTwo = new Player("Player 2",2);
        GM.Players.Add(PlayerOne);
        GM.Players.Add(PlayerTwo);
        
        // === CENTER BUTTONS ===
        var rollButton = new Button
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, 30),
            Text = "Roll Dice",
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = new Color(50, 50, 50),
            BorderColour = Color.Gray,
            CornerRadius = 6f,
            OnClick = () => PlayerOne.RollDice()
        };
        rollButton.AutoSize();


        var passButton = new Button
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, -30), // Position above roll button
            Text = "Pass",
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = new Color(50, 50, 50),
            BorderColour = Color.Gray,
            CornerRadius = 6f,
            OnClick = () => Console.WriteLine("Round passed")
        };
        passButton.AutoSize();
        

        var lockButton = new Button
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, 90),
            Text = "Lock Hand",
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = new Color(50, 50, 50),
            BorderColour = Color.Gray,
            CornerRadius = 6f,
            OnClick = () => Console.WriteLine("Hand locked")
        };
        lockButton.AutoSize();
        
        passButton.MatchWidth(lockButton);
        rollButton.MatchWidth(lockButton);
        
        _buttons.Add(lockButton);
        _ui.Add(lockButton);
        _buttons.Add(rollButton);
        _ui.Add(rollButton);
        _buttons.Add(passButton);
        _ui.Add(passButton);

        Button.SpaceApart(_buttons, new Vector2 (0, 4));
        
        // === ROLLING TRAYS 
        var p1tray = new Panel
        {
            Anchor = Anchor.MiddleCentre,
            Size = new Vector2(300, 340),
            Offset = new Vector2(-240, 20),
            FillColour = new Color(100, 100, 100),
            BorderColour = Color.Gray,
            BorderThickness = 2f,
            CornerRadius = 10f,
        };

        var p2tray = new Panel
        {
            Anchor = Anchor.MiddleCentre,
            Size = new Vector2(300, 340),
            Offset = new Vector2(240, 20),
            FillColour = new Color(100, 100, 100),
            BorderColour = Color.Gray,
            BorderThickness = 2f,
        };
        
        // === SCORE AND SCORED HAND BUTTONS ===
        var p1ScoreDisplay = new Panel
        {
            Anchor = Anchor.TopLeft,
            Size = new Vector2(300, 50),
            Offset = new Vector2(0, -60),
            FillColour = new Color(100, 100, 100),
            BorderColour = Color.White,
            BorderThickness = 2f,
            CornerRadius = 10f,
        };
        p1tray.AddChild(p1ScoreDisplay);
        var p1ScoreDisplayText = new Label
        {
            Anchor = Anchor.MiddleLeft,
            Offset = new Vector2(50, 0),
            Text = "Held Dice: 2000",
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            TextColour = Color.White,
        };
        p1ScoreDisplay.AddChild(p1ScoreDisplayText);
        
        _ui.Add(p1ScoreDisplay);
        _ui.Add(p1tray);
        _ui.Add(p2tray);
        
        PlayerOne.Tray = p1tray;
        PlayerTwo.Tray = p2tray;
        
        // === SPAWN DICE IN TRAY ===

        
        // Dice dimensions (adjust these to match your dice sprite size)
        float dw = 55; // dice width
        float dh = 55; // dice height
        
        // Calculate spawn bounds with padding
    
        
        // Spawn dice at random positions within bounds
        Random random = new Random();


        foreach (Player player in GM.Players)
        {
            var playerString = "P" + player.PNumber;
            var trayBounds = player.Tray.GetBounds(Core.GraphicsDevice.Viewport.Bounds);
            
            int spawnXLower = trayBounds.Left + (int)Global.TRAY_PADDING;
            int spawnXUpper = trayBounds.Right - (int)dw - (int)Global.TRAY_PADDING;
            int spawnYLower = trayBounds.Top + (int)Global.TRAY_PADDING;
            int diceYUpper = trayBounds.Bottom - (int)dh - (int)Global.TRAY_PADDING;
           
            for (int i = 0; i<numberOfDice ; i++) // spawn n dice, add to player's dice set and object manager
            {
                int x = random.Next(spawnXLower, spawnXUpper);
                int y = random.Next(spawnYLower, diceYUpper);
            
                Dice die = new Dice(dw, dh, new Vector2(x, y));
                ObjManager.BirthObject(die, playerString + "D" + i);
                player.AddDice(die, "D" + i);
            }
        }

        
        
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        var screen = Core.GraphicsDevice.Viewport.Bounds;
        
        foreach (var btn in _buttons)
            btn.Update(screen);
        

        foreach (Player player in GM.Players)
        {
            player.Update(gameTime);
            for (int i = 0; i < player.ActiveDice.Count; i++)
            {
                Physics.CheckTrayBounds(player.ActiveDice[i], player.Tray);
                for (int j = i + 1; j < player.ActiveDice.Count; j++)
                {
                    Physics.CircleCollision(player.ActiveDice[i], player.ActiveDice[j]);
                }
            }
        }

            
        base.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(20, 20, 25));
        
        var screen = Core.GraphicsDevice.Viewport.Bounds;
        
        // Draw shapes first
        _shapeBatch.Begin();
        foreach (var element in _ui)
            element.Draw(_shapeBatch, null, screen);
        _shapeBatch.End();
        
        // Then draw text/images on top
        _spriteBatch.Begin();
        foreach (var element in _ui)
            element.Draw(null, _spriteBatch, screen);
        //then draw dice
        foreach (Dice die in ObjManager.ObjList.OfType<Dice>()) 
        {
            if (die.Visible)
                Core.SpriteManager.Draw(_spriteBatch, die.TextureKey, die, die.Rotation);
        }
        _spriteBatch.End();
        
        
        base.Draw(gameTime);
    }
}

