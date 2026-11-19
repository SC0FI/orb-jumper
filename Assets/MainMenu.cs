using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    bool forward = true;
    float rotation = 0;
    int coins = 0;
    [SerializeField]
    RectTransform rectTransform;
    [SerializeField]
    private TextMeshProUGUI coinsText;
    [SerializeField]

    Button[] levels = {};

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = "Coins: " + coins;
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].interactable = false;
        }
        
        UnlockLevels(levels);


    }

    // Update is called once per frame
    void Update()
    {
        //-10, 10

        if (forward)
        {
            rotation += 0.1f;
            rectTransform.Rotate(new Vector3(0, 0, 0.1f));
        } else
        {
            rotation -= 0.1f;
            rectTransform.Rotate(new Vector3(0, 0, -0.1f));
        }


        if (rotation > 9.9)
        {
            forward = false;
        } else if (rotation < -9.9)
        {
            forward = true;
        }
        
    }

    public void UnlockLevels(Button[] levels)
    {
        int level = PlayerPrefs.GetInt("level");
        print(level);
        for (int i = 0; i <= level; i++)
        {
            levels[i].interactable = true;
        }
    }

    public void StartGame(string scene)
    {
        print("Loading");
        try
        {
            SceneManager.LoadScene(scene);
        } catch (Exception e)
        {
            print(e.ToString());
        }
        
    }
}
