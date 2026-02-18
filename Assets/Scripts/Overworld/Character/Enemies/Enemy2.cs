using System.Collections;
using UnityEngine;

public class Enemy2 : Enemy1
{
    [SerializeField] GameObject bullet;     // Prefab de la bala
    [SerializeField] Transform shootPosition;
    [SerializeField] float shootForce;

    [SerializeField] float shootCooldown = 5;
    float shootTimer = 0;

    protected override void Update()
    {
        base.Update();
        shootTimer += Time.deltaTime;
    }

    protected override void AttackUpdate()
    {
        if (attackCoroutine != null || shootTimer < shootCooldown) return;
        attackCoroutine = StartCoroutine(IEShoot());
    }

    private IEnumerator IEShoot()
    {
        if (agent.enabled) agent.isStopped = true;

        Vector3 vectorToPlayer2 = (playerTransform.position - shootPosition.position).normalized;

        GameObject shot = Instantiate(bullet, shootPosition.position, Quaternion.LookRotation(vectorToPlayer2));
        shot.GetComponent<Rigidbody>().AddForce(vectorToPlayer.normalized * shootForce, ForceMode.Impulse);

        yield return new WaitForSeconds(2);

        shootTimer = 0;
        attackCoroutine = null;
        currentState = EnemyStates.Chase;
        if (agent.enabled) agent.isStopped = false;
    }
}
