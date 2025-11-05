using Microsoft.Xna.Framework;

namespace ChickenRun;

public class GameObject
{
    public static int SizeMod { get; private set; } = 4;
    public Atlas atlas { get; protected set; }
    public Vector2 position { get; protected set; }
    public Vector2 size { get; protected set; }
    public int frame { get; protected set; } = 0;
    public Rectangle rectangle
    {
        get
        {
            return new Rectangle
            (
                (int)position.X,
                (int)position.Y,
                (int)size.X * SizeMod,
                (int)size.Y * SizeMod
            );
        }
    }

    public GameObject(Atlas atlas, Vector2 position, Vector2 size)
    {
        this.atlas = atlas;
        this.position = position;
        this.size = size;
    }
}