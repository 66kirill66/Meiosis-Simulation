using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class SimulationEvents : MonoBehaviour
{
    public static SimulationEvents Instance { get; private set; }
    [DllImport("__Internal")]
    public static extern void SendEvent(string eventName); // TO:Do add function in JSLIB

    public GameObject guidedPlaces;
    public GameObject handPng;
    private readonly float speed = 2f;
    private Vector3 startHandPosition;
    private Coroutine currentCoroutine;

    private string requestedEvent;

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

    public void SendEventToPlethora(SimulationEventsTypes eventType)
    {
        string eventName = eventType.ToString();
        if (!Application.isEditor)
        {
            SendEvent(eventName);
            if (requestedEvent == eventName)
            {
                BackToStart();
            }
        }
    }

    public void OnGuidedEventRequest(string eventName)
    {
        requestedEvent = eventName;
        Transform child = guidedPlaces.transform.Find(eventName);
        StopCoroutine();
        if (child != null)
        {
            handPng.SetActive(true);
            currentCoroutine = StartCoroutine(SetHandPosition(child));
        }
        else
        {
            Debug.LogError($"Guide place '{eventName}' not found.");
        }
    }

    public void OnUserEventRequest(string eventName)
    {
        SimulationEventsTypes? eventType = TypeHelper.Parse<SimulationEventsTypes>(eventName);

        if (!eventType.HasValue)
        {
            Debug.LogError($"Error: Invalid ViewType value in JSON: {eventName}");
            return;
        }
        switch (eventType)
        {
            case SimulationEventsTypes.ClickStart:
                FindObjectOfType<MeiosisCycle>().OnCycleStart();
                break;
            case SimulationEventsTypes.ClickFertilizationStart:
                FindObjectOfType<ChaildCreator>().CreateChaild();
                break;
            case SimulationEventsTypes.ClickBloodIcon:               
            case SimulationEventsTypes.ClickCorianderIcon:
            case SimulationEventsTypes.ClickEarwaxIcon:
            case SimulationEventsTypes.ClickFrecklesIcon:
                ChromosomesViewLogic.Instance.ClickOnToolTipButtone(eventType.ToString());
                break;
            case SimulationEventsTypes.SelectLongChromosome:
                ChromosomesViewLogic.Instance.ClickOnChromosomeByUserRequest(7);
                break;
            case SimulationEventsTypes.SelectShortChromosome:
                ChromosomesViewLogic.Instance.ClickOnChromosomeByUserRequest(11);
                break;
        }
    }

    public void BackToStart()
    {
        StopCoroutine();
        currentCoroutine = StartCoroutine(Back());
    }

    private void StopCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    public IEnumerator SetHandPosition(Transform position)
    {
        Vector3 to = position.transform.position;
        while (Vector3.Distance(handPng.transform.position, to) > 0.001f)
        {
            handPng.transform.position = Vector3.Lerp(handPng.transform.position, to, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }
    public IEnumerator Back()
    {
        handPng.SetActive(false);
        while (Vector3.Distance(handPng.transform.position, startHandPosition) > 0.001f)
        {
            handPng.transform.position = Vector3.Lerp(handPng.transform.position, startHandPosition, 1);
            yield return null;
        }
        yield return new WaitForEndOfFrame();
    }
}


