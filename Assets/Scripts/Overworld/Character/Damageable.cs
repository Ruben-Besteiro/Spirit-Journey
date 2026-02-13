using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public event Action<DamageInfo> OnDamaged;

    public void TakeDamage(DamageInfo info)
    {
        OnDamaged?.Invoke(info);
    }
}

public struct DamageInfo
{
    public float amount;
    public GameObject source;

    public DamageInfo(float amount, GameObject source)
    {
        this.amount = amount;
        this.source = source;
    }
}


