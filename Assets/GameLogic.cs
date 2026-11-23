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
    public enum GoalType
    {
        [InspectorName("Coin Goal")]
        Coin,
        [InspectorName("Location Goal")]
        Location
    }
    [SerializeField] private GoalType goal = GoalType.Coin;
    [SerializeField] private GameObject goalLocation;

    public bool gameActive = true;


    [HideInInspector]
    public int coins = 0;
    [HideInInspector]
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

    private RectTransform worldspace;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalCoinsInLevel = GameObject.FindGameObjectsWithTag("Pickup").Length;
        SetCountText();
        //winText.gameObject.SetActive(false);
        //endButton.gameObject.SetActive(false);
        endMenu.SetActive(false);
        if (goal == GoalType.Location)
        {
            worldspace = goalLocation.GetComponentInChildren<RectTransform>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (goal == GoalType.Location)
        {
            worldspace.LookAt(Camera.main.transform);
        }
    }

    public void AddCoin()
    {
        coins += 1;
        if (coins == totalCoinsInLevel && won == false && goal == GoalType.Coin      /*coins == 4 && won == false*/)
        {
            EndGame("Win");
        }
    }

    public void CheckLocationGoal(GameObject enteredObject)
    {
        if (goal == GoalType.Location && enteredObject == goalLocation && !won)
        {
            won = true;
            EndGame("Win");
        }
    }

    public void SetCountText()
    {
        coinsText.text = "Coins: " + coins.ToString() + " / " +  totalCoinsInLevel;
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
        gameActive = false;
        playerController.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        endMenu.SetActive(true);
        Destroy(GameObject.FindGameObjectsWithTag("Enemy")[0]);
        
       

    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    

    public void Pause()
    {
        if (gameActive) { 
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
        } else
        {
            print("Cant Pause! Game is over!");
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}



//thing to make selecting goal in editor easier
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(GameLogic))]
public class GameLogicEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "goal", "goalLocation");

        var goal = serializedObject.FindProperty("goal");
        UnityEditor.EditorGUILayout.PropertyField(goal);

        if (goal.enumValueIndex == (int)GameLogic.GoalType.Location)
        {
            var location = serializedObject.FindProperty("goalLocation");
            UnityEditor.EditorGUILayout.PropertyField(location);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif