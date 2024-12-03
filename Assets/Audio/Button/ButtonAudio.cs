using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        audioManager.PlayButtonSound(audioManager.ButtonHoverSFX);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioManager.PlayButtonSound(audioManager.ButtonClickSFX);
    }
}
