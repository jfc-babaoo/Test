using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaquinScript : MonoBehaviour
{
    GameObject grid;

    void Start()
    {
        int tileToHide = Random.Range(0, transform.childCount);
        transform.GetChild(tileToHide).gameObject.SetActive(false);
        GameManager.instance.hiddenTile = transform.GetChild(tileToHide).GetComponent<TileController>();
        PlaceTiles();
    }

    void PlaceTiles()
    {
        grid = GameObject.Find("Grid");
        List<SlotScript> slots = new List<SlotScript>();
        for (int i = 0; i < grid.transform.childCount; i++)
            slots.Add(grid.transform.GetChild(i).GetComponent<SlotScript>());

        for (int i = 0; i < transform.childCount; i++)
        {
            int index = Random.Range(0, slots.Count);
            TileController tile = transform.GetChild(i).GetComponent<TileController>();
            if (!tile.gameObject.activeSelf) continue;  //ignore the hidden tile
            Debug.Log("Slots remaining " + slots.Count + "current : " + index, this);
            tile.transform.position = slots[index].transform.position;
            slots[index].holdingTile = tile.winPosition;
            slots.Remove(slots[index]);
        }
    }
}
