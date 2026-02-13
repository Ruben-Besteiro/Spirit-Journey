using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class AttackBox : MonoBehaviour
{
    [Header("Damage")]
    public GameObject source;
    public float damage = 10f;

    [Header("Tag Filter")]
    public List<string> invalidTags = new();

    [Header("Debug")]
    public bool debugHits;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidTarget(other))
            return;

        Damageable dmg = other.GetComponentInParent<Damageable>();

        if (dmg == null)
            return;

        DamageInfo info = new DamageInfo(damage, source);
        dmg.TakeDamage(info);

        if (debugHits)
            Debug.Log($"{name} hit {other.name} for {damage}");
    }
    private bool IsValidTarget(Collider other)
    {
        if (invalidTags == null || invalidTags.Count == 0)
            return true;

        foreach (var tag in invalidTags)
        {
            if (other.CompareTag(tag))
                return false;
        }

        return true;
    }

    public void Setup(GameObject source)
    {
        this.source = source;
        invalidTags.Add(source.transform.tag);
    }
}
