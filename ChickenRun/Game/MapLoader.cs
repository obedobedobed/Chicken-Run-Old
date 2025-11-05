using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChickenRun;

public class MapLoader
{
    private Atlas blockAtlas;
    private Atlas finishAtlas;
    private Dictionary<char, int> charToAtlasFrame;
    private Dictionary<char, int> charToFinishFrame;
    private Vector2 blockSize;

    public MapLoader
    (Texture2D blocksAtlasTexture, Texture2D finishAtlasTexture, Vector2 blockSize)
    {
        blockAtlas = new Atlas(blocksAtlasTexture, blockSize);
        finishAtlas = new Atlas(finishAtlasTexture, blockSize);

        charToAtlasFrame = new Dictionary<char, int>
        {
            { '$', 0 },
            { '%', 1 },
            { '#', 2 },
        };

        charToFinishFrame = new Dictionary<char, int>
        {
            { '(', 0 },
            { '*', 1 },
            { ')', 2 },
        };

        this.blockSize = blockSize;
    }

    public Block[,] LoadMap(string pathToMap)
    {
        string[] mapStrings = File.ReadAllLines(pathToMap);
        char[,] map = new char[mapStrings.Length, mapStrings[0].Length];

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                map[y, x] = mapStrings[y][x];
            }
        }

        var tmpBlocksArray = new Block[map.GetLength(0), map.GetLength(1)];
                
        int yPos = 0;

        for (int y = 0; y < map.GetLength(0); y++)
        {
            int xPos = 0;

            for (int x = 0; x < map.GetLength(1); x++)
            {
                char mapChar = map[y, x];

                Atlas atlas = blockAtlas;
                int frame;
                bool canGetFrameFinish = false;
                bool isFinishBlock = false;

                bool canGetFrameBlock =
                charToAtlasFrame.TryGetValue(mapChar, out frame);

                if (!canGetFrameBlock)
                {
                    canGetFrameFinish =
                    charToFinishFrame.TryGetValue(mapChar, out frame);
                    atlas = finishAtlas;
                    isFinishBlock = true;
                }

                if (!canGetFrameBlock && !canGetFrameFinish)
                {
                    xPos += (int)blockSize.X * GameObject.SizeMod;
                    continue;
                }

                tmpBlocksArray[y, x] = new Block
                (
                    atlas: atlas,
                    position: new Vector2(xPos, yPos),
                    size: blockSize, frame: frame,
                    isFinishBlock
                );

                xPos += (int)blockSize.X * GameObject.SizeMod;
            }

            yPos += (int)blockSize.Y * GameObject.SizeMod;
        }

        return tmpBlocksArray;
    }

    public void DrawMap(SpriteBatch spriteBatch, Matrix matrix, Block[,] blocks)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: matrix);

        int yPos = 0;

        for (int y = 0; y < blocks.GetLength(0); y++)
        {
            int xPos = 0;

            for (int x = 0; x < blocks.GetLength(1); x++)
            {
                Block block = blocks[y, x];

                if (block == null)
                {
                    xPos += (int)blockSize.X;
                    continue;
                }

                spriteBatch.Draw
                (
                    block.atlas.texture, block.rectangle,
                    block.atlas.textureRectangles[block.frame],
                    Color.White
                );

                xPos += (int)blockSize.X * GameObject.SizeMod;
            }

            yPos += (int)blockSize.Y * GameObject.SizeMod;
        }

        spriteBatch.End();
    }
}