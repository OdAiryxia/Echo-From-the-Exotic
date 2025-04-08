using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ModalWindowTemplate
{
    public string title;
    public Sprite image;
    [TextArea(3, 10)] public string context;

    public string confirmText;
    public string declineText;
    public string alternateText;
}
