using UnityEngine;


public class SimulationConfig : MonoBehaviour
{
    public static SimulationConfig Instance { get; private set; }
    public int SimulationConfigId { get; private set; }

    [SerializeField] CameraController cameraController;
    public GameObject chromosomesView;
    public GameObject familyView;
    public GameObject meiosisView;
    public GameObject fertilizationView;
    public GameObject doubleMeiosisView;
    public GameObject mitosisMacro;
    public GameObject mitosis;
    public bool isInit;

    public float moveSpeed;

    public ViewType viewT = ViewType.MeiosisView;
    private void Awake()
    {

        meiosisView.SetActive(false);
        familyView.SetActive(false);
        fertilizationView.SetActive(false);
        chromosomesView.SetActive(false);
        doubleMeiosisView.SetActive(false);
        mitosisMacro.SetActive(false);
        mitosis.SetActive(false);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


    }
    void Start()
    {
        // Set simulation config in editor
        if (Application.isEditor)
        {
            int id = 1;
            string viewType = viewT.ToString();
            Data data = new Data { id = id, viewType = viewType };


            var json = JsonUtility.ToJson(data);
            Debug.Log(json);

            SetSimulationConfigId(json);
        }
    }

    public void SetSimulationConfigId(string json)
    {
        Data data = JsonUtility.FromJson<Data>(json);
        if (data == null)
        {
            Debug.LogError("Error: Failed to parse JSON into Data object");
            return;
        }
        SimulationConfigId = data.id;

        ViewType? viewType = TypeHelper.Parse<ViewType>(data.viewType);

        if (!viewType.HasValue)
        {
            Debug.LogError($"Error: Invalid ViewType value in JSON: {data.viewType}");
            return;
        }
        moveSpeed = 2;
        viewT = viewType.Value;

        switch (viewType)
        {
            case ViewType.MeiosisView:
                cameraController.StartCameraSize(6);
                meiosisView.SetActive(true);
                CycleLogic.Instance.UpdateCycleNumber(0);
                break;
            case ViewType.FamilyView:
                familyView.SetActive(true);
                break;
            case ViewType.FertilizationView:
                fertilizationView.SetActive(true);
                break;
            case ViewType.ChromosomesView:
                chromosomesView.SetActive(true);
                break;
            case ViewType.DoubleMeiosisView:
                moveSpeed = 2.5f;
                cameraController.StartCameraSize(14);
                doubleMeiosisView.SetActive(true);
                break;
            case ViewType.MitosisMacroView:
                mitosisMacro.SetActive(true);
                break;
            case ViewType.MitosisView:
                cameraController.StartCameraSize(6);
                mitosis.SetActive(true);
                CycleLogic.Instance.UpdateCycleNumber(0);
                break;
            default:
                break;
               
        }
        LangSupport.Instance.SetLang();
        cameraController.cameraSizeOnDivision = viewType.Value == ViewType.MeiosisView ? 14.5f : 16;
        Debug.Log($"ViewType set to: {viewType.Value}");
        isInit = true;
    }
}
