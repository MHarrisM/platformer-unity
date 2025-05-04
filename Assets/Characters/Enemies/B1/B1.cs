using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class B1 : EnemyBase
{


    private bool isJumpSlam = false;
    [SerializeField] private float jumpSlamUpAmount;
    [SerializeField] private float jumpSlamSpeed;
    private bool isBarrel = false;
    [SerializeField] private float brChargeUpTime = 0.5f;
    [SerializeField] private float brChargeSpeed = 15.0f;
    [SerializeField] private float brChargeDuration = 1.0f;
    //[SerializeField] private float brChargeDamage = 2.0f;     //Right now determined by the hitbox
    [SerializeField] private EnemyAttackHitbox attackHitbox;


    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    protected override void DetectPlayer()
    {
        base.DetectPlayer();
    }
    protected override void ProcessTarget()
    {
        //base.ProcessTarget();
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > detectionRange * 1.5f)
        {
            target = null;
            return;
        }
        spriteRenderer.flipX = target.position.x < transform.position.x;


        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }
    protected override void Attack()
    {
        if (!isJumpSlam)
        {
            JumpSlam();
        }
        isJumpSlam = false;

    }
    
    private void JumpSlam()
    {
        Vector2 velocity = rb.velocity;
        isJumpSlam = true;
        attackHitbox.Activate(brChargeDuration);

        Vector2 direction = (target.position - transform.position).normalized;// Towards Player
        //direction.y = jumpSlamUpAmount;
        velocity = direction * jumpSlamSpeed;
        velocity.y += jumpSlamUpAmount;
        rb.velocity = velocity; 
        lastAttackTime = Time.time;
    }

    private IEnumerator BarrelRollAttack()
    {
        isBarrel = true;
        


        yield return new WaitForSeconds(brChargeUpTime);
        if (target == null) yield break;
        animator.SetBool("isBarrel", isBarrel);

        //Check Collision detection and damage
        attackHitbox.Activate(brChargeDuration);

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * brChargeSpeed;
        yield return new WaitForSeconds(brChargeDuration);

        rb.velocity = Vector2.zero;
        //if (target.TryGetComponent<IDamageable>(out var damageable))
        //{
        //    damageable.TakeDamage(damage);
        //}
        isBarrel = false;
        animator.SetBool("isBarrel", isBarrel);
        lastAttackTime = Time.time;

    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
