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
    public GameManager GM = new GameManager();
    
    private int numberOfDice = 6;

    public Player PlayerOne;
    public Player PlayerTwo;
    
    public Panel p1tray;
    public Panel p2tray;

    public Label p1HandScoreText;
    public Label p1RoundScoreText;
    public Label p2HandScoreText;
    public Label p2RoundScoreText;

    private Color white = Core.Colours.Paint("white");
    private Color maroon = Core.Colours.Paint("maroon");
    private Color hot_pink = Core.Colours.Paint("hot pink");
    private Color barbie_pink = Core.Colours.Paint("barbie pink");
    private Color perfect_pink = Core.Colours.Paint("perfect pink");
    private Color dusty_pink = Core.Colours.Paint("dusty pink");
    private Color light_pink = Core.Colours.Paint("light pink");

    private List<Dice> _allActiveDice; 
    
    public override void Initialize()
    {
        
        base.Initialize();
        Core.ExitOnEscape = true; 
        

        
    }

    public List<Dice> SceneActiveDice()
    {
        List<Dice> result = new List<Dice>();
        foreach (Player player in GM.Players)
        {
            foreach (Dice dice in player.DiceList)
            {
                if (dice.State != DieState.played)
                {
                    result.Add(dice);
                }
            }
        }

        return result; 
    } 
    

    public override void LoadContent()
    {
        
        PlayerOne = new Player("Player 1",1);
        PlayerTwo = new Player("Player 2",2);
        GM.Players.Add(PlayerOne);
        GM.Players.Add(PlayerTwo);
        
        // === P1 BUTTONS ===
        var rollButton1 = new Button
        {
            Anchor = Anchor.BottomLeft,
            Offset = new Vector2(0, 57),
            Text = "Roll",
            TextColour = dusty_pink,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = maroon,
            BorderColour = perfect_pink,
            CornerRadius = 6f,
            OnClick = () => PlayerOne.RollDice()
        };
        rollButton1.AutoSize();


        var passButton1 = new Button
        {
            Anchor = Anchor.BottomCentre,
            Offset = new Vector2(0, 57), 
            Text = "Pass",
            TextColour = dusty_pink,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = maroon,
            BorderColour = perfect_pink,
            CornerRadius = 6f,
            OnClick = () => Console.WriteLine("Round passed")
        };
        passButton1.AutoSize();

        var lockButton1 = new Button
        {
            Anchor = Anchor.BottomRight,
            Offset = new Vector2(0, 57),
            Text = "Lock",
            TextColour = dusty_pink,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(44, 12),
            FillColour = maroon,
            BorderColour = perfect_pink,
            CornerRadius = 6f,
            OnClick = () =>
            {
                GM.AddRoundScore(PlayerOne);
                PlayerOne.RemoveHeldDice();
            }, 
        };
        lockButton1.AutoSize();
        
        
        
        passButton1.MatchWidth(lockButton1);
        rollButton1.MatchWidth(lockButton1);
        
        // === P2 BUTTONS ===
        var rollButton2 = new Button
        {
            Anchor = Anchor.BottomLeft,
            Offset = new Vector2(0, 57),
            Text = "Roll",
            TextColour = dusty_pink,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = maroon,
            BorderColour = perfect_pink,
            CornerRadius = 6f,
            OnClick = () => PlayerTwo.RollDice()
        };
        rollButton2.AutoSize();


        var passButton2 = new Button
        {
            Anchor = Anchor.BottomCentre,
            Offset = new Vector2(0, 57), 
            Text = "Pass",
            TextColour = dusty_pink,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(20, 12),
            FillColour = maroon,
            BorderColour = perfect_pink,
            CornerRadius = 6f,
            OnClick = () => Console.WriteLine("Round passed")
        };
        passButton2.AutoSize();
        

        var lockButton2 = new Button
        {
            Anchor = Anchor.BottomRight,
            Offset = new Vector2(0, 57),
            Text = "Lock",
            TextColour = dusty_pink,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            Padding = new Vector2(44, 12),
            FillColour = maroon,
            BorderColour = perfect_pink,
            CornerRadius = 6f,
            OnClick = () => GM.AddRoundScore(PlayerTwo)
        };
        lockButton2.AutoSize();
        
        passButton2.MatchWidth(lockButton2);
        rollButton2.MatchWidth(lockButton2);

        
        // === ROLLING TRAYS 
        p1tray = new Panel
        {
            Anchor = Anchor.MiddleCentre,
            Size = new Vector2(410, 410),
            Offset = new Vector2(-220, 20),
            FillColour = maroon,
            BorderColour = perfect_pink,
            BorderThickness = 4f,
            CornerRadius = 10f,
            OnClick = () => PlayerOne.StopDice()
        };
        p1tray.AddChild(rollButton1);
        p1tray.AddChild(passButton1);
        p1tray.AddChild(lockButton1);

        _buttons.Add(rollButton1);
        _buttons.Add(passButton1);
        _buttons.Add(lockButton1);

        p2tray = new Panel
        {
            Anchor = Anchor.MiddleCentre,
            Size = new Vector2(410, 410),
            Offset = new Vector2(220, 20),
            FillColour = maroon,
            BorderColour = perfect_pink,
            BorderThickness = 2f,
            OnClick = () => PlayerTwo.StopDice()
        };
        p2tray.AddChild(rollButton2);
        p2tray.AddChild(passButton2);
        p2tray.AddChild(lockButton2);

        _buttons.Add(rollButton2);
        _buttons.Add(passButton2);
        _buttons.Add(lockButton2);
        
        // === ROUND SCORE AND SCORED HAND BUTTONS ===
        var p1HandScoreBubble = new Panel
        {
            Anchor = Anchor.TopLeft,
            Size = new Vector2(204, 55),
            Offset = new Vector2(0, -60),
            FillColour = perfect_pink,
            BorderColour = maroon,
            BorderThickness = 4f,
            CornerRadius = 10f,
        };
        p1tray.AddChild(p1HandScoreBubble);
         p1HandScoreText = new Label
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, 4),
            Text = GM.ScoreText(PlayerOne, ScoreType.Hand),
            Font = Core.Content.Load<SpriteFont>("Fonts/digital36"),
            TextColour = Color.White,
        };
        p1HandScoreBubble.AddChild(p1HandScoreText);
        var p1RoundScoreBubble = new Panel
        {
            Anchor = Anchor.TopRight,
            Size = new Vector2(204, 55),
            Offset = new Vector2(0,-60),
            FillColour = barbie_pink,
            BorderColour = maroon,
            BorderThickness = 4f,
            CornerRadius = 10f,
        };
        p1tray.AddChild(p1RoundScoreBubble);
        p1RoundScoreText = new Label
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, 4),
            Text = PlayerOne.RoundScoreText,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital36"),
            TextColour = Color.White,
        };
        p1RoundScoreBubble.AddChild(p1RoundScoreText);

        
        //PlAYER 2
        var p2HandScoreBubble = new Panel
        {
            Anchor = Anchor.TopLeft,
            Size = new Vector2(204, 55),
            Offset = new Vector2(0, -60),
            FillColour = perfect_pink,
            BorderColour = maroon,
            BorderThickness = 4f,
            CornerRadius = 10f,
        };
        p2tray.AddChild(p2HandScoreBubble);
        p2HandScoreText = new Label
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, 4),
            Text = PlayerOne.HandScoreText,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital36"),
            TextColour = Color.White,
        };
        p2HandScoreBubble.AddChild(p2HandScoreText);
        var p2RoundScoreBubble = new Panel
        {
            Anchor = Anchor.TopRight,
            Size = new Vector2(204, 55),
            Offset = new Vector2(0,-60),
            FillColour = barbie_pink,
            BorderColour = maroon,
            BorderThickness = 4f,
            CornerRadius = 10f,
        };
        p2tray.AddChild(p2RoundScoreBubble);
        p2RoundScoreText = new Label
        {
            Anchor = Anchor.MiddleCentre,
            Offset = new Vector2(0, 4),
            Text = PlayerTwo.RoundScoreText,
            Font = Core.Content.Load<SpriteFont>("Fonts/digital36"),
            TextColour = Color.White,
        };
        p2RoundScoreBubble.AddChild(p2RoundScoreText);
        
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

        _allActiveDice = SceneActiveDice();
        
        foreach (var btn in _buttons)
            btn.Update(screen);

        p1tray.Update(screen); 
        p2tray.Update(screen);
        GM.Update(gameTime);
        
        p1HandScoreText.Text = GM.ScoreText(PlayerOne, ScoreType.Hand);
        p1RoundScoreText.Text = GM.ScoreText(PlayerOne, ScoreType.Round);
        p2HandScoreText.Text = GM.ScoreText(PlayerTwo, ScoreType.Hand);
        p2RoundScoreText.Text = GM.ScoreText(PlayerTwo, ScoreType.Round);
        

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
        Core.GraphicsDevice.Clear(Core.Colours.Paint("light pink"));
        
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

        foreach (Dice die in _allActiveDice)
        {
            if (die.Visible)
            {
                Core.SpriteManager.Draw(_spriteBatch, die.TextureKey, die, die.Rotation);
            }
        }
        
        
        _spriteBatch.End();
        
        
        /// === Have to call shapebatch again so that dice outlines are drawn on top of dice ===
        _shapeBatch.Begin();

        foreach (Dice die in _allActiveDice )
        {
            DrawDiceOutline(_shapeBatch, die);
        }
        


        _shapeBatch.End();
        
        
        base.Draw(gameTime);
    }
    public void DrawDiceOutline(ShapeBatch shapeBatch, Dice die)
    {
        if (die.State == DieState.held)
        {
            DrawOutline(die, Color.Gold, 2f, 2f, 5f, die.Rotation);
                    
        }
        if (IsMouseOverDice(die) && die.State != DieState.held)
        {
            DrawOutline(die, Color.White, 2f, 2f, 5f, die.Rotation);

        }

        void DrawOutline(Dice die, Color colour, float thickness, float offset, float rounded, float rotation)
        {
            Vector2 pos = die.Position - new Vector2(offset, offset);
            Vector2 size = die.Size + new Vector2(offset, offset);
            shapeBatch.BorderRectangle(pos, size, colour, thickness, rounded, rotation);
        }
            

    }
    private bool IsMouseOverDice(Dice die)
    {
        if (die.Box.Contains(Core.Input.Mouse.Position))
            return true;
            
        return false; 
    }

    
}

