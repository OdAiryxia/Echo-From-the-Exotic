using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBgm : MonoBehaviour
{
    private AudioSource bgm;

    private void Awake()
    {
        bgm = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (ProgressManager.instance.audioSource != bgm)
        {
            ProgressManager.instance.audioSource = bgm;
        }
    }
}
