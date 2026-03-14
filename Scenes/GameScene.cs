using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Apos.Shapes;
using DiceMG;
using DiceMG.Input;
using DiceMG.Scenes;
using DiceMG.UI;
using System.Collections.Generic;
using System.Linq;

namespace DiceMG.Scenes;

public class GameScene : Scene
{
    public static int numberOfDice = 6;

    private List<UIElement> _ui = new();
    private SpriteBatch _spriteBatch = Core.SpriteBatch;
    private ShapeBatch  _shapeBatch  = Core.ShapeBatch;

    public static ObjectManager ObjManager = new ObjectManager();
    private ObjPhysics Physics = new ObjPhysics();
    public GameManager GM = new GameManager(numberOfDice, ObjManager);

    private GameState SceneState = GameState.NewGame;

    public Player PlayerOne;
    public Player PlayerTwo;

    private PlayerHUD _p1Hud;
    private PlayerHUD _p2Hud;

    // Scene-level labels
    public  Label GameStateLabel;
    private Label _playerTurnLabel;
    private Label _bustLabel;

    private const float BUST_DISPLAY_TIME = 2.5f;
    private float _bustTimer = 0f;

    private List<Dice> _allActiveDice;

    // Colour palette
    private Color hot_pink    = Core.Colours.Paint("hot pink");
    private Color barbie_pink = Core.Colours.Paint("barbie pink");
    private Color light_pink  = Core.Colours.Paint("light pink");

    // ── Lifecycle ─────────────────────────────────────────────────────

    public override void Initialize()
    {
        base.Initialize();
        Core.ExitOnEscape = true;
    }

    public override void LoadContent()
    {
        Subscribe(GM);

        PlayerOne = new Player("Player 1", 1);
        PlayerTwo = new Player("Player 2", 2);
        GM.AddPlayer(PlayerOne);
        GM.AddPlayer(PlayerTwo);

        // ── Player HUDs ───────────────────────────────────────────────
        _p1Hud = new PlayerHUD(
            player:        PlayerOne,
            gm:            GM,
            numberOfDice:  numberOfDice,
            objManager:    ObjManager,
            trayOffset:    new Vector2(-220, 20),
            barFillColour: hot_pink,
            isLeftSide:    true);

        _p2Hud = new PlayerHUD(
            player:        PlayerTwo,
            gm:            GM,
            numberOfDice:  numberOfDice,
            objManager:    ObjManager,
            trayOffset:    new Vector2(220, 20),
            barFillColour: barbie_pink,
            isLeftSide:    false);

        PlayerOne.Tray = _p1Hud.Tray;
        PlayerTwo.Tray = _p2Hud.Tray;
        PlayerOne.PlayerPanel = _p1Hud.PlayerPanel;
        PlayerTwo.PlayerPanel = _p2Hud.PlayerPanel;

        _ui.Add(_p1Hud.Tray);
        _ui.Add(_p2Hud.Tray);
        _ui.Add(_p1Hud.PlayerPanel);
        _ui.Add(_p2Hud.PlayerPanel);
        _ui.Add(_p1Hud.TotalScoreLabel);
        _ui.Add(_p2Hud.TotalScoreLabel);

        // ── Scene-level labels ────────────────────────────────────────
        SceneState = GM.State;
        GameStateLabel = new Label
        {
            Anchor     = Anchor.TopLeft,
            Offset     = new Vector2(8, 8),
            Text       = GM.State.ToString(),
            Font       = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            TextColour = Color.White,
        };
        _ui.Add(GameStateLabel);

        _playerTurnLabel = new Label
        {
            Anchor     = Anchor.TopCentre,
            Offset     = new Vector2(0, 20),
            Size       = new Vector2(300, 60),
            Font       = Core.Content.Load<SpriteFont>("Fonts/digital36"),
            TextColour = Color.White,
            IsVisible  = true,
        };
        _ui.Add(_playerTurnLabel);

        _bustLabel = new Label
        {
            Anchor     = Anchor.MiddleCentre,
            Offset     = new Vector2(0, 0),
            Text       = "BUST!",
            Font       = Core.Content.Load<SpriteFont>("Fonts/digital88"),
            TextColour = Color.Red,
            IsVisible  = false,
            CentreText = true,
        };
        _ui.Add(_bustLabel);

        // ── Spawn dice and start ──────────────────────────────────────
        foreach (Player player in GM.Players)
            player.CreateDiceList(numberOfDice, ObjManager);

        GM.StartGame();

        base.LoadContent();
    }

    // ── Update ────────────────────────────────────────────────────────

