using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    // Esto es un contenedor de datos persistentes
    // Por ejemplo las cosas que se guardarán en el archivo de guardado

    public static GameDataManager Instance;

    [SerializeField] public int money = 0;
    [SerializeField] public CharacterUpgradeData foxUpgrades;
    [SerializeField] public CharacterUpgradeData lizardUpgrades;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
