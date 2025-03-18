using System.Collections.Generic;
using UnityEngine;

public class CycleLogic : MonoBehaviour
{
    public int cycleNumber;
    [SerializeField] GameObject cyclePanel;
    [SerializeField] CameraController cameraController;
    public List<Transform> cyclesTransform = new List<Transform>();

    public static CycleLogic Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cyclePanel.transform.position = cyclesTransform[cycleNumber].position;
    }

    public void UpdateCycleNumber(int number)
    {
        cycleNumber = number;
        if(cycleNumber == 3)
        {
            cameraController.SetCameraPosition();
        }
    }
}
