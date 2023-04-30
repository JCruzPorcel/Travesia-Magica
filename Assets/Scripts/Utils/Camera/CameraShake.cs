using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float shakeIntensity = 20f;
    private float shakeTime = .2f;

    private float timer;
    private CinemachineBasicMultiChannelPerlin _cbmcp;


    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        StopShake();
    }

    public void ShakeCamera()
    {
        _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntensity;

        timer = shakeTime;
    }

    public void ShakeCameraWhenHit(float intensity)
    {
        _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = intensity;

        timer = shakeTime;
    }

    public void StopShake()
    {
        _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;

        timer = 0f;
    }


    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                StopShake();
            }
        }
    }
}
