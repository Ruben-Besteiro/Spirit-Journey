using UnityEngine;

public class debug1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerModeManager>().unlockedBenditions[1] = false;
            GameManager.Instance.TrySave();
            print("Lagarto bloqueado");
        }
    }
}
