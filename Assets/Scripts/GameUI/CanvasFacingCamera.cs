using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFacingCamera : MonoBehaviour
{
    private Camera cam;
    private GameObject canvas;

    void Start()
    {
        canvas = transform.Find("Canvas").gameObject;
        cam = Camera.main;
    }

    void Update()
    {
        canvas.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
    }
}
