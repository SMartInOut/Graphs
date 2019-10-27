using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
	#region - Properties -
	public GameObject VertexPrefab;
	public GameObject EdgePrefab;
	public int VertexCount = 10;
	public int EdgeCount = 30;
	#endregion
	
	#region - Private fields -
	private Vertex selectedVertex;
	private List<Vertex> vertices;
	
	private Edge selectedEdge;
	private List<Edge> edges;
	#endregion
	
	public bool IsStronglyConnected()
	{
		return false;
	}

	public bool IsWeaklyConnected()
	{
		return false;
	}
	
	public IEnumerator TraverseBFS(Vertex root)
	{
		Queue<Vertex> queue = new Queue<Vertex>(root.Neighbours);
		
		while (queue.Count > 0)
		{
			var next = queue.Dequeue();
			foreach (var neighbour in next.Neighbours)
			{
				if (!queue.Contains(neighbour))
				{
					queue.Enqueue(neighbour);
				}
			}
		}

		return null;
	}
	
	#region - Initialization -
	private void GenerateRandomGraph()
	{
		GenerateVertices();
		GenerateEdges();
		
		Camera.main.orthographicSize = VertexCount * 1.1f;
	}
	
	private void GenerateVertices()
	{
		vertices = new List<Vertex>(VertexCount);
		
		GameObject verticesGO = new GameObject("Vertices");
		verticesGO.transform.parent = transform;
		
		float anglePerVertex = 2 * Mathf.PI / VertexCount;
		
		for (int i = 0; i < VertexCount; i++)
		{
			var vertexPosition = VertexCount * new Vector2(Mathf.Sin(anglePerVertex * i), Mathf.Cos(anglePerVertex * i));
			
			GameObject vertexObject = Instantiate(VertexPrefab, vertexPosition, Quaternion.identity);
			vertexObject.name = "Vertex" + i;
			vertexObject.transform.parent = verticesGO.transform;
			
			Vertex vertex = vertexObject.GetComponent<Vertex>();
			vertex.ID = i;
			vertex.Label = i.ToString();
			vertex.Weight = 0;
			vertex.OnSelectVertex += SelectVertex;
			
			vertices.Add(vertex);
		}
	}
	
	private void GenerateEdges()
	{
		edges = new List<Edge>(EdgeCount);
		
		GameObject edgesGO = new GameObject("Edges");
		edgesGO.transform.parent = transform;
		
		System.Random prng = new System.Random();
		
		for (int i = 0; i < EdgeCount; i++)
		{
			int sourceIndex;
			int targetIndex;
			
			// Retry to get a new edge by getting a new source and target if the edge with chosen source and target already exists (no multigraph)
			do
			{
				sourceIndex = prng.Next(0, vertices.Count);
				targetIndex = prng.Next(0, vertices.Count);
				
				// Retry to get target if source and target are the same vertex (no loops)
				while (sourceIndex == targetIndex)
				{
					targetIndex = prng.Next(0, vertices.Count);
				}
			}
			while (edges.Exists(e => e.Source == vertices[sourceIndex] && e.Target == vertices[targetIndex]));
			
			var source = vertices[sourceIndex];
			var target = vertices[targetIndex];
			var edgeEndPoints = new Vector3[] { source.transform.position, target.transform.position };
			var edgePosition = edgeEndPoints[0] + (edgeEndPoints[1] - edgeEndPoints[0]) / 2;
			
			GameObject edgeObject = Instantiate(EdgePrefab, edgePosition, Quaternion.identity);
			edgeObject.name = "Edge" + sourceIndex + "-" + targetIndex;
			edgeObject.GetComponent<LineRenderer>().SetPositions(edgeEndPoints);
			edgeObject.transform.parent = edgesGO.transform;
			
			Edge edge = edgeObject.GetComponent<Edge>();
			edge.ID = i;
			edge.Source = source;
			edge.Target = target;
			edge.Label = i.ToString();
			edge.Weight = 0;
			edge.OnSelectEdge += SelectEdge;
			
			edges.Add(edge);
			
			source.Neighbours.Add(target);
			source.OutDegree++;
			target.InDegree++;
		}
		
		/*foreach (var edge in edges)
		{
			if (edges.Exists(e => e.Source == edge.Target && e.Target == edge.Source))
			{
				var edgeLR = edge.GetComponent<LineRenderer>();
				var edgeDirection = (edgeLR.GetPosition(1) - edgeLR.GetPosition(0)).normalized;
				edgeLR.positionCount = 5;
				edgeLR.SetPosition(4, edgeLR.GetPosition(1));
				edgeLR.useWorldSpace = false;
				edgeLR.SetPosition(1, Vector3.right * 0.4f + edgeDirection * 0.25f);
				edgeLR.SetPosition(2, Vector3.right);
				edgeLR.SetPosition(3, Vector3.right * 0.4f + edgeDirection * 0.25f);
				edgeLR.useWorldSpace = true;
			}
		}*/
	}
	#endregion
	
	#region - Selection -
	private void SelectVertex(Vertex vertex)
	{
		selectedVertex = vertex;
		selectedEdge = null;
		Debug.Log(selectedVertex.GetComponent<Vertex>().Label + " selected");
	}
	
	private void SelectEdge(Edge edge)
	{
		selectedEdge = edge;
		selectedVertex = null;
		Debug.Log(selectedEdge.GetComponent<Edge>().Label + " selected");
	}
	#endregion
	
	#region - MonoBehaviour methods -
	private void Reset()
	{
		VertexCount = 10;
		EdgeCount = 30;
	}
	
	// Start is called before the first frame update
	private void Start()
    {
		GenerateRandomGraph();
    }
	
    // Update is called once per frame
    private void Update()
    {
        
    }
	#endregion
}
