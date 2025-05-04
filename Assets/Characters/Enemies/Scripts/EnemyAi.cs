using UnityEngine;

public class EnemyAI : EnemyBase
{



    protected override void DetectPlayer()
    {
        base.DetectPlayer();
    }
    protected override void ProcessTarget()
    {
        base.ProcessTarget();
    }
    protected override void Attack()
    {
        base.Attack();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
