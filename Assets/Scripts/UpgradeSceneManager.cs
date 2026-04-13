using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeSceneManager : MonoBehaviour
{
    [SerializeField] CharacterUpgradeData foxUpgradeData;
    [SerializeField] CharacterUpgradeData lizardUpgradeData;
    private CharacterUpgradeData currentUpgradeData;
    [SerializeField] Button foxButton;
    [SerializeField] Button lizardButton;

    [SerializeField] private GameObject botonAtaque;
    [SerializeField] private GameObject botonRango;
    [SerializeField] private GameObject botonVelocidad;
    [SerializeField] private GameObject botonIrParaAtras;
    [SerializeField] private GameObject botonZorro;
    [SerializeField] private GameObject botonLagarto;

    [SerializeField] private TextMeshProUGUI bigText;
    [SerializeField] private Sprite foxOn;
    [SerializeField] private Sprite foxOff;
    [SerializeField] private Sprite lizardOn;
    [SerializeField] private Sprite lizardOff;

    private void Start()
    {
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        bigText.text = GameDataManager.Instance.money + " €";
        bigText.color = Color.yellow;
    }

    public void UpgradeAttack()
    {
        if (currentUpgradeData == null || currentUpgradeData.damageStage >= 3) return;
        if (GameDataManager.Instance.money < 100 * currentUpgradeData.damageStage + 100)
        {
            Debug.Log("Tu ere pobre tu no tiene aifon " + GameDataManager.Instance.money + " " + (100 * currentUpgradeData.damageStage + 100));
            return;
        }
        else
        {
            GameDataManager.Instance.money -= 100 * currentUpgradeData.damageStage + 100;
            UpdateMoneyText();
            currentUpgradeData.damageStage = Mathf.Min(currentUpgradeData.damageStage + 1, 3);
            Debug.Log("Upgrade Attack: " + currentUpgradeData.damageStage);
        }
    }
    public void UpgradeRange()
    {
        if (currentUpgradeData == null || currentUpgradeData.rangeStage >= 3) return;
        if (GameDataManager.Instance.money < 100 * currentUpgradeData.rangeStage + 100)
        {
            Debug.Log("Tu ere pobre tu no tiene aifon " + GameDataManager.Instance.money + " " + (100 * currentUpgradeData.rangeStage + 100));
            return;
        }
        else
        {
            GameDataManager.Instance.money -= 100 * currentUpgradeData.rangeStage + 100;
            UpdateMoneyText();
            currentUpgradeData.rangeStage = Mathf.Min(currentUpgradeData.rangeStage + 1, 3);
            Debug.Log("Upgrade Range: " + currentUpgradeData.rangeStage);
        }
    }

    public void UpgradeSpeed()
    {
        if (currentUpgradeData == null || currentUpgradeData.speedStage >= 3) return;
        if (GameDataManager.Instance.money < 100 * currentUpgradeData.speedStage + 100)
        {
            Debug.Log("Tu ere pobre tu no tiene aifon " + GameDataManager.Instance.money + " " + (100 * currentUpgradeData.speedStage + 100));
            return;
        }
        else
        {
            GameDataManager.Instance.money -= 100 * currentUpgradeData.speedStage + 100;
            UpdateMoneyText();
            currentUpgradeData.speedStage = Mathf.Min(currentUpgradeData.speedStage + 1, 3);
            Debug.Log("Upgrade Speed: " + currentUpgradeData.speedStage + 100);
        }
    }

    public void GoBack()
    {
        GameSceneManager.Instance.LoadScene("TitleScreen", SceneTransition.FadeBlack, false);
    }

    public void PickFox()
    {
        currentUpgradeData = foxUpgradeData;
        botonZorro.GetComponent<Image>().sprite = foxOn;
        botonLagarto.GetComponent<Image>().sprite = lizardOff;
    }

    public void PickLizard()
    {
        currentUpgradeData = lizardUpgradeData;
        botonZorro.GetComponent<Image>().sprite = foxOff;
        botonLagarto.GetComponent<Image>().sprite = lizardOn;
    }
}
