using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public float damage = 15f;
    public LayerMask targetLayer;
    private bool isActive = false;

    public void Activate(float duration)
    {
        isActive = true;
        CheckForImmediateHits();
        StartCoroutine(DeactivateAfter(duration));
    }

    private IEnumerator DeactivateAfter(float time)
    {
        yield return new WaitForSeconds(time);
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        TryDamage(other);


    }

    private void CheckForImmediateHits()
    {
       
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, targetLayer);
        
        foreach (var hit in hits)
        {
            TryDamage(hit);
        }
    }


    private void TryDamage(Collider2D other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(damage);
            }
           
        }
    }
}

