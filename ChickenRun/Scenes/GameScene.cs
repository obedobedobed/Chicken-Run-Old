using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;

namespace ChickenRun;

public class GameScene : Scene
{
    // Game
    private int map;
    private bool gameWin = false;
    private bool gameOver = false;
    private bool pause = false; 
    private const float EXIT_AFTER_WIN_TIME = 3.5f;
    private float timeToExitAfterWin = EXIT_AFTER_WIN_TIME;

    // Audio
    private Song song;
    private SoundEffectInstance gameWinSound;
    private SoundEffectInstance gameOverSound;
    private bool playedGameWin = false;
    private bool playedGameOver = false;

    // Text
    private SpriteFont arial;
    private const string GAME_OVER_TEXT = "Game Over!";
    private const string PAUSE_TEXT = "Pause";

    // Scene objects
    private OrthographicCamera camera;
    private Vector2 cameraPosition;
    private Player player;
    private Block[,] blocks;
    public List<Block> finishBlocks { get; private set; } = new List<Block>();
    private MapLoader mapLoader;
    private Texture2D menuBackground;
    private SceneButton exitButton;
    private SceneButton restartButton;
    private ToggleButton continueButton;

    // Cup
    private Texture2D cupTexture;
    private Texture2D cupBgTexture;
    private float cupBgRotation = 0f;

    public GameScene(GraphicsDeviceManager graphics, Game1 game) : base (graphics, game) { }

    public void Initialize(int map)
    {
        // Assigning map variable
        this.map = map;

        // Creating camera
        camera = new OrthographicCamera(graphics.GraphicsDevice);
        cameraPosition = new Vector2
        (
            graphics.PreferredBackBufferWidth / 2,
            graphics.PreferredBackBufferHeight / 2
        );
    }

    public override void Load(ContentManager Content, string ContentRootDirectory)
    {
        // Loading blocks atlas
        var blocksAtlasTexture = Content.Load<Texture2D>("Sprites/BlocksAtlas");
        var finishAtlasTexture = Content.Load<Texture2D>("Sprites/FinishAtlas");

        // Creating map loader
        mapLoader = new MapLoader(blocksAtlasTexture, finishAtlasTexture, new Vector2(16, 16));
        blocks = mapLoader.LoadMap($"Content/Maps/map{map}.txt");

        foreach (Block block in blocks)
        {
            if (block == null) continue;
            if (block.isFinish)
            {
                finishBlocks.Add(block);
            }
        }

        // Loading player atlas
        var playerAtlasTexture = Content.Load<Texture2D>("Sprites/ChickenAtlas");
        var playerAtlas = new Atlas
        (
            texture: playerAtlasTexture,
            rectangleSize: new Vector2(16, 16)
        );

        // Creating player
        player = new Player
        (
            atlas: playerAtlas,
            position: new Vector2(-50, 500),
            size: new Vector2(16, 16),
            speed: 11f,
            blocks, finishBlocks.ToArray(),
            game: game
        );

        // Loading menu bg
        menuBackground = Content.Load<Texture2D>("Sprites/MenuBackground");

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
        exitButton = new SceneButton
        (
            atlas: buttonsAtlas,
            position: new Vector2(graphics.PreferredBackBufferWidth / 2 - 120, 250),
            size: new Vector2(24, 24),
            sceneToLoad: new MainMenuScene(graphics, game),
            buttonIcons: buttonsIconsAtlas, icon: 1,
            game: game
        );

        restartButton = new SceneButton
        (
            atlas: buttonsAtlas,
            position: new Vector2(graphics.PreferredBackBufferWidth / 2 + 40, 250),
            size: new Vector2(24, 24),
            sceneToLoad: new GameScene(graphics, game), sceneParameters: map,
            buttonIcons: buttonsIconsAtlas, icon: 0,
            game: game
        );

        continueButton = new ToggleButton
        (
            atlas: buttonsAtlas,
            position: new Vector2(graphics.PreferredBackBufferWidth / 2 + 40, 250),
            size: new Vector2(24, 24), enabled: true,
            buttonIcons: buttonsIconsAtlas, enabledIcon: 6, disabledIcon: 6,
            game: game, ableToChangeStateByAny: true
        );

        // Loading fonts
        arial = Content.Load<SpriteFont>("Fonts/Arial");

        // Loading cup
        cupTexture = Content.Load<Texture2D>("Sprites/Cup");
        cupBgTexture = Content.Load<Texture2D>("Sprites/CupBackground");

        // Loading audio
        song = Content.Load<Song>("Audio/Music");
        gameWinSound = Content.Load<SoundEffect>("Audio/GameWin").CreateInstance();
        gameWinSound.Volume = 0.5f;
        gameOverSound = Content.Load<SoundEffect>("Audio/GameOver").CreateInstance();
        gameOverSound.Volume = 0.5f;

        // Starting playing song
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.3f;
        MediaPlayer.Play(song);
    }

    public override void Update(GameTime gameTime)
    {
        // Checking for pause
        if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !gameWin)
        {
            pause = true;
            MediaPlayer.Pause();
        }

        if (!gameOver && !pause && !gameWin)
        {
            // Updating objects
            player.Update(gameTime);
            camera.LookAt(cameraPosition);

            // Updating camera position
            if (player.position.X >= graphics.PreferredBackBufferWidth / 2 - player.rectangle.Width)
            {
                cameraPosition.X += player.CalculateCameraMove();
            }

            // Checking for player lose
            if (player.position.X + player.rectangle.Width < camera.Position.X)
            {
                gameOver = true;
                MediaPlayer.Stop();
            }
        }

