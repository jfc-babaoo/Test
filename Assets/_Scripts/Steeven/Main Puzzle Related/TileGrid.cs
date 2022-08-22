using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MoveType { Horizontal, Vertical }
public enum PuzzleState { Solved, Shuffling, InProgress, Failed }
/// <summary>
/// Class that does all the verification needed to get the game up and running
/// </summary>
public class TileGrid : Singleton<TileGrid>
{
    #region Fields
    //Internal fields:
    internal Image emptyTile;

    internal PuzzleState puzzleState;

    internal int canMoveId = -1;
    internal int currentScore = 0;
    internal int shuffleMoveRemaining = 0;

    internal bool runTimer = true;

    [SerializeField]
    internal List<Transform> tilesTransform;

    //Serialized fields:
    Tile previousTile;
    [Tooltip("How many moves does the shuffle should last?")]
    [SerializeField] int shuffleLenght = 12;
    [Tooltip("Require Text mesh pro package")]
    [SerializeField] TMP_Text currentScoreText;
    [Tooltip("Require Text mesh pro package")]
    [SerializeField] TMP_Text bestScoreText;
    [Tooltip("Require Text mesh pro package")]
    [SerializeField] TMP_Text timerText;
    [Tooltip("How many time does the player has to solve it in second? Default 180")]
    [SerializeField] float maxTimeAllowed = 180f;



    public SaveAndLoadManager saveManager;
    public TMP_Text endGameText;

    [Tooltip("The EndGame Panel GameObject")]
    public GameObject endGamePanel;

    int bestScore = 0;
    float currentTimer;
    [HideInInspector]
    public List<Vector3> shuffleOffsets;

    #endregion


    private void Awake()
    {
        canMoveId = -1;
    }

    private void Start()
    {
        emptyTile = tilesTransform[4].GetComponent<Image>();
        shuffleMoveRemaining = shuffleLenght;
        //UpdateBestScore();
        ShuffleIt();
        shuffleMoveRemaining = shuffleLenght;
    }


    private void Update()
    {

        runTimer = (puzzleState == PuzzleState.InProgress);
        if(endGamePanel!=null)
            endGamePanel.SetActive(puzzleState == PuzzleState.Solved);

        if (maxTimeAllowed > 0)
        {
            if (runTimer)
            {
                maxTimeAllowed -= Time.deltaTime;
                UpdateTimer(maxTimeAllowed);
            }
        }
        else
        {
            puzzleState = PuzzleState.Failed;
            return;
        }


    }

    /// <summary>
    /// Loop through all the 9 tiles, check if they are at their original position, if they ALL are, the hole tile it turned on again, and shows the center tile image, then save 
    /// </summary>
    internal void CheckIfSolved()
    {
        foreach (Transform tile in tilesTransform)
        {
            if (!tile.GetComponent<Tile>().IsAtStartingCoord())
            {
                return;
            }
        }

        emptyTile.enabled = true;
        saveManager.SaveWithPlayerPrefs(currentScore,endGameText);

        foreach (Transform tile in tilesTransform)
        {
            tile.GetComponent<Tile>().canMove = false;
        }

        puzzleState = PuzzleState.Solved;
    }

    /// <summary>
    /// returns the hole's local position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetHolePosition()
    {
        return tilesTransform[4].localPosition;
    }

    /// <summary>
    /// returns the hole's transform
    /// </summary>
    /// <returns></returns>
    public Transform GetHoleTransform()
    {
        return tilesTransform[4];
    }

    /// <summary>
    /// Select a tile that is right next the hole
    /// </summary>
    /// <param name="selectedTile"></param>
    /// <returns></returns>
    public Tile SelectOneTileAroundTheHole(Tile selectedTile)
    {
        Tile tile = selectedTile;
        Vector3 holePosition = GetHolePosition();
        Vector3 deltaPosition = tile.transform.localPosition - holePosition;

        //If the tile isn't directly near the hole && is not the hole
        if ((!Mathf.Approximately(deltaPosition.y, 0) &&
            !Mathf.Approximately(deltaPosition.x, 0)) ||
            (Mathf.Abs(deltaPosition.x) > 400) ||
            (Mathf.Abs(deltaPosition.y) > 400) &&
            selectedTile == previousTile &&
            !selectedTile.isHole)
        {
            return null;
        }
        else
        {
            return tile;
        }

    }

    //The puzzle is always doable since the shuffling start from a solved puzzle
    /// <summary>
    /// lunch the tile selection, and then move it, else we retry
    /// </summary>
    public void ShuffleIt()
    {

        //Set the state to shuffling
        puzzleState = PuzzleState.Shuffling;

        //pick a random tile around the hole
        Tile selectedtTile = SelectOneTileAroundTheHole(tilesTransform[Random.Range(0, tilesTransform.Count)].GetComponent<Tile>());

        while (shuffleMoveRemaining > 0)
        {
            //if the tile is good we move it
            if (selectedtTile != null && !selectedtTile.isHole && selectedtTile != previousTile)
            {
                Vector3 targetCoord = GetHoleTransform().GetComponent<Tile>().coord;

                emptyTile.transform.localPosition = selectedtTile.transform.localPosition;
                selectedtTile.transform.localPosition = targetCoord;
                selectedtTile.UpdateCoord();
                emptyTile.transform.GetComponent<Tile>().UpdateCoord();
                previousTile = selectedtTile;
                shuffleMoveRemaining--;
                shuffleOffsets.Add(targetCoord);
            }
            //else we retry
            else
            {
                ShuffleIt();
            }
        }
        //once finished to shuffle,
        //we set the puzzle state to In progress,
        //the player can now try

        puzzleState = PuzzleState.InProgress;

    }

    internal void UpdateScore(int v)
    {
        currentScore += v;
        currentScoreText.text = currentScore.ToString();
    }

    internal void UpdateBestScore()
    {
        bestScore = saveManager.LoadWithPlayerPrefs();
        bestScoreText.text = bestScore.ToString();
    }

    /// <summary>
    /// Format the time text so it shows it the way a timer should be
    /// </summary>
    /// <param name="currentTime"></param>
    public void UpdateTimer(float currentTime)
    {
        currentTime += 1;
        float mins = Mathf.FloorToInt(currentTime / 60);
        float secs = Mathf.FloorToInt(currentTime % 60);
        if (currentTime <= 30)
        {
            timerText.color = Color.red;

        }

        timerText.text = string.Format("{0:00} : {1:00}", mins, secs);
    }
}
