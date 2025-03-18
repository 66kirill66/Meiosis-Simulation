using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CellLogic : MonoBehaviour
{
    public bool isMainCell;
    // Chromosomes prefabs
    [SerializeField] GameObject chromosomPrefab;
    [SerializeField] GameObject chromosomeLongTopPrefab;
    [SerializeField] GameObject chromosomeLongBottomPrefab;

    [SerializeField] GameObject chromosomTransform; // chromosomes Place
    public List<Transform> chromosomPositions = new List<Transform>(); // spawn position
    public List<Color> colorsList = new List<Color>();


    public GameObject cellToDuplicate;
    private CellLogic the_leftCell;
    private CellLogic the_rightCell; 

    private List<CentrioleLogic> centriolList = new List<CentrioleLogic>();
    public List<ChromosomLogic> cromosomsList = new List<ChromosomLogic>();

    public bool conectTheCentrions;

    public bool isAlineOn;

    public Transform rightCenterPlace;
    public Transform leftCenterPlace;

    public Transform rightPlace;
    public Transform leftPlace;

    public Transform centerPlace;

    public AlignOnType alineType;

    public bool chromosomsMoving;
    public bool sopuration;
    public int separationNumber;

    public bool isReplaicment;

    void Start()
    {
        centriolList.AddRange(GetComponentsInChildren<CentrioleLogic>());      
    }

    // Update is called once per frame
    void Update()
    {
        if (conectTheCentrions)
        {
            conectTheCentrions = false;
            SetCentrionPositions();
        }
        if (isAlineOn)
        {
            StartCoroutine(AlineOn());
        }

        if (isReplaicment)
        {
            isReplaicment = false;
            ApplyReplacement();
        }
    }

    /// <summary>
    /// Determines and sets positions for centrioles (CentrioleLogic),
    /// linking them to the corresponding chromosomes (Chromosome).
    /// 
    /// Operation logic:
    /// 1. Determine the left and right centriole from the `centriolList` list.
    /// 2. Create a position map (`Dictionary`), where the key is a combination of `isRight`, `isOnTop`, `isOnCenter`,
    /// and the value is an action that sets the corresponding point (`topPoint`, `centerPoint`, `bottomPoint`).
    /// 3. Go through the `cromosomsList` list, find the corresponding centriole (`left` or `right`) and set the points.
    /// 4. After distributing the points, call `StartConectToChromosomes()` for each centriole.
    /// </summary>
    public void SetCentrionPositions()
    {
        // 1. Determine the left and right centrioles
        var left = centriolList.FirstOrDefault(c => c.isLeft);
        var right = centriolList.FirstOrDefault(c => !c.isLeft);

        // If at least one centriole is missing, exit from function
        if (left == null || right == null)
            return;

        // 2. Position map: defines the correspondence between flags and centriole properties
        Dictionary<(bool isRight, bool isOnTop, bool isOnCenter), System.Action<CentrioleLogic, Transform>> positionMap =
            new Dictionary<(bool, bool, bool), System.Action<CentrioleLogic, Transform>>
            {
            { (true,  true,  false), (c, t) => c.topPoint = t },     // Chromosome top right → assign topPoint to right centriole
            { (true,  false, true ), (c, t) => c.centerPoint = t },  // Chromosome in the center on the right → assign centerPoint to the right centriole
            { (true,  false, false), (c, t) => c.bottomPoint = t },  // Chromosome bottom right → assign bottomPoint to right centriole
            { (false, true,  false), (c, t) => c.topPoint = t },     // Chromosome top left → left centriole assign topPoint
            { (false, false, true ), (c, t) => c.centerPoint = t },  // Chromosome in the center on the left → assign centerPoint to the left centriole
            { (false, false, false), (c, t) => c.bottomPoint = t }   // Chromosome bottom left → left centriole assigns bottomPoint
            };

        // 3. Go through the list of chromosomes and distribute their positions
        foreach (var cromosom in cromosomsList)
        {
            var targetCentriole = cromosom.isOnRight ? right : left; // Determine which centriole the chromosome belongs to
            var key = (cromosom.isOnRight, cromosom.isOnTop, cromosom.isOnCenter);

            // If there is a corresponding entry in the dictionary, call the action (set the point)
            if (positionMap.TryGetValue(key, out var setPosition))
            {
                setPosition(targetCentriole, cromosom.transform);
            }
        }

        // 4. We begin the process of connecting centrioles with chromosomes
        left.StartConectToChromosomes();
        right.StartConectToChromosomes();
    }

    public void ApplyReplacement()
    {
        GameObject leftTop = null;
        GameObject leftCenter = null;
        GameObject leftBottom = null;

        GameObject rightTop = null;
        GameObject rightCenter = null;
        GameObject rightBottom = null;
        foreach (var cromosom in cromosomsList)
        {
            ChromosomLogic chromosomLogic = cromosom.GetComponent<ChromosomLogic>();
            GameObject selectedChromosome = (Random.Range(0, 2) == 1) ? chromosomLogic.sisterChromitids : cromosom.gameObject;

            if (chromosomLogic.isOnTop && !chromosomLogic.isOnRight)
                leftTop = selectedChromosome;
            else if (!chromosomLogic.isOnTop && !chromosomLogic.isOnRight && chromosomLogic.isOnCenter)
                leftCenter = selectedChromosome;
            else if (!chromosomLogic.isOnTop && !chromosomLogic.isOnRight && !chromosomLogic.isOnCenter)
                leftBottom = selectedChromosome;
            else if (chromosomLogic.isOnTop && chromosomLogic.isOnRight)
                rightTop = selectedChromosome;
            else if (!chromosomLogic.isOnTop && chromosomLogic.isOnRight && chromosomLogic.isOnCenter)
                rightCenter = selectedChromosome;
            else if (!chromosomLogic.isOnTop && chromosomLogic.isOnRight && !chromosomLogic.isOnCenter)
                rightBottom = selectedChromosome;
        }

        // Start the exchange for all possible pairs
        TrySwapPieces(leftTop, rightTop);
        TrySwapPieces(leftCenter, rightCenter);
        TrySwapPieces(leftBottom, rightBottom);
    }

    // Function to check and start swapping
    private void TrySwapPieces(GameObject left, GameObject right)
    {
        if (left && right)
        {
            int randomNumber = Random.Range(0, 4);
            Transform L_piece = left.GetComponent<ReplacementLogic>().pieceList[randomNumber].transform;
            Transform R_piece = right.GetComponent<ReplacementLogic>().pieceList[randomNumber].transform;

            Transform parentA = L_piece.parent;
            Transform parentB = R_piece.parent;

            StartCoroutine(SwapPieces(L_piece, R_piece, parentA, parentB, 2f)); // 2 секунды на смену мест
        }
    }

    // Coroutine for smooth swapping with parent change
    private IEnumerator SwapPieces(Transform pieceA, Transform pieceB, Transform parentA, Transform parentB, float duration)
    {
        Vector3 startPosA = pieceA.position;
        Quaternion startRotA = pieceA.rotation;

        Vector3 startPosB = pieceB.position;
        Quaternion startRotB = pieceB.rotation;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Smooth movement of positions
            pieceA.position = Vector3.Lerp(startPosA, startPosB, t);
            pieceB.position = Vector3.Lerp(startPosB, startPosA, t);

            // Smooth change of rotation
            pieceA.rotation = Quaternion.Slerp(startRotA, startRotB, t);
            pieceB.rotation = Quaternion.Slerp(startRotB, startRotA, t);

            yield return null;
        }

        // Final alignment
        pieceA.position = startPosB;
        pieceA.rotation = startRotB;
        pieceB.position = startPosA;
        pieceB.rotation = startRotA;

        // Move objects to their new parents
        pieceA.SetParent(parentB);
        pieceB.SetParent(parentA);
    }



    private IEnumerator AlineOn()
    {
        if (chromosomsMoving)
        {
            yield return new WaitForEndOfFrame();
        }

        SetChromosomesAlignOn(alineType);
        isAlineOn = false;
    }

    public void SetChromosomesAlignOn(AlignOnType location, string chromosomeType = "")
    {
        switch (location)
        {
            case AlignOnType.Center:
                if (Comunication.Instance.divisionNumber < 1) { CycleLogic.Instance.UpdateCycleNumber(1); }
                else { CycleLogic.Instance.UpdateCycleNumber(4); }
                StartCoroutine(MoveTo(centerPlace, chromosomeType));
                break;
            case AlignOnType.Poles:               
                StartCoroutine(MoveTo(transform, chromosomeType, 2.5f));
                break;
            case AlignOnType.ToTheRightOfCenter:
                StartCoroutine(MoveTo(rightCenterPlace, chromosomeType));
                break;
            case AlignOnType.TheRightPole:
                StartCoroutine(MoveTo(rightPlace, chromosomeType));
                break;
            case AlignOnType.ToTheLeftOfCenter:
                StartCoroutine(MoveTo(leftCenterPlace, chromosomeType));
                break;
            case AlignOnType.TheLeftPole:
                StartCoroutine(MoveTo(leftPlace, chromosomeType));
                break;
            case AlignOnType.Random:
                if (chromosomPositions.Count == 0)
                {
                    Debug.LogWarning("No more available positions!");
                    return;
                }
                else
                {
                    foreach (var cromosom in cromosomsList)
                    {
                        cromosom.SetRandomPositionInCell(GetRandomTransform());
                    }
                }
                break;
        }
        alineType = location;
    }

    private Transform GetRandomTransform()
    {
        int randomIndex = Random.Range(0, chromosomPositions.Count);
        Transform newTransform = chromosomPositions[randomIndex];

        int lastIndex = chromosomPositions.Count - 1;
        chromosomPositions[randomIndex] = chromosomPositions[lastIndex];
        chromosomPositions.RemoveAt(lastIndex);
        return newTransform;
    }

    public void SetMoveToward(MoveTowardType location, string chromosomeType)
    {
        switch (location)
        {
            case MoveTowardType.LeftPole:
                alineType = AlignOnType.TheLeftPole;
                StartCoroutine(MoveTo(leftPlace, chromosomeType));
                break;
            case MoveTowardType.RightPole:
                alineType = AlignOnType.TheRightPole;
                StartCoroutine(MoveTo(rightPlace, chromosomeType));
                break;
            case MoveTowardType.NearestPole:               
                alineType = AlignOnType.Poles;
                if (Comunication.Instance.divisionNumber < 1) { CycleLogic.Instance.UpdateCycleNumber(2); }
                else { CycleLogic.Instance.UpdateCycleNumber(5); }
                StartCoroutine(MoveTo(transform, chromosomeType, 2.5f, 4));
                SetCentrionPositions();

                if (isMainCell)
                {
                    SimulationEventsTypes eventType = chromosomeType == "Homologous" ? 
                        SimulationEventsTypes.HomologousChromosomesMovedToNearestPole :
                        SimulationEventsTypes.SisterChromatidsMovedToNearestPole;
                    SimulationEvents.Instance.SendEventToPlethora(eventType);
                }
                break;
        }
    }


    private IEnumerator MoveTo(Transform side, string chromosomeType = "", float offset_x = 0.45f, float deley = 0) 
    {
        float moveSpeed = SimulationConfig.Instance.moveSpeed;
        float offset_y = 2.5f;
        yield return new WaitForSeconds(deley);
        if (Comunication.Instance.divisionNumber == 1 && separationNumber < 1)
        {
            separationNumber++;
            offset_x = 0.01f;
            ChromosomeSeparation();
        }

        if (chromosomsMoving)
        {
            yield return new WaitForEndOfFrame();
        }
        else if (!chromosomsMoving)
        {
            chromosomsMoving = true;
            foreach (var cromosom in cromosomsList)
            {
                cromosom.MoveToPosition(side, moveSpeed, offset_x, offset_y);
            }
        }
        yield return new WaitForSeconds(moveSpeed + 0.5f); //
        chromosomsMoving = false;
        UpdatePositionLocation(chromosomeType);
    }

    private void UpdatePositionLocation(string chromosomeType)
    {
        if (isMainCell)
        {
            if (alineType == AlignOnType.Center)
            {
                SimulationEventsTypes eventType = chromosomeType == "Homologous" ? 
                    SimulationEventsTypes.ChromosomesAlignedOnCenter :
                    SimulationEventsTypes.SisterChromatidsAlignedOnCenter;
                SimulationEvents.Instance.SendEventToPlethora(eventType);
            }
            Comunication.Instance.UpdatePositionedLocationToWeb(chromosomeType, alineType.ToString());
        }
    }

   

    public void Division(bool setCellBelonging = true) // start from rule
    {
        foreach (var centriol in centriolList)
        {
            centriol.ResetState();
        }

        StartCoroutine(CellDivision(setCellBelonging));
    }

    public void PairUP()
    {
        SetChromosomesList();
        IncorrectDistribution();
    }

    private void SetChromosomesList()
    {
        if (cromosomsList.Count == 0)
        {
            cromosomsList.AddRange(chromosomTransform.GetComponentsInChildren<ChromosomLogic>());
        }
    }

    public void OnChromisomsDuplicate() 
    {
        SetChromosomesList();
        foreach (var cromosom in cromosomsList)
        {
            cromosom.Duplicate();
        }
    }

    public void SisterChromatidsFormed() 
    {
        SetChromosomesList();
        foreach (var cromosom in cromosomsList)
        {
            cromosom.SisterChromatidsFormed();
        }
    }

    public void IncorrectDistribution()
    {
        SetChromosomeState(0, isOnCenter: false, isOnTop: false, isOnRight: true);
        SetChromosomeState(1, isOnCenter: false, isOnTop: true, isOnRight: false);
        SetChromosomeState(2, isOnCenter: false, isOnTop: false, isOnRight: false);
        SetChromosomeState(3, isOnCenter: true, isOnTop: false, isOnRight: false);
        SetChromosomeState(4, isOnCenter: true, isOnTop: false, isOnRight: true);
        SetChromosomeState(5, isOnCenter: false, isOnTop: true, isOnRight: true);

        SetChromosomesAlignOn(AlignOnType.Center);
    }
    private void SetChromosomeState(int index, bool isOnCenter, bool isOnTop, bool isOnRight)
    {
        if (index < 0 || index >= cromosomsList.Count) return;

        cromosomsList[index].isOnCenter = isOnCenter;
        cromosomsList[index].isOnTop = isOnTop;
        cromosomsList[index].isOnRight = isOnRight;
    }

    private void SetPlaceValueAndScale(int i, ChromosomLogic chromosomLogic, int randomNumber)
    {
        GameObject chromosom = chromosomLogic.gameObject;

        chromosom.transform.localScale += GetScaleOffset(i);

        // Determine the position of the chromosome
        chromosomLogic.isOnTop = IsOnTop(i);
        chromosomLogic.isOnCenter = IsOnCenter(i);
        chromosomLogic.isOnRight = GetIsOnRight(i, randomNumber);
    }

    /// <summary>
    /// Returns the scale offset based on the chromosome position.
    /// </summary>
    private Vector3 GetScaleOffset(int index)
    {
        float scaleOffset = (index < 2) ? 0.6f : (index < 4) ? 0.3f : -0.1f; //- 0 old
        return new Vector3(scaleOffset, scaleOffset, 0);
    }

    private bool IsOnTop(int index) => index < 2;

    private bool IsOnCenter(int index) => index > 1 && index < 4;

    /// <summary>
    /// Determines whether the chromosome is located on the right depending on `randomNumber`.
    /// </summary>
    private bool GetIsOnRight(int index, int randomNumber)
    {
        // Correspondence table
        Dictionary<int, HashSet<int>> rightPositions = new Dictionary<int, HashSet<int>>
    {
        { 0, new HashSet<int> { 1, 3, 5 } },
        { 1, new HashSet<int> { 0, 2, 4 } },
        { 2, new HashSet<int> { 1, 2, 4 } },
        { 3, new HashSet<int> { 0, 3, 4 } },
        { 4, new HashSet<int> { 1, 2, 5 } },
        { 5, new HashSet<int> { 1, 2, 4 } },
        { 6, new HashSet<int> { 0, 3, 5 } }
    };

        if (rightPositions.TryGetValue(randomNumber, out var validIndices))
        {
            return validIndices.Contains(index);
        }

        Debug.LogError($"Unexpected randomNumber value: {randomNumber}");
        return false;
    }

    public void CreateChromosoms()
    {
        int randomNumber = Random.Range(0, 7);
        for (int i = 0; i < 6; i++)
        {
            GameObject prefab = GetChromosomePrefab(i);

            if (prefab == null || chromosomPositions[i] == null)
            {
                Debug.LogError($"Error: No prefab or position found for chromosome {i}");
                continue; // Skip the rest of the code and move on to the next iteration
            }
            GameObject chromosom = Instantiate(prefab, chromosomPositions[i].transform.position, Quaternion.identity, chromosomTransform.transform);
            ChromosomLogic chromosomLogic = chromosom.AddComponent<ChromosomLogic>();
            chromosomLogic.ChangeColorOfChildren(colorsList[i]);
            chromosomLogic.objectToDuplicate = prefab;

            SetPlaceValueAndScale(i, chromosomLogic, randomNumber);
        }
    }
    private GameObject GetChromosomePrefab(int index)
    {
        if (index < 2) return chromosomeLongTopPrefab;    // 0, 1 → Upper long chromosome
        if (index >= 2 && index < 4) return chromosomeLongBottomPrefab; // 2, 3 → Lower long chromosome
        return chromosomPrefab;  // 4, 5 → Normal chromosome
    }

    private IEnumerator CellDivision( bool setCellBelonging = true)
    {
        GameObject rightCell = Instantiate(cellToDuplicate, transform.position, Quaternion.identity);
        the_rightCell = rightCell.GetComponent<CellLogic>();
        the_rightCell.isMainCell = false;
        rightCell.name = "Right Cell";

        GameObject leftCell = gameObject;
        the_leftCell = GetComponent<CellLogic>();
        leftCell.name = "Left Cell";

        Vector3 startScaleLeft = leftCell.transform.localScale;
        Vector3 startScaleRight = leftCell.transform.localScale;

        if (setCellBelonging)
        {
            SetCellBelonging();

            Vector3 targetScaleLeft = new Vector3(startScaleLeft.x / 2, startScaleLeft.y, startScaleLeft.z);
            Vector3 targetScaleRight = new Vector3(startScaleRight.x / 2, startScaleRight.y, startScaleRight.z);

            CycleLogic.Instance.UpdateCycleNumber(Comunication.Instance.divisionNumber == 1 ? 3 : 6);
            yield return StartCoroutine(MoveAndResize(leftCell, startScaleLeft, targetScaleLeft, rightCell, startScaleRight, targetScaleRight));

            yield return StartCoroutine(MoveAndResize(leftCell, targetScaleLeft, startScaleLeft, rightCell, targetScaleRight, startScaleRight));

            if (Comunication.Instance.divisionNumber == 1)
            {
                yield return StartCoroutine(MoveFurther(leftCell, rightCell));
            }
        }
        else
        {
            CycleLogic.Instance.UpdateCycleNumber(Comunication.Instance.divisionNumber == 1 ? 3 : 6);
            yield return StartCoroutine(MoveFurther(leftCell, rightCell));
        }
        // add the created cell to the list for the next division frome rules
        Comunication.Instance.cellLogicList.Add(the_rightCell);
    }


    public void SetRandomPosition()
    {
        alineType = AlignOnType.Random;
        StartCoroutine(AlineOn());
    }

    //cell movment
    public IEnumerator MoveAndResize(
        GameObject leftCell, Vector3 targetScaleLeft, Vector3 startScaleLeft,
        GameObject rightCell, Vector3 targetScaleRight, Vector3 startScaleRight)
    {
        float moveDistance = 5;
        Vector3 startPosLeft = leftCell.transform.position;
        Vector3 startPosRight = rightCell.transform.position;

        Vector3 targetPosLeft = startPosLeft + new Vector3(-moveDistance / 2, 0, 0);
        Vector3 targetPosRight = startPosRight + new Vector3(moveDistance / 2, 0, 0);
        float duration = 2f; // Animation time
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration); // Smooth transition

            leftCell.transform.position = Vector3.Lerp(startPosLeft, targetPosLeft, t);
            rightCell.transform.position = Vector3.Lerp(startPosRight, targetPosRight, t);

            leftCell.transform.localScale = Vector3.Lerp(targetScaleLeft, startScaleLeft, t);
            rightCell.transform.localScale = Vector3.Lerp(targetScaleRight, startScaleRight, t);

            yield return null;
        }
        // Set final values ​​to avoid rounding errors
        leftCell.transform.position = targetPosLeft;
        rightCell.transform.position = targetPosRight;
    }

    // Additional cell movement if divisionNumber == 1
    private IEnumerator MoveFurther(GameObject leftCell, GameObject rightCell)
    {
        float duration = 2f;
        float time = 0;

        Vector3 startPosLeft = leftCell.transform.position;
        Vector3 startPosRight = rightCell.transform.position;

        // Final positions (further expansion of cells)
        Vector3 targetPosLeft = startPosLeft + new Vector3(-5, 0, 0);
        Vector3 targetPosRight = startPosRight + new Vector3(5, 0, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            leftCell.transform.position = Vector3.Lerp(startPosLeft, targetPosLeft, t);
            rightCell.transform.position = Vector3.Lerp(startPosRight, targetPosRight, t);

            yield return null;
        }
        leftCell.transform.position = targetPosLeft;
        rightCell.transform.position = targetPosRight;
        if (isMainCell && Comunication.Instance.divisionNumber == 1)
        {
            Comunication.Instance.OnFirstDivisionDone();
        }
        else { Debug.Log("Second Division Done"); }
    }



    /// <summary>
    /// separation of chromosomes into cells
    /// depending on AlignOnType we know which cell the chromosome belongs to
    /// when a cell divides, the chromosomes move with the cell
    ///  "c" is the function parameter, each element of the list.
    /// </summary>
    private void SetCellBelonging()
    {
        List<ChromosomLogic> leftList = the_leftCell.cromosomsList;
        List<ChromosomLogic> rightList = the_rightCell.cromosomsList;
        switch (alineType)
        {
            case AlignOnType.Center:
            case AlignOnType.Poles:
                RemoveChromosomes(leftList, c => c.isOnRight);  //  delete everything on the right from the leftList
                RemoveChromosomes(rightList, c => !c.isOnRight); //  delete everything on the left from the rightList
                break;

            case AlignOnType.ToTheRightOfCenter:
            case AlignOnType.TheRightPole:
                RemoveChromosomes(leftList, _ => true); //  delete everything on leftList
                break;

            case AlignOnType.ToTheLeftOfCenter:
            case AlignOnType.TheLeftPole:
                RemoveChromosomes(rightList, _ => true); //  delete everything on rightList
                break;
        }
    }

    /// <summary>
    /// Removes chromosomes from the list based on the `predicate` condition.
    ///  For example - System.Func<ChromosomLogic, bool>  -  "c => c.isOnRight"
    /// </summary>
    private void RemoveChromosomes(List<ChromosomLogic> list, System.Func<ChromosomLogic, bool> predicate) 
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (predicate(list[i]))
            {
                Destroy(list[i].gameObject);
                list.RemoveAt(i);
            }
        }
    }

    /// separation of sisters chromosomes after the first Division
    public void ChromosomeSeparation()
    {
        for (int i = cromosomsList.Count - 1; i >= 0; i--)
        {
            if (cromosomsList[i].sisterChromitids)
            {
                ChromosomLogic chromosome = cromosomsList[i].sisterChromitids.AddComponent<ChromosomLogic>();
                cromosomsList[i].isOnRight = false;
                chromosome.isOnRight = true;
                chromosome.isOnCenter = cromosomsList[i].isOnCenter;
                chromosome.isOnTop = cromosomsList[i].isOnTop;

                // Detach the object from its current parent
                cromosomsList[i].sisterChromitids.transform.SetParent(null);

                // Attach to new object
                cromosomsList[i].sisterChromitids.transform.SetParent(chromosomTransform.transform, true);
                cromosomsList.Add(chromosome);
            }
        }
    }
}