        // Updating UI
        if (gameOver)
        {
            if (!playedGameOver) gameOverSound.Play();
            restartButton.Update();
            exitButton.Update();
            if (exitButton.pressed || restartButton.pressed) gameOverSound.Stop();
            playedGameOver = true;
        }
        else if (gameWin)
        {
            player.Update(gameTime);
            if (!playedGameWin) gameWinSound.Play();
            MediaPlayer.Stop();
            if ((timeToExitAfterWin -= (float)gameTime.ElapsedGameTime.TotalSeconds) <= 0)
            {
                game.LoadScene(new MainMenuScene(graphics, game));
            }
            playedGameWin = true;
        }
        else if (pause)
        {
            continueButton.Update();
            exitButton.Update();

            pause = continueButton.isEnabled;

            if (!pause)
            {
                MediaPlayer.Resume();
                continueButton.isEnabled = true;
            }
        }

        // Updating cup bg rotation
        cupBgRotation += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (cupBgRotation >= 360f) cupBgRotation = 0f;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var matrix = camera.GetViewMatrix();

        // Drawing map
        mapLoader.DrawMap(spriteBatch, matrix, blocks);

        // Drawing NOT-UI components
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: matrix);

        // Drawing player
        spriteBatch.Draw
        (
            player.atlas.texture, player.rectangle,
            player.atlas.textureRectangles[player.frame],
            Color.White, 0f, Vector2.Zero, player.flip, 0f
        );

        // Drawing menu background (game over / pause)
        if (gameOver || pause)
        {
            // Drawing bg
            spriteBatch.Draw
            (
                menuBackground,
                new Rectangle((int)camera.Position.X, 0,
                graphics.PreferredBackBufferWidth + 10,
                graphics.PreferredBackBufferHeight + 10),
                Color.White
            );

            // Drawing game over text
            if (gameOver)
            {
                spriteBatch.DrawString
                (
                    arial, GAME_OVER_TEXT,
                    new Vector2(graphics.PreferredBackBufferWidth / 2 + camera.Position.X - 170, 150),
                    Color.White
                );
            }
            // Drawing pause text
            else if (pause)
            {
                spriteBatch.DrawString
                (
                    arial, PAUSE_TEXT,
                    new Vector2(graphics.PreferredBackBufferWidth / 2 + camera.Position.X - 90, 150),
                    Color.White
                );
            }
        }

        spriteBatch.End();

        // Drawing UI components
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Drawing buttons
        if (gameOver || pause)
        {
            // Exit button
            spriteBatch.Draw
            (
                exitButton.atlas.texture, exitButton.rectangle,
                exitButton.atlas.textureRectangles[exitButton.frame],
                Color.White
            );

            spriteBatch.Draw
            (
                exitButton.buttonIcons.texture, exitButton.rectangle,
                exitButton.buttonIcons.textureRectangles[exitButton.icon],
                Color.White
            );
        }

        if (gameOver)
        {
            // Restart button
            spriteBatch.Draw
            (
                restartButton.atlas.texture, restartButton.rectangle,
                restartButton.atlas.textureRectangles[restartButton.frame],
                Color.White
            );

            spriteBatch.Draw
            (
                restartButton.buttonIcons.texture, restartButton.rectangle,
                restartButton.buttonIcons.textureRectangles[restartButton.icon],
                Color.White
            );
        }
        else if (pause)
        {
            // Continue button
            spriteBatch.Draw
            (
                continueButton.atlas.texture, continueButton.rectangle,
                continueButton.atlas.textureRectangles[continueButton.frame],
                Color.White
            );

            spriteBatch.Draw
            (
                continueButton.buttonIcons.texture, continueButton.rectangle,
                continueButton.buttonIcons.textureRectangles[continueButton.icon],
                Color.White
            );
        }
        else if (gameWin)
        {
            var cupSize = new Vector2
            (
                cupTexture.Width * GameObject.SizeMod,
                cupTexture.Height * GameObject.SizeMod
            );

            var cupBgSize = new Vector2
            (
                cupBgTexture.Width * GameObject.SizeMod,
                cupBgTexture.Height * GameObject.SizeMod
            );

            spriteBatch.Draw
            (
                cupBgTexture,
                new Rectangle
                (
                    graphics.PreferredBackBufferWidth / 2,
                    graphics.PreferredBackBufferHeight / 2,
                    (int)cupBgSize.X, (int)cupBgSize.Y
                ), null, Color.White, cupBgRotation,
                new Vector2(cupBgTexture.Width / 2f, cupBgTexture.Height / 2f),
                SpriteEffects.None, 0f
            );

            spriteBatch.Draw
            (
                cupTexture,
                new Rectangle
                (
                    (int)(graphics.PreferredBackBufferWidth / 2 - cupSize.X / 2),
                    (int)(graphics.PreferredBackBufferHeight / 2 - cupSize.Y / 2),
                    (int)cupSize.X, (int)cupSize.Y
                ), Color.White
            );
        }

        spriteBatch.End();
    }

    public void OnFinish()
    {
        gameWin = true;
        game.CompleteMap(map);
    }
}