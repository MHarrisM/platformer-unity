using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float maxHealth = 3;
    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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

        spriteRenderer.color = Color.clear; // Flash red

        yield return new WaitForSeconds(0.1f); // Short flash

        spriteRenderer.color = originalColor;
    }
}
