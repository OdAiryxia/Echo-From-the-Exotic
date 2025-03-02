using Flower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    FlowerSystem flowerSys;
    public static ProgressManager instance;

    public int currentChapter = 0; // ·í«e³¹¸`

    //private bool prologuePlayed = false;
    public bool isStory = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        flowerSys = FlowerManager.Instance.CreateFlowerSystem("FlowerSystem", true);
        flowerSys.textSpeed = 0.05f;
    }

    void Start()
    {
        flowerSys.SetScreenReference(1920, 1080);
        flowerSys.RegisterCommand("LockButton", LockButton);
        flowerSys.RegisterCommand("ReleaseButton", ReleaseButton);

        //prologuePlayed = PlayerPrefs.GetInt("ProloguePlayed", 0) == 1;

        PlayerPrefs.SetInt("CurrentChapter", 0);
        PlayerPrefs.SetInt("ProloguePlayed", 0);

        flowerSys.SetupDialog();
        flowerSys.SetupUIStage();
        flowerSys.ReadTextFromResource("hide");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) | Input.GetMouseButtonDown(0))
        {
            // Continue the messages, stoping by [w] or [lr] keywords.
            flowerSys.Next();
        }
    }


    public void StartDialogue(int chapter)
    {
        switch (chapter)
        {
            case 0:
                flowerSys.ReadTextFromResource("prologue");
                Debug.Log("a");
                PlayerPrefs.SetInt("ProloguePlayed", 1);
                PlayerPrefs.Save();
                break;
            case 1:
                break;
            default:
                break;
        }
    }

    public void NextChapter()
    {
        currentChapter++;
        PlayerPrefs.SetInt("CurrentChapter", currentChapter);
        PlayerPrefs.Save();
    }

    private void LockButton(List<string> _params)
    {
        isStory = true;
    }

    private void ReleaseButton(List<string> _params)
    {
        isStory = false;
    }
}
