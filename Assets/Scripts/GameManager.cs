using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            if (instance == null)
            {
                instance = value;
                DontDestroyOnLoad(value);
            }
            else
            {
                Destroy(value);
            }
        }
    }
    private static GameManager instance;
    public Transform ghostParent;
    public Pacman pacman;
    public Transform pellets;
    public Transform powerPellets;
    public Transform Nodes;
    public Transform[] portals;
    private int ghostEatingStreak;
    [SerializeField]
    private int ghostPoints;
    [SerializeField]
    private int pelletPoints;
    [SerializeField]
    private int powerPelletPoints;
    public int SCORE;
    public Difficulty difficulty;
    public int score{ get; private set; }
    public int lives{ get; private set; }
    public Graph map;
    private void Start()
    {
        NewGame();
        instance= this;
        StartCoroutine(LoadGraphInstance());
    }
    private IEnumerator LoadGraphInstance()
    {
        yield return null;
        map = new Graph();
        foreach (Transform node in Nodes)
        {
            Node current = node.GetComponent<NodeController>().graphNode;
            RaycastHit2D[] hits = new RaycastHit2D[100];
            RaycastHit2D toBeAdded=new RaycastHit2D();
            node.GetComponent<Collider2D>().Raycast(Vector2.up, hits);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider!= null)
                {
                    if (Physics2D.BoxCast(node.position, Vector2.one * 0.5f, 0.0f, Vector2.up, hit.distance, pacman.obstacleLayer).collider != null)
                    {
                        continue;
                    }
                    if (hit.collider.GetComponent<NodeController>())
                    {
                        if (toBeAdded.collider == null)
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = hit;
                        }
                        else
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = toBeAdded.distance > hit.distance ? hit : toBeAdded;
                        }

                    }
                }
            }
            if (!(current.edges.ContainsKey(Vector2.up)))
            {
                if (toBeAdded.collider != null)
                {
                    current.AddEdge(toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance, Vector2.up);
                    map.edgeList.Add((current, toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance, Vector2.up));
                }
                else
                {
                    current.AddEdge(null, 99999, Vector2.up);
                }
                
            }
            toBeAdded= new RaycastHit2D();
            node.GetComponent<Collider2D>().Raycast(Vector2.down, hits);
            foreach (RaycastHit2D hit in hits)
                if (hit.collider!= null)
                {
                    if (Physics2D.BoxCast(node.position, Vector2.one * 0.5f, 0.0f, Vector2.down, hit.distance, pacman.obstacleLayer).collider != null)
                    {
                        continue;
                    }
                    if (hit.collider.GetComponent<NodeController>())
                    {
                        if (toBeAdded.collider == null)
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = hit;
                        }
                        else
                        {
                            if (hit.transform.position!=node.position)
                                toBeAdded = toBeAdded.distance > hit.distance ? hit : toBeAdded;
                        }
                    }
                }
            if (!(current.edges.ContainsKey(Vector2.down)))
            {
                if (toBeAdded.collider != null)
                {
                    current.AddEdge(toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance, Vector2.down);
                    map.edgeList.Add((current, toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance, Vector2.down));
                }
                else
                {
                    current.AddEdge(null, 99999, Vector2.down);
                }
            }
            node.GetComponent<Collider2D>().Raycast(Vector2.left, hits);
            toBeAdded = new RaycastHit2D();
            foreach (RaycastHit2D hit in hits)
                if (hit.collider!= null)
                {
                    if (Physics2D.BoxCast(node.position, Vector2.one * 0.5f, 0.0f, Vector2.left, hit.distance, pacman.obstacleLayer).collider != null)
                    {
                        continue;
                    }
                    if (hit.collider.GetComponent<NodeController>()) 
                    {
                        if (toBeAdded.collider == null)
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = hit;
                        }
                        else
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = toBeAdded.distance > hit.distance ? hit : toBeAdded;
                        }
                    }

                }
            if (!(current.edges.ContainsKey(Vector2.left)))
            {
                if (toBeAdded.collider != null)
                {

                    current.AddEdge(toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance,Vector2.left);
                    map.edgeList.Add((current, toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance, Vector2.left));
                }
                else
                {
                    current.AddEdge(null, 99999, Vector2.left);
                }
            }

            node.GetComponent<Collider2D>().Raycast(Vector2.right, hits);
            toBeAdded = new RaycastHit2D();
            foreach (RaycastHit2D hit in hits)
                if (hit.collider != null)
                {
                    if (Physics2D.BoxCast(node.position, Vector2.one * 0.5f, 0.0f, Vector2.right, hit.distance, pacman.obstacleLayer).collider != null)
                    {
                        continue;
                    }
                    if (hit.collider.GetComponent<NodeController>())
                    {
                        if (toBeAdded.collider == null)
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = hit;
                        }
                        else
                        {
                            if (hit.transform.position != node.position)
                                toBeAdded = toBeAdded.distance > hit.distance ? hit : toBeAdded;
                        }

                    }
                }
            if (!(current.edges.ContainsKey(Vector2.right)))
            {
                if (toBeAdded.collider != null)
                {
                    current.AddEdge(toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance,Vector2.right);
                    map.edgeList.Add((current, toBeAdded.collider.GetComponent<NodeController>().graphNode, toBeAdded.distance, Vector2.right));

                }
                else
                {
                    current.AddEdge(null, 99999, Vector2.right);
                }

            }
            map.AddNode(current);
        }
    }
    private void Update()
    {
        if(this.lives <= 0 && Input.anyKeyDown){
            NewGame();
        }
        SCORE = score;
    }

    private void NewGame(){
        SetScore(0);
        SetLives(3);
        NewRound();
        foreach (Transform g in ghostParent)
        {
            g.GetComponent<Ghost>().Init();
        }
        ghostEatingStreak = 0;
    }

    private void NewRound(){
        foreach(Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
        }
        foreach (Transform powerPellet in this.powerPellets)
        {
            powerPellet.gameObject.SetActive(true);
        }
        ResetState();
    }

    private void ResetState(){
        for(int i=0; i<this.ghostParent.childCount; i++)
        {
            this.ghostParent.GetChild(i).gameObject.SetActive(true);
        }

        this.pacman.gameObject.SetActive(true);
    }

    private void GameOver(){
        for(int i=0; i<this.ghostParent.childCount; i++)
        {
            this.ghostParent.GetChild(i).gameObject.SetActive(false);
        }

        this.pacman.gameObject.SetActive(false);
    }

    private void SetScore(int score){
        this.score = score;
    }

    private void SetLives(int lives){
        this.lives = lives;
    }

    public void GhostEaten(){
        if (pacman.powerPelletActive)
        {
            ghostEatingStreak += 1;
            SetScore(this.score + ghostPoints*ghostEatingStreak);
        }
        else
        {
            PacmanEaten();
        }

    }
    public void PelletEaten()
    {
        SetScore(this.score + pelletPoints);
    }
    public void PowerPelletEaten()
    {
        SetScore(this.score + powerPelletPoints);
        foreach (Transform ghost in ghostParent)
        {
            ghost.GetComponent<Ghost>().Scared();
        }
    }
    public void PacmanEaten(){
        this.pacman.gameObject.SetActive(false);
        SetLives(this.lives - 1);

        if(this.lives > 0){
            Invoke(nameof(ResetState), 3.0f);
        }else{
            GameOver();
        }
    }
}
public enum Difficulty
{
    Easy,Normal,Hard,Coup
}