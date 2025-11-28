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

namespace DiceMG.Scenes;

public class GameScene : Scene

{
    
    private List<UIElement> _ui = new();
    private List<Button> _buttons = new();
    private SpriteBatch _spriteBatch = Core.SpriteBatch;
    private ShapeBatch _shapeBatch = Core.ShapeBatch;

    private List<String> _buttonText = new List<string> {"Button 1", "Button 2", "Button 3"};
    public override void Initialize()
    {
        
        base.Initialize();
        Core.ExitOnEscape = true; 
    }
    

    public override void LoadContent()
    {
        
        // === CENTER BUTTONS ===
        var rollButton = new Button
        {
            Anchor = Anchor.MiddleCentre,
            Text = "Roll Dice",
            Font = Core.Content.Load<SpriteFont>("Fonts/digital18"),
            AutoSize = true,
            Padding = new Vector2(20, 12),
            FillColour = new Color(50, 50, 50),
            BorderColour = Color.Gray,
            CornerRadius = 6f,
            OnClick = () => Console.WriteLine("Rolled!")
        };
        
        _buttons.Add(rollButton);
        _ui.Add(rollButton);
        
        base.LoadContent();
    }
    public override void Update(GameTime gameTime)
    {
        var screen = Core.GraphicsDevice.Viewport.Bounds;
        
        foreach (var btn in _buttons)
            btn.Update(screen);
        
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
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}

