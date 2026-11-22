using System;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public int coins = 0;
    public int totalCoinsInLevel;
    bool won = false;
    [SerializeField]
    private TextMeshProUGUI coinsText;
    [SerializeField]
    private TextMeshProUGUI winText;
    [SerializeField]
    private GameObject endMenu;
    [SerializeField]
    private Button endButton;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private EnemyMovement enemyMovement;
    [SerializeField]
    private PlayerController playerController;

    private bool paused = false;

    [SerializeField]
    private GameObject pauseMenu;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCountText();
        //winText.gameObject.SetActive(false);
        //endButton.gameObject.SetActive(false);
        endMenu.SetActive(false);
        totalCoinsInLevel = GameObject.FindGameObjectsWithTag("Pickup").Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCoin()
    {
        coins += 1;
        if (coins == totalCoinsInLevel && won == false      /*coins == 4 && won == false*/)
        {
            EndGame("Win");
        }
    }

    public void SetCountText()
    {
        coinsText.text = "Coins: " + coins.ToString();
    }

    public void EndGame(String End)
    {
        if (End == "Lose")
        {
            winText.GetComponent<TextMeshProUGUI>().text = "You Lose";
        } else
        {
            PlayerPrefs.SetInt("level", int.Parse(Regex.Match(SceneManager.GetActiveScene().name, @"\d+").Value));
            int totalCoins = PlayerPrefs.GetInt("coins");
            PlayerPrefs.SetInt("coins", totalCoins + coins);
            PlayerPrefs.Save();
        }

        //winText.gameObject.SetActive(true);
        //endButton.gameObject.SetActive(true);
        endMenu.SetActive(true);
        Destroy(GameObject.FindGameObjectsWithTag("Enemy")[0]);
        
       

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        if (paused) {
            //UNPAUSE
            enemyMovement.Pause(false);
            playerController.Pause(false);
            pauseMenu.gameObject.SetActive(false);
            paused = false;
        } else
        {
            //PAUSE
            enemyMovement.Pause(true);
            playerController.Pause(true);
            pauseMenu.gameObject.SetActive(true);
            paused = true;
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
