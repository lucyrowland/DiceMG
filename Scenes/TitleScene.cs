using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System; 
using DiceMG.Input;
using DiceMG; 
using DiceMG.Scenes;
using Apos.Shapes;

namespace DiceMG.Scenes;

public class TitleScene : Scene
{
    private const string DICEY_TEXT = "Dicey";
    private const string DIVAS_TEXT = "Divas";
    private const string PRESS_ENTER_TEXT = "Press Enter to Start";
    private const string SCENE_CHANGE_TEXT = "Press 1 to change scene";
    
    private SpriteFont _font;
    private SpriteFont _font5x;

    private Vector2 _diceyTextPos; 
    private Vector2 _divasTextPos; 
    private Vector2 _pressEnterTextPos;
    private Vector2 _sceneChangeTextPos;
    
    private Vector2 _diceyTextSize;
    private Vector2 _divasTextSize; 
    private Vector2 _pressEnterTextSize;
    private Vector2 _sceneChangeTextSize;
    
    

    public override void Initialize()
    {
        base.Initialize();

        Core.ExitOnEscape = true; 
        
        _font = Content.Load<SpriteFont>("Fonts/File");
        _font5x = Content.Load<SpriteFont>("Fonts/5x");
        
        _diceyTextPos = new Vector2(440, 100);
        _diceyTextSize = _font5x.MeasureString(DICEY_TEXT) * 0.5f; 
        
        _divasTextPos = new Vector2(500, 207);
        _divasTextSize = _font5x.MeasureString(DIVAS_TEXT) * 0.5f; 
        
        _pressEnterTextPos = new Vector2(440, 520);
        _pressEnterTextSize = _font.MeasureString(PRESS_ENTER_TEXT) * 0.5f;
        
        _sceneChangeTextPos = new Vector2(405, 600);
        _sceneChangeTextSize = _font.MeasureString(PRESS_ENTER_TEXT) * 0.5f;
        
        
    }

    public override void LoadContent()
    {
        base.LoadContent();

    }

    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.KeyPressed(Keys.Enter)) Core.ChangeScene(new GameScene());
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.Pink);
        
        Core.SpriteBatch.Begin();
        
        
        Core.SpriteBatch.DrawString(_font5x, DICEY_TEXT, _diceyTextPos + new Vector2(4,4), Color.DeepPink*0.5f);
        Core.SpriteBatch.DrawString(_font5x, DICEY_TEXT, _diceyTextPos, Color.White);
        Core.SpriteBatch.DrawString(_font5x, DIVAS_TEXT, _divasTextPos + new Vector2(4,4), Color.DeepPink*0.5f);
        Core.SpriteBatch.DrawString(_font5x, DIVAS_TEXT, _divasTextPos, Color.White);
        
        Core.SpriteBatch.DrawString(_font, PRESS_ENTER_TEXT, _pressEnterTextPos, Color.White);
        Core.SpriteBatch.DrawString(_font, SCENE_CHANGE_TEXT, _sceneChangeTextPos, Color.White);
        
        Core.SpriteBatch.End();
    }
}