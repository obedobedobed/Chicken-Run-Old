using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ChickenRun;

public abstract class Button : GameObject
{
    
    protected bool canBePressed = false;
    public Atlas buttonIcons { get; private set; }
    public int icon { get; protected set; }
    protected Game1 game;
    protected bool pressedLastFrame = false;

    public Button(Atlas atlas, Vector2 position, Vector2 size,
     Atlas buttonIcons, int icon, Game1 game) : base(atlas, position, size)
    {
        this.buttonIcons = buttonIcons;
        this.icon = icon;
        this.game = game;
    }

    public void Update()
    {
        // Getting cursor state
        var cursorState = Mouse.GetState();

        // Reassigning canBePressed
        canBePressed = false;

        // Creating cursor rectangle
        var cursorRectangle = new Rectangle
        (
            cursorState.X,
            cursorState.Y,
            1, 1
        );

        // Checking for cursor rectangle in button
        if (rectangle.Intersects(cursorRectangle))
        {
            frame = 1;
            canBePressed = true;
        }
        else frame = 0;

        // If pressed button
        if (cursorState.LeftButton == ButtonState.Pressed && canBePressed && !pressedLastFrame)
        {
            pressedLastFrame = true;
            OnPress();
        }
        // If released button
        else if (cursorState.LeftButton == ButtonState.Released)
        {
            pressedLastFrame = false;
        }
    }

    protected abstract void OnPress();
}