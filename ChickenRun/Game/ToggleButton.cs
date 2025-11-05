using Microsoft.Xna.Framework;

namespace ChickenRun;

public class ToggleButton : Button
{
    public bool isEnabled
    {
        get
        {
            return enabled;
        }
        set
        {
            if (ableToChangeStateByAny) enabled = value;
        }
    }
    
    private bool enabled = false;
    private int enabledIcon;
    private int disabledIcon;
    private bool ableToChangeStateByAny = false;

    public ToggleButton(Atlas atlas, Vector2 position, Vector2 size, bool enabled, Atlas buttonIcons,
    int enabledIcon, int disabledIcon, Game1 game, bool ableToChangeStateByAny = false)
    : base(atlas, position, size, buttonIcons, enabledIcon, game)
    {
        this.enabled = enabled;
        this.enabledIcon = enabledIcon;
        this.disabledIcon = disabledIcon;
        this.ableToChangeStateByAny = ableToChangeStateByAny;

        if (!enabled) icon = disabledIcon;
    }

    protected override void OnPress()
    {
        enabled = !enabled;

        switch (enabled)
        {
            case true: icon = enabledIcon; break;
            case false: icon = disabledIcon; break;
        }
    }
}