using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NodeController : MonoBehaviour
{
    public Node graphNode;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        if (graphNode == null)
        graphNode = new Node();
        graphNode.ChangeCoordinates((transform.position.x, transform.position.y));
        graphNode.Occupy(Occupier.Empty);
        gameObject.layer = 10;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int i = 0;
        foreach ((Vector2 k, Edge v) in graphNode.edges)
        {
            if (v.destination == null)
            {
                i++;
            }
        }
        /*
        if (collision.name.Contains("Pacman"))
        {
            graphNode.Occupy(Occupier.Pacman);
            if (graphNode.edges.ContainsKey(-collision.GetComponent<Pacman>().direction))
                graphNode.edges[collision.GetComponent<Pacman>().direction].destination.edges[-collision.GetComponent<Pacman>().direction].occupier = Occupier.Empty;
        }
        else if (collision.name.Contains("Ghost"))
        {
            graphNode.Occupy(Occupier.Ghost);
            if (graphNode.edges.ContainsKey(-collision.GetComponent<Ghost>().direction))

                graphNode.edges[collision.GetComponent<Ghost>().direction].destination.edges[-collision.GetComponent<Ghost>().direction].occupier = Occupier.Empty;

        }
        */
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Contains("Pacman"))
        {
            graphNode.Occupy(Occupier.Empty);
            if (graphNode.edges.ContainsKey(collision.GetComponent<Pacman>().direction))
                graphNode.edges[collision.GetComponent<Pacman>().direction].occupier=Occupier.Pacman;
        }
        else if (collision.name.Contains("Ghost"))
        {
            graphNode.Occupy(Occupier.Empty);
            if(graphNode.edges.ContainsKey(collision.GetComponent<Ghost>().direction))
                graphNode.edges[collision.GetComponent<Ghost>().direction].occupier = Occupier.Ghost;
        }
    }
}
