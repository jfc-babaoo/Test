using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SP_Tiles : MonoBehaviour
{
    [Tooltip("This tileToCopy's position is used to set this tile's position")]
    internal Tile tileToCopy;
    [HideInInspector]
    [Tooltip("identification number")]
    public int id;

    [HideInInspector]
    [Tooltip("Used to find the right tile to copy")]
    public Tile[] tiles;

    Texture2D quadTexture;

    private void Start()
    {
        id = transform.GetSiblingIndex() + 1;
        tiles = FindObjectsOfType<Tile>();


#if UNITY_IOS // directives de préprocesseur, permettent d'assigner les bonnes textures basée sur la plateforme cible (IOS ou Android ) dans ce cas
            quadTexture = Resources.Load<Texture2D>("Apple/" + id.ToString());//pick the right png file to assign as texture from the Apple folder
#elif UNITY_ANDROID
        quadTexture = Resources.Load<Texture2D>("Android/" + id.ToString()); //pick the right png file to assign as texture from the Android folder
#endif
        GetTheRightTexture();
        GetTileToCopyRef();
        SetMyLocalPosition(transform, tileToCopy.GetComponent<RectTransform>().rect.width);
    }


    /// <summary>
    /// Find the right Tile of the main puzzle to copy it's position 
    /// </summary>
    public void GetTileToCopyRef()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].id == id)
                tileToCopy = tiles[i];

        }
    }


    // Update is called once per frame
    void Update()
    {
        SetMyLocalPosition(transform, 300);
    }

    /// <summary>
    /// switch the shader to avoid having to set up a directional light in the scene
    /// Sets the main texture of this quad, to the field tileTexture, from the resources folder
    /// </summary>
    void GetTheRightTexture()
    {
        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture"); //Dont forget to add this shader to the player settings, always included shader add 1
        GetComponent<MeshRenderer>().material.mainTexture = quadTexture;
    }

    /// <summary>
    /// Sets the localPosition of this tile, based on the tileToCopy localposition divided by the width of the tileToCopy
    /// </summary>
    /// <param name="thisLocalPos"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public void SetMyLocalPosition(Transform thisLocalPos, float width)
    {
        thisLocalPos.localPosition = new Vector3(tileToCopy.transform.localPosition.x / width,
                                                        tileToCopy.transform.localPosition.y / width,
                                                        tileToCopy.transform.localPosition.z / width);


    }
}
