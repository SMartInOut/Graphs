using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
	public Graph Graph;

	public int VertexCount = 10;
	public int EdgeCount = 30;
	
	#region - Initialization -
	public void GenerateRandomGraph(bool isDirected)
	{
		GenerateVertices();
		GenerateEdges(isDirected);
	}
	
	private void GenerateVertices()
	{
		Graph.Vertices = new List<Vertex>(VertexCount);
		
		GameObject verticesGO = new GameObject("Vertices");
		verticesGO.transform.parent = Graph.transform;
		
		float anglePerVertex = 2 * Mathf.PI / VertexCount;
		
		// Arrange the vertices in a circle
		for (int i = 0; i < VertexCount; i++)
		{
			var vertexPosition = VertexCount * new Vector2(Mathf.Sin(anglePerVertex * i), Mathf.Cos(anglePerVertex * i));
			GenerateVertex(i, i.ToString(), vertexPosition).transform.parent = verticesGO.transform;
		}
	}

	private GameObject GenerateVertex(int id, string label, Vector3 position)
	{
		GameObject vertexObject = Instantiate(Graph.VertexPrefab, position, Quaternion.identity);
		vertexObject.name = "Vertex" + id;
		
		Vertex vertex = vertexObject.GetComponent<Vertex>();
		vertex.ID = id;
		vertex.Label = label;
		vertex.Weight = 0;
		vertex.OnSelectVertex += Graph.SelectVertex;
		
		Graph.Vertices.Add(vertex);

		return vertexObject;
	}

	private void GenerateEdges(bool isDirected)
	{
		Graph.Edges = new List<Edge>(EdgeCount);
		
		GameObject edgesGO = new GameObject("Edges");
		edgesGO.transform.parent = Graph.transform;
		
		System.Random prng = new System.Random();
		
		for (int i = 0; i < (isDirected ? EdgeCount : EdgeCount * 2); i += (isDirected ? 1 : 2))
		{
			int sourceIndex;
			int targetIndex;
			
			// Retry to get a new edge by getting a new source and target if the edge with chosen source and target already exists (no multigraph)
			do
			{
				sourceIndex = prng.Next(0, Graph.Vertices.Count);
				targetIndex = prng.Next(0, Graph.Vertices.Count);
				
				// Retry to get target if source and target are the same vertex (no loops)
				while (sourceIndex == targetIndex)
				{
					targetIndex = prng.Next(0, Graph.Vertices.Count);
				}
			}
			while (Graph.Edges.Exists(e => e.Source == Graph.Vertices[sourceIndex] && e.Target == Graph.Vertices[targetIndex]
										|| (!isDirected && e.Source == Graph.Vertices[targetIndex] && e.Target == Graph.Vertices[sourceIndex])));
			
			var source = Graph.Vertices[sourceIndex];
			var target = Graph.Vertices[targetIndex];
			GenerateEdge(source, target, i, i.ToString()).transform.parent = edgesGO.transform;

			if (!isDirected)
			{
				GenerateEdge(target, source, i+1, i+1.ToString()).transform.parent = edgesGO.transform;
			}
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

	private GameObject GenerateEdge(Vertex source, Vertex target, int id, string label)
	{
		var edgeEndPoints = new Vector3[] { source.transform.position, target.transform.position };
		var edgePosition = edgeEndPoints[0] + (edgeEndPoints[1] - edgeEndPoints[0]) / 2;

		// LineRenderer and EdgeCollider2D work with local positions, therefore the edgeEndPoints have to be recalculated from world to local coordinates
		var edgeEndPointsLocal = new Vector3[] { edgeEndPoints[0] - edgePosition, edgeEndPoints[1] - edgePosition };
		
		GameObject edgeObject = Instantiate(Graph.EdgePrefab, edgePosition, Quaternion.identity);
		edgeObject.name = "Edge" + source.ID + "-" + target.ID;
		var lineRenderer = edgeObject.GetComponent<LineRenderer>();
		lineRenderer.SetPositions(edgeEndPointsLocal);
		var edgeCollider = edgeObject.GetComponent<EdgeCollider2D>();
		edgeCollider.points = edgeEndPointsLocal.Select(eep => (Vector2)eep).ToArray();
		
		Edge edge = edgeObject.GetComponent<Edge>();
		edge.ID = id;
		edge.Source = source;
		edge.Target = target;
		edge.Label = label;
		edge.Weight = 0;
		edge.OnSelectEdge += Graph.SelectEdge;
		
		Graph.Edges.Add(edge);
		
		source.Neighbours.Add(target);
		source.OutDegree++;
		target.InDegree++;

		return edgeObject;
	}
	#endregion

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		foreach (var edge in Graph.Edges)
		{
			if (edge != null)
			{
				Gizmos.DrawLine(edge.transform.TransformPoint(edge.GetComponent<EdgeCollider2D>().points[0]), edge.transform.TransformPoint(edge.GetComponent<EdgeCollider2D>().points[1]));
			}
		}
	}
	
	// Start is called before the first frame update
	private void Start()
	{
		Graph = Instantiate(Graph, transform.position, Quaternion.identity);
		GenerateRandomGraph(true);
		var output = Graph.TraverseBFS(Graph.Vertices[0]);
		foreach (var item in output)
		{
			Debug.Log("(" + item.Vertex.Label + ", " + (item.Predecessor?.Label ?? "null") + ", " + item.Distance + ")");
		}

		Camera.main.orthographicSize = Graph.VertexCount * 1.1f;
	}

	// Update is called once per frame
	private void Update()
	{
		
	}
}
