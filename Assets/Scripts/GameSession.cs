using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    #region Variables
    public Player player;
    public RandomSpawner spawner;

    public int currentScore = 0;

    public float timeBeforeRoundStart = 3f;
    public int currentRound = 0;
    public int currentActualRound = 0;
    public float roundTime = 60f;
    public int amountOfBreakablesLeftInRound = 0;

    public Text scoreText;
    public Text roundText;
    public Text timerText;
    public GameObject gameOverPanel;
    public Text finalScoreText;

    [SerializeField] float currentTimeBeforeRoundStart;
    [SerializeField] float currentRoundTime;

    public bool roundHasStarted = false;
    #endregion

    #region Unity Callback Functions
    void Awake() {
        currentRoundTime = roundTime;
        currentTimeBeforeRoundStart = timeBeforeRoundStart;
    }

    void Update() {
        if (!roundHasStarted && !gameOverPanel.activeSelf) {
            RoundHasntStarted();
        } else if (roundHasStarted && !gameOverPanel.activeSelf) {
            RoundHasStarted();
        }
    }
    #endregion

    #region Functions
    void RoundHasntStarted() {
        currentTimeBeforeRoundStart -= Time.deltaTime;

        UpdateRoundTimer((int) currentTimeBeforeRoundStart);

        if(currentTimeBeforeRoundStart <= 0) {
            roundHasStarted = true;

            player.SetCanMove(true);

            StartRound();

            currentRoundTime = roundTime;
        }
    }

    void RoundHasStarted() {
        currentRoundTime -= Time.deltaTime;

        UpdateRoundTimer((int) currentRoundTime);

        if (currentRoundTime <= 0) {
            GameOver();
        }

        if(amountOfBreakablesLeftInRound <= 0) {
            EndRound();
        }
    }

    void StartRound() {
        currentRound += 1;

        if (currentRound > 100) {
            currentRound = 80;
        }

        currentActualRound += 1;

        roundText.text = currentActualRound.ToString();

        player.RandomizeCurrentColor();
        amountOfBreakablesLeftInRound = Random.Range(1, currentRound);
        spawner.SpawnRandomly(player, amountOfBreakablesLeftInRound, currentRound);
    }

    void EndRound() {
        roundHasStarted = false;
        player.SetCanMove(false);
        spawner.DespawnAll();
        currentTimeBeforeRoundStart = timeBeforeRoundStart;
    }

    void UpdateRoundTimer(int timer) {
        timerText.text = timer.ToString();
    }

    void GameOver() {
        gameOverPanel.SetActive(true);
        finalScoreText.text = currentScore.ToString();
        EndRound();        
    }

    public void AddScore(int score) {
        this.currentScore += score;
        scoreText.text = currentScore.ToString();
        scoreText.GetComponent<Animator>().Play("TextAnimation");
    }
 
    public void DecreaseAmountOfBreakablesLeft(int amount) {
        this.amountOfBreakablesLeftInRound -= amount;
    }
    #endregion
}
