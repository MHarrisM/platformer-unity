using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    private Animator animator;
     
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private Vector2 attackOffset = new Vector2(1f, 0f);
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private PlayerAttackHitbox attackHitbox;
    private float lastAttackTime;
    

    [Header("Combo System")]
    [SerializeField] private float comboResetTime = 1.0f;
    [SerializeField] private int maxComboStep = 3;
    private int currentComboStep = 0;
    private float lastComboTime;

    [Header("Combo System")]
    [SerializeField] private float chargeTime = 1.5f;
    private float chargeStartTime;
    private bool isCharging;
    private bool hasReleasedCharge;
    public bool IsAttacking {  get; private set; }
    AnimatorStateInfo stateInfo;
   


    //[SerializeField] private GameObject[] slashFXPrefabs;         //If animatons


    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        
        playerController = GetComponent<PlayerController>();   
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!stateInfo.IsName("Attack") && !IsAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
            isCharging = true;
            hasReleasedCharge = false;
            chargeStartTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (isCharging)
            {
                isCharging = false;
                hasReleasedCharge = true;

                float heldTime = Time.time - chargeStartTime;
                if (heldTime >= chargeTime)
                {
                    ChargeAttack();
                }
                //else
                //{
                    
                //    if (!stateInfo.IsName("Attack")){
                //        StartCoroutine(AttackRoutine());
                //    }
                    
                //    //PerformComboAttack();
                //}
            }
        }
        if (Time.time - lastComboTime > comboResetTime)
        {
            currentComboStep = 0;
        }
    }
    IEnumerator AttackRoutine()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        IsAttacking = true;
        attackHitbox.gameObject.SetActive(true);
        playerController.isFlippingDisabled = true;
        attackHitbox.Activate(0.12f);
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(attackCooldown);
        
        playerController.isFlippingDisabled = false;
        if (attackCooldown > 0f)
        {
            yield return new WaitForSeconds(attackCooldown);
        }
        attackHitbox.gameObject.SetActive(false);
        IsAttacking = false;
    }
    void PerformComboAttack()
    {
        currentComboStep++;
        if (currentComboStep > maxComboStep)
            currentComboStep = 1;

        lastComboTime = Time.time;

        Debug.Log($"Combo Attack Step {currentComboStep}");
        ExecuteAttack(currentComboStep); // different hitboxes/sounds per step
    }

    void ChargeAttack()
    {
        Debug.Log("CHARGED ATTACK UNLEASHED!");
        ExecuteAttack(-1); // -1 = charged attack
    }




    void ExecuteAttack(int comboIndex)
    {
        Vector2 direction = playerController.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + Vector2.Scale(attackOffset, direction);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(1f, 1f), 0f, enemyLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                float finalDamage = damage;
                //Instantiate(slashFXPrefabs[comboIndex], center, Quaternion.identity);         //Normal Animation
                if (comboIndex == -1)
                {
                    //Instantiate(slashFXPrefabs[comboIndex], center, Quaternion.identity);     //Charge animation
                    finalDamage *= 3; // Charge attack = double damage
                }

                else if (comboIndex >= maxComboStep)
                {
                    //Instantiate(slashFXPrefabs[comboIndex], center, Quaternion.identity);   //Heavy Animation
                    finalDamage *= 2;   //Last attack of combo
                }
                damageable.TakeDamage(finalDamage);


                if (hit.TryGetComponent<EffectHandler>(out var effectHandler))
                {
                    Vector2 attackerPos = transform.position;
                    var kb = new KnockbackEffect(attackerPos, 8f, 0.2f);
                    effectHandler.ApplyEffect(kb);
                }


            }
        }
        Debug.Log($"Attack step {comboIndex} hit {hits.Length} targets");
        Debug.DrawLine(transform.position, center,Color.red, 0.2f);
    }


    void OnDrawGizmosSelected()
    {
        if (playerController == null) return;

        Vector2 direction = playerController.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + Vector2.Scale(attackOffset, direction);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector2(1f, 1f));
    }

}
