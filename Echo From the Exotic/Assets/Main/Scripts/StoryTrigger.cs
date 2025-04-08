using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [SerializeField] private int progress;

    void Start()
    {
        if (ProgressManager.instance.currentChapter == progress)
        {
            ProgressManager.instance.StartChapter(ProgressManager.instance.currentChapter);
        }

        ProgressManager.instance.NextChapter();
    }
}
