using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody2D))]
public class Pacman : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }
    private bool teleported = false;
    public bool powerPelletActive = false;
    [SerializeField]
    private float powerPelletTime = 5.0f;
    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.startingPosition = this.transform.position;
    }
    public IEnumerator ActivatePowerPellet()
    {
        powerPelletActive = true;
        yield return new WaitForSeconds(powerPelletTime);
        powerPelletActive = false;
    }
    private void Start()
    {
        ResetState();
    }
    public void ResetState()
    {
        this.speedMultiplier = 1.0f;
        this.direction = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.startingPosition;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
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
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction * this.speed * this.speedMultiplier;
        this.rigidbody.velocity = translation;
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
            rigidbody.rotation = Vector3.Angle(this.direction, Vector2.right);
            if (this.direction == Vector2.down)
                rigidbody.rotation = -rigidbody.rotation;
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
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Pellet"))
        {
            GameManager.Instance.PelletEaten();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("PowerPellet"))
        {
            StartCoroutine(ActivatePowerPellet());
            GameManager.Instance.PowerPelletEaten();
            Destroy(collision.gameObject); Destroy(collision.gameObject);
        }
    }

}
