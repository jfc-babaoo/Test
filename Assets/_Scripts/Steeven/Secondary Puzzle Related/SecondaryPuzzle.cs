using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryPuzzle : MonoBehaviour
{
    //PuzzleState puzzleState;

    internal Texture2D image;

    SP_Tiles emptyTile;
    SP_Tiles[,] tiles;

    void Start()
    {
        CreatePuzzle(3);
    }

    void CreatePuzzle(int tilesPerLine)
    {
        tiles = new SP_Tiles[tilesPerLine, tilesPerLine];

        for (int y = 0; y < tilesPerLine; y++)
        {
            for (int x = 0; x < tilesPerLine; x++)
            {
                GameObject tileObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tileObject.transform.parent = transform;
                tileObject.transform.localPosition = (tilesPerLine - 1) * .5f * -Vector2.one + new Vector2(x, y);
                tileObject.transform.rotation = transform.rotation;

                SP_Tiles tile = tileObject.AddComponent<SP_Tiles>();

                tiles[x, y] = tile;

                if (y == 1 && x == tilesPerLine - 2)
                {
                    tileObject.AddComponent<SP_EmptyTile>();
                    tileObject.GetComponent<MeshRenderer>().enabled = false;
                    emptyTile = tile;

                }
            }
        }



    }
}
