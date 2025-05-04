using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 5;
    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Animator animator;
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage! Current health: {currentHealth}");
        StartCoroutine(HitFlash());
        animator.SetTrigger("hurt");
        if (currentHealth <= 0)
        {
            Debug.Log("Player Died!");
           StartCoroutine(Die());
            animator.SetTrigger("die");
        }
    }

    private IEnumerator Die()
    {
        // TODO: Add death FX, sound
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    private IEnumerator HitFlash()
    {
        
        
        for (int i = 0; i < 5; i++)
        {
            spriteRenderer.color = Color.clear; // Flash red

            yield return new WaitForSeconds(0.1f); // Short flash

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        };
    }



}
