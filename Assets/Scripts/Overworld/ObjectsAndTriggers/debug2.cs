using UnityEngine;

public class debug2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerModeManager>().unlockedBenditions[1] = true;
            GameManager.Instance.TrySave();
            print("Lagarto desbloqueado");
        }
    }
}
