using Microsoft.Xna.Framework.Input;

namespace ChickenRun;

static class Extensions
{
    public static bool IsAnyKeyPressed(this KeyboardState keyboard)
    {
        return keyboard.GetPressedKeyCount() != 0;
    }

    public static bool IsAllKeysUnpressed(this KeyboardState keyboard)
    {
        return keyboard.GetPressedKeyCount() == 0;
    }
}