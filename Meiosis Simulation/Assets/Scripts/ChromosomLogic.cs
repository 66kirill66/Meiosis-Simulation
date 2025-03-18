using System.Collections;
using UnityEngine;

public class ChromosomLogic : MonoBehaviour
{
    public GameObject objectToDuplicate;
    public GameObject sisterChromitids;
    public bool flipX = true;
    public float transformY;
    public float transformX;

    private Color thisColor;

    private bool isDuplicate;

    public bool isOnTop;
    public bool isOnRight;
    public bool isOnCenter;

    public bool is_In_LeftCell;

    void Start()
    {
      
    }

    public void ChangeColorOfChildren(Color color)
    {
        thisColor = color;
        Helper.ChangeColorRecursively(transform, color);
    }

    public void MoveToPosition(Transform newPosition, float moveSpeed, float Offset_X = 0, float Offset_Y = 0)
    {
        Vector3 targetPosition = CalculateTargetPosition(newPosition.position, ref Offset_X, Offset_Y);

        StartCoroutine(MovePosition(targetPosition, moveSpeed));
    }

    /// <summary>
    /// Calculates the final position of an object depending on its location (isOnTop, isOnRight, isOnCenter).
    /// </summary>
    private Vector3 CalculateTargetPosition(Vector3 basePosition, ref float Offset_X, float Offset_Y)
    {
        float Offset_XTop = Offset_X;
        float Offset_Xcenter = Offset_X;
        float Offset_XBottom = Offset_X;
        if (Comunication.Instance.divisionNumber == 0)
        {
            Offset_XTop += 0.15f;
            Offset_Xcenter += 0f;
            Offset_XBottom -= 0.1f;
        }

        // tuple for more convenient position determination
        return (isOnTop, isOnRight, isOnCenter) switch
        {
            (true, false, false) => new Vector3(basePosition.x - Offset_XTop, basePosition.y + Offset_Y, 0), // Top left
            (true, true, false) => new Vector3(basePosition.x + Offset_XTop, basePosition.y + Offset_Y, 0), // Top right
            (false, false, true) => new Vector3(basePosition.x - Offset_Xcenter, basePosition.y, 0), // Center left
            (false, true, true) => new Vector3(basePosition.x + Offset_Xcenter, basePosition.y, 0), // Center right
            (false, false, false) => new Vector3(basePosition.x - Offset_XBottom, basePosition.y - Offset_Y, 0), // Bottom left
            (false, true, false) => new Vector3(basePosition.x + Offset_XBottom, basePosition.y - Offset_Y, 0), // Bottom right
            _ => basePosition // Return the base position by default
        };
    }


    public void SetRandomPositionInCell(Transform newPosition)
    {
        //float randomOffset_X = Random.Range(-2f, 2.5f);
        //float randomOffset_Y = Random.Range(-2f, 2.5f);
        //Vector3 newPos = new Vector3(newPosition.position.x + randomOffset_X, newPosition.position.y + randomOffset_Y);
        float moveSpeed = 2;
       // StartCoroutine(MovePosition(newPos, moveSpeed));
        StartCoroutine(MovePosition(newPosition.position, moveSpeed));
    }

    public void Duplicate()
    {
        if (!isDuplicate)
        {
            isDuplicate = true;

            GameObject newObject = Instantiate(objectToDuplicate, transform.position, Quaternion.identity, this.transform);
            sisterChromitids = newObject;

            Helper.ChangeColorRecursively(sisterChromitids.transform, thisColor);
            
            Vector3 newPos = new Vector3(transform.position.x + 0.3f, transform.position.y, 0);
            float moveSpeed = 1;
            StartCoroutine(MovePosition(newPos, moveSpeed, newObject));
        }
        else { Debug.Log("this is already sisterChromatids"); }

    }
    public void SisterChromatidsFormed()
    {
        if (sisterChromitids)
        {          
             StartCoroutine(RotateObject(sisterChromitids, 1));
        }
    }
    private IEnumerator RotateObject(GameObject obj, float duration)
    {
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
        Quaternion startRotation = Quaternion.Euler(0, 0, 0);
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            obj.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.rotation = targetRotation; 
        StartCoroutine(MoveToLocalPosition(Vector3.zero, sisterChromitids.transform));
    }
    IEnumerator MoveToLocalPosition(Vector3 targetLocalPos, Transform child)
    {
        Vector3 startPos = child.localPosition; 
        float elapsedTime = 0;
        float moveDuration = 1;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            child.localPosition = Vector3.Lerp(startPos, targetLocalPos, elapsedTime / moveDuration);
            yield return null;
        }
    }

    private IEnumerator MovePosition(Vector3 targetPosition,float moveSpeed, GameObject objToMove = null)
    {
        GameObject obj = objToMove != null ? objToMove : gameObject;

        while (Vector3.Distance(obj.transform.position, targetPosition) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            yield return null;
        }

        obj.transform.position = targetPosition; 
    }
}
