using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cameraMain;
    public float cameraSizeOnDivision;
    private void Awake()
    {
        cameraSizeOnDivision = 14.5f;
        cameraMain = GetComponent<Camera>();
    }
    void Start()
    {
       
    }

    public void SetCameraPosition()
    {
        StartCoroutine(ChangeCameraSize(cameraSizeOnDivision));
    }

    public void StartCameraSize(float value)
    {
        cameraMain = GetComponent<Camera>();
        cameraMain.orthographicSize = value;
    }

    private IEnumerator ChangeCameraSize(float cameraSize)
    {
       // Camera cameraMain = GetComponent<Camera>();
        while (cameraMain.orthographicSize < cameraSize)
        {
            cameraMain.orthographicSize += 1.5f * Time.deltaTime;
            yield return null;
        }
    }
}
