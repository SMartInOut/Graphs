using System;
using UnityEngine;

public class Edge : MonoBehaviour
{
	public int ID;
	public Vertex Source;
	public Vertex Target;
	public string Label;
	public int Weight;
	
	public event Action<Edge> OnSelectEdge;
	
	private void OnMouseDown()
	{
		OnSelectEdge?.Invoke(this);
	}
}
