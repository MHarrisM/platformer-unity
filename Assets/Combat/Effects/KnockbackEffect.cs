using UnityEngine;

public class KnockbackEffect : EffectBase
{
    private Vector2 kbDirection;
    private float kbForce;

    public KnockbackEffect(Vector2 sourcePosition, float force, float duration) : base(duration)
    {
        this.kbForce = force;
        this.kbDirection = sourcePosition;
    }

    public override void Apply(GameObject target)
    {
        if (target == null) return;

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (rb == null) return;
        Vector2 dir = (rb.transform.position - (Vector3)kbDirection).normalized;
        dir.x = 0.3f;
        dir.y = Mathf.Clamp(dir.y + 0.3f, -1f, 1f);
        rb.AddForce(dir * kbForce, ForceMode2D.Impulse);


        //if (target.TryGetComponent<PlayerController>(out var pc))
        //{
        //    pc.DisableMovement(duration); // You’ll define this method
        //}

        // You could also start a coroutine here via a Mono helper if needed
    }
}
