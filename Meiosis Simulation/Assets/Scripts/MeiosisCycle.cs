using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeiosisCycle : MonoBehaviour
{
    public int cycleNumber;
    [SerializeField] Button startCycle;

    private void Awake()
    {
        LangSupport.Instance.startText = startCycle.GetComponentInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        cycleNumber = 1;
        startCycle.onClick.AddListener(OnCycleStart);
    }

    public void OnCycleStart()
    {
        int meiosisId = MainObjects.Instance.MeiosisProcessId;
        Comunication.Instance.StartMeiosis(meiosisId);
        startCycle.onClick.RemoveListener(OnCycleStart);
        SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.ClickStart);
        startCycle.interactable = false;

    }
}
