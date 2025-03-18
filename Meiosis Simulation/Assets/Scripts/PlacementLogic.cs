using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementLogic : MonoBehaviour
{
    public int chromosomeNumber;
    public Vector3 tablePlace;
    private ChromosomesViewLogic chromosomesViewLogic;
    private void Awake()
    {
        chromosomesViewLogic = FindObjectOfType<ChromosomesViewLogic>();
    }
    void Start()
    {
        
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Click();
        }
    }

    public void Click()
    {
        chromosomesViewLogic.CheckClickOnChromosome(this, chromosomeNumber);
    }

    public void SetPlace()
    {
        StartCoroutine(MoveToTarget(tablePlace, 1f)); // 1 seconds to move
    }

    private IEnumerator MoveToTarget(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(targetPosition, Quaternion.identity);
        GetComponent<Collider>().enabled = false;
    }
}
