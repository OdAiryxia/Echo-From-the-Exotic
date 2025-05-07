using Flower;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;
    public FlowerSystem flowerSys;

    public CinemachineFreeLook cam;
    public GameObject player;
    public AudioSource audioSource;

    public StoryPosition[] storyPositions;
    public ModalWindowTemplate[] modalWindowTemplates;
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
        cam = new GameObject("storyCam").AddComponent<CinemachineFreeLook>();
        cam.Priority = 5;

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);

        flowerSys = FlowerManager.Instance.CreateFlowerSystem("FlowerSystem", true);
        flowerSys.textSpeed = 0.05f;
        flowerSys.SetScreenReference(Screen.currentResolution.width, Screen.currentResolution.height);
        flowerSys.RegisterCommand("LockButton", LockButton);
        flowerSys.RegisterCommand("ReleaseButton", ReleaseButton);
        flowerSys.RegisterCommand("SetPosition",SetPosition);
        flowerSys.RegisterCommand("SetModalWindow", SetModalWindow);
        flowerSys.RegisterCommand("ReturnCamera", ReturnCamera);
        flowerSys.RegisterCommand("StartCpt", StartCpt);
        flowerSys.RegisterCommand("NextCpt", NextCpt);
        flowerSys.RegisterCommand("AudioPlay", AudioPlay);
        flowerSys.RegisterCommand("AudioStop", AudioStop);
        flowerSys.RegisterCommand("SetModalWindowInputText", SetModalWindowInputText);
        flowerSys.SetVariable("PlayerName", "數媒系學生");

        flowerSys.SetupDialog();
        flowerSys.SetupUIStage();
        flowerSys.ReadTextFromResource("hide");
    }

    void Update()
    {
        if (!ModalWindowManager.instance.isWindow)
        {
            if (Input.GetKeyDown(KeyCode.Space) | Input.GetMouseButtonDown(0))
            {
                // Continue the messages, stoping by [w] or [lr] keywords.
                flowerSys.Next();
            }
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

    void SetPosition(List<string> _params)
    {
        if (_params.Count > 0)
        {
            if (int.TryParse(_params[0], out int index))
            {
                if (storyPositions[index].camPos != null)
                {
                    cam.Priority = 20;
                    cam.gameObject.transform.position = storyPositions[index].camPos.transform.position;
                    cam.gameObject.transform.rotation = storyPositions[index].camPos.transform.rotation;
                }

                if (storyPositions[index].playerPos != null)
                {
                    player.GetComponent<CharacterController>().enabled = false;
                    player.gameObject.transform.position = storyPositions[index].playerPos.transform.position;
                    player.gameObject.transform.rotation = storyPositions[index].playerPos.transform.rotation;
                    player.GetComponent<CharacterController>().enabled = true;
                }

                if (storyPositions[index].otherPos != null)
                {
                    if (index > 0 && storyPositions[index - 1].otherPos != null)
                    {
                        storyPositions[index - 1].otherPos.SetActive(false);
                    }
                    storyPositions[index].otherPos.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("無法將參數轉換為整數：" + _params[0]);
            }
        }
        else
        {
            Debug.LogWarning("缺少參數！");
        }
    }

    void SetModalWindow(List<string> _params)
    {
        currentIndex = 0;
        ShowCurrentModal();
    }

    void SetModalWindowInputText(List<string> _params)
    {
        ModalWindowManager.instance.ShowInputField();
    }

    void ReturnCamera(List<string> _params)
    {
        cam.Priority = 5;
    }
    public void StartCpt(List<string> _params)
    {
        StartChapter(currentChapter);
    }

    public void NextCpt(List<string> _params)
    {
        currentChapter++;
    }

    void AudioPlay(List<string> _params)
    {
        audioSource.Play();
    }

    void AudioStop(List<string> _params)
    {
        audioSource.Stop();
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
                flowerSys.ReadTextFromResource("prologue_1");
                break;
            case 2:
                flowerSys.ReadTextFromResource("chapter_1");
                break;
            case 3:
                flowerSys.ReadTextFromResource("chapter_1_1");
                break;
            case 4:
                flowerSys.ReadTextFromResource("free");
                break;
            default:
                break;
        }
    }

    public void NextChapter()
    {
        currentChapter++;;
    }

    public int currentIndex = 0;
    private void ShowCurrentModal()
    {
        if (currentIndex >= modalWindowTemplates.Length)
        {
            ModalWindowManager.instance.Close();
            return;
        }

        ModalWindowTemplate currentTemplate = modalWindowTemplates[currentIndex];

        ModalWindowManager.instance.ShowVertical(
            currentTemplate.title,
            currentTemplate.image,
            currentTemplate.context,
            currentTemplate.confirmText, () =>
            {
                currentIndex++;
                ModalWindowManager.instance.Close();
                ShowCurrentModal(); // 顯示下一個
            },
            currentTemplate.declineText, () =>
            {
                ModalWindowManager.instance.Close();
            },
            currentTemplate.alternateText, () =>
            {
                ModalWindowManager.instance.Close();
            }
        );
    }

    public void SetPlayerName()
    {
        flowerSys.SetVariable("PlayerName", DataContainer.instance.playerName);
        Debug.Log($"set player name to {DataContainer.instance.playerName}");
    }
}
