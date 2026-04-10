using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameDataManager : MonoBehaviour
{
    // Esto es un contenedor de datos persistentes
    // Por ejemplo las cosas que se guardarán en el archivo de guardado

    public static GameDataManager Instance;

    [SerializeField] public float money = 0;
    [SerializeField] public CharacterUpgradeData foxUpgrades;
    [SerializeField] public CharacterUpgradeData lizardUpgrades;
    [SerializeField] public int selectedCharacter = 0;      // El ID de la transformación que escogimos

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        print("Escena " + SceneManager.GetActiveScene().name + ": Tienes " + money + " de dinero");
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Click detectado por DebugInput");
        }
    }
}