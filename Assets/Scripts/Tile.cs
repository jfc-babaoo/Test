using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
	public int id;
	public bool isCorrect;
	public Tile(int tileId)
	{
		id = tileId;
	}

	public Canvas canvas;
	private RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("Begin Drag");
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("End Drag");
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("On Drop");
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
		BorderMap();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Triggered");
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
