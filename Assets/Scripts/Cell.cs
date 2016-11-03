using UnityEngine;
using System.Collections;

public class Cell : MonoBehaviour 
{
	[System.Serializable]
	public class ListMaterial 
	{
		public Material normal;
		public Material dead;
	}

	public ListMaterial materials;

	private MeshRenderer _cachedMeshRenderer;

	public int age = 0;
	private bool isdead = false;
	private bool isdying = false;
	private bool isNewBorn = false;

	void Awake()
	{
        _cachedMeshRenderer = GetComponentInChildren<MeshRenderer>();
	}

	void OnEnable()
	{
		age = 0;
		isdying = false;
		isdead = isNewBorn = true;
		GameDirector.Age += Age;
	}

	void OnDisable()
	{
		GameDirector.Age += Age;
	}

	void Age()
	{
        if (isdead == false)
        {
            age++;
            _cachedMeshRenderer.material.SetFloat("_Age", age);
        }
	}

	public Cell SetNormal()
	{
		_cachedMeshRenderer.material = materials.normal;
		isNewBorn = isdying = isdead = false;
		return this;
	}
		
	public Cell SetNewBorn()
	{
		age = 0;
		isdying = false;
		isdead = isNewBorn = true;
		return this;
	}

	public Cell SetDying()
	{
		isNewBorn = isdead = false;
		isdying = true;
		return this;
	}

	public Cell SetDead()
	{
		_cachedMeshRenderer.material = materials.dead;
		isdead = true;
		isNewBorn = isdying = false;
		return this;
	}

	public bool IsDying()
	{
		return isdying;
	}

	public bool IsDead()
	{
		return isdead;
	}

	public bool IsNewBorn()
	{
		return isNewBorn;
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButton(0))
			SetNewBorn();
	}
}
