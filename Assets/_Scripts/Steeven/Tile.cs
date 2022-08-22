using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Class used to handle the drag input the player provide and react as it should 
/// </summary>
public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Fields
    TileGrid tileGrid;

    [SerializeField] internal int id;
    [SerializeField] internal bool isHole;
    [SerializeField] internal bool canMove;
    internal bool isMoving;

    [SerializeField] private float startMove;
    [SerializeField] private float endMove;
    [SerializeField] private MoveType moveType;
    private Image tileSprite;

    public Vector3 coord;
    public Vector3 startingCoord;

    #endregion

    private void Awake()
    {
        id = transform.GetSiblingIndex() + 1;
        tileSprite = GetComponent<Image>();

#if UNITY_IOS
            tileSprite.sprite = Resources.Load<Sprite>("Apple/" + id.ToString());
            
#elif UNITY_ANDROID
        tileSprite.sprite = Resources.Load<Sprite>("Android/" + id.ToString());

#endif

        startingCoord = new Vector2(transform.localPosition.x, transform.localPosition.y);
        coord = startingCoord;
        tileGrid = TileGrid.Instance;
    }


    /// <summary>
    /// one of the 3 most importants Interfaces for this app.
    /// This one allows us to do all the verifications in order to:
    ///     Allows the tile to move after the next conditions are checked
    ///         determine if the player is trying to move a tile right next the hole
    ///         determine witch direction is moving the player: 
    ///             Horizontal 
    ///             Vertical
    ///         
    ///          determine where the endPosition should be
    ///     
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Don't move the hole
        if (isHole || tileGrid.puzzleState != PuzzleState.InProgress)
            return;

        //Don't move another tile if a tile is currently misplaced
        if (tileGrid.canMoveId != id && tileGrid.canMoveId != -1)
            return;


        //This tile is currently misplaced, you must not reset startMove and endMove
        if (tileGrid.canMoveId == id)
            return;



        //Check move delta
        Vector3 holePosition = tileGrid.GetHolePosition();
        Vector3 deltaPosition = transform.localPosition - holePosition;

        //If the tile isn't directly near the hole
        if ((!Mathf.Approximately(deltaPosition.y, 0) && !Mathf.Approximately(deltaPosition.x, 0)) ||
            (Mathf.Abs(deltaPosition.x) > 400) || (Mathf.Abs(deltaPosition.y) > 400))
            return;


        //Check move Direction , start and end
        if (Mathf.Approximately(deltaPosition.y, 0))
        {
            if (deltaPosition.x > 0)
            {
                moveType = MoveType.Horizontal;
                startMove = transform.localPosition.x;
                endMove = holePosition.x;
                //hole's on the left
            }
            else if (deltaPosition.x < 0)
            {
                moveType = MoveType.Horizontal;
                startMove = transform.localPosition.x;
                endMove = holePosition.x;
                //hole's on the right
            }
        }
        else if (Mathf.Approximately(deltaPosition.x, 0))
        {
            if (deltaPosition.y > 0)
            {
                moveType = MoveType.Vertical;
                startMove = transform.localPosition.y;
                endMove = holePosition.y;
                //hole's down
            }
            else if (deltaPosition.y < 0)
            {
                moveType = MoveType.Vertical;
                startMove = transform.localPosition.y;
                endMove = holePosition.y;
                //hole's up
            }
        }

        canMove = true;
        tileGrid.canMoveId = id;
        isMoving = true;
    }
    /// <summary>
    /// Move the tile in the possible direction
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {

        if (isHole || !canMove || tileGrid.puzzleState != PuzzleState.InProgress) return;

        switch (moveType)
        {

            case MoveType.Horizontal:
                transform.localPosition += new Vector3(eventData.delta.x, 0f);
                if (startMove < endMove)
                {
                    transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, startMove, endMove), transform.localPosition.y);
                }
                else
                {
                    transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, endMove, startMove), transform.localPosition.y);
                }
                break;
            case MoveType.Vertical:

                transform.localPosition += new Vector3(0f, eventData.delta.y);

                if (startMove < endMove)
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, startMove, endMove));
                }
                else
                {
                    transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, endMove, startMove));

                }
                break;
        }

    }

    /// <summary>
    /// Check what we did with the tile, and act accordingly
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isHole || !canMove || tileGrid.puzzleState != PuzzleState.InProgress) return;

        coord = transform.localPosition;

        switch (moveType)
        {
            case MoveType.Horizontal:
                //Tile placed back at its original position
                if (Mathf.Approximately(transform.localPosition.x, startMove))
                {
                    canMove = false;
                    tileGrid.canMoveId = -1;

                }
                //Tile placed over the hole
                else if (Mathf.Approximately(transform.localPosition.x, endMove))
                {
                    canMove = false;
                    tileGrid.canMoveId = -1;
                    //Move hole to previous position
                    tileGrid.GetHoleTransform().localPosition = new Vector2(startMove, tileGrid.GetHoleTransform().localPosition.y);
                    tileGrid.UpdateScore(1);
                }
                //Tile released before it touch the border goes back to start
                else if (!Mathf.Approximately(transform.localPosition.x, endMove))
                {
                    canMove = false;
                    tileGrid.canMoveId = -1;
                    transform.localPosition = new Vector2(startMove, coord.y);

                }
                break;
            case MoveType.Vertical:
                //Tile placed back at its original position
                if (Mathf.Approximately(transform.localPosition.y, startMove))
                {
                    canMove = false;
                    tileGrid.canMoveId = -1;
                }
                //Tile placed over the hole
                else if (Mathf.Approximately(transform.localPosition.y, endMove))
                {
                    canMove = false;
                    tileGrid.canMoveId = -1;
                    //Move hole to previous position
                    tileGrid.GetHoleTransform().localPosition = new Vector2(tileGrid.GetHoleTransform().localPosition.x, startMove);
                    tileGrid.UpdateScore(1);

                }
                //Tile released before it touch the border goes back to start
                else if (!Mathf.Approximately(transform.localPosition.y, endMove))
                {
                    canMove = false;
                    tileGrid.canMoveId = -1;
                    transform.localPosition = new Vector2(coord.x, startMove);

                }
                break;
        }

        tileGrid.CheckIfSolved();
        isMoving = false;
    }

    /// <summary>
    /// Check this tile's actual local position  
    /// </summary>
    public void UpdateCoord()
    {
        coord = transform.localPosition;
    }

    public bool IsAtStartingCoord()
    {
        UpdateCoord();
        return coord == startingCoord;
    }
}
