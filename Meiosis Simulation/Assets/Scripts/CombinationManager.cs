using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : MonoBehaviour
{
    [SerializeField] bool isSensitivity;
    [SerializeField] string bloodType;
    [SerializeField] bool dryEarwax;
    [SerializeField] bool isFreckles;
    [SerializeField] OptionsManager optionsManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetCombination()
    {
        optionsManager.UpdateOptionSprites(isSensitivity, dryEarwax, isFreckles);
    }

}
