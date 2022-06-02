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
    public Vector2Int StartPosition;
    public Vector2 direction = Vector2.zero;
    Vector2 nextDirection = Vector2.zero;
    public LayerMask obstacleLayer ;
    [HideInInspector]
    public int remainingTime = 0;
    public Node lastNode;
    public Node destinationNode;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(Initialize());
    }

     private void Update()
        {
        if (this.nextDirection != Vector2.zero)
        {
            SetDirection(this.nextDirection);
        }
        if (Input.GetKey("up"))
        {
            this.nextDirection = Vector2.up;
        }
        else if (Input.GetKey("down"))
        {
            this.nextDirection = Vector2.down;
        }
        else if (Input.GetKey("left"))
        {
            this.nextDirection = Vector2.left;
        }
        else if (Input.GetKey("right"))
        {
            this.nextDirection = Vector2.right;
        }
    }
    private void FixedUpdate()
    {
        rigidbody.rotation = 0;
        rigidbody.angularVelocity = 0;
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction *  (scared?scaredSpeed:speed);
        this.rigidbody.velocity = translation;
    }
    private IEnumerator Initialize()
    {
        yield return null;
        StartCoroutine(Navigate());
    }
    private IEnumerator Navigate(string Target= "Pacman")
    {
        while (true)
        {
            yield return null;
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
            if (this.direction == -nextDirection)
            {
                destinationNode = lastNode;
            }
            this.direction = direction;
            rigidbody.rotation = 0;
            rigidbody.angularVelocity= 0;
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
        remainingTime = (int)scaredTime;
        for (int i = 0; i < scaredTime; i++)
        {
            remainingTime -= 1;
            yield return new WaitForSeconds(1);
        }
        scared = false;
    }
    public void Eaten()
    {
        StopAllCoroutines();
        StartCoroutine(returnToStart());
    }
    IEnumerator returnToStart()
    {
        GetComponent<AnimateGhost>().bodySprite.enabled = false;
        GetComponent<AnimateGhost>().eyesSprite.enabled = true;
        scared=false;
        eaten = true;
        while (new Vector2Int((int)transform.position.x,(int)transform.position.y) != StartPosition)
        {
            StartCoroutine(Navigate("Start"));
            yield return null;
        }
        StopAllCoroutines();
        StartCoroutine(Initialize());
        GetComponent<AnimateGhost>().bodySprite.enabled = true;
        eaten = false;
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (!scared)
                GameManager.Instance.PacmanEaten();
            else
                Eaten();
        }
    }
}
