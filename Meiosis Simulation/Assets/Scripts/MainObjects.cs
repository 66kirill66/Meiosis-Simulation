using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjects : MonoBehaviour
{
    public int ChromosomId { get; private set; }
    public int ChromosomesHomologousId { get; private set; }
    public int SisterChromatidsId { get; private set; }
    public int TheCellId { get; private set; }
    public int MeiosisProcessId { get; private set; }

    private List<CellLogic> cellsList = new List<CellLogic>();
    public static MainObjects Instance;

    public bool isMicroView;
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
        if (Application.isEditor)
        {
            StartCoroutine(CreateChromosoms());

        }
    }

    // from web
    public void SetSisterChromatidsId(int id)
    {
        SisterChromatidsId = id;
    }
    public void SetMeiosisProcessId(int id)
    {
        MeiosisProcessId = id;
    }
    public void SetChromosomesHomologousId(int id)
    {
        ChromosomesHomologousId = id;
    }
    public void SetTheCellId(int id)
    {
        TheCellId = id;
    }
    public void SetChromosomId(int id)
    {
        ChromosomId = id;
        StartCoroutine(CreateChromosoms());
    }

    private IEnumerator CreateChromosoms()
    {
        if (!SimulationConfig.Instance.isInit)
        {
            yield return new WaitForEndOfFrame();
        }

        if (cellsList.Count == 0)
        {
            cellsList.AddRange(FindObjectsOfType<CellLogic>());
            foreach (var cell in cellsList)
            {
                cell.CreateChromosoms();
            }
        }
    }
}
