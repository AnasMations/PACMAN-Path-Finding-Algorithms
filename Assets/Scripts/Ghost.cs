using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(CircleCollider2D))]
public class Ghost : MonoBehaviour
{
    public bool scared = false;
    public bool eaten = false;
    bool teleported = false;
    [SerializeField]
    private float scaredTime = 5.0f;
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float scaredSpeed = 4.0f;
    public new Rigidbody2D rigidbody;
    Vector2 direction = Vector2.zero;
    Vector2 nextDirection = Vector2.zero;
    LayerMask obstacleLayer ;
    
    public Node lastNode;
    
    public Node destinationNode;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        Navigate();
    }
    private void FixedUpdate()
    {
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction *  (scared?scaredSpeed:speed);
        this.rigidbody.velocity = translation;
    }

    private void Navigate()
    {
        switch (GameManager.Instance.difficulty)
        {
            case Difficulty.Easy:
                break;
            case Difficulty.Normal:
                break;
            case Difficulty.Hard:
                break;
            case Difficulty.Coup:
                break;
        }
    }
    public void MoveUp()
    {
        SetDirection(Vector2.up);
    }
    public void MoveDown()
    {
        SetDirection(Vector2.down);
    }
    public void MoveLeft()
    {
        SetDirection(Vector2.left);
    }
    public void MoveRight()
    {
        SetDirection(Vector2.right);
    }
    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
            teleported = false;
        }
        else
        {
            this.nextDirection = direction;
        }
    }
    public bool Occupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.obstacleLayer);
        return hit.collider != null;
    }
    public void Scared()
    {
        StartCoroutine(ScaredHelper());
    }
    private IEnumerator ScaredHelper()
    {
        scared = true;
        yield return new WaitForSeconds(scaredTime);
        scared = false;
    }
    public void Eaten()
    {
        StopAllCoroutines();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "portal")
        {
            if (collision.name == "leftPortal" && !teleported)
            {
                transform.position = GameManager.Instance.portals[1].transform.position;
            }
            else if (collision.name == "rightPortal" && !teleported)
            {
                transform.position = GameManager.Instance.portals[0].transform.position;
            }
            if (!teleported)
                teleported = true;
        }
    }
}
