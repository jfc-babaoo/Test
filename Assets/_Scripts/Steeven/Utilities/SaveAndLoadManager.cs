using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Custom class used to save and load data (best score in this case) using Unity PlayerPrefs
/// </summary>
public class SaveAndLoadManager : MonoBehaviour
{

    internal int bestScoreRegistered = 100;
    internal float bestTime = 0f;
    
    private void Awake()
    {
        
        bestScoreRegistered = LoadWithPlayerPrefs();
        

    }
    
    /// <summary>
    /// Save using PlayerPrefs
    /// </summary>
    /// <param name="currentScore"></param>
    public void SaveWithPlayerPrefs(int currentScore, TMP_Text text)
    {
        if (currentScore < bestScoreRegistered)
        {
            bestScoreRegistered = currentScore;
            text.text = "Felicitations tu viens de faire un nouveau meilleur score!";
            PlayerPrefs.SetInt("BestScore", bestScoreRegistered);
            PlayerPrefs.Save();
        }
        else if (currentScore == bestScoreRegistered)
        {
            text.text = "Genial, tu as reussi a egaler l'actuel meilleur score!";
            Debug.Log("Génial, tu as réussi à égaler l'actuel meilleur score!");
        }
        else
        {
            text.text = "Quel dommage, je suis sur que tu peux faire mieux!";
            Debug.Log("Quel dommage, je suis sûr que tu peux faire mieux!");
            return;
        }

    }
    /// <summary>
    /// Load using PlayerPrefs
    /// </summary>
    /// <param name="BestScore"></param>
    /// <returns></returns>
    public int LoadWithPlayerPrefs(string BestScore = "BestScore")
    {

        return PlayerPrefs.GetInt(BestScore, 100);



    }
}
