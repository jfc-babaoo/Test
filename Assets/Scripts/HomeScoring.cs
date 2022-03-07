using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScoring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		if (PlayerPrefs.HasKey("Score") && PlayerPrefs.HasKey("Chrono"))
			GetComponent<Text>().text = PlayerPrefs.GetInt("Score").ToString();
    }
}
