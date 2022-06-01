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

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
           
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
