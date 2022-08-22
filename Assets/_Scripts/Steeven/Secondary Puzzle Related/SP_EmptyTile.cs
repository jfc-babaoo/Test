using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_EmptyTile : MonoBehaviour
{
    void SetMyLocalPositionToTheHolePosition()
    {
        transform.localPosition = new Vector3(TileGrid.Instance.GetHolePosition().x / 300,
                                                TileGrid.Instance.GetHolePosition().y / 300,
                                                    TileGrid.Instance.GetHolePosition().z / 300);
        TileGrid.Instance.CheckIfSolved();
    }

    void Update()
    {
        SetMyLocalPositionToTheHolePosition();
    }
}
