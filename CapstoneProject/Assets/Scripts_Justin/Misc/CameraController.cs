using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Script for performing camera movements, such as screen shake
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin perlin;

    private float currentInensity = 0f;
    private float shakeTimer = 0f;
    private float currentShakeTimer = 0f;


    public void ShakeCamera(float intensity, float time)
    {
        if (intensity < currentInensity) return;
        shakeTimer = time;
        currentShakeTimer = time;
        currentInensity = intensity;
        perlin.AmplitudeGain = intensity;
    }

    private void Update()
    {
        if(currentShakeTimer > 0)
        {
            currentShakeTimer -= Time.deltaTime;
            if (currentShakeTimer <= 0f)
            {
                perlin.AmplitudeGain = 0f;
                currentInensity = 0f;
            }
        }
    }
}
