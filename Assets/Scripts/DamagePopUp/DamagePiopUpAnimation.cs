using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePiopUpAnimation : MonoBehaviour
{
    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;

    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector3 origin;

    private void Awake()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void ResetAnimation(Vector3 startPosition)
    {
        time = 0;
        transform.localScale = Vector3.one;
        tmp.color = new Color(1, 1, 1, 1); 
        transform.position = startPosition; 
        origin = startPosition; 
    }

    private void Update()
    {
        time += Time.deltaTime;

        tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        transform.position = origin + new Vector3(0, 1 + heightCurve.Evaluate(time), 0);
    }
}
