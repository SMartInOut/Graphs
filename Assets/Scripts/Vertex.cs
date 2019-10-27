using System;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
	public int ID;
	public string Label;
	public int Weight;
	public List<Vertex> Neighbours;
	public int OutDegree;
	public int InDegree;
	
	public event Action<Vertex> OnSelectVertex;
	
	private void OnMouseDown()
	{
		OnSelectVertex?.Invoke(this);
	}
}
