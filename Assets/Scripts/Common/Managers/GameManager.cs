using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    private string rutaCompleta;
    private SaveData data;
    private List<ISaveable> saveableObjects;

    [SerializeField] private float maxTime;
    public float remainingTime;
    public bool timerEnabled = true;
    public int killsToWin = 5;
    public int kills = 0;

    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public TextMeshProUGUI killsRemainingText;

    [SerializeField] private Canvas resultsCanvas;
    [SerializeField] private TextMeshProUGUI wonOrLostText;
    [SerializeField] private TextMeshProUGUI plusMoneyText;

    private bool isPaused = false;
    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        // Configurar la ruta
        rutaCompleta = Path.Combine(Application.persistentDataPath, "SaveData.sav");

        // Por si no existe
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(rutaCompleta));
        }
        catch (Exception)
        {
            // Si no se tiene permisos para crear la carpeta entonces se juega sin guardar y ya est
        }

        remainingTime = maxTime;
    }

    private void Start()
    {
        data = TryLoad();
        saveableObjects = FindAllSaveableObjects();
        foreach (ISaveable obj in saveableObjects)
        {
            obj.LoadData(data);
        }

        timerEnabled = true;        // Esto es obligatorio ponerlo aqu y no arriba porque si no no va
 
        try
        {
            timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
            killsRemainingText = GameObject.Find("KillsRemainingText").GetComponent<TextMeshProUGUI>();
        }
        catch (Exception)
        {
            print("No hay textos en la escena" + SceneManager.GetActiveScene().name);
        }
    }

    private void Update()
    {
        if (timerEnabled)
        {
            remainingTime -= Time.deltaTime;
            timerText.text = remainingTime.ToString("F2");

            if (remainingTime <= 0)
            {
                timerEnabled = false;
                remainingTime = 0;
                timerText.text = remainingTime.ToString("F2");
                StartCoroutine(WonOrLost(false));
            }
        }
    }

    /* PAUSE SYSTEM */
    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        timerEnabled = false;

        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        timerEnabled = true;

        OnGameResumed?.Invoke();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // Todo esto ahora mismo no se utiliza
    private List<ISaveable> FindAllSaveableObjects()
    {
        var foundSaveableObjects = FindObjectsByType(
            typeof(MonoBehaviour),
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).OfType<ISaveable>();

        return new List<ISaveable>(foundSaveableObjects);
    }

    public void TrySave()
    {
        if (data == null)
        {
            data = new SaveData();
        }

        saveableObjects = FindAllSaveableObjects();

        foreach (ISaveable obj in saveableObjects)
        {
            string objectName = (obj as MonoBehaviour)?.gameObject.name ?? "Unknown";
            obj.SaveData(data);
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                using (StreamWriter write = new StreamWriter(stream))
                {
                    write.Write(json);
                }
            }
        }
        catch (Exception)
        {
            // Si por lo que sea no se puede guardar entonces no se hace nada
        }
    }

    public SaveData TryLoad()
    {
        SaveData loadedData;

        if (File.Exists(rutaCompleta))
        {
            try
            {
                string fileData;

                using (FileStream stream = new FileStream(rutaCompleta, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    fileData = reader.ReadToEnd();
                }

                loadedData = JsonUtility.FromJson<SaveData>(fileData);
            }
            catch (Exception)
            {
                loadedData = new SaveData();
            }
        }
        else
        {
            // Si no existe el archivo de guardado pues lo creamos
            loadedData = new SaveData();
        }

        return loadedData;
    }

    public IEnumerator WonOrLost(bool won)
    {
        PauseGame();
        resultsCanvas.gameObject.SetActive(true);
        wonOrLostText.text = won ? "Has ganado" : "Has perdido";
        wonOrLostText.color = won ? Color.green : Color.red;
        plusMoneyText.text = won ? "+100" : "+0";
        plusMoneyText.color = won ? Color.white : Color.yellow;
        yield return new WaitForSeconds(1.5f);
        GameSceneManager.Instance.LoadScene("TitleScreen", SceneTransition.FadeBlack, false);
    }
}