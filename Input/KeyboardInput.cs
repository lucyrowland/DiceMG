using Microsoft.Xna.Framework.Input;

namespace DiceMG.Input;

public class KeyboardInput
{
    public KeyboardState State { get; set; }
    public KeyboardState PreviousState { get; set; }

    public KeyboardInput()
    {
        PreviousState = new KeyboardState();
        State = Keyboard.GetState();
    }
    public void Update()
    {
        PreviousState = State;
        State = Keyboard.GetState();
    }
    public bool IsKeyDown(Keys key)
    {
        return State.IsKeyDown(key);
    }

    public bool IsKeyUp(Keys key)
    {
        return State.IsKeyUp(key);
    }
    public bool KeyPressed(Keys key)
    {
        return State.IsKeyDown(key) && PreviousState.IsKeyUp(key);
    }
    public bool KeyReleased(Keys key)
    {
        return State.IsKeyUp(key) && PreviousState.IsKeyDown(key);
    }
}