    public override void Update(GameTime gameTime)
    {
        var screen = Core.GraphicsDevice.Viewport.Bounds;

        _allActiveDice = SceneActiveDice();

        _p1Hud.Update(screen);
        _p2Hud.Update(screen);

        _p1Hud.UpdateButtonStates(GM.ActivePlayer == PlayerOne, GM.State);
        _p2Hud.UpdateButtonStates(GM.ActivePlayer == PlayerTwo, GM.State);

        GM.Update(gameTime);

        // Once all dice have stopped rolling, validate the roll
        if (GM.State == GameState.RollComplete)
        {
            bool allStopped = GM.ActivePlayer.ActiveDice.Count > 0
                && GM.ActivePlayer.ActiveDice.All(d => d.State != DieState.rolling);
            if (allStopped)
                GM.CheckRollValidity();
        }

        // Show BUST! for a moment then switch players
        if (GM.State == GameState.BustedRoll)
        {
            _bustTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_bustTimer <= 0f)
            {
                GM.BustedAndSwitchPlayer();
                GM.ProcessUntilWaiting();
            }
        }

        _bustLabel.IsVisible = GM.State == GameState.BustedRoll;

        _p1Hud.UpdateScoreText(GM);
        _p2Hud.UpdateScoreText(GM);

        _playerTurnLabel.Text      = GM.ActivePlayer.Name;
        _playerTurnLabel.IsVisible = GM.State == GameState.RoundStart;

        SceneState          = GM.State;
        GameStateLabel.Text = SceneState.ToString();

        foreach (Player player in GM.Players)
        {
            player.Update(gameTime);
            for (int i = 0; i < player.ActiveDice.Count; i++)
            {
                Physics.CheckTrayBounds(player.ActiveDice[i], player.Tray);
                for (int j = i + 1; j < player.ActiveDice.Count; j++)
                    Physics.CircleCollision(player.ActiveDice[i], player.ActiveDice[j]);
            }

            bool isActive = player == GM.ActivePlayer;
            player.DiceList.ForEach(d => d.Visible = isActive);
        }

        base.Update(gameTime);
    }

    // ── Draw ──────────────────────────────────────────────────────────

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(light_pink);

        var screen = Core.GraphicsDevice.Viewport.Bounds;

        // Shapes pass — panels, score bars
        _shapeBatch.Begin();
        foreach (var element in _ui)
            element.Draw(_shapeBatch, null, screen);
        _p1Hud.DrawScoreBar(_shapeBatch, screen, GM.GoalScore);
        _p2Hud.DrawScoreBar(_shapeBatch, screen, GM.GoalScore);
        _shapeBatch.End();

        // Text + sprite pass — labels, dice sprites
        _spriteBatch.Begin();
        foreach (var element in _ui)
            element.Draw(null, _spriteBatch, screen);
        foreach (Dice die in _allActiveDice)
            if (die.Visible)
                Core.SpriteManager.Draw(_spriteBatch, die.TextureKey, die, die.Rotation);
        _spriteBatch.End();

        // Second shapes pass — dice outlines on top of sprites
        _shapeBatch.Begin();
        foreach (Dice die in _allActiveDice)
            DrawDiceOutline(_shapeBatch, die);
        _shapeBatch.End();

        base.Draw(gameTime);
    }

    // ── Helpers ───────────────────────────────────────────────────────

    public List<Dice> SceneActiveDice()
    {
        var result = new List<Dice>();
        foreach (Player player in GM.Players)
            foreach (Dice dice in player.DiceList)
                if (dice.State != DieState.played)
                    result.Add(dice);
        return result;
    }

    public void DrawDiceOutline(ShapeBatch sb, Dice die)
    {
        if (die.State == DieState.held)
            DrawOutline(die, Color.Gold, 2f, 2f, 5f, die.Rotation);
        else if (IsMouseOverDice(die))
            DrawOutline(die, Color.White, 2f, 2f, 5f, die.Rotation);

        static void DrawOutline(Dice die, Color colour, float thickness, float offset, float rounded, float rotation)
        {
            var pos  = die.Position - new Vector2(offset, offset);
            var size = die.Size     + new Vector2(offset, offset);
            Core.ShapeBatch.BorderRectangle(pos, size, colour, thickness, rounded, rotation);
        }
    }

    private bool IsMouseOverDice(Dice die) =>
        die.Box.Contains(Core.Input.Mouse.Position);

    // ── State subscription ────────────────────────────────────────────

    public void Subscribe(GameManager gm) =>
        gm.StateChangedHandler += HandleStateChange;

    public void SetState(GameState state) => SceneState = state;

    private void HandleStateChange(object sender, GameStateArgs e)
    {
        SetState(e.NewState);
        if (e.NewState == GameState.BustedRoll)
            _bustTimer = BUST_DISPLAY_TIME;
    }
}
