using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private CharacterUpgradeData upgradeData;
    [SerializeField] private GameObject botonJugar;
    [SerializeField] private GameObject botonMejorar;
    [SerializeField] private GameObject botonAtaque;
    [SerializeField] private GameObject botonRango;
    [SerializeField] private GameObject botonVelocidad;

    public void enterUpgradeMenu()
    {
        botonJugar.gameObject.SetActive(false);
        botonMejorar.gameObject.SetActive(false);
        botonAtaque.gameObject.SetActive(true);
        botonRango.gameObject.SetActive(true);
        botonVelocidad.gameObject.SetActive(true);
    }
    public void UpgradeAttack()
    {
        upgradeData.damageStage = Mathf.Min(upgradeData.damageStage + 1, 3);
        Debug.Log("Upgrade Attack: " + upgradeData.damageStage);
    }
    public void UpgradeRange()
    {
        upgradeData.rangeStage = Mathf.Min(upgradeData.rangeStage + 1, 3);
        Debug.Log("Upgrade Range: " + upgradeData.rangeStage);
    }

    public void UpgradeSpeed()
    {
        upgradeData.speedStage = Mathf.Min(upgradeData.speedStage + 1, 3);
        Debug.Log("Upgrade Speed: " + upgradeData.speedStage);
    }
}
