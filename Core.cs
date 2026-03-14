using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DiceMG.Input;
using Apos.Shapes;
using DiceMG.Scenes;
using MonoGameGum;

namespace DiceMG;

public class Core : Game
{
    internal static Core s_instance;

    public static Core Instance => s_instance;

    public static GraphicsDeviceManager Graphics { get; private set; }
    public static GraphicsDevice GraphicsDevice { get; private set; }

    public static ColourDirectory Colours { get; private set; }

    public static SpriteBatch SpriteBatch { get; private set; }
    public static SpriteManager SpriteManager { get; private set; }

    public static ContentManager Content { get; private set; }

    public static InputManager Input { get; private set; }

    public static ShapeBatch ShapeBatch { get; private set; }

    public static ObjectManager GameObjManager { get; private set; }

    public static GameManager GM { get; private set; }

    public static GumService GumUI { get; private set; }

    public static bool ExitOnEscape { get; set; }

    public Core(string title, int width, int height, bool fullscreen)
    {
        if (s_instance != null)
            throw new InvalidOperationException("Only a single Core instance can be created");

        s_instance = this;
        Content = base.Content;
        Content.RootDirectory = "Content";
        Window.Title = title;
        IsMouseVisible = true;
        ExitOnEscape = true;

        Graphics = new GraphicsDeviceManager(this);
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullscreen;
        Graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        SpriteManager = new SpriteManager(GraphicsDevice);
        ShapeBatch = new ShapeBatch(GraphicsDevice, Content);
        GameObjManager = new ObjectManager();
        Colours = new ColourDirectory();
        Input = new InputManager();
        GM = new GameManager();
        _ = new SceneManager(); // creates instance which subscribes to GM state changes

        base.Initialize();
    }

    protected override void LoadContent()
    {
        for (int i = 1; i <= 6; i++)
            SpriteManager.LoadTexture("DPurp" + i, Content.Load<Texture2D>("DPurp" + i));

        SpriteManager.LoadTexture("tray", Content.Load<Texture2D>("tray_sprite"));
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);
        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
            Exit();

        SceneManager.CurrentScene?.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        SceneManager.CurrentScene?.Draw(gameTime);
        base.Draw(gameTime);
    }
}
