using UnityEngine;

public class Lizard_SO : Player_SO
{
    public Lizard_SO()
    {
        speed = 6;
        hp = 1;
        jumpHeight = 1.8f;
        attackDamage = 1;
        attackCooldown = 1;

        hasASecondaryAttack = true;
        canClimbWalls = true;
    }
}