using Microsoft.Xna.Framework;
using DiceMG.Scenes;

namespace DiceMG;

public class DiceyDivas : Core
{
    public DiceyDivas() : base("Dicey Divas", 1280, 720, false)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();
        Global.ScreenHeight = GraphicsDevice.Viewport.Height;
        Global.ScreenWidth = GraphicsDevice.Viewport.Width;

        SceneManager.ChangeScene(new GameScene());
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}
