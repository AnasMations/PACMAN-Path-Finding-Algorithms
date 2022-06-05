using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node()
    {
        occupier = Occupier.Empty;
        xCoordinate= 0;
        yCoordinate= 0;
        edges=new Dictionary<Vector2,Edge> ();
    }
    public Node((float, float) coordinates, Occupier occupier, Dictionary<Vector2,Edge> edges)
    {
        this.xCoordinate = coordinates.Item1;
        this.yCoordinate = coordinates.Item2;
        this.occupier = occupier;
        this.edges = edges;
    }
    [SerializeField]
    public float xCoordinate;
    public float yCoordinate;
    public Occupier occupier = Occupier.Empty;
    [SerializeField]
    public Dictionary<Vector2,Edge> edges;
    public void AddEdge(Node other,float distance,Vector2 dir)
    {
        AddEdge(new Edge(other,distance,Occupier.Empty),dir);
    }
    public void AddEdge(Edge other,Vector2 dir)
    {
        foreach (Edge edge in edges.Values)
            if(edge.destination!=null&&other.destination!=null)
                if (other.destination.xCoordinate == edge.destination.xCoordinate && other.destination.yCoordinate == edge.destination.yCoordinate)
                {
                    return;
                }
        edges.Add(dir,other);
    }
    public void RemoveEdge(Edge other)
    {
        foreach ((Vector2 k, Edge v) in edges)
            if (v == other)
            {
                edges.Remove(k);
                return;
            }


    }
    public void RemoveEdge(Node other)
    {
        foreach ((var dir,var edge)in edges)
        {
            if (edge.destination == other)
            {
                other.RemoveEdge(edge);
                return;
            }
        }
    }
    public void ChangeCoordinates((float, float) newCoordinates)
    {
        xCoordinate=newCoordinates.Item1;
        yCoordinate=newCoordinates.Item2;
    }
    public void Occupy(Occupier nodeOccupier)
    {
        occupier = nodeOccupier;
    }
    public void TransferOccupierToNode(Node source)
    {
        foreach ((var dir,var edge) in edges)
        {
            if (edge.destination == source)
            {
                TransferOccupierToNode(edge);
                return;
            }
        }
    }
    public void TransferOccupierFromNode(Node destination)
    {
        foreach ((var dir, var edge)in edges)
        {
            if (edge.destination == destination)
            {
                TransferOccupierFromNode(edge);
                return;
            }
        }
    }
    public void TransferOccupierToNode(Edge source)
    {
        occupier = source.occupier;
        source.occupier= Occupier.Empty;
    }

    public void TransferOccupierFromNode(Edge destination)
    {
        destination.occupier = occupier;
        occupier = Occupier.Empty;
    }

}
[System.Serializable]
public class Edge
{
    public Node destination;
    public float cost;
    public Occupier occupier;

    public Edge(Node destination, float cost, Occupier occupier)
    {
        this.destination = destination;
        this.cost = cost;
        this.occupier = occupier;
    }
}

public class Graph
{
    public Graph()
    {
        nodes = new List<Node>();
    }
    public List<Node> nodes {get; private set;}
    public void AddNode(Node newNode)
    {
        nodes.Add(newNode);
    }
    public void RemoveNode(Node toBeRemoved)
    {
        nodes.Remove(toBeRemoved);
    }
    public Node getNodeWithCoordinates(float x, float y)
    {
        foreach (Node node in nodes)
        {
            if (node.xCoordinate == x && node.yCoordinate == y)
                return node;
        }
        return null;
    }
    public HashSet<(Node , Node , float, Vector2)> edgeList = new HashSet<(Node , Node , float, Vector2)> ();
}
public enum Occupier
{
    Pacman, Ghost,Empty
}