using Flower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    FlowerSystem flowerSys;
    [SerializeField] private int progress;
    [SerializeField] private StoryPosition[] storyPositions;
    [SerializeField] private ModalWindowTemplate[] modalWindowTemplates;

    void Start()
    {
        flowerSys = FlowerManager.Instance.GetFlowerSystem("FlowerSystem");
        if (ProgressManager.instance.currentChapter == progress)
        {
            ProgressManager.instance.storyPositions = storyPositions;
            ProgressManager.instance.modalWindowTemplates = modalWindowTemplates;
            ProgressManager.instance.StartChapter(ProgressManager.instance.currentChapter);
        }

        ProgressManager.instance.NextChapter();
    }
}
