using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    private Vector2Int StartPosition;
    public Vector2 direction = Vector2.zero;
    Vector2 nextDirection = Vector2.zero;
    public LayerMask obstacleLayer ;
    public LayerMask nodeLayer;
    [HideInInspector]
    public int remainingTime = 0;
    public Node lastNode;
    public Node nextDestinationNode;
    public Node destinationNode;
    public Vector2 startNodeCoordinates;
    public delegate void navigator(Node desitation);
    navigator navi;
    private Transform pacmanPos;
    // Start is called before the first frame update
    void Start()
    {
        pacmanPos = GameObject.Find("Pacman").transform;
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(Initialize());
        StartPosition = new Vector2Int(0, 2);
    }

    private void Update()
    {
        /*
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
            }*/
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
        int dir = (transform.position.x - transform.parent.position.x) > 0 ? 1 : -1;
        while ((transform.position.x > transform.parent.position.x+0.05f)|| (transform.position.x < transform.parent.position.x - 0.05f))
        {
            yield return null;
            dir = (transform.position.x - transform.parent.position.x) > 0 ? 1 : -1;
            SetDirection(dir>0?Vector2.left:Vector2.right);
        }
        while (!Occupied(Vector3.up))
        {
            yield return null;
            SetDirection(Vector2.up);
        }
        yield return null;
        while (Mathf.Abs(transform.position.x)< Mathf.Abs (startNodeCoordinates.x))
        { 
            SetDirection((dir > 0 ? Vector2.left : Vector2.right));
            yield return null;
        }

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.Instance.pacman.GetComponent<Collider2D>(), false);
        GetComponent<AnimateGhost>().bodySprite.enabled = true;
        eaten = false;
        scared = false;
        StartCoroutine(Navigate());

    }

    private IEnumerator Navigate(string Target= "Pacman")
    {
        while (true)
        {
            yield return null;
            if (Target == "Pacman" && eaten)
            {
                Target = "Start";
            }
            switch (GameManager.Instance.difficulty)
            {
                case Difficulty.Easy:
                    navi = randDir;
                    break;
                case Difficulty.Normal:
                    navi = Astar;
                    break;
                case Difficulty.Hard:
                    break;
                case Difficulty.Coup:
                    break;
                default:
                    yield break;
            }
            switch (Target)
            {
                case "Pacman":
                    destinationNode = GameManager.Instance.pacman.destinationNode;    
                    break;
                case "Start":
                    destinationNode = GameManager.Instance.map.getNodeWithCoordinates(startNodeCoordinates.x*(transform.position.x>0?1:-1),startNodeCoordinates.y);
                    break;
            }
            if(navi!=null)
                navi(destinationNode);
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
                nextDestinationNode = lastNode;
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
        StopCoroutine(Navigate());
        StartCoroutine(returnToStart());
    }
    IEnumerator returnToStart()
    {
        GetComponent<AnimateGhost>().bodySprite.enabled = false;
        GetComponent<AnimateGhost>().eyesSprite.enabled = true;
        scared=false;
        eaten = true;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.Instance.pacman.GetComponent<Collider2D>(), true);
        StartCoroutine(Navigate("Start"));
        while (new Vector2(transform.position.x,transform.position.y) != startNodeCoordinates)
        {
            yield return null;
        }
        while ((int)transform.position.y!= StartPosition.y)
        {
            yield return null;
            SetDirection(Vector2.down);
        }

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
        if (collision.name.Contains("Node"))
        {
            lastNode = collision.GetComponent<NodeController>().graphNode;
            if (lastNode.edges.ContainsKey(direction))
                nextDestinationNode = lastNode.edges[direction].destination;
            else
                nextDestinationNode = lastNode;
        }
        

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Respawn")
        {
            eaten= false;
            StopAllCoroutines();
            StartCoroutine(Initialize());
        }
    }

    private void randDir(Node Destination =null)
    {
        List<Vector2> directions = lastNode.edges.Keys.ToList<Vector2>();
        directions.Remove(-direction);
        int i = 0;
        if (directions.Count > 0)
        {
            i = Mathf.RoundToInt(Random.Range(0, directions.Count));
            while (directions.Count > 0 && Occupied(directions[i]))
            {
                directions.RemoveAt(i);
                i = Mathf.RoundToInt(Random.Range(0, directions.Count));
            }
        }
        i = Mathf.RoundToInt(Random.Range(0, directions.Count));
        if (i == directions.Count)
        {
            i = Mathf.RoundToInt(Random.Range(0, directions.Count - 1));
        }
        if (directions.Count > i)
            SetDirection(directions[i]);
    }
    public void Djkstra(Node Destination)
    {

    }

    public void Astar(Node Destination)
    {
        List<Vector2> directions = lastNode.edges.Keys.ToList<Vector2>();
        Debug.Log(directions[0]);
        //directions.Remove(-direction);

        direction = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach(Vector2 availableDirection in directions)
        {
            Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y, 0.0f);
            float distance = (this.transform.position - newPosition).sqrMagnitude;

            if(minDistance > distance)
            {
                direction = availableDirection;
                minDistance = distance;
            }
        }

        SetDirection(direction);

    }

    public void simpleRandom(Node Destination)
    {
        List<Vector2> directions = lastNode.edges.Keys.ToList<Vector2>();
        directions.Remove(-direction);
        SetDirection(directions[Mathf.RoundToInt(Random.Range(0, directions.Count))]);
    }

    public void Amoon(Node Destination)
    {
        List<Vector2> directions = lastNode.edges.Keys.ToList<Vector2>();
        directions.Remove(-direction);
        Vector2 distance = this.gameObject.transform.position - pacmanPos.position;
        distance.x = Mathf.RoundToInt(distance.x);
        distance.y = Mathf.RoundToInt(distance.y);

        if(distance.x > 0 && !Occupied(Vector2.left))
        {
            SetDirection(Vector2.left);
        }
        else if(distance.x < 0 && !Occupied(Vector2.right))
        {
            SetDirection(Vector2.right);
        }
        else if(distance.y < 0 && !Occupied(Vector2.up))
        {
            SetDirection(Vector2.up);
        }
        else if(distance.y > 0 && !Occupied(Vector2.down))
        {
            SetDirection(Vector2.down);
        }
        else
        {
            SetDirection(directions[Mathf.RoundToInt(Random.Range(0, directions.Count))]);
        }
        Debug.Log(distance);
    }

    /*public void KindaGood(Node Destination)
    {
        if (Destination == null)
            return;
        if (Mathf.Abs(destinationNode.yCoordinate - transform.position.y) > Mathf.Abs(destinationNode.xCoordinate - transform.position.x) && !Occupied(Vector2.up * (destinationNode.yCoordinate - transform.position.y > 0 ? 1 : -1)))
        {
            SetDirection(Vector2.up * (destinationNode.yCoordinate - transform.position.y > 0 ? 1 : -1));
        }
        else if(!Occupied(Vector2.right * (destinationNode.xCoordinate - transform.position.x > 0 ? 1 : -1)))
        {
            SetDirection(Vector2.right * (destinationNode.xCoordinate - transform.position.x > 0 ? 1 : -1));
        }
        else
        {
            if (Mathf.Abs(destinationNode.yCoordinate - transform.position.y) > Mathf.Abs(destinationNode.xCoordinate - transform.position.x))
            {
                if (lastNode.edges[Vector2.right * (destinationNode.xCoordinate - transform.position.x > 0 ? 1 : -1)*-1].destination.edges.ContainsKey(Vector2.up * (destinationNode.yCoordinate - transform.position.y > 0 ? 1 : -1)))
                    SetDirection(Vector2.right * (destinationNode.xCoordinate - transform.position.x > 0 ? 1 : -1) * -1);
            }
            else
            {
                if (lastNode.edges[Vector2.up * (destinationNode.yCoordinate - transform.position.y > 0 ? 1 : -1)*-1].destination.edges.ContainsKey(Vector2.right *(destinationNode.xCoordinate - transform.position.x > 0 ? 1 : -1)))
                    SetDirection(Vector2.up * (destinationNode.yCoordinate - transform.position.y > 0 ? 1 : -1)*-1);

            }
        }

    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (!scared&&!eaten)
                GameManager.Instance.PacmanEaten();
            else
                Eaten();
        }
    }
}
