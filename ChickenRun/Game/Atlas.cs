using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChickenRun;

public class Atlas
{
    public Texture2D texture { get; private set; }
    public Rectangle[] textureRectangles { get; private set; }

    public Atlas(Texture2D texture, Vector2 rectangleSize)
    {
        this.texture = texture;

        textureRectangles = new Rectangle
        [
            (texture.Width / (int)rectangleSize.X) *
            (texture.Height / (int)rectangleSize.Y)
        ];

        int iteration = 0;

        int yPos = 0;

        for (int y = 0; y < texture.Height / (int)rectangleSize.Y; y++)
        {
            int xPos = 0;

            for (int x = 0; x < texture.Width / (int)rectangleSize.X; x++)
            {
                textureRectangles[iteration] = new Rectangle
                (
                    xPos, yPos,
                    (int)rectangleSize.X, (int)rectangleSize.Y
                );

                iteration++;
                xPos += (int)rectangleSize.X;
            }

            yPos += (int)rectangleSize.Y;
        }
    }
}