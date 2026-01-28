using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu( fileName = "NewDialogue", menuName = "Game/Dialogue" )]
public class DialogueData_SO : ScriptableObject
{
    public string speakerName;
    public float typeSpeed = 0.04f;

    public List<DialogueLanguageBlock> localizedDialogues;

    public List<string> GetLinesForCurrentLanguage()
    {
        Language currentLanguage = LocalizationManager.Instance.GetCurrentLanguage();

        foreach (var block in localizedDialogues)
        {
            if (block.language == currentLanguage)
            { return block.lines; }
        }

        Debug.LogWarning($"Dialogue missing language {currentLanguage}, using fallback");
        return localizedDialogues.Count > 0
            ? localizedDialogues[0].lines
            : new List<string>();
    }
}

[System.Serializable]
public class DialogueLanguageBlock
{
    public Language language;

    [TextArea(3, 5)]
    public List<string> lines;
}
