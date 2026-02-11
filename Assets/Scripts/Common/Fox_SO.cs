using UnityEngine;

public class Fox_SO : Player_SO
{
    public Fox_SO()
    {
        speed = 100;
        hp = 1;
        jumpHeight = 100;
        attackDamage = 1;
        attackCooldown = 1;

        hasASecondaryAttack = false;
        canClimbWalls = false;
    }
}
