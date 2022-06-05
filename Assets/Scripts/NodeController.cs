using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class NodeController : MonoBehaviour
{
    public Node graphNode;
    public bool portalNode=false;
    public NodeController connectedPortal;
    public Vector2 portalDir;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        if (graphNode == null)
        graphNode = new Node();
        graphNode.ChangeCoordinates((transform.position.x, transform.position.y));
        graphNode.Occupy(Occupier.Empty);
        gameObject.layer = 10;
        if (portalNode)
        {
            graphNode.ChangeCoordinates((transform.position.x, transform.position.y));
            graphNode.AddEdge(connectedPortal.graphNode, 0, portalDir);
        }
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
