using System.Collections.Generic;
using UnityEngine;

public class PlayerModeManager : MonoBehaviour, ISaveable
{
    [Header("Available Modes")]
    public List<PlayerModeData> availableModes;
    private List<PlayerModeRuntime> runtimeModes = new();
    private PlayerModeRuntime currentRuntimeMode;
    private int selectedIndex = 0;

    public bool[] unlockedBenditions;

    [SerializeField] private PlayerModeData defaultMode;
    private PlayerModeRuntime defaultRuntimeMode;
    public int GetSelectedIndex() => selectedIndex;


    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenPerSecond = 20f;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    
    private PlayerController controller;

    //eventos
    public System.Action<int> OnModeSelectionChanged;
    public System.Action<PlayerModeRuntime> OnModeActivated;
    public System.Action OnModeDeactivated;
    public System.Action<float, float> OnStaminaChanged;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<PlayerController>();

        defaultRuntimeMode = new PlayerModeRuntime(defaultMode, controller);
        currentRuntimeMode = defaultRuntimeMode;

        foreach (var data in availableModes)
        {
            runtimeModes.Add(new PlayerModeRuntime(data, controller));
        }
    }

    private void Update()
    {
        HandleStamina();
    }

    // -- Stamina --

    private void HandleStamina()
    {
        if (currentRuntimeMode != null)
        {
            if (currentRuntimeMode.IsActive)
            {
                currentStamina -= currentRuntimeMode.data.staminaDrainPerSecond * Time.deltaTime;
                OnStaminaChanged?.Invoke(currentStamina, maxStamina);

                if (currentStamina <= 0f)
                {
                    currentStamina = 0f;
                    DeactivateActiveMode();
                }
            }
            else
            {
                if (currentStamina < maxStamina)
                {
                    currentStamina += staminaRegenPerSecond * Time.deltaTime;
                    OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                    currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
                }
            }
        }
    }

    // Si las bendiciones se desbloquean de forma lineal
    public void SelectNextMode()
    {
        if (runtimeModes.Count == 0) return;

        do
        {
            selectedIndex++;
            if (selectedIndex >= unlockedBenditions.Length) selectedIndex = 0;
        } while (!unlockedBenditions[selectedIndex]);

        OnModeSelectionChanged?.Invoke(selectedIndex);
        DeactivateActiveMode();
    }

    public void SelectPreviousMode()
    {
        if (runtimeModes.Count == 0) return;

        do
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = unlockedBenditions.Length - 1;
        } while (!unlockedBenditions[selectedIndex]);

        OnModeSelectionChanged?.Invoke(selectedIndex);
        DeactivateActiveMode();
    }

    // -- Activar modos --

    public void ToggleSelectedMode()
    {
        if (runtimeModes.Count == 0) return;

        var selectedMode = runtimeModes[selectedIndex];

        if (currentRuntimeMode == selectedMode)
        {
            DeactivateActiveMode();
            return;
        }

        ActivateMode(selectedMode);
    }

    private void ActivateMode(PlayerModeRuntime mode)
    {
        if (currentStamina <= 0f)
            return;

        DeactivateActiveMode();

        OnModeActivated?.Invoke(mode);
        currentRuntimeMode = mode;
        currentRuntimeMode.Activate();
    }

    private void DeactivateActiveMode()
    {
        if (currentRuntimeMode == null) return;

        OnModeDeactivated.Invoke();
        currentRuntimeMode.Deactivate();
        currentRuntimeMode = defaultRuntimeMode;
    }
    public List<PlayerModeData> GetAvailableModes()
    {
        return availableModes;
    }

    // -- Funciones públicas --

    public float ModifyMoveSpeed(float baseValue)
        => currentRuntimeMode.IsActive == true ? currentRuntimeMode.ModifyMoveSpeed(baseValue) : baseValue;

    public float ModifyJumpForce(float baseValue)
        => currentRuntimeMode.IsActive == true ? currentRuntimeMode.ModifyJumpForce(baseValue) : baseValue;

    public bool CanWallJump()
        => currentRuntimeMode.IsActive == true && currentRuntimeMode.CanWallJump();
    public bool CanWallClimb()
        => currentRuntimeMode.IsActive == true && currentRuntimeMode.CanWallClimb();
    public bool CanDoubleJump()
        => currentRuntimeMode.IsActive == true && currentRuntimeMode.CanDoubleJump();

    public void SaveData(SaveData data)
    {
        data.unlockedBenditions = unlockedBenditions;
    }

    public void LoadData(SaveData data)
    {
        print(data.unlockedBenditions);
        unlockedBenditions = data.unlockedBenditions;
    }
}