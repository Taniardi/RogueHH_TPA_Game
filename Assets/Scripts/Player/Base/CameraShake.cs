using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake Instance;
    public static CameraShake GetInstance()
    {
        return Instance;
    }

    private Camera cam;
    float shakeDur = 0f;
    float shakeMag = 0.6f;
    float damingSpeed = 1f;
    
    Vector3 initialPos;

    void Start()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        initialPos = transform.localPosition;
    }

    void Update()
    {
        if(shakeDur > 0)
        {
            transform.localPosition = initialPos + Random.insideUnitSphere * shakeMag;
            shakeDur -= Time.deltaTime * damingSpeed;
        }
        else
        {
            shakeDur = 0;
            transform.localPosition = initialPos;
        }
        if (Input.GetKeyDown(KeyCode.Pause))
        {
            Shake();
        }
    }

    public void Shake(float duration = .1f, float magnitude = 0.6f)
    {
        shakeDur = duration;
        shakeMag = magnitude;
    }

}
