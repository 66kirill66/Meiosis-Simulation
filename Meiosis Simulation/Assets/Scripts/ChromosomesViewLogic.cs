using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChromosomesViewLogic : MonoBehaviour
{
    public static ChromosomesViewLogic Instance { get; private set; }

    [SerializeField] GameObject chromosomeTLong;
    [SerializeField] GameObject chromosomeBLong;
    [SerializeField] GameObject chromosomeNormal;

    [SerializeField] List<Transform> spawnTransforms = new List<Transform>();
    [SerializeField] List<Transform> tablePlaces = new List<Transform>();
    [SerializeField] List<PlacementLogic> placementLogics = new List<PlacementLogic>();
    private readonly Dictionary<int, int> clickRequirements = new Dictionary<int, int>
    {
    { 7, 0 },
    { 11, 2 },
    };

    public List<GameObject> bloodToolTip = new List<GameObject>();
    public List<GameObject> corianderToolTip = new List<GameObject>();
    public List<GameObject> earwaxToolTip = new List<GameObject>();
    public List<GameObject> fricklesToolTip = new List<GameObject>();
    private bool fricklesToolTipActive;
    private bool earwaxToolTipActive;
    private bool isChromosomesDistributed;

    public int clickCount;
    [SerializeField] GameObject motherChromosomes;
    [SerializeField] List<GameObject> motherChromosomesList = new List<GameObject>();

    [SerializeField] GameObject longChromosomeEvent;
    [SerializeField] GameObject shortChromosomeEvent;
    [SerializeField] GameObject chromosomesNumbers;



    private void Awake()
    {
        chromosomesNumbers.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        motherChromosomes.SetActive(false);
        clickCount = 0;
    }

    void Start()
    {
        SpawnChromosomes();
    }


    /// <summary>
    /// Spawns six chromosomes at random positions from a predefined list of spawn points.
    /// Each chromosome has a predefined type, scale offset, and target placement position.
    /// </summary>
    /// <remarks>
    /// - Ensures there are at least 6 spawn points before spawning.
    /// - Selects a random spawn position for each chromosome and removes it from the available list.
    /// - Assigns each chromosome a predefined chromosome number, scale offset, and target placement.
    /// - Uses predefined arrays to simplify logic and avoid multiple conditional checks.
    /// </remarks>
    public void SpawnChromosomes()
    {
        if (spawnTransforms.Count < 6)
        {
            Debug.LogError("Not enough spawn points!");
            return;
        }

        // Set data
        int[] chromosomeNumbers = { 7, 7, 9, 9, 11, 11 };
        float[] scaleOffsets = { 0.05f, 0.05f, 0f, 0f, -0.05f, -0.05f };
        int[] tablePlaceIndices = { 0, 0, 1, 1, 2, 2 };
        float[] xOffsets = { 0, -1.2f, 0, -1.2f, 0, -1.2f };
        GameObject[] chromosomePrefabs = {
            chromosomeTLong, chromosomeTLong,
            chromosomeBLong, chromosomeBLong,
            chromosomeNormal, chromosomeNormal 
        };

        for (int i = 0; i < 6; i++)
        {
            // Generate a random spawn point
            int randomIndex = UnityEngine.Random.Range(0, spawnTransforms.Count);
            Vector3 spawnPosition = spawnTransforms[randomIndex].position;
            spawnTransforms.RemoveAt(randomIndex);

            // Create a chromosome
            Quaternion rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-360f, 360f));
            GameObject chromosome = Instantiate(chromosomePrefabs[i], spawnPosition, rotation);

            // Apply scale
            Vector3 newScale = chromosome.transform.localScale + Vector3.one * scaleOffsets[i];
            chromosome.transform.localScale = newScale;

            // Add PlacementLogic
            PlacementLogic pl = chromosome.AddComponent<PlacementLogic>();
            pl.chromosomeNumber = chromosomeNumbers[i];

            // Set tablePlace taking into account xOffset
            Transform targetTablePlace = tablePlaces[tablePlaceIndices[i]];
            pl.tablePlace = new Vector3(targetTablePlace.position.x + xOffsets[i], targetTablePlace.position.y);
            placementLogics.Add(pl);
        }
        SetChromosomeEventsPlace();
    }


    /// <summary>
    /// Checks if chromosomes can be activated based on the number of clicks.
    /// If all conditions are met, starts their movement.
    /// </summary>
    /// <param name="placementLogic">A reference to the PlacementLogic object responsible for placing the chromosome.</param>
    public void CheckClickOnChromosome(PlacementLogic placementLogic, int chromosomeNumber)
    {
        if (clickRequirements.TryGetValue(chromosomeNumber, out int requiredClicks) && clickCount >= requiredClicks && clickCount < requiredClicks + 2)
        {
            clickCount++;
            placementLogic.SetPlace();
            //send event
            if(clickCount == 2) // select 2 long chromosemes
            {
                SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.SelectLongChromosome);
            }
           // if (chromosomeNumber == 11)
            if (clickCount == 4) //select 2 short Chromosome
            {
                SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.SelectShortChromosome);
            }
            if (clickCount == 4)
            {
                isChromosomesDistributed = true;
                Invoke(nameof(PlacmentOtherChromosomes), 2);
                Invoke(nameof(ActivateMotherChromosomes), 3);
            }
        }
    }

    public void ClickOnChromosomeByUserRequest(int chromosomeNumber)
    {
        foreach(PlacementLogic i in placementLogics)
        {
            if(i.chromosomeNumber == chromosomeNumber)
            {
                i.Click();
            }
        }
    }
    private void SetChromosomeEventsPlace()
    {
        float offsetY = 0.5f;
        Vector3 longPosition = new Vector3(placementLogics[0].transform.position.x, placementLogics[0].transform.position.y - offsetY);
        Vector3 shortosition = new Vector3(placementLogics[5].transform.position.x, placementLogics[5].transform.position.y - offsetY);
        longChromosomeEvent.transform.position = longPosition;
        shortChromosomeEvent.transform.position = shortosition;
    }

    public void PlacmentOtherChromosomes()
    {
        placementLogics[2].SetPlace();
        placementLogics[3].SetPlace();
    }

    private void ActivateMotherChromosomes()
    {
        motherChromosomes.SetActive(true);
        chromosomesNumbers.SetActive(true);
    }

    /// <summary>
    /// Assign a color depending on the Name of the pressed button
    /// </summary>
    public void ClickOnToolTipButtone(string eventName) // to:do refactoring of the function
    {
        if (isChromosomesDistributed)
        {
            SimulationEventsTypes? eventType = TypeHelper.Parse<SimulationEventsTypes>(eventName);

            if (!eventType.HasValue)
            {
                Debug.LogError($"Error: Invalid ViewType value in JSON: {eventName}");
                return;
            }

            Color color_1;
            Color color_2;
            List<GameObject> currentlist = new List<GameObject>();
            switch (eventType)
            {
                case SimulationEventsTypes.ClickBloodIcon:
                    color_1 = Color.blue;
                    color_2 = new Color32(148, 227, 255, 255); // color (R:148, G:227, B:255)                  
                    currentlist = bloodToolTip;
                    Helper.ChangeColorRecursively(placementLogics[2].transform, color_1);
                    Helper.ChangeColorRecursively(placementLogics[3].transform, color_1);

                    Helper.ChangeColorRecursively(motherChromosomesList[2].transform, color_2);
                    Helper.ChangeColorRecursively(motherChromosomesList[3].transform, color_2);
                    break;
                case SimulationEventsTypes.ClickCorianderIcon:
                    color_1 = Color.yellow;
                    color_2 = new Color32(120, 102, 4, 255); // color (R:120, G:102, B:4)
                    currentlist = corianderToolTip;
                    Helper.ChangeColorRecursively(placementLogics[0].transform, color_1);
                    Helper.ChangeColorRecursively(placementLogics[1].transform, color_2);

                    Helper.ChangeColorRecursively(motherChromosomesList[0].transform, color_1);
                    Helper.ChangeColorRecursively(motherChromosomesList[1].transform, color_2);
                    break;
                case SimulationEventsTypes.ClickEarwaxIcon:
                    currentlist = earwaxToolTip;
                    earwaxToolTipActive = true;
                    break;
                case SimulationEventsTypes.ClickFrecklesIcon:
                    currentlist = fricklesToolTip;
                    fricklesToolTipActive = true;
                    break;
            }
            SimulationEvents.Instance.SendEventToPlethora(eventType.Value);
            if (fricklesToolTipActive && earwaxToolTipActive)
            {
                color_1 = new Color32(27, 38, 31, 255);
                color_2 = new Color32(49, 125, 79, 255);
               // currentlist = eleventhChromazoneList;
                Helper.ChangeColorRecursively(placementLogics[4].transform, color_1);
                Helper.ChangeColorRecursively(placementLogics[5].transform, color_2);

                Helper.ChangeColorRecursively(motherChromosomesList[4].transform, color_2);
                Helper.ChangeColorRecursively(motherChromosomesList[5].transform, color_2);
            }
            ActivateObjects_In(currentlist);
        }       
    }

    public void ActivateObjects_In(List<GameObject> listToActivate)
    {
        foreach (GameObject i in listToActivate)
        {
            i.SetActive(true);
        }
    }
}
