using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChaildCreator : MonoBehaviour
{
    private bool motherSensitivity;
    private bool motherDryEarwax;
    private bool motherFreckles;

    private bool fatherSensitivity;
    private bool fatherDryEarwax;
    private bool fatherFreckles;

    [SerializeField] Button startButtone;

    private void Awake()
    {
        startButtone.onClick.AddListener(() => CreateChaild());
        LangSupport.Instance.startText = startButtone.GetComponentInChildren<TextMeshProUGUI>();
        
    }

    public void SetMotherCombination(bool isSensitivity, bool dryEarwax, bool isFreckles)
    {
        motherSensitivity = isSensitivity;
        motherDryEarwax = dryEarwax;
        motherFreckles = isFreckles;
    }

    public void SetFatherCombination(bool isSensitivity, bool dryEarwax, bool isFreckles)
    {
        fatherSensitivity = isSensitivity;
        fatherDryEarwax = dryEarwax;
        fatherFreckles = isFreckles;
    }

    public void CreateChaild()
    {
        Debug.Log("Chaild Created");
        SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.ClickFertilizationStart);
    }


}
