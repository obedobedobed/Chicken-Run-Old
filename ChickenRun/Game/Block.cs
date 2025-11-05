using Microsoft.Xna.Framework;

namespace ChickenRun;

public class Block : GameObject
{
    private const int GROUND_COLLIDER_HEIGHT = 5;
    public bool isFinish { get; private set; } = false;

    public Rectangle IntersectionCollider
    {
        get
        {
            return new Rectangle
            (
                (int)position.X,
                (int)position.Y + GROUND_COLLIDER_HEIGHT,
                (int)size.X * SizeMod,
                (int)(size.Y * SizeMod - GROUND_COLLIDER_HEIGHT * 2)
            );
        }
    }

    public Rectangle TopGroundCollider
    {
        get
        {
            return new Rectangle
            (
                (int)position.X + 1,
                (int)position.Y,
                (int)size.X * SizeMod - 2,
                GROUND_COLLIDER_HEIGHT * SizeMod
            );
        }
    }

    public Rectangle DownGroundCollider
    {
        get
        {
            return new Rectangle
            (
                (int)position.X + 1,
                (int)(position.Y + (size.Y * SizeMod) - GROUND_COLLIDER_HEIGHT * SizeMod),
                (int)size.X * SizeMod - 2,
                GROUND_COLLIDER_HEIGHT * SizeMod
            );
        }
    }

    public Block(Atlas atlas, Vector2 position, Vector2 size, int frame,
    bool isFinish = false) : base(atlas, position, size)
    {
        this.frame = frame;
        this.isFinish = isFinish;
    }
}