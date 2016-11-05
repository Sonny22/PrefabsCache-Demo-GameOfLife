using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PrefabsCache;

public class GameDirector : MonoBehaviour 
{
	public float interval = 0.5f;

    public GUIStyle styleGUI;

	private Hashtable cells = new Hashtable(100);

    private Camera cam;

	void Awake()
	{
		cam = Camera.main;

        cells.Clear();

		StartCoroutine(Generation());
	}
        	
	IEnumerator Generation()
	{
		while (true) 
        {
			foreach(Cell cell in cells.Values)
			{
                if (cell.IsAlive == false && cell.neighbours == 3) // New born
                {
                    cell.SetNewBorn();
                }
                else if (cell.IsAlive && cell.neighbours <= 1) // Alone
                {
                    cell.SetDying();
                }
                else if (cell.IsAlive && cell.neighbours >= 4) // Over population
                {
                    cell.SetDying();
                }
			}
                
			// 2nd pass: resolve new born and dying cells
            Dictionary<Cell.Coords, int> toAddWithNeighbours = new Dictionary<Cell.Coords, int>(100);
            foreach(Cell cell in cells.Values)
			{
                if (cell.IsDying)
                { 
                    cell.SetDead();

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (i == 0 && j == 0)
                                continue;

                            Cell.Coords c = new Cell.Coords(cell.coords.x + i, cell.coords.y + j);

                            (cells[c] as Cell).neighbours--;
                        }
                    }
                }
                else if (cell.IsNewBorn)
                {
                    cell.SetNormal();

                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (i == 0 && j == 0)
                                continue;

                            Cell.Coords c = new Cell.Coords(cell.coords.x + i, cell.coords.y + j);

                            Cell neighbour = (cells[c] as Cell);

                            if (neighbour == null)
                            {
                                if (toAddWithNeighbours.ContainsKey(c))
                                    toAddWithNeighbours[c] = toAddWithNeighbours[c] + 1;
                                else
                                    toAddWithNeighbours.Add(c, 1);
                            }
                            else
                            {
                                neighbour.neighbours++;
                            }
                        }
                    }
                }
                else if (cell.IsAlive)
                {
                    cell.IncrementAge();
                }
			}

            foreach (KeyValuePair<Cell.Coords,int> entry in toAddWithNeighbours)
            {
                Cell cell = new Cell(entry.Key);
                cell.neighbours = entry.Value;
                cells.Add(entry.Key, cell);
            }
                
			yield return new WaitForSeconds(interval);
		}
	}

    void Update()
	{
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton (0))
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);

            Cell.Coords c = new Cell.Coords(Mathf.RoundToInt(worldPos.x - 0.5f), Mathf.RoundToInt(worldPos.y - 0.5f));

            if (cells.ContainsKey(c) && (cells[c] as Cell).IsDead == false)
                return;
            
            AddOrGetAt(c).SetNewBorn();
        }
	}

    private Cell AddOrGetAt(Cell.Coords c)
    {
        Cell cell = (cells[c] as Cell);

        if (cell == null)
        {
            cell = new Cell(c);
            cells.Add(c, cell);
        }

        return cell;
    }

    void OnGUI()
    {
        GUIContent str = new GUIContent("Game of Life");

        Vector2 v = styleGUI.CalcSize(str);

        GUI.Label(new Rect(0f, 0f, v.x, v.y), str, styleGUI);
    }
}
