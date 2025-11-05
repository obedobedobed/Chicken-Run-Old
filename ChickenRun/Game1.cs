using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChickenRun;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Scenes
    public Scene scene { get; private set; }

    // Game
    private float elapsedGameTime;
    public int map = -1;
    private GameBackground gameBackground;
    public bool vSyncEnabled { get; private set; } = true;

    // Game data
    private bool[] completedMaps = [false, false, false, false, false];

    // FPS Counting
    private int fpsCount;
    private int fpsInCounting;
    private const float FPS_COUNT_TIME = 1f;
    private float timeToCountFps = FPS_COUNT_TIME;

    public GraphicsDeviceManager GetGraphics
    {
        get
        {
            return _graphics;
        }
    }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        string vSyncText = vSyncEnabled ? " (VSync)" : string.Empty;
        Window.Title = $"Chicken Run - {fpsCount} FPS" + vSyncText;
        _graphics.PreferredBackBufferHeight = 640;
        _graphics.PreferredBackBufferWidth = 1152;
        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        // Loading data
        if (File.Exists(SavesManager.savesPath + SavesManager.saveFileName))
        {
            var saveData = SavesManager.Load();

            completedMaps = saveData.completedMaps;
            vSyncEnabled = saveData.vSyncEnabled;
        }
        else
        {
            Directory.CreateDirectory(SavesManager.savesPath);
            using (File.Create(SavesManager.savesPath + SavesManager.saveFileName)) { }
        }

        // Enabling VSync
        _graphics.SynchronizeWithVerticalRetrace = vSyncEnabled;
        IsFixedTimeStep = vSyncEnabled;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        LoadScene(new MainMenuScene(_graphics, this));

        gameBackground = new GameBackground(Content.Load<Texture2D>("Sprites/GameBackground"));
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here

        elapsedGameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Updating bg and scene
        gameBackground.Update(gameTime);
        scene.Update(gameTime);

        // Checking VSync toggle button state
        if (scene is MainMenuScene mainMenuScene)
        {
            if (mainMenuScene.buttons[mainMenuScene.buttons.Length - 2]
            is ToggleButton toggleButtonVSync)
            {
                vSyncEnabled = toggleButtonVSync.isEnabled;
                _graphics.SynchronizeWithVerticalRetrace = vSyncEnabled;
                IsFixedTimeStep = vSyncEnabled;
                _graphics.ApplyChanges();
                SavesManager.Save(new SaveData(completedMaps, vSyncEnabled));
            }

            if (mainMenuScene.buttons[mainMenuScene.buttons.Length - 1]
            is ToggleButton toggleButtonExit)
            {
                if (toggleButtonExit.isEnabled) Exit();
            }
        }

        // Updating title
        string vSyncText = vSyncEnabled ? " (VSync)" : string.Empty;
        Window.Title = $"Chicken Run - {fpsCount} FPS" + vSyncText;
            
        // Counting FPS
        if ((timeToCountFps -= elapsedGameTime) <= 0)
        {
            timeToCountFps = FPS_COUNT_TIME;
            fpsCount = fpsInCounting;
            fpsInCounting = 0;
        }

        fpsInCounting++;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        gameBackground.Draw(_spriteBatch);
        scene.Draw(_spriteBatch);

        base.Draw(gameTime);
    }

    public void LoadScene(Scene scene)
    {
        this.scene = scene;

        (scene as GameScene)?.Initialize(map);
        (scene as MainMenuScene)?.Initialize(completedMaps);

        scene.Load(Content, Content.RootDirectory);

        var saveData = SavesManager.Load();
        completedMaps = saveData.completedMaps;
    }

    public void CompleteMap(int mapNumber)
    {
        completedMaps[mapNumber - 1] = true;
        SavesManager.Save(new SaveData(completedMaps, vSyncEnabled));
    }
}
