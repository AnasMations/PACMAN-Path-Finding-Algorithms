using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class NodeController : MonoBehaviour
{
    public Node graphNode;
    // Start is called before the first frame update
    void Start()
    {
        graphNode = new Node();
        graphNode.ChangeCoordinates(((int)transform.position.x, (int)transform.position.y));
        graphNode.Occupy(Occupier.Empty);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
