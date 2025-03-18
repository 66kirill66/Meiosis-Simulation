using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] GameObject coriander;
    [SerializeField] GameObject coriander_sensitivity;
    [SerializeField] GameObject bloodType;
    [SerializeField] GameObject earwax_dry;
    [SerializeField] GameObject earwax_wet;
    [SerializeField] GameObject freckles;
    [SerializeField] GameObject no_freckles;

    public List<Transform> chromosomeTransformList = new List<Transform>();
    public List<Transform> chromosomeList = new List<Transform>();
    public List<GameObject> createdChromosomesList = new List<GameObject>();
    public List<Color> colorsList = new List<Color>();

    public bool isMotherCells;
    private ChaildCreator chaildCreator;
    private bool isChromosomesCreated;

    private void Awake()
    {
        chaildCreator = FindObjectOfType<ChaildCreator>();
        coriander.SetActive(false);
        coriander_sensitivity.SetActive(false);
        bloodType.SetActive(false);
        earwax_dry.SetActive(false);
        earwax_wet.SetActive(false);
        freckles.SetActive(false);
        no_freckles.SetActive(false);
    }
    void Start()
    {
       
    }

    public void UpdateOptionSprites(bool isSensitivity, bool dryEarwax, bool isFreckles)
    {
        coriander.SetActive(true);
        bloodType.SetActive(true);
        freckles.SetActive(true);
        coriander_sensitivity.SetActive(isSensitivity);
        earwax_dry.SetActive(dryEarwax);
        earwax_wet.SetActive(!dryEarwax);
        no_freckles.SetActive(!isFreckles);

        if (isMotherCells) { chaildCreator.SetMotherCombination(isSensitivity, dryEarwax, isFreckles); }
        else { chaildCreator.SetFatherCombination(isSensitivity, dryEarwax, isFreckles); }

        if (!isChromosomesCreated)
        {
            isChromosomesCreated = true;
            CreateChromosomes();
        }
        SetColor(isSensitivity, isMotherCells, dryEarwax);
    }

    public void CreateChromosomes()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject cromosome = Instantiate(chromosomeList[i], chromosomeTransformList[i].position, Quaternion.identity).gameObject;
            createdChromosomesList.Add(cromosome);
        }
    }
    public void SetColor(bool isSensitivity,bool bloodType, bool dryEarwax)
    {
        Color color_1 = isSensitivity ? colorsList[0] : colorsList[1];
        Color color_2 = bloodType ? colorsList[2] : colorsList[3];
        Color color_3 = dryEarwax ? colorsList[5] : colorsList[4];
        Helper.ChangeColorRecursively(createdChromosomesList[0].transform, color_1);
        Helper.ChangeColorRecursively(createdChromosomesList[1].transform, color_2);
        Helper.ChangeColorRecursively(createdChromosomesList[2].transform, color_3);
    }
}
