using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioManager : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            button.gameObject.AddComponent<ButtonAudio>();
        }
    }
}
