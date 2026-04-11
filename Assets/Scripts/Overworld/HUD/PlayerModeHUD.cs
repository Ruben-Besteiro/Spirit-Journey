using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerModeHUD : MonoBehaviour
{
    [Header("References (Mode)")]
    [SerializeField] private PlayerModeManager manager;
    [SerializeField] private Transform modesContainer;
    [SerializeField] private Image modeIconPrefab;
    [SerializeField] private Image staminaFillImage;

    [Header("References (HP)")]
    [SerializeField] private PlayerController controller;
    [SerializeField] private Image hpFillImage;

    private List<Image> modeIcons = new();

    private void Start()
    {
        print("Character: " + GameDataManager.Instance.selectedCharacter);

        if (manager == null || controller == null)
        {
            Debug.LogWarning("HUD has no PlayerModeManager or PlayerController assigned.");
            return;
        }

        CreateModeIcons();
        SubscribeToEvents();
        InitializeHUD();
    }

    private void SubscribeToEvents()
    {
        manager.OnModeSelectionChanged += UpdateSelection;
        manager.OnModeActivated += UpdateActivation;
        manager.OnModeDeactivated += ClearActivation;
        manager.OnStaminaChanged += UpdateStamina;

        controller.OnDamaged += UpdateHP;
    }

    private void OnDestroy()
    {
        if (manager == null) return;

        manager.OnModeSelectionChanged -= UpdateSelection;
        manager.OnModeActivated -= UpdateActivation;
        manager.OnModeDeactivated -= ClearActivation;
        manager.OnStaminaChanged -= UpdateStamina;

        controller.OnDamaged -= UpdateHP;
    }

    private void CreateModeIcons()
    {
        /*var modes = manager.GetAvailableModes();

        foreach (var mode in modes)
        {
            Image icon = Instantiate(modeIconPrefab, modesContainer);
            icon.sprite = mode.inactiveSprite;
            modeIcons.Add(icon);
        }*/
    }

    private void UpdateHP(DamageInfo info)
    {
        // Esto se ejecuta ANTES de que el jugador reciba el da�o
        hpFillImage.fillAmount = (controller.currentHP - info.amount) / controller.maxHP;
    }

    private void InitializeHUD()
    {
        //UpdateSelection(0);
        UpdateStamina(manager.CurrentStamina, manager.MaxStamina);
    }

    private void UpdateSelection(int selectedIndex)
    {
        for (int i = 0; i < modeIcons.Count; i++)
        {
            if (i == selectedIndex)
                modeIcons[i].color = Color.white;
            else
                modeIcons[i].color = Color.gray;
        }
    }

    private void UpdateActivation(PlayerModeRuntime activeMode)
    {
        /*int index = GameDataManager.Instance.selectedCharacter;
        modeIcons[index].sprite = activeMode.data.activeSprite;*/
        if (GameDataManager.Instance.selectedCharacter == 0) GameObject.Find("Player Capsule").GetComponent<Renderer>().material.color = new Color(255, 1, 0);
        else if (GameDataManager.Instance.selectedCharacter == 1) GameObject.Find("Player Capsule").GetComponent<Renderer>().material.color = Color.green;
    }

    private void ClearActivation()
    {
        /*List<PlayerModeData> modes = manager.GetAvailableModes();
        for (int i = 0; i < modes.Count || i < modeIcons.Count; i++)
        { modeIcons[i].sprite = modes[i].inactiveSprite; }*/
        GameObject.Find("Player Capsule").GetComponent<Renderer>().material.color = Color.gray;
    }

    private void UpdateStamina(float current, float max)
    {
        staminaFillImage.fillAmount = current / max;
    }
}