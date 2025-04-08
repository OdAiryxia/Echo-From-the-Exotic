using Flower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;
    private FlowerSystem flowerSys;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        flowerSys = FlowerManager.Instance.CreateFlowerSystem("FlowerSystem", true);
        flowerSys.textSpeed = 0.05f;
        flowerSys.SetScreenReference(1920, 1080);
        flowerSys.RegisterCommand("LockButton", LockButton);
        flowerSys.RegisterCommand("ReleaseButton", ReleaseButton);
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

    #region Commands
    void LockButton(List<string> _params)
    {
        isStory = true;
    }

    void ReleaseButton(List<string> _params)
    {
        isStory = false;
    }
    #endregion

    public int currentChapter = 0;
    public bool isStory = false;

    public void StartChapter(int chapter)
    {
        switch (chapter)
        {
            case 0:
                flowerSys.ReadTextFromResource("prologue");
                break;
            case 1:
                break;
            default:
                break;
        }
    }

    public void NextChapter()
    {
        currentChapter++;;
    }
}
