using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class GameLogicScript : MonoBehaviour
{
    [SerializeField]private TMP_Text resultText;
    [SerializeField]private TMP_Text timerText;
    [SerializeField]private TMP_Text scoreText;
    [SerializeField]private TMP_Text computerPlayedText;
    [SerializeField]private TMP_Text playerPlayedText;
    [SerializeField]private Slider timerSlider;
    [SerializeField]private AudioSource endGameSound;
    [SerializeField]private AudioSource tieGameSound;
    [SerializeField]private AudioSource startGameSound;
    [SerializeField]private AudioSource winGameSound;
    [SerializeField]private UnityEvent gameLost; 
    [SerializeField]private UnityEvent gameStart; 
    [SerializeField]private UnityEvent gameReset;
    private enum Choice { Rock, Paper, Scissors, Lizard, Spock }
    private bool playerHasChosen;
    private float timer;
    private bool gameStarted;
    int playerScore=0;

#region Timer update logic

  void Update()
    {
        if (gameStarted)
        {
            timer += Time.deltaTime;
            float time=1-timer;
            timerSlider.value=time;
            timerText.text=time.ToString("n2");

            if (timer >= 1f) //Logic for computer to win by default if player doesn't make a choice
            {
                if (!playerHasChosen)
                {
                    Choice computer = (Choice)Random.Range(0, 5);
                    resultText.text = $"Computer: {computer} vs Player: No choice \nComputer wins by default!";
                    gameStarted = false;
                    computerPlayedText.text=computer.ToString();
                    playerPlayedText.text="No choice";
                    endGameSound.Play();
                    StartCoroutine(EndGame());
                }
            }
        }
    }
    
#endregion

#region Player choice and winner logic

    void PlayerChoice(int playerChoice) //Logic for player to make a choice
    {
        if (gameStarted)
        {
            playerHasChosen = true;
            gameStarted = false; // Stop the timer when player makes a choice
            Choice player = (Choice)playerChoice;
            Choice computer = (Choice)Random.Range(0, 5);
            computerPlayedText.text=computer.ToString();
            playerPlayedText.text=player.ToString();

            string result = DetermineWinner(player, computer); // Determine the winner

            if (result == "Player wins!")
             { 
                winGameSound.Play();
                playerScore++; 
                UpdateScoreText();
                StartCoroutine(StartGame());
             }
             else if (result == "Computer wins!")
             {
                endGameSound.Play();
                StartCoroutine(EndGame());
             }
             else if (result == "It's a tie!")
             {
                tieGameSound.Play();
                StartCoroutine(StartGame());
             }
            resultText.text = $"Computer: {computer} vs Player: {player}\n{result}";
        }
    }

    private string DetermineWinner(Choice player, Choice computer) //Logic to determine the winner
    {
        if (player == computer)
        {
            return "It's a tie!";
        }

        switch (player)
        {
            case Choice.Rock:
                if (computer == Choice.Scissors || computer == Choice.Lizard)
                    return "Player wins!";
                break;
            case Choice.Paper:
                if (computer == Choice.Rock || computer == Choice.Spock)
                    return "Player wins!";
                break;
            case Choice.Scissors:
                if (computer == Choice.Paper || computer == Choice.Lizard)
                    return "Player wins!";
                break;
            case Choice.Lizard:
                if (computer == Choice.Paper || computer == Choice.Spock)
                    return "Player wins!";
                break;
            case Choice.Spock:
                if (computer == Choice.Scissors || computer == Choice.Rock)
                    return "Player wins!";
                break;
        }

        return "Computer wins!";
    }

#endregion
   
#region Button click events

    private void UpdateScoreText()
     { 
        scoreText.text = "Score : " + playerScore;
     }

    public void OnPlayButtonClick() //Loginc for play button click
    {
        startGameSound.Play();
        StartCoroutine(ResetGame());
        StartCoroutine(StartGame()); 
    }

#endregion

#region Game modes

    IEnumerator StartGame() //Logic to start the game
    {
        gameStart.Invoke();
        yield return new WaitForSeconds(3);
        playerHasChosen = false;
        timer = 0f;
        gameStarted = true;
    }

    IEnumerator EndGame() //Logic to end the game
    {
        yield return new WaitForSeconds(3);
        gameLost.Invoke();
    }

    IEnumerator ResetGame() //Logic to reset the game
    {
        yield return null;
        playerScore=0;
        gameReset.Invoke();
    }
    
#endregion

}
