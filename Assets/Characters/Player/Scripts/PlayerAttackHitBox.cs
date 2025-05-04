using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public float damage = 15f;
    public LayerMask targetLayer;
    private bool isActive = false;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    [SerializeField] private PlayerController playerController;
    

    private void Start()
    {
        
        
    }

    public void Activate(float duration)
    {
        isActive = true;
        
        hitEnemies.Clear();
        CheckForImmediateHits();
        
        StartCoroutine(DeactivateAfter(duration));
    }

    private IEnumerator DeactivateAfter(float time)
    {
        yield return new WaitForSeconds(time);
        hitEnemies.Clear();
        
        isActive = false;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        TryDamage(other);


    }

    private void CheckForImmediateHits()
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        float actualradius = circle.radius * transform.lossyScale.x;
        Vector2 center = (Vector2)circle.transform.position + circle.offset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, actualradius, targetLayer);
        
        foreach (var hit in hits)
        {
            TryDamage(hit);
        }
    }
    void OnDrawGizmos()
    {

            Gizmos.color = Color.red;
           

            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            float actualradius = circle.radius * transform.lossyScale.x;
            Vector2 center = (Vector2)circle.transform.position + circle.offset;
            Gizmos.DrawWireSphere(center, actualradius);
            
        
    }


    private void TryDamage(Collider2D other)
    {
        Debug.DrawLine(transform.position, other.transform.position, Color.red, 0.2f);
        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent<IDamageable>(out var dmg))
            {
                dmg.TakeDamage(damage);
            }
            hitEnemies.Add(other.gameObject);
        }
    }
}

