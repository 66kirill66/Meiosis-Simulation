using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentrioleLogic : MonoBehaviour
{
    //public Transform endPos;
    private SpriteRenderer spriteRenderer;

    public Transform startLinePoint; // The point where the line comes from (eg centrosome)

    private Transform chromosome1; 
    private Transform chromosome2;
    private Transform chromosome3;   

    public Transform endPoint1;
    public Transform endPoint2;   
    public Transform endPoint3;  
    private LineRenderer lineRenderer;
    public bool isLeft;

    public Transform topPoint;
    public Transform bottomPoint;
    public Transform centerPoint;
    void Start()
    {
        if (!lineRenderer)
        {
            if(TryGetComponent(out LineRenderer lr))
            {
                lineRenderer = lr;
            }
            else
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
        }
       
       
        if (lineRenderer)
        {           
            lineRenderer.sortingOrder = 3;
            lineRenderer.positionCount = 6;

            // Setting up material and width
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;

            SetSpriteRenderer(false);
            spriteRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (lineRenderer)
        {
            startLinePoint = transform;

            // Update line positions
            lineRenderer.SetPosition(0, startLinePoint.position);
            lineRenderer.SetPosition(1, endPoint1.position);

            lineRenderer.SetPosition(2, startLinePoint.position);
            lineRenderer.SetPosition(3, endPoint2.position);

            lineRenderer.SetPosition(4, startLinePoint.position);
            lineRenderer.SetPosition(5, endPoint3.position);

            if (chromosome1 && chromosome2 && chromosome3)
            {
                endPoint1.position = chromosome1.transform.position;
                endPoint2.position = chromosome2.transform.position;
                endPoint3.position = chromosome3.transform.position;
            }
        }
    }

    public void StartConectToChromosomes()
    {
        StartCoroutine(MoveToPosition());
    }
    private IEnumerator MoveToPosition()
    {
        // Reset positions
        endPoint1.position = transform.position;
        endPoint2.position = transform.position;
        endPoint3.position = transform.position;
        SetSpriteRenderer(true);

        StartCoroutine(MoveToTarget(endPoint1, topPoint.position, 1.5f));
        StartCoroutine(MoveToTarget(endPoint2, bottomPoint.position, 1.5f));
        StartCoroutine(MoveToTarget(endPoint3, centerPoint.position, 1.5f));

        yield return new WaitForSeconds(3);
        chromosome1 = topPoint;
        chromosome2 = centerPoint;
        chromosome3 = bottomPoint;
    }



    // UNIVERSAL FUNCTION OF MOVEMENT TO A POINT
    private IEnumerator MoveToTarget(Transform endPoint, Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(endPoint.position, targetPosition) > 0.01f) 
        {
            endPoint.position = Vector3.MoveTowards(endPoint.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        endPoint.position = targetPosition; // Guarantee that the object is in place
    }

    public void SetSpriteRenderer(bool value)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        spriteRenderer.enabled = value;
        lineRenderer.enabled = value;
    }

    public void ResetState()
    {
        // Turn off visual elements
        SetSpriteRenderer(false);
        // Reset chromosomes
        chromosome1 = null;
        chromosome2 = null;
        chromosome3 = null;
    }
}
