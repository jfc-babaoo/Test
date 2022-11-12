using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TILE_POSITION
{
    NONE,
    TOPLEFT,
    TOPCENTER,
    TOPRIGHT,
    MIDLEFT,
    MIDCENTER,
    MIDRIGHT,
    BOTLEFT,
    BOTCENTER,
    BOTRIGHT
}

public class TileController : MonoBehaviour
{
    [SerializeField] public TILE_POSITION winPosition;
    Vector2 beginDragPosition;
    bool isDragging = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Debug.Log("begin drag");


                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                    if (hit.collider.gameObject == this)
                    {
                        isDragging = true;
                        beginDragPosition = transform.position;
                    }
                    break;
                case TouchPhase.Ended:
                    Debug.Log("end drag");
                    hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                    if (hit.transform.CompareTag("Slot"))
                        transform.position = hit.transform.position;
                    else
                        transform.position = beginDragPosition;
                    isDragging = false;
                    break;
            }

            if (isDragging)
            {
                Debug.Log("dragging");
                Vector3 dragPos = touch.deltaPosition;
                transform.position += dragPos;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
                if (hit.transform != null)
                {
                    Debug.Log(hit.transform.tag, hit.transform);
                    if (hit.transform == transform)
                        BeginDrag();
                }
            }
            if (isDragging)
            {
                Drag();
                if (Input.GetMouseButtonUp(0))
                    EndDrag();
            }
        }
    }

    void BeginDrag()
    {
        Debug.Log("begin drag", this);
        GetComponent<Collider2D>().enabled = false;
        isDragging = true;
        beginDragPosition = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
        if (hit.transform != null)
        {
            SlotScript slot = hit.transform.GetComponent<SlotScript>();
            if (slot != null)
                slot.holdingTile = TILE_POSITION.NONE;

        }
    }

    void Drag()
    {
        transform.position = (Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    void EndDrag()
    {
        transform.position = beginDragPosition;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
        if (hit.transform != null)
            if (hit.transform.CompareTag("Slot") && Vector2.Distance(beginDragPosition, hit.transform.position) == GetComponent<SpriteRenderer>().size.x)
            {
                hit.transform.GetComponent<SlotScript>().holdingTile = winPosition;
                transform.position = (Vector2)hit.transform.position;
                GameManager.instance.CheckWin();
            }


        isDragging = false;
        GetComponent<Collider2D>().enabled = true;
        Debug.Log("end drag", this);
    }
}
