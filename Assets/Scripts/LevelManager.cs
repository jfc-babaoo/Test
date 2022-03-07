using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SceneTransitionSystem;

public class LevelManager : MonoBehaviour
{
	public List<Sprite> spritesLevel;

	private List<Tile> tiles;
	private List<Tile> shuffleTiles;
	public uint nbrMove = 0;
	public bool victory = false;

	// Start is called before the first frame update
	void Start()
	{
		tiles = new List<Tile>();
		for (int i = 0; i < 9; i++)
			tiles.Add(new Tile(i));
		ShuffleTiles();
		Placement();
	}

	/// <summary>
	/// Initialize tiles with id and sprite
	/// </summary>
	void InitTile()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform tile = transform.GetChild(i);
			tile.GetComponent<Image>().sprite = spritesLevel[i];
			tile.GetComponent<Tile>().id = i;
		}
	}
	/// <summary>
	/// Shuffle the tiles until you have a resolvable puzzle
	/// </summary>
	void ShuffleTiles()
	{
		shuffleTiles = tiles.OrderBy(id => Random.value).ToList();
		while (!IsResolvable())
			shuffleTiles = tiles.OrderBy(id => Random.value).ToList();
	}

	/// <summary>
	/// Check if the "Taquin" Game is solvable
	/// </summary>
	/// <returns>return true if the taquin game is solvable</returns>
	bool IsResolvable()
	{
		int j = 0;
		int nbrColumnLine = 3;
		int n = nbrColumnLine * nbrColumnLine - 1;
		int nbrPermute = 0;
		Tile[] copy = shuffleTiles.ToArray();

		for (int i = 0; i <= n; ++i)
		{
			if ((copy[i].id = shuffleTiles[i].id) == 0)
				j = i;
		}
		for (nbrPermute = (n - j) % nbrColumnLine + (n - j) / nbrColumnLine; n > 0; --n)
		{
			if (n != j)
			{
				copy[j] = copy[n];
				j = n;
				++nbrPermute;//permutation
			}
			while (copy[--j].id != n) ; //j = next case to move
		}
		return (1 & nbrPermute) != 0;
	}

	/// <summary>
	/// Place the tiles in the world space according to the shuffled list
	/// </summary>
	void Placement()
	{
		Vector3 upperLeftOffset = new Vector3(-100, 100);
		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild(shuffleTiles[i].id).localPosition = new Vector3(i % 3 * 100, i / 3 * -100) + upperLeftOffset;
	}

	/// <summary>
	/// Debug to display the tiles id
	/// </summary>
	/// <param name="listTiles">list of tiles to display</param>
	void PrintTiles(List<Tile> listTiles)
	{
		for (int i = 0; i < listTiles.Count; i++)
		{
			Debug.Log(listTiles[i].id);
		}
	}

	/// <summary>
	/// Button event to go home scene
	/// </summary>
	public void GoToHomeScene()
	{
		STSSceneManager.LoadScene("Home");
	}

	/// <summary>
	/// Launch the victory according to the conditions
	/// </summary>
	public void VictoryGame()
	{
		if (victory = IsPuzzleResolve())
			return;
	}

	/// <summary>
	/// Check if the puzzle is resolve
	/// </summary>
	private bool IsPuzzleResolve()
	{
		foreach (Tile tile in GetComponentsInChildren<Tile>())
		{
			if (tile.isCorrect == false && tile.id != 4)
				return false;
		}
		return true;
	}
}
