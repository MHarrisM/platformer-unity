using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 3;
    private float currentHealth;
    private Color originalColor;

    [Header("Detection")]
    public float detectionRange = 5f;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    [Header("Movement")]
    public float moveSpeed = 2f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public int damage = 1;

    protected float lastAttackTime;
    protected Transform target;
    protected Rigidbody2D rb;
    
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;
        
        originalColor = spriteRenderer.color;
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            DetectPlayer();
        }
        else
        {
            ProcessTarget();
        }
    }
    


    //Control Detection of player logic here
    protected virtual void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (hit != null)
        {
            target = hit.transform;
        }
    }


    //Control what to do when the player is found or not found
    protected virtual void ProcessTarget()
    {
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > detectionRange * 1.5f)
        {
            target = null;
            return;
        }

        // Face player
        spriteRenderer.flipX = target.position.x < transform.position.x;

        if (distance > attackRange)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }



    //Actual Combat logic of enemy
    protected virtual void Attack()
    {
        Debug.Log("Basic attack!");
        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(damage);
        }
    }
    protected virtual void DamageOnContanct()
    {
        //TODO - if imple enemies causing damage if they touch player
    }



    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage!");
        StartCoroutine(HitFlash());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // TODO: Add death FX, sound
        Destroy(gameObject);
    }

    private IEnumerator HitFlash()
    {
       
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(0.3f);
        


        spriteRenderer.color = Color.clear; // Flash red

        yield return new WaitForSeconds(0.1f); // Short flash

        spriteRenderer.color = originalColor;
    }
}


