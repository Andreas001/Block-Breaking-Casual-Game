using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Vector3 originalCameraPosition;

    float shakeAmount = 0;

    public Camera mainCamera;

    private void Awake() {
        if (!mainCamera) {
            mainCamera = Camera.main;
        }

        originalCameraPosition = mainCamera.transform.position;
    }

    void CameraShake() {
        if (shakeAmount > 0) {
            float quakeAmount = Random.value * shakeAmount * 2 - shakeAmount;
            Vector3 position = mainCamera.transform.position;
            position.x += quakeAmount;
            position.y += quakeAmount;
            position.z = mainCamera.transform.position.z;

            mainCamera.transform.position = position;
        }
    }

    public void StartShaking(Collision2D collision) {
        shakeAmount = collision.relativeVelocity.magnitude * .0025f;
        InvokeRepeating("CameraShake", 0, .01f);
        Invoke("StopShaking", 0.3f);
    }

    void StopShaking() {
        CancelInvoke("CameraShake");
        mainCamera.transform.position = originalCameraPosition;
    }
}
