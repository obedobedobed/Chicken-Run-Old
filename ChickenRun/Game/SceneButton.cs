using Microsoft.Xna.Framework;

namespace ChickenRun;

public class SceneButton : Button
{
    private Scene sceneToLoad;
    private int sceneParameters;
    public bool pressed { get; private set; } = false;

    public SceneButton(Atlas atlas, Vector2 position, Vector2 size, Scene sceneToLoad,
    Atlas buttonIcons, int icon, Game1 game, int sceneParameters = 0)
    : base (atlas, position, size, buttonIcons, icon, game)
    {
        this.sceneToLoad = sceneToLoad;
        this.sceneParameters = sceneParameters;
    }

    protected override void OnPress()
    {
        pressed = true;
        game.map = sceneParameters;
        game.LoadScene(sceneToLoad);
    }
}