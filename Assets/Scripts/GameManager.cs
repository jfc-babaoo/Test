using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject ApplePrefab;
    [SerializeField] GameObject AndroidPrefab;

    [SerializeField] Text timerText;
    [SerializeField] Canvas GameOverCanvas;
    [SerializeField] Canvas WinCanvas;

    [SerializeField] GameObject grid;

    [Min(0)] float timer = 180f;    //player has 3 minutes (180 seconds) to complete the puzzle
    public TileController hiddenTile;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else instance = this;

        Time.timeScale = 1f;

        if (Application.platform == RuntimePlatform.IPhonePlayer)
            Instantiate(ApplePrefab);
        else if (Application.platform == RuntimePlatform.Android)
            Instantiate(AndroidPrefab);
        else
            Debug.LogWarning("Current platform neither Android nor iPhone: " + Application.platform.ToString());
        if (Random.Range(0, 2) == 0) Instantiate(ApplePrefab); else Instantiate(AndroidPrefab);
    }

    void Start()
    {
        Time.timeScale = 1f;
    }


    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;
        int iTime = Mathf.CeilToInt(timer);
        timerText.text = "Time remaining : " + iTime.ToString() + " seconds";
        if (timer == 0f)
            GameOver();
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        GameOverCanvas.gameObject.SetActive(true);
    }

    public void CheckWin()
    {
        bool win = true;
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            SlotScript slot = grid.transform.GetChild(i).GetComponent<SlotScript>();
            if (slot.holdingTile == TILE_POSITION.NONE) continue;   //ignore the slot that hold nothing
            if (slot.slotPosition != slot.holdingTile)
                win = false;    //if the current tile is not in its win position, the player has not won yet
        }
        if (win)
        {
            WinCanvas.gameObject.SetActive(true);
            Time.timeScale = 0f;
            hiddenTile.gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
