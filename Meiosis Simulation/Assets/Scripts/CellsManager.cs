using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CellsManager : MonoBehaviour
{
    public List<DuplicateAnimation> cellsList = new List<DuplicateAnimation>();
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject cutWound;
    public int createCount;
    int count = 0;

    private void Start()
    {
        StartCoroutine(StartDuplicate(createCount));
    }

    List<DuplicateAnimation> GetRandomSelection(List<DuplicateAnimation> source, int count)
    {
        if (source.Count == 0)
        {
            Debug.LogError("Пустой список объектов!");
            return new List<DuplicateAnimation>();
        }

        List<DuplicateAnimation> copy = new List<DuplicateAnimation>(source);
        int n = copy.Count;

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, n);
            (copy[i], copy[randomIndex]) = (copy[randomIndex], copy[i]); // Обмен местами
        }

        return copy.GetRange(0, count); // Берём нужное количество элементов
    }

    IEnumerator StartDuplicate(int duplicateCounts)
    {
        while(count < duplicateCounts)
        {
            if (cellsList.Count < 18)
            {
                Debug.LogWarning("В списке меньше 30 элементов! Выбраны все доступные.");
            }

            List<DuplicateAnimation> selectedObjects = GetRandomSelection(cellsList, Mathf.Min(18, cellsList.Count));
            Debug.Log("Выбрано " + selectedObjects.Count + " объектов.");

            foreach (var cell in selectedObjects)
            {
                cell.Duplicate(cellPrefab);
            }
            selectedObjects.Clear(); // new
            cellsList.Clear(); // new
            count++;
            if (count > 3)
            {
                SpriteRenderer cutWoundSprite = cutWound.GetComponent<SpriteRenderer>();
                // Определяем новое значение альфы
                float newAlpha = Mathf.Max(cutWoundSprite.color.a - 0.2f, 0f); // Уменьшаем на 0.2, но не ниже 0
                                                                               // Анимируем уменьшение альфы
                cutWoundSprite.DOFade(newAlpha, 1).OnComplete(() =>
                {
                    if (newAlpha <= 0f)
                    {
                        cutWound.gameObject.SetActive(false);
                    }
                });
            }
            yield return new WaitForSeconds(3);
        }
    }

}
