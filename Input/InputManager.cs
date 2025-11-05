using Microsoft.Xna.Framework;

namespace DiceMG.Input;

public class InputManager
{
    public KeyboardInput Keyboard { get; set; }
    public MouseInput Mouse { get; set; }

    public InputManager()
    {
        Keyboard = new KeyboardInput();
        Mouse = new MouseInput();
    }

    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();
    }
}