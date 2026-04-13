using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.InputSystem;

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

        // Desactivar bloqueo del cursor de la escena anterior
        // De lo contrario, los botones dejan de ir
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PickCharacter()
    {
        titleCanvas.SetActive(false);
        pickCharacterCanvas.SetActive(true);
    }

    public void SelectFox()
    {
        GameDataManager.Instance.selectedCharacter = 0;
        print("Cargando escena...");
        GameSceneManager.Instance.LoadScene("MAP_Level", SceneTransition.FadeBlack, false);
    }

    public void SelectLizard()
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
        moneyText.text = GameDataManager.Instance.money.ToString() + " €";
    }
}
