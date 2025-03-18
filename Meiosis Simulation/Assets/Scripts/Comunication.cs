using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class Comunication : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void callTNITfunction();

    [DllImport("__Internal")]
    public static extern void onResetDone();

    [DllImport("__Internal")]
    public static extern void EntityClick(int id);

    [DllImport("__Internal")]
    public static extern void MeiosisBegins(int meiosisId);

    [DllImport("__Internal")]
    public static extern void UpdatePositionedLocation(int id, string location);

    [DllImport("__Internal")]
    public static extern void OnWereFormed(int sisterChromatidsId);

    [DllImport("__Internal")]
    public static extern void FirstDivisionDone(int cellId);

    [DllImport("__Internal")]
    public static extern void ReplacementIsOver(int homologousId);


    public List<CellLogic> cellLogicList = new List<CellLogic>();

    public static Comunication Instance { get; private set; }
    static bool initFirstTime = true;

    public int divisionNumber;

    public bool isCellDivision;

    public bool isFirstDivisionDone;

    private bool onRandomPositions;

    public int cellsCountOnSecondDivision;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!Application.isEditor && initFirstTime == true)
        {
            callTNITfunction();
            initFirstTime = false;
        }
        else
        {
            if (!Application.isEditor)
            {
                onResetDone();
            }
        }
    }
    private void Start()
    {
        // To Check in Editor
        if (Application.isEditor)
        {
            Invoke(nameof(OnChromisomsDuplicate), 1);
            Invoke(nameof(OnSisterChromatidsFormed), 2);

            //check wrong rule
            // Invoke(nameof(OnPairUP), 2);
        }
    }
    private void Update()
    {
        if (isCellDivision)
        {
            isCellDivision = false;
            ApplyDivision();

            //check wrong rule
           // OnCellDuplicate();
        }

        // update the number of cells to know when the second division is finished
        if (!onRandomPositions && cellLogicList.Count == (cellsCountOnSecondDivision = SimulationConfig.Instance.viewT == ViewType.DoubleMeiosisView ? 8 : 4))
        {
            Debug.Log("OnSecondDivisionDone :" + cellsCountOnSecondDivision);
            Debug.Log("OnSecondDivisionDone cells count :" + cellLogicList.Count);
            foreach (var cellLogic in cellLogicList)
            {
                cellLogic.SetRandomPosition();
            }
            onRandomPositions = true;
        }
    }


    public void SetSimulationStateInUnity(string state)
    {
        switch (state)
        {
            case "PAUSED":
                Time.timeScale = 0;
                break;
            case "RUNNING":
                Time.timeScale = 1;
                break;
        }
    }

    public void StartMeiosis(int meiosisId)
    {
        if (!Application.isEditor)
        {
            MeiosisBegins(meiosisId);
        }          
    }
    public void ClickOnEntity(int id)
    {
        if (!Application.isEditor)
        {
            EntityClick(id);
        }
    }

    public void ResetSimulatiom()
    {
        SceneManager.LoadScene(0);
    }

    public void OnChromisomsDuplicate()  // from web
    {
        CheckCellsList();
        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.OnChromisomsDuplicate();
        }
        SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.ChromosomesReplicated);
    }

    public void OnPairUP()
    {
        CheckCellsList();
        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.PairUP();
        }
    }

    public void OnSisterChromatidsFormed()
    {
        StartCoroutine(OnSisterChromatidsFormedDeley());
    }


    private IEnumerator OnSisterChromatidsFormedDeley()
    {
        CheckCellsList();
        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.SisterChromatidsFormed();
        }

        yield return new WaitForSeconds(3);
        SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.SisterChromatidsFormed);
        if (!Application.isEditor)
        {
            int id = MainObjects.Instance.SisterChromatidsId;
            OnWereFormed(id);
        }
    }

    public void UpdatePositionedLocationToWeb(string chromosomeType, string location)
    {
        Debug.Log(location);
        if (!Application.isEditor)
        {
            int id = chromosomeType == "Homologous" ? MainObjects.Instance.ChromosomesHomologousId : MainObjects.Instance.SisterChromatidsId;
            Debug.Log("UpdatePositionedLocationToWeb :" + location);
            UpdatePositionedLocation(id, location);
        }
    }

    public void AlignOn(string json) // from web
    {
        Data data = JsonUtility.FromJson<Data>(json);
        string locationType = data.locationType;
        string entityTpe = data.entityType;
        AlignOnType? location = TypeHelper.Parse<AlignOnType>(locationType);

        if (!location.HasValue)
        {
            Debug.LogError($"Error: Invalid LocationType value: {locationType}");
             return;
        }

        CheckCellsList();

        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.SetChromosomesAlignOn(location.Value, entityTpe); // .Value, sure that it exists
        }
    }

    private void CheckCellsList()
    {
        if (cellLogicList.Count == 0)
        {
            cellLogicList.AddRange(FindObjectsOfType<CellLogic>());
        }
    }

    public void MoveToward(string json)  // from web
    {
        Data data = JsonUtility.FromJson<Data>(json);
        string locationType = data.locationType;
        string entityTpe = data.entityType;

        MoveTowardType? location = TypeHelper.Parse<MoveTowardType>(locationType);

        if (!location.HasValue)
        {
            Debug.LogError($"Error: Invalid LocationType value: {locationType}");
            return;
        }

        CheckCellsList();

        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.SetMoveToward(location.Value, entityTpe); // .Value =  sure that it exists
        }
    }


    public void StartReplacement()
    {
        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.ApplyReplacement();
        }
        Invoke(nameof(OnReplacementOver), 3);
    }

    private void OnReplacementOver()
    {
        if (!Application.isEditor)
        {
            SimulationEvents.Instance.SendEventToPlethora(SimulationEventsTypes.HomologousChromosomesUndergoCrossingOver); 
            int id = MainObjects.Instance.ChromosomesHomologousId;
            ReplacementIsOver(id);
        }
    }

    public void ApplyDivision()
    {
        SimulationEventsTypes eventType = divisionNumber == 0 ? SimulationEventsTypes.CellUnderwentFirstDivision : SimulationEventsTypes.CellUnderwentSecondDivision;
        SimulationEvents.Instance.SendEventToPlethora(eventType);
        divisionNumber++;
        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.Division(); 
        }
    }

    public void OnCellDuplicate()
    {
       
        divisionNumber++;
        foreach (var cellLogic in cellLogicList)
        {
            cellLogic.Division(false);
        }
    }

    public void OnFirstDivisionDone()
    {
        if (!Application.isEditor)
        {           
            if (!isFirstDivisionDone)
            {
                isFirstDivisionDone = true;
                int id = MainObjects.Instance.TheCellId;
                FirstDivisionDone(id);
            }
        }    
    }
}

