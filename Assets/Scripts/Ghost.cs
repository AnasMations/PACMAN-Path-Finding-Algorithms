using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Ghost : MonoBehaviour
{
    public bool scared = false;
    public bool eaten = false;
    private bool initilized = false;
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
    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;
    [HideInInspector]
    public int remainingTime = 0;
    public Node lastNode;
    public Node nextDestinationNode;
    public Node destinationNode;
    public Vector2 startNodeCoordinates;
    public Vector2 InitNode;
    public delegate void navigator(Node desitation);
    navigator navi;
    private Transform pacmanPos;
    public DestinationSelector destinator;
    // Start is called before the first frame update
    void Start()
    {
        pacmanPos = GameObject.Find("Pacman").transform;
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(Initialize());
        StartPosition = new Vector2Int(0, 2);
    }

    public void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(Initialize());
        StartPosition = new Vector2Int(0, 2);
    }
    
    private void Update()
    {

        if (nextDestinationNode == null)
        {
            nextDestinationNode = lastNode;
        }
    }

    private void FixedUpdate()
    {
        rigidbody.rotation = 0;
        rigidbody.angularVelocity = 0;
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction * (scared ? scaredSpeed : speed);
        this.rigidbody.velocity = translation;

    }
    
    private IEnumerator Initialize()
    {
        int dir = (transform.position.x - transform.parent.position.x) > 0 ? 1 : -1;
        while ((transform.position.x > transform.parent.position.x + 0.05f) || (transform.position.x < transform.parent.position.x - 0.05f))
        {
            yield return null;
            dir = (transform.position.x - transform.parent.position.x) > 0 ? 1 : -1;
            SetDirection(dir > 0 ? Vector2.left : Vector2.right);
        }
        while (!Occupied(Vector2.up))
        {
            yield return null;
            SetDirection(Vector2.up);
        }
        yield return null;
        while (Mathf.Abs(transform.position.x) < Mathf.Abs(startNodeCoordinates.x))
        {
            SetDirection((dir > 0 ? Vector2.left : Vector2.right));
            yield return null;
        }

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.Instance.pacman.GetComponent<Collider2D>(), false);
        GetComponent<AnimateGhost>().bodySprite.enabled = true;
        eaten = false;
        scared = false;
        
        StartCoroutine(Navigate("Init"));
        while (lastNode.xCoordinate != InitNode.x && lastNode.yCoordinate!= InitNode.y)
        {
            yield return null;
        }
        StopCoroutine(Navigate("Init"));
        initilized= true;
        yield return null;
        StartCoroutine(Navigate());

    }

    private IEnumerator Navigate(string Target = "Pacman")
    {
        while (true)
        {
            yield return null;
            if (!initilized)
            {

                Target = "Init";
            }
            else
                Target = "Pacman";
            
            if (Target == "Pacman" && eaten)
            {
                Target = "Start";
            }
            switch (GameManager.Instance.difficulty)
            {
                case Difficulty.Easy:
                    navi = bellman;
                    break;
                case Difficulty.Normal:
                    navi = Astar;
                    break;
                case Difficulty.Hard:
                    navi = Dijkstra;
                    break;
                case Difficulty.Coup:
                    navi = BFS;
                    break;
                default:
                    yield break;
            }
            switch (Target)
            {
                case "Pacman":
                    destinationNode = GameManager.Instance.pacman.destinationNode;
                    switch (destinator)
                    {
                        case DestinationSelector.BehindPacman:
                            destinationNode = GameManager.Instance.pacman.lastNode;
                            break;
                        case DestinationSelector.TwoInfornt:
                            destinationNode = destinationNode.edges[GameManager.Instance.pacman.direction].destination;
                            if (destinationNode == null)
                            {
                                destinationNode = GameManager.Instance.pacman.destinationNode;
                                if (GameManager.Instance.pacman.direction.y == 0)
                                {
                                    destinationNode = destinationNode.edges[Vector2.up].destination;
                                    if (destinationNode == null)
                                    {
                                        destinationNode = GameManager.Instance.pacman.destinationNode;
                                        destinationNode = destinationNode.edges[Vector2.down].destination;
                                        if (destinationNode == null)
                                        {
                                            destinationNode = GameManager.Instance.pacman.destinationNode;
                                        }
                                    }
                                }
                                else
                                {
                                    destinationNode = GameManager.Instance.pacman.destinationNode;
                                    destinationNode = destinationNode.edges[Vector2.left].destination;
                                    if (destinationNode == null)
                                    {
                                        destinationNode = GameManager.Instance.pacman.destinationNode;
                                        destinationNode = destinationNode.edges[Vector2.right].destination;
                                        if (destinationNode == null)
                                        {
                                            destinationNode = GameManager.Instance.pacman.destinationNode;
                                        }
                                    }
                                }
                            }
                            break;
                        case DestinationSelector.SideNode:
                            if (GameManager.Instance.pacman.direction.y == 0)
                            {
                                destinationNode = destinationNode.edges[Vector2.up].destination;
                                if (destinationNode == null)
                                {
                                    destinationNode = GameManager.Instance.pacman.lastNode;
                                    destinationNode = destinationNode.edges[Vector2.down].destination;
                                    if (destinationNode == null)
                                    {
                                        destinationNode = GameManager.Instance.pacman.destinationNode;
                                    }
                                }
                            }

                            else
                            {
                                destinationNode = destinationNode.edges[Vector2.left].destination;
                                if (destinationNode == null)
                                {
                                    destinationNode = GameManager.Instance.pacman.lastNode;
                                    destinationNode = destinationNode.edges[Vector2.right].destination;
                                    if (destinationNode == null)
                                    {
                                        destinationNode = GameManager.Instance.pacman.destinationNode;
                                    }
                                }
                            }
                            break;
                        case DestinationSelector.InFrontOfPacman:
                            destinationNode = GameManager.Instance.pacman.destinationNode;
                            break;
                        case DestinationSelector.DirectLineOfSight:
                            while (destinationNode.edges[GameManager.Instance.pacman.direction].destination != null)
                            {
                                destinationNode = destinationNode.edges[GameManager.Instance.pacman.direction].destination;
                            }
                            break;
                    }
                    break;
                case "Start":
                    destinationNode = GameManager.Instance.map.getNodeWithCoordinates(startNodeCoordinates.x * (transform.position.x > 0 ? 1 : -1), startNodeCoordinates.y);
                    break;
                case "Init":

                    destinationNode = GameManager.Instance.map.getNodeWithCoordinates(InitNode.x, InitNode.y);
                    break;
            }

            
            if (destinationNode == null)
            {

                destinationNode = GameManager.Instance.pacman.lastNode;
            }

            if (Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 5f, LayerMask.NameToLayer("Pacman")).collider != null&&!(scared||eaten))
            {
                destinationNode = GameManager.Instance.pacman.destinationNode;
            }
            if (navi != null && !scared)
            {
                navi(destinationNode);
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
                nextDestinationNode = lastNode;
            }
            this.direction = direction;
            rigidbody.rotation = 0;
            rigidbody.angularVelocity = 0;
            this.nextDirection = Vector2.zero;

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

    public bool FutureOccupied(Vector2 futurePos, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(futurePos, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.obstacleLayer);
        return hit.collider != null;
    }

    public void Scared()
    {
        if (eaten)
        {
            scared = false;
            return;
        }
        SetDirection(-direction);
        StartCoroutine(ScaredHelper());
    }

    private IEnumerator ScaredHelper()
    {
        remainingTime += (int)scaredTime;
        scared = true;
        while (remainingTime > 0)
        {
            remainingTime -= 1;
            scared = true;
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
        scared = false;
        eaten = true;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.Instance.pacman.GetComponent<Collider2D>(), true);
        StopCoroutine(Navigate());
        StartCoroutine(Navigate("Start"));
        while (new Vector2(lastNode.xCoordinate, lastNode.yCoordinate) != new Vector2(startNodeCoordinates.x * (transform.position.x > 0 ? 1 : -1), startNodeCoordinates.y))
        {
            yield return null;
        }
        while ((int)transform.position.x != StartPosition.x)
        {
            SetDirection(Vector2.right * (transform.position.x > 0 ? -1 : 1));
            yield return null;
        }
        while (transform.position.y != StartPosition.y)
        {
            yield return null;
            SetDirection(Vector2.down);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Node"))
        {
            if (scared)
            {
                List<Vector2> availDirs = new List<Vector2>();
                foreach (var e in collision.GetComponent<NodeController>().graphNode.edges)
                {
                    if (e.Value.destination != null)
                        availDirs.Add(e.Key);
                }
                availDirs.Remove(-direction);
                if (availDirs.Count > 0)
                {
                    
                    Vector2 dir = availDirs[Mathf.RoundToInt(Random.Range(0, availDirs.Count))];
                    Debug.Log(name + dir.ToString());
                    SetDirection(dir);
                }
            }
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
            eaten = false;
            StopAllCoroutines();
            StartCoroutine(Initialize());
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Node"))
        {
            if (collision.gameObject.GetComponent<NodeController>().portalNode)
            {
                if (!teleported)
                {
                    transform.position = collision.gameObject.GetComponent<NodeController>().connectedPortal.transform.position;
                    teleported = true;
                }
            }
            else
            {
                teleported = false;
            }
        }
    }

    private void randDir(Node Destination = null)
    {
        List<Vector2> directions = lastNode.edges.Keys.ToList();
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

    public (float, float) nodeKey(Node node)
    {
        var res = (node.xCoordinate, node.yCoordinate);
        return res;
    }

    public void bellman(Node Destination)
    {
        var edges = GameManager.Instance.map.edgeList.ToList();
        var n = GameManager.Instance.map.nodes.Count(); //we want n of nodes not edges

        var src = lastNode;
        var dst = Destination;

        // Debug.Log("Bellman");

        Dictionary<(double, double), double> dist = new Dictionary<(double, double), double>(); // distance
        Dictionary<(double, double), Node> pred = new Dictionary<(double, double), Node>(); // preceding node for each node

        // foreach (var edge in edges) { //getting all connected nodes //todo: get from map.nodes (easier)
        //     dist[nodeKey(edge.Item1)] = double.PositiveInfinity;
        //     dist[nodeKey(edge.Item2)] = double.PositiveInfinity;
        // }

        foreach (var node in GameManager.Instance.map.nodes)
        {
            dist[nodeKey(node)] = double.PositiveInfinity;
        }

        // Algorithm
        dist[nodeKey(src)] = 0;

        for (int i = 0; i < n; i++)
        {
            foreach (var edge in edges)
            {
                var edgeSrc = edge.Item1;
                var edgeDst = edge.Item2;
                var edgeWgt = edge.Item3;
                // var edgeDir = edge.Item4;

                if (dist[nodeKey(edgeSrc)] != double.PositiveInfinity && ((dist[nodeKey(edgeSrc)] + edgeWgt) < dist[nodeKey(edgeDst)]))
                {
                    dist[nodeKey(edgeDst)] = dist[nodeKey(edgeSrc)] + edgeWgt;
                    pred[nodeKey(edgeDst)] = edgeSrc;
                }
            }
        }


        var current = dst; // backtracking from last node (dst) to (src) to find next step
        var next = current;
        // var path = new List<Node>();
        while (pred.ContainsKey(nodeKey(current)))
        {
            // while current node has previous node:
            next = current;
            current = pred[nodeKey(current)];
        }

        foreach (var edge in src.edges)
        {
            if (edge.Value.destination == next)
            {
                SetDirection(edge.Key);
                return;
            }
        }

        // foreach (var edge in edges) {
        //     if (edge.Item1 == src && edge.Item2 == next) {
        //         SetDirection(edge.Item4);
        //     }
        // }

    }

    public void Dijkstra(Node Destination)
    {
        Vector2 nextDir = Vector2.zero;
        Dictionary<Node, float> DistanceSet = new Dictionary<Node, float>();
        Dictionary<Node, bool> shortestPathSet = new Dictionary<Node, bool>();
        Dictionary<Node, Node> pathDict = new Dictionary<Node, Node>();
        pathDict.Add(lastNode, null);
        foreach (Node n in GameManager.Instance.map.nodes)
        {
            DistanceSet.Add(n, float.MaxValue);
        }
        DistanceSet[lastNode] = 0;
        foreach (Node n in GameManager.Instance.map.nodes)
        {
            Node minNode = null;
            float min = float.MaxValue;
            foreach (Node N in GameManager.Instance.map.nodes)
            {
                if (!shortestPathSet.ContainsKey(N) && DistanceSet[N] <= min)
                {

                    min = DistanceSet[N];
                    minNode = N;
                }
            }
            if (minNode != null)
            {
                shortestPathSet.Add(minNode, true);
                foreach (Node N in GameManager.Instance.map.nodes)
                {

                    Edge e = null;
                    if (minNode.edges.Count < 4)
                        continue;
                    if (e == null)
                        e = minNode.edges[Vector2.up].destination == N ? minNode.edges[Vector2.up] : null;
                    if (e == null)
                        e = minNode.edges[Vector2.down].destination == N ? minNode.edges[Vector2.down] : null;
                    if (e == null)
                        e = minNode.edges[Vector2.left].destination == N ? minNode.edges[Vector2.left] : null;
                    if (e == null)
                        e = minNode.edges[Vector2.right].destination == N ? minNode.edges[Vector2.right] : null;
                    if (!shortestPathSet.ContainsKey(N) && e != null && DistanceSet[minNode] != float.MaxValue && e.cost + DistanceSet[minNode] < DistanceSet[N])
                    {
                        if (N != lastNode)
                        {
                            if (!pathDict.ContainsKey(N))
                                pathDict.Add(N, minNode);
                            else
                                pathDict[N] = minNode;
                        }
                        DistanceSet[N] = DistanceSet[minNode] + e.cost;
                    }
                }

            }
        }
        Node a = Destination;
        while (a!=null &&pathDict[a] != lastNode)
        {
            a = pathDict[a];
        }
        Vector2 dir = Vector2.zero;
        if (dir == Vector2.zero)
            dir = lastNode.edges[Vector2.up].destination == a ? Vector2.up : Vector2.zero;
        if (dir == Vector2.zero)
            dir = lastNode.edges[Vector2.down].destination == a ? Vector2.down : Vector2.zero;
        if (dir == Vector2.zero)
            dir = lastNode.edges[Vector2.left].destination == a ? Vector2.left : Vector2.zero;
        if (dir == Vector2.zero)
            dir = lastNode.edges[Vector2.right].destination == a ? Vector2.right : Vector2.zero;
        SetDirection(dir);


    }

    public void BFS(Node Destination)
    {

        Queue<Node> parents = new Queue<Node>();
        List<Node> visited = new List<Node>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
        // List<Node> adjacent = new List<Node>();

        visited.Add(lastNode);
        parents.Enqueue(lastNode);
        prev.Add(lastNode, null);
        Node a = null;
        while (parents.Count() != 0)
        {

            Node parent = parents.Dequeue();

            if (parent == Destination)
            {

                a = Destination;
                if (prev[a] != null || a == lastNode)
                {

                    while (a != null)
                    {

                        if (prev[a] == lastNode)
                        {

                            break;
                        }
                        a = prev[a];

                    }
                }
                Vector2 nextdir = Vector2.zero;
                if (nextdir == Vector2.zero)
                    nextdir = lastNode.edges[Vector2.up].destination == a ? Vector2.up : Vector2.zero;
                if (nextdir == Vector2.zero)
                    nextdir = lastNode.edges[Vector2.right].destination == a ? Vector2.right : Vector2.zero;
                if (nextdir == Vector2.zero)
                    nextdir = lastNode.edges[Vector2.down].destination == a ? Vector2.down : Vector2.zero;
                if (nextdir == Vector2.zero)
                    nextdir = lastNode.edges[Vector2.left].destination == a ? Vector2.left : Vector2.zero;

                SetDirection(nextdir);
                return;
            }
            if (parent.edges[Vector2.up].destination != null)
                if (!visited.Contains(parent.edges[Vector2.up].destination))
                {
                    visited.Add(parent.edges[Vector2.up].destination);
                    parents.Enqueue(parent.edges[Vector2.up].destination);
                    prev.Add(parent.edges[Vector2.up].destination, parent);
                }

            if (parent.edges[Vector2.right].destination != null)
                if (!visited.Contains(parent.edges[Vector2.right].destination))
                {
                    visited.Add(parent.edges[Vector2.right].destination);
                    parents.Enqueue(parent.edges[Vector2.right].destination);
                    prev.Add(parent.edges[Vector2.right].destination, parent);
                }
            if (parent.edges[Vector2.down].destination != null)
                if (!visited.Contains(parent.edges[Vector2.down].destination))
                {
                    visited.Add(parent.edges[Vector2.down].destination);
                    parents.Enqueue(parent.edges[Vector2.down].destination);
                    prev.Add(parent.edges[Vector2.down].destination, parent);
                }

            if (parent.edges[Vector2.left].destination != null)
                if (!visited.Contains(parent.edges[Vector2.left].destination))
                {
                    visited.Add(parent.edges[Vector2.left].destination);
                    parents.Enqueue(parent.edges[Vector2.left].destination);
                    prev.Add(parent.edges[Vector2.left].destination, parent);
                }
        }


    }

    public void Coup(Node Destination)
    {

    }

    public void Astar(Node Destination)
    {
        List<Vector2> directions = lastNode.edges.Keys.ToList();
        Vector2 dir = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach (Vector2 availableDirection in directions)
        {
            Vector3 newPosition = new Vector3(lastNode.xCoordinate,lastNode.yCoordinate)+ new Vector3(availableDirection.x, availableDirection.y, 0.0f);
            float distance = (newPosition - new Vector3(Destination.xCoordinate, Destination.yCoordinate)).sqrMagnitude;
            
            if (minDistance > distance)
            {
                dir = availableDirection;
                minDistance = distance;
            }
        }

        if (Occupied(dir))
        {
            directions.Remove(-direction);
            SetDirection(directions[Mathf.RoundToInt(Random.Range(0, directions.Count))]);
        }
        else
        {
            SetDirection(dir);
        }

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

        if (distance.x > 0 && !Occupied(Vector2.left))
        {
            SetDirection(Vector2.left);
        }
        else if (distance.x < 0 && !Occupied(Vector2.right))
        {
            SetDirection(Vector2.right);
        }
        else if (distance.y < 0 && !Occupied(Vector2.up))
        {
            SetDirection(Vector2.up);
        }
        else if (distance.y > 0 && !Occupied(Vector2.down))
        {
            SetDirection(Vector2.down);
        }
        else
        {
            SetDirection(directions[Mathf.RoundToInt(Random.Range(0, directions.Count))]);
        }
    }
    /*
    public void KindaGood(Node Destination)
    {

        if (Destination == null)
        {
            return;
        }
        int yDiff = Mathf.RoundToInt(destinationNode.yCoordinate) - Mathf.RoundToInt(lastNode.yCoordinate);
        int xDiff = Mathf.RoundToInt(destinationNode.xCoordinate) - Mathf.RoundToInt(lastNode.xCoordinate);
        if (xDiff == 0)
        {
            {
                if (!FutureOccupied(new Vector2(lastNode.xCoordinate,lastNode.yCoordinate),Vector2.up * (yDiff > 0 ? 1 : -1)))
                {
                    SetDirection(Vector2.up * (yDiff > 0 ? 1 : -1));
                    return;
                }
                else if (lastNode.edges[Vector2.left].destination != null && lastNode.edges[Vector2.left].destination.edges[Vector2.up * (yDiff > 0 ? 1 : -1)].destination != null)
                {
                    SetDirection(Vector2.left);
                    return;
                }
                else if (lastNode.edges[Vector2.right].destination != null && lastNode.edges[Vector2.right].destination.edges[Vector2.up * (yDiff > 0 ? 1 : -1)].destination != null)
                {
                    SetDirection(Vector2.right);
                    return;
                }

            }
        }
        if (yDiff == 0)
        {
            {
                if (!FutureOccupied(new Vector2(lastNode.xCoordinate, lastNode.yCoordinate),Vector2.right * (xDiff > 0 ? 1 : -1)))
                {
                    SetDirection(Vector2.right * (xDiff > 0 ? 1 : -1));
                    return;
                }
                else if (lastNode.edges[Vector2.up].destination != null && lastNode.edges[Vector2.up].destination.edges[Vector2.right * (xDiff > 0 ? 1 : -1)].destination != null)
                {
                    SetDirection(Vector2.up);
                    return;
                }
                else if (lastNode.edges[Vector2.down].destination != null && lastNode.edges[Vector2.down].destination.edges[Vector2.right * (xDiff > 0 ? 1 : -1)].destination != null)
                {
                    SetDirection(Vector2.down);
                    return;
                }
            }
        }
        if (Mathf.Abs(yDiff) < Mathf.Abs(xDiff)&& Mathf.RoundToInt(destinationNode.yCoordinate)!= Mathf.RoundToInt(lastNode.yCoordinate))
        {
            if (!FutureOccupied(new Vector2(lastNode.xCoordinate, lastNode.yCoordinate),Vector2.up * (yDiff > 0 ? 1 : -1)))
            {
                SetDirection(Vector2.up * (yDiff > 0 ? 1 : -1));
                return;
            }
            else if (lastNode.edges[Vector2.left].destination!=null&& lastNode.edges[Vector2.left].destination.edges[Vector2.up * (yDiff > 0 ? 1 : -1)].destination!=null)
            {
                SetDirection(Vector2.left);
                return;
            }
            else if(lastNode.edges[Vector2.right].destination != null && lastNode.edges[Vector2.right].destination.edges[Vector2.up * (yDiff> 0 ? 1 : -1)].destination != null)
            {
                SetDirection(Vector2.right);
                return;
            }
        }
        if(Mathf.RoundToInt(destinationNode.xCoordinate)!= Mathf.RoundToInt(lastNode.xCoordinate))
        {
            if (!FutureOccupied(new Vector2(lastNode.xCoordinate, lastNode.yCoordinate),Vector2.right * (xDiff> 0 ? 1 : -1)))
            {
                SetDirection(Vector2.right * (xDiff > 0 ? 1 : -1));
                return;
            }
            else if (lastNode.edges[Vector2.up].destination != null && lastNode.edges[Vector2.up].destination.edges[Vector2.right * (xDiff> 0 ? 1 : -1)].destination != null)
            {
                SetDirection(Vector2.up);
                return;
            }
            else if (lastNode.edges[Vector2.down].destination != null && lastNode.edges[Vector2.down].destination.edges[Vector2.right * (xDiff > 0 ? 1 : -1)].destination != null)
            {
                SetDirection(Vector2.down);
                return;
            }
        }
    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (!scared && !eaten)
                GameManager.Instance.PacmanEaten();
            else
            {
                scared = false;
                Eaten();
            }
        }

    }

}

public enum DestinationSelector
{
    InFrontOfPacman, BehindPacman, DirectLineOfSight, SideNode, TwoInfornt
}