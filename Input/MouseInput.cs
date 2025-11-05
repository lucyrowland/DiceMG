using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace DiceMG.Input;

public enum MouseButton
{
    Left,
    Middle, 
    Right, 
    XButton1, 
    XButton2
}
public class MouseInput
{
    public MouseState State { get; set; }
    public MouseState PreviousState { get; set; }

    public Point Position
    {
        get => State.Position;
        set => SetPosition(value.X, value.Y);
    }

    public int x
    {
        get=> State.X;
        set => SetPosition(value, State.Y);
    }

    public int y
    {
        get => State.Y;
        set => SetPosition(State.X, value);
    }
    public Point PositionDelta => State.Position - PreviousState.Position;
    public int XDelta => State.X - PreviousState.X;
    public int YDelta => State.Y - PreviousState.Y;
    public bool Moved => PositionDelta != Point.Zero;
    
    public int ScrollWheel => State.ScrollWheelValue;
    public int ScrollWheelDelta => State.ScrollWheelValue - PreviousState.ScrollWheelValue;

    public MouseInput()
    {
        PreviousState = new MouseState();
        State = Mouse.GetState();
    }

    public void Update()
    {
        PreviousState = State;
        State = Mouse.GetState();
    }

    public bool IsButtonDown(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return State.LeftButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return State.MiddleButton == ButtonState.Pressed;
            case MouseButton.Right:
                return State.RightButton == ButtonState.Pressed;
            case MouseButton.XButton1:
                return State.XButton1 == ButtonState.Pressed;
            case MouseButton.XButton2:
                return State.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }

    public bool IsButtonUp(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return State.LeftButton == ButtonState.Released;
            case MouseButton.Middle:
                return State.MiddleButton == ButtonState.Released;
            case MouseButton.Right:
                return State.RightButton == ButtonState.Released;
            case MouseButton.XButton1:
                return State.XButton1 == ButtonState.Released;
            case MouseButton.XButton2:
                return State.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }

    public bool ButtonPressed(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return State.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;
            case MouseButton.Middle:
                return State.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
            case MouseButton.Right:
                return State.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;
            case MouseButton.XButton1:
                return State.XButton1 == ButtonState.Pressed && PreviousState.XButton1 == ButtonState.Released;
            case MouseButton.XButton2:
                return State.XButton2 == ButtonState.Pressed && PreviousState.XButton2 == ButtonState.Released;
            default:
                return false;
        }
    }

    public bool ButtonReleased(MouseButton button)
    {
        switch (button)
        {
            case MouseButton.Left:
                return State.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
            case MouseButton.Middle:
                return State.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;
            case MouseButton.Right:
                return State.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;
            case MouseButton.XButton1:
                return State.XButton1 == ButtonState.Released && PreviousState.XButton1 == ButtonState.Pressed;
            case MouseButton.XButton2:
                return State.XButton2 == ButtonState.Released && PreviousState.XButton2 == ButtonState.Pressed;
            default:
                return false;
        }
    }
    
    public void SetPosition(int x, int y)
    {
        Mouse.SetPosition(x, y);
        State = new MouseState(
            x,
            y,
            State.ScrollWheelValue,
            State.LeftButton,
            State.MiddleButton,
            State.RightButton,
            State.XButton1,
            State.XButton2
        );
    }
}