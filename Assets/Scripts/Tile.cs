using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
	public int id;
	public Vector2 positionInitial;
	public Tile(int tileId)
	{
		id = tileId;
	}

	public Canvas canvas;
	public bool isCorrect = false;
	private RectTransform rectTransform;
	private bool isMove = false;
	private Vector2 startPosition;
	private Vector2 tmpPosition;
	private LevelManager lvlManager;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		positionInitial = rectTransform.anchoredPosition;
		lvlManager = transform.parent.GetComponent<LevelManager>();
	}

	private void Update()
	{
		if (isMove)
		{
			isCorrect = (positionInitial == rectTransform.anchoredPosition) ? true : false;
			isMove = false;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		startPosition = rectTransform.anchoredPosition;

	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isMove = true;

	}

	public void OnDrop(PointerEventData eventData)
	{
		tmpPosition = rectTransform.anchoredPosition;
		if (startPosition != tmpPosition)
		{
			startPosition = tmpPosition;
			lvlManager.nbrMove++;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
		BorderMap();
	}
	

	/// <summary>
	/// Limit the drag & drop to the border of te map
	/// </summary>
	private void BorderMap()
	{
		if (rectTransform.anchoredPosition.x > 100)
			rectTransform.anchoredPosition = new Vector2(100, rectTransform.anchoredPosition.y);
		else if (rectTransform.anchoredPosition.x < -100)
			rectTransform.anchoredPosition = new Vector2(-100, rectTransform.anchoredPosition.y);
		if (rectTransform.anchoredPosition.y > 100)
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 100);
		else if (rectTransform.anchoredPosition.y < -100)
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -100);
	}
}
