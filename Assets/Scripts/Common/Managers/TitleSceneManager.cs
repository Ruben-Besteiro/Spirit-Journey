using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class TitleSceneManager : MonoBehaviour
{
    public static TitleSceneManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI moneyText;

    [SerializeField] private GameObject titleCanvas;
    [SerializeField] private GameObject pickCharacterCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(UpdateMoneyText());
    }

    public void PickCharacter()
    {
        titleCanvas.SetActive(false);
        pickCharacterCanvas.SetActive(true);
    }

    public void LoadLevelFox()
    {
        GameDataManager.Instance.selectedCharacter = 0;
        print("Cargando escena...");
        GameSceneManager.Instance.LoadScene("MAP_Level", SceneTransition.FadeBlack, false);
    }

    public void LoadLevelLizard()
    {
        GameDataManager.Instance.selectedCharacter = 1;
        print("Cargando escena...");
        GameSceneManager.Instance.LoadScene("MAP_Level", SceneTransition.FadeBlack, false);
    }

    // Actualizamos el dinero con un delay de 1 frame
    private IEnumerator UpdateMoneyText()
    {
        yield return null;
        moneyText.color = Color.yellow;
        moneyText.text = GameDataManager.Instance.money.ToString();
    }
}
