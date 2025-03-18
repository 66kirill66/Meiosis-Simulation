using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using ArabicSupport;

public class LangSupport : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void SendLangCode();

    public static string langCode;
    private static bool currentIsRTL;
    private static string currentStartText;
    public TextMeshProUGUI startText;
    public static LangSupport Instance { get; private set; }

    public class LangData
    {
        public bool isRTL;
        public string start;

    }
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
        if (!Application.isEditor)
        {
            SetAllOptions();
        }
    }
    public void SetLang()
    {
        if (!Application.isEditor)
        {
            SendLangCode();
        }
    }

    public void SetChanges(string json) // from web
    {
        LangData dataLang = JsonUtility.FromJson<LangData>(json);
        currentIsRTL = dataLang.isRTL;
        currentStartText = dataLang.start;
        SetAllOptions();
    }

    public void SetAllOptions()
    {
        SetOptions(startText, currentIsRTL, currentStartText);
    }

    public void SetOptions(TextMeshProUGUI objText, bool isRTL, string text, bool changeTextSide = false)
    {
        TextAlignmentOptions option;
        if (changeTextSide == false)
        {

            option = TextAlignmentOptions.Center;
        }
        else
        {
            option = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;

        }
        LanguageAssignment(objText, text, isRTL, option);
    }


    private void LanguageAssignment(TextMeshProUGUI objectText, string currentText, bool isRTL, TextAlignmentOptions side)
    {
        if (objectText != null)
        {
            objectText.enableWordWrapping = false;
            objectText.alignment = side;

            if (langCode != "ar")
            {
                objectText.text = currentText;
                objectText.isRightToLeftText = isRTL;
            }
            else
            {
                objectText.text = ArabicFixer.Fix(currentText, false, false);
                objectText.isRightToLeftText = false;
            }
        }
    }
}
