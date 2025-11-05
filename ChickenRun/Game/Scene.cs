using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChickenRun;

// Base class for scenes
public abstract class Scene
{
    protected ContentManager Content;
    protected GraphicsDeviceManager graphics;
    protected Game1 game;

    public Scene(GraphicsDeviceManager graphics, Game1 game)
    {
        this.graphics = graphics;
        this.game = game;
    }

    public abstract void Load(ContentManager Content, string ContentRootDirectory);
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}