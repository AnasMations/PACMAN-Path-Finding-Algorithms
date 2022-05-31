using UnityEngine;

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
    public Transform[] portals;
    private int ghostEatingStreak;
    [SerializeField]
    private int ghostPoints;
    [SerializeField]
    private int pelletPoints;
    [SerializeField]
    private int powerPelletPoints;
    public int SCORE;
    public int score{ get; private set; }
    public int lives{ get; private set; }

    private void Start()
    {
        NewGame();
        instance= this;
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
        ghostEatingStreak = 0;
    }

    private void NewRound(){
        foreach(Transform pellet in this.pellets)
        {
            pellet.gameObject.SetActive(true);
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
