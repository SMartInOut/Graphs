using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
	#region - Properties -
	public GameObject VertexPrefab;
	public GameObject EdgePrefab;
	public List<Vertex> Vertices;
	public List<Edge> Edges;
	public int VertexCount => Vertices.Count;
	public int EdgeCount => Edges.Count;
	#endregion
	
	#region - Private fields -
	private Vertex selectedVertex;
	private Edge selectedEdge;
	#endregion
	
	public bool IsStronglyConnected()
	{
		return false;
	}

	public bool IsWeaklyConnected()
	{
		return false;
	}
	
	public IEnumerable<(Vertex Vertex, Vertex Predecessor, int Distance)> TraverseBFS(Vertex root)
	{
		var queue = new Queue<Vertex>();
		var visited = new bool[VertexCount];
		var bfsInfos = new List<(Vertex Vertex, Vertex Predecessor, int Distance)>();
		
		var rootIndex = Vertices.IndexOf(root);
		visited[rootIndex] = true;
		bfsInfos.Add((root, null, 0));
		queue.Enqueue(root);
		
		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			foreach (var neighbour in current.Neighbours)
			{
				var neighbourIndex = Vertices.IndexOf(neighbour);
				if (!visited[neighbourIndex])
				{
					visited[neighbourIndex] = true;
					bfsInfos.Add((neighbour, current, bfsInfos.Find(info => info.Vertex == current).Distance + 1));
					queue.Enqueue(neighbour);
				}
			}
		}
		
		return bfsInfos;
	}
	
	#region - Selection -
	public void SelectVertex(Vertex vertex)
	{
		selectedVertex = vertex;
		selectedEdge = null;
		Debug.Log("Vertex " + selectedVertex.Label + " selected");
	}
	
	public void SelectEdge(Edge edge)
	{
		selectedEdge = edge;
		selectedVertex = null;
		Debug.Log("Edge " + selectedEdge.Label + " selected");
	}
	#endregion
	
	#region - MonoBehaviour methods -
	private void Reset()
	{
	}
	
	// Start is called before the first frame update
	private void Start()
    {
		
    }
	
    // Update is called once per frame
    private void Update()
    {
        //if (Input.OnMouseButtonDown(0))
		{
			// if mouse hits collider of vertex v
			// drag v to mouseposition
			// edges have to change accordingly
		}
    }
	#endregion
}
