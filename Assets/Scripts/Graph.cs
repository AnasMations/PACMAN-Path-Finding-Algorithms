using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node()
    {
        occupier = Occupier.Empty;
        coordinates = (0, 0);
        edges=new HashSet<(Node, int,Occupier)> ();
    }
    public Node((int, int) coordinates, Occupier occupier, HashSet<(Node, int, Occupier)> edges)
    {
        this.coordinates = coordinates;
        this.occupier = occupier;
        this.edges = edges;
    }
    [SerializeField]
    public (int, int) coordinates { get; private set; }
    public Occupier occupier = Occupier.Empty;
    [SerializeField]
    public HashSet<(Node, int, Occupier)> edges { get; private set; }
    public void AddEdge(Node other,int distance)
    {
        edges.Add((other,distance,Occupier.Empty));
    }
    public void AddEdge((Node, int,Occupier) other)
    {
        edges.Add(other);
    }
    public void RemoveEdge((Node ,int,Occupier)other)
    {
        edges.Remove(other);
    }
    public void RemoveEdge(Node other)
    {
        foreach (var node in edges)
        {
            if (node.Item1 == other)
            {
                other.RemoveEdge(node);
                return;
            }
        }
    }
    public void ChangeCoordinates((int, int) newCoordinates)
    {
        coordinates=newCoordinates;
    }
    public void Occupy(Occupier nodeOccupier)
    {
        occupier = nodeOccupier;
    }
    public void TransferOccupierToNode(Node source)
    {
        foreach (var node in edges)
        {
            if (node.Item1 == source)
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
            if (node.Item1 == destination)
            {
                TransferOccupierFromNode(node);
                return;
            }
        }
    }
    public void TransferOccupierToNode((Node, int, Occupier) source)
    {
        occupier = source.Item3;
        source.Item3 = Occupier.Empty;
    }

    public void TransferOccupierFromNode((Node, int, Occupier) destination)
    {
        destination.Item3 = occupier;
        occupier = Occupier.Empty;
    }

}
public class Graph
{
    public Graph()
    {
        nodes = new HashSet<Node>();
    }
    HashSet<Node> nodes;
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