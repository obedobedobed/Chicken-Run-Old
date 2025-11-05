using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChickenRun;

public class GameBackground
{
    private Texture2D texture;
    private Vector2 position;
    private const float SPEED = 2;
    private const int SPEED_MOD = 25;
    private Rectangle rectangle
    {
        get
        {
            return new Rectangle (-(int)position.X, (int)position.Y, 4608, 640);
        }
    }

    public GameBackground(Texture2D texture)
    {
        this.texture = texture;
    }

    public void Update(GameTime gameTime)
    {
        if (-position.X < -(rectangle.Width / 2)) position.X = 0;
        position.X += SPEED * SPEED_MOD * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(texture, rectangle, Color.White);

        spriteBatch.End();
    }
}