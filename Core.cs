using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DiceMG.Input;
using Apos.Shapes;
using DiceMG.Scenes;

namespace DiceMG;

public class Core : Game
{
    internal static Core s_instance;
    
    //reference to core instance
    public static Core Instance => s_instance;
    
    //scene management variables 
    private static Scene s_currentScene;
    private static Scene s_nextScene;
    
    //load in graphics device manager and graphics device
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static GraphicsDevice GraphicsDevice { get; private set; }
    
    //this will be the spritebatch for all 2d rendering
    public static SpriteBatch SpriteBatch { get; private set; }
    public static SpriteManager SpriteManager { get; private set; }
    
    //this will be used to load in assets from the content folder
    public static ContentManager Content { get; private set; }
    
    //load in homemade input manager made in the input folder 
    public static InputManager Input { get; private set; }
    
    public static ShapeBatch ShapeBatch { get; private set; }
    
    public static ObjectManager GameObjManager { get; private set; }
    
    public static bool ExitOnEscape { get; set; }
    
    //declare core instance with game title, game window dimensions, and full screen bool 
    public Core(string title, int width, int height, bool fullscreen)
    {
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }
        
        //store reference to engine for global access
        s_instance = this;
        Content = base.Content; 
        Content.RootDirectory = "Content";
        Window.Title = title;
        IsMouseVisible = true;
        ExitOnEscape = true;
            
        
        //change graphics setting based on core declaration
        Graphics = new GraphicsDeviceManager(this);
        Graphics.PreferredBackBufferWidth = width;
        Graphics.PreferredBackBufferHeight = height;
        Graphics.IsFullScreen = fullscreen;
        Graphics.ApplyChanges();
        
    }

    protected override void Initialize()
    {
        
        //set core graphics device to a reference of base games graphics device
        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        SpriteManager = new SpriteManager(GraphicsDevice);
        ShapeBatch = new ShapeBatch(GraphicsDevice, Content);
        GameObjManager = new ObjectManager();
        
        //create instances for spritebatch and input manager
        //SpriteBatch = new SpriteBatch(GraphicsDevice);
        Input = new InputManager();
        
        base.Initialize();  
    }

    protected override void LoadContent()
    {
        // Load all dice face textures (1-6)
        for (int i = 1; i <= 6; i++)
        {
            SpriteManager.LoadTexture("DPurp" + i, Content.Load<Texture2D>("DPurp" + i));
        }
        //load tray texture
        SpriteManager.LoadTexture("tray", Content.Load<Texture2D>("tray_sprite"));
        

    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update(gameTime);
        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        {
            Exit(); 
        }

        if (s_nextScene != null) TransitionScene(); 
        if (s_currentScene != null) s_currentScene.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        if (s_currentScene != null) s_currentScene.Draw(gameTime);
        base.Draw(gameTime);
    }

    public static void ChangeScene(Scene scene)
    {
        if(s_currentScene != null) s_nextScene = scene;
    }

    public static void TransitionScene()
    {
        if (s_currentScene != null) s_currentScene.Dispose();
        GC.Collect(); 
        
        s_currentScene = s_nextScene;
        s_nextScene = null;
        
        if(s_currentScene != null) s_currentScene.Initialize();
    }
}