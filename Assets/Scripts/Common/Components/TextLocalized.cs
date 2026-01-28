using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TextLocalized : MonoBehaviour
{
    [SerializeField] private string key;

    [SerializeField] private TMP_Text text;

    private void Start()
    {
        if (text == null)
        { text = GetComponent<TMP_Text>(); }
        
        UpdateText();
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        text.text = LocalizationManager.Get(key);
    }
}
