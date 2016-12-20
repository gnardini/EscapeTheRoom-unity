using System;
using UnityEngine;

public class MouseLook : MonoBehaviour {
    
    private float mouseSensitivity = 50.0f;
    private float clampAngle = 80.0f;

    private float rotY; // rotation around the up/y axis
    private float rotX; // rotation around the right/x axis

    private bool _paused;
    private bool _first;

    void Start () {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        _paused = false;
        _first = true;
    }

    void Update () {
        if (_paused) {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        if (_first && (mouseX != 0 || mouseY != 0)) {
            _first = false;
            return;
        }

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }

    public void SetPaused(bool paused) {
        _paused = paused;
    }

}
