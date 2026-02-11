using UnityEngine;

[CreateAssetMenu(menuName = "Player SO")]
public class Player_SO : ScriptableObject
{
    public string benditionName = "";

    [Header("Stats")]
    [SerializeField] public float speed;
    [SerializeField] public float hp;
    [SerializeField] public float jumpHeight;
    [SerializeField] public float attackDamage;
    [SerializeField] public float attackCooldown;

    [Header("Abilities")]
    [SerializeField] public bool hasASecondaryAttack;
    [SerializeField] public bool canClimbWalls;
}
