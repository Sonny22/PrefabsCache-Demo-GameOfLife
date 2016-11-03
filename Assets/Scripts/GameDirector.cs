using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameDirector : MonoBehaviour 
{
	public const int size = 100;

	public float interval = 0.5f;

    public GUIStyle styleGUI;

	public Cell[,] cells = new Cell[size,size];
	public GameObject prefab;

	public static event System.Action Age;

	[Serializable]
	public class Coords
	{
		public int x;
		public int y;

		public Coords(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

	void Awake()
	{
		Cell[] cs = GameObject.FindObjectsOfType<Cell>();

		// Retrieving the cells set up manually
		foreach (Cell c in cs) 
		{
			cells [(int)c.transform.position.x, (int)c.transform.position.y] = c;
		}

		for (int x = 0; x < size; x++) 
		{
			for (int y = 0; y < size; y++) 
			{
				if (cells[x,y] == null)
					cells[x,y] = AddCell(x, y);
			}
		}

		StartCoroutine(Generation());
	}

	Cell AddCell(int x, int y)
	{
		GameObject g = (GameObject)Instantiate(prefab, new Vector3(x, y), Quaternion.identity);
		g.transform.parent = this.transform;
		return g.GetComponent<Cell> ().SetDead();
	}
	
	IEnumerator Generation()
	{
		while (true) {
			if (Age != null)
				Age ();

			// 1st pass: resolving birth and death
			for (int x = 0; x < size; x++) {
				for (int y = 0; y < size; y++) {
					int neighbours = 0;
					List<Coords> neighboursCoords = new List<Coords> ();

					for (int i = -1; i < 2; i++) {
						for (int j = -1; j < 2; j++) {
							if (i == 0 && j == 0)
								continue;

							try {
								if (cells [x + i, y + j].IsDead () == false) {
									neighbours++;
									neighboursCoords.Add (new Coords (x + i, y + j));
								}
							} catch (System.Exception) {
								
							}
						}
					}

					if (cells [x, y].IsDead () && neighbours == 3) // New born
						cells [x, y].SetNewBorn ();
					else if (cells [x, y].IsDead () == false && neighbours <= 1) // Alone
						cells [x, y].SetDying ();
					else if (cells [x, y].IsDead () == false && neighbours >= 4) // Over populaiton
						cells [x, y].SetDying ();
				}
			}

			// 2nd pass: resolve new born and dying cells
			for (int x = 0; x < size; x++) {
				for (int y = 0; y < size; y++) {
					if (cells [x, y].IsDying ())
						cells [x, y].SetDead ();
					else if (cells [x, y].IsNewBorn ())
						cells [x, y].SetNormal ();
				}
			}

			yield return new WaitForSeconds(interval);
		}
	}

    void OnGUI()
    {
        GUIContent str = new GUIContent("Game of Life");

        Vector2 v = styleGUI.CalcSize(str);

        GUI.Label(new Rect(0f, 0f, v.x, v.y), str, styleGUI);
    }
}
