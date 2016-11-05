using UnityEngine;
using System.Collections;
using PrefabsCache;
using System;

public class Cell : IEquatable<Cell>
{
	[Serializable] 
	public class Coords : IEquatable<Coords>
	{
		public int x;
        private string str;
		public int y;

		public Coords(int x, int y)
		{
			this.x = x;
			this.y = y;

            str = String.Join(",", new string[] {x.ToString(), y.ToString()});
		}

		public override int GetHashCode ()
		{
			return str.GetHashCode();
		}

		public override bool Equals (object obj)
		{
			return Equals(obj as Coords);
		}

		public bool Equals(Coords obj)
		{
			return obj != null && this.x == obj.x && this.y == obj.y;
		}

		public Vector3 ToVector3()
		{
			return new Vector3 (x, y);
		}
	}

	public Coords coords;
	public int neighbours = 0;
	public GameObject body = null;
    private CellBody cellBody = null;

    public int age = 0;

    private enum State
    {
        NewBorn,
        Normal,
        Dying,
        Dead
    }

    private State state = State.Dead;

    public bool IsAlive
    {
        get { return state == State.Normal || state == State.Dying; }
    }

    public bool IsNewBorn
    {
        get { return state == State.NewBorn; }
    }

    public bool IsDying
    {
        get { return state == State.Dying; }
    }

	public Cell(Coords coords)
	{
		this.coords = coords;
	}

	public override int GetHashCode ()
	{
		return coords.GetHashCode ();
	}

	public override bool Equals (object obj)
	{
		return Equals(obj as Cell);
	}

	public bool Equals(Cell obj)
	{
		return obj != null && this.coords.Equals(obj.coords);
	}

    public void SetNewBorn()
    {
        age = 0;
        state = State.NewBorn;
    }

    public void SetNormal()
    {
        body = Prefab.CellBodySphere.GetInstance(new Vector3(coords.x, coords.y));
        cellBody = body.GetComponent<CellBody>();
        state = State.Normal;
    }

    public void SetDying()
    {
        state = State.Dying;
    }

    public void SetDead()
    {
        Prefab.FreeInstance(body.gameObject);
        body = null;
        cellBody = null;
        state = State.Dead;
    }

    public void IncrementAge()
    {
        cellBody.SetAge(++age);
    }
}

public class CellBody : MonoBehaviour 
{
	private MeshRenderer _cachedMeshRenderer;

	void Awake()
	{
        _cachedMeshRenderer = GetComponentInChildren<MeshRenderer>();
	}

	public void SetAge(int age)
	{
        _cachedMeshRenderer.material.SetFloat("_Age", age);
    }
}
