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
        edges=new List<Edge> ();
    }
    public Node((float, float) coordinates, Occupier occupier, List<Edge> edges)
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
    public List<Edge> edges;
    public void AddEdge(Node other,float distance)
    {
        AddEdge(new Edge(other,distance,Occupier.Empty));
    }
    public void AddEdge(Edge other)
    {
        foreach (Edge edge in edges)
            if (other.destination.xCoordinate == edge.destination.xCoordinate && other.destination.yCoordinate == edge.destination.yCoordinate)
            {
                return;
            }
        edges.Add(other);
    }
    public void RemoveEdge(Edge other)
    {
        edges.Remove(other);
    }
    public void RemoveEdge(Node other)
    {
        foreach (var node in edges)
        {
            if (node.destination == other)
            {
                other.RemoveEdge(node);
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
        foreach (var node in edges)
        {
            if (node.destination == source)
            {
                TransferOccupierToNode(node);
                return;
            }
        }
    }
    public void TransferOccupierFromNode(Node destination)
    {
        foreach (var node in edges)
        {
            if (node.destination == destination)
            {
                TransferOccupierFromNode(node);
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
    public List<Node> nodes { get; }
    public void AddNode(Node newNode)
    {
        nodes.Add(newNode);
    }
    public void RemoveNode(Node toBeRemoved)
    {
        nodes.Remove(toBeRemoved);
    }
}
public enum Occupier
{
    Pacman, Ghost,Empty
}