using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ChickenRun;

public class MainMenuScene : Scene
{
    // Scene objects
    private Texture2D logo;
    public Button[] buttons { get; private set; } = new Button[7];
    private bool[] completedMaps;

    public MainMenuScene(GraphicsDeviceManager graphics, Game1 game) : base(graphics, game) { }

    public void Initialize(bool[] completedMaps)
    {
        // Assigning completed maps array
        this.completedMaps = completedMaps;
    }

    public override void Load(ContentManager Content, string ContentRootDirectory)
    {
        // Loading logo
        logo = Content.Load<Texture2D>("Sprites/Logo");

        // Loading buttons atlases
        var buttonsAtlasTexture = Content.Load<Texture2D>("Sprites/ButtonAtlas");
        var buttonsAtlas = new Atlas
        (
            texture: buttonsAtlasTexture,
            rectangleSize: new Vector2(24, 24)
        );

        var buttonsIconsAtlasTexture = Content.Load<Texture2D>("Sprites/ButtonSpriteAtlas");
        var buttonsIconsAtlas = new Atlas
        (
            texture: buttonsIconsAtlasTexture,
            rectangleSize: new Vector2(24, 24)
        );

        // Creating buttons
        int buttonXPos = 280;

        for (int i = 0; i < buttons.Length - 2; i++)
        {
            int icon = completedMaps[i] ? 2 : 3;
            
            buttons[i] = new SceneButton
            (
                atlas: buttonsAtlas,
                position: new Vector2(buttonXPos, 400),
                size: new Vector2(24, 24),
                sceneToLoad: new GameScene(graphics, game),
                sceneParameters: i + 1,
                buttonIcons: buttonsIconsAtlas, icon: icon,
                game: game
            );

            buttonXPos += buttons[i].rectangle.Width + 20;
        }

        buttons[5] = new ToggleButton
        (
            atlas: buttonsAtlas,
            position: new Vector2(20, graphics.PreferredBackBufferHeight - 120),
            size: new Vector2(24, 24), enabled: game.vSyncEnabled,
            buttonIcons: buttonsIconsAtlas,
            enabledIcon: 4, disabledIcon: 5,
            game: game
        );

        buttons[6] = new ToggleButton
        (
            atlas: buttonsAtlas,
            position: new Vector2
            (graphics.PreferredBackBufferWidth - 20 - 24 * GameObject.SizeMod,
            graphics.PreferredBackBufferHeight - 120),
            size: new Vector2(24, 24), enabled: false,
            buttonIcons: buttonsIconsAtlas,
            enabledIcon: 1, disabledIcon: 1,
            game: game
        );
    }
    
    public override void Update(GameTime gameTime)
    {
        foreach (Button button in buttons)
        {
            button.Update();
        }
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Drawing logo
        spriteBatch.Draw
        (
            logo, new Rectangle
            (graphics.PreferredBackBufferWidth / 2 - 310, 100,
            logo.Width * GameObject.SizeMod, logo.Height * GameObject.SizeMod), Color.White
        );

        // Drawing buttons
        foreach (Button button in buttons)
        {
            spriteBatch.Draw
            (
                button.atlas.texture, button.rectangle,
                button.atlas.textureRectangles[button.frame],
                Color.White
            );

            spriteBatch.Draw
            (
                button.buttonIcons.texture, button.rectangle,
                button.buttonIcons.textureRectangles[button.icon],
                Color.White
            );
        }

        spriteBatch.End();
    }
}