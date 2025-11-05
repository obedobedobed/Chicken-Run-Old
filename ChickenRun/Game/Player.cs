using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChickenRun;

public class Player : GameObject
{
    // Moving
    private float speed;
    private const float SPEED_MOD = 25f;

    // Gravity
    private const float GRAVITY = -9.81f;
    private const float GRAVITY_MOD = 30f;
    private bool isGrounded = false;
    private bool gravityDirectionDown = true;
    private bool pressedKeyLastFrame = false;

    // Animation
    private const float FRAME_TIME = 0.3f;
    private float timeToFrame = FRAME_TIME;
    public SpriteEffects flip;

    // Other
    private float elapsedGameTime = 0f;
    private bool playerLose = false;
    private readonly Block[,] blocks;
    private readonly Block[] finishBlocks;
    private Game1 game;
    private bool playerWin = false;

    public Player(Atlas atlas, Vector2 position, Vector2 size, float speed, Block[,] blocks,
    Block[] finishBlocks, Game1 game) : base(atlas, position, size)
    {
        this.speed = speed;
        this.blocks = blocks;
        this.finishBlocks = finishBlocks;
        this.game = game;
    }

    public void Update(GameTime gameTime)
    {
        elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Fixing bugs
        if (position.Y > game.GetGraphics.PreferredBackBufferHeight)
        {
            position = new Vector2(-50, 500);
        }

        // Updating components
        if (!playerLose && !playerWin)
        {
            GetInput();
            Move();
            Gravity();
            Animate();

            // Checking for intersects with finish
            foreach (Block block in finishBlocks)
            {
                if (block == null || !block.isFinish) continue;

                if (rectangle.Intersects(block.rectangle))
                {
                    flip = SpriteEffects.None;
                    playerWin = true;
                    (game.scene as GameScene)?.OnFinish();
                }
            }
        }
        else if (playerWin)
        {
            OnWin();
        }
    }

    private void GetInput()
    {
        // Getting keyboard state
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsAnyKeyPressed() && !pressedKeyLastFrame && isGrounded)
        {
            gravityDirectionDown = !gravityDirectionDown;
            pressedKeyLastFrame = true;
        }
        else if (keyboardState.IsAllKeysUnpressed())
        {
            pressedKeyLastFrame = false;
        }
    }

    private void Move()
    {
        // Calculating move
        float toMove = speed * SPEED_MOD * elapsedGameTime;

        // Applying move
        position += new Vector2(toMove, 0);

        // Checking collisions
        foreach (Block block in blocks)
        {
            if (block == null || block.isFinish) continue;

            if (rectangle.Intersects(block.IntersectionCollider))
            {
                position -= new Vector2(toMove, 0);
            }
        }
    }

    public float CalculateCameraMove()
    {
        // Calculating move
        float toMove = speed * SPEED_MOD * elapsedGameTime;

        // Returninh move
        return toMove;
    }

    private void Gravity()
    {
        // Reassigning isGrounded
        isGrounded = false;

        // Calculating gravity
        float toMove = gravityDirectionDown ? GRAVITY * GRAVITY_MOD * elapsedGameTime :
        GRAVITY * GRAVITY_MOD * elapsedGameTime * -1;

        // Applying gravity
        position -= new Vector2(0, toMove);

        // Checking for collisions
        foreach (Block block in blocks)
        {
            if (block == null || block.isFinish) continue;

            switch (gravityDirectionDown)
            {
                case true:
                    if (rectangle.Intersects(block.TopGroundCollider))
                    {
                        if (rectangle.Intersects(block.IntersectionCollider)) break;
                        isGrounded = true;
                        position = new Vector2(position.X, block.TopGroundCollider.Top - rectangle.Height);
                        toMove = 0;
                    }
                    break;
                case false:
                    if (rectangle.Intersects(block.DownGroundCollider))
                    {
                        if (rectangle.Intersects(block.IntersectionCollider)) break;
                        isGrounded = true;
                        position = new Vector2(position.X, block.DownGroundCollider.Bottom);
                        toMove = 0;
                    }
                    break;
            }
        }
    }

    private void Animate()
    {
        flip = gravityDirectionDown ? SpriteEffects.None : SpriteEffects.FlipVertically;

        if ((timeToFrame -= elapsedGameTime) <= 0)
        {
            switch (frame)
            {
                case 0: frame = 1; break;
                case 1: frame = 0; break;
            }

            timeToFrame = FRAME_TIME;
        }
    }

    public void Lose()
    {
        playerLose = true;
    }

    public void OnWin()
    {
        position = new Vector2(position.X, position.Y - speed * SPEED_MOD * elapsedGameTime);
    }
}