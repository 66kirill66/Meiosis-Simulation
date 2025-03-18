using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateAnimation : MonoBehaviour
{
    private CellsManager cellsManager;
    public bool isLiftSide;
    public bool isMoveToTop;



    private void Awake()
    {
        cellsManager = FindObjectOfType<CellsManager>();
       
    }
    private void Start()
    {
       
    }
    public void Duplicate(GameObject cellPrefab)
    {
        GameObject newCell = Instantiate(cellPrefab, transform.position, transform.rotation, cellsManager.transform);
        DuplicateAnimation duplicate = newCell.GetComponent<DuplicateAnimation>();
        duplicate.isLiftSide = isLiftSide;
        duplicate.isMoveToTop = !isMoveToTop;
        //int randomSide = Random.Range(0, 2);
        //Debug.Log(randomSide);

        Vector3 side = isLiftSide ? Vector3.right : Vector3.left;
       // Vector3 sideV = randomSide == 0 ? Vector3.up : Vector3.down;
        Vector3 sideV = isMoveToTop? Vector3.up : Vector3.down;
      
        Vector3 endSide = side + sideV;
        StartCoroutine(Move(newCell, endSide));
    }

    IEnumerator Move(GameObject cell, Vector3 side)
    {
       
        float duration = 2f;
        float time = 0;

        Vector3 startPosLeft = cell.transform.position;
        Vector3 targetPosLeft = startPosLeft + side * 1.3f;// / 1.5f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            cell.transform.position = Vector3.Lerp(startPosLeft, targetPosLeft, t);

            yield return null;
        }
       
        cellsManager.cellsList.Add(cell.GetComponent<DuplicateAnimation>());
        cell.GetComponentInChildren<Collider2D>().enabled = true;
       
    }
}
