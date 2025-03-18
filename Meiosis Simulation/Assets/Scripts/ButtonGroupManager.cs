using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroupManager : MonoBehaviour
{
    public List<Button> leftButtons;  // Список левых кнопок
    public List<Button> rightButtons; // Список правых кнопок

    private Button activeLeftButton = null;
    private Button activeRightButton = null;

    private void Start()
    {
        // Добавляем обработчики кликов ко всем кнопкам
        foreach (Button btn in leftButtons)
        {
            btn.onClick.AddListener(() => OnLeftButtonClicked(btn));
        }

        foreach (Button btn in rightButtons)
        {
            btn.onClick.AddListener(() => OnRightButtonClicked(btn));
        }
    }

    private void OnLeftButtonClicked(Button clickedButton)
    {
        if (activeLeftButton == clickedButton) return; // Если уже активна, ничего не делаем

        // Сбрасываем все кнопки в левой группе
        ResetButtons(leftButtons);

        // Делаем нажатую кнопку активной
        activeLeftButton = clickedButton;
        SetButtonActive(clickedButton);
    }

    private void OnRightButtonClicked(Button clickedButton)
    {
        if (activeRightButton == clickedButton) return;

        ResetButtons(rightButtons);
        activeRightButton = clickedButton;
        SetButtonActive(clickedButton);
    }

    private void ResetButtons(List<Button> buttonGroup)
    {
        foreach (Button btn in buttonGroup)
        {
            SetButtonInactive(btn);
        }
    }

    private void SetButtonActive(Button btn)
    {
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white; // Цвет активной кнопки
        btn.colors = colors;
    }

    private void SetButtonInactive(Button btn)
    {
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.gray; // Цвет неактивной кнопки
        btn.colors = colors;
    }
}
