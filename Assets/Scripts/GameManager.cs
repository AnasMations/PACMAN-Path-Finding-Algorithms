using UnityEngine;
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
            }
            else
            {
                Destroy(value);
            }
        }
    }
    private static GameManager instance;
    public Ghost[] ghosts;
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
    public Graph mapOverView
    {
        get
        {
            return map;
        }
        private set
        {
            if (map == null)
            {
                map = value;
            }
        }
    }
    private Graph map;
    private void Start()
    {
        NewGame();
        instance= this;
        LoadGraphInstance();
    }
    private void LoadGraphInstance()
    {
        map = new Graph();
        foreach (Transform node in Nodes)
        {
            Node current = node.GetComponent<NodeController>().graphNode;
            RaycastHit2D hit = Physics2D.Raycast(node.position, Vector2.up, 1000.0f, LayerMask.NameToLayer("Node"));
            if (hit.collider!= null)
            {
                Debug.Log("qaa");
                current.AddEdge(hit.collider.GetComponent<NodeController>().graphNode, (int)hit.distance);
            }
            hit = Physics2D.Raycast(node.position, Vector2.down, 1000.0f, LayerMask.NameToLayer("Node"));
            if (hit.collider!= null)
            {
                Debug.Log("qaaa");
                current.AddEdge(hit.collider.GetComponent<NodeController>().graphNode, (int)hit.distance);
            }
            hit = Physics2D.Raycast(node.position, Vector2.left, 1000.0f, LayerMask.NameToLayer("Node"));
            if (hit.collider!= null)
            {
                Debug.Log("qaac");
                current.AddEdge(hit.collider.GetComponent<NodeController>().graphNode, (int)hit.distance);
            }
            hit = Physics2D.Raycast(node.position, Vector2.right, 1000.0f, LayerMask.NameToLayer("Node"));
            if (hit.collider!= null)
            {
                Debug.Log("qaaaa");
                current.AddEdge(hit.collider.GetComponent<NodeController>().graphNode, (int)hit.distance);
            }
            map.AddNode(current);
            Debug.Log(current.edges.Count);
        }
    }
    private void Update()
    {
        if(this.lives <= 0 && Input.anyKeyDown){
            NewGame();
        }
        SCORE = score;
        LoadGraphInstance();

    }

    private void NewGame(){
        SetScore(0);
        SetLives(3);
        NewRound();
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
        for(int i=0; i<this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(true);
        }

        this.pacman.gameObject.SetActive(true);
    }

    private void GameOver(){
        for(int i=0; i<this.ghosts.Length; i++)
        {
            this.ghosts[i].gameObject.SetActive(false);
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
        foreach (Ghost ghost in ghosts)
        {

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