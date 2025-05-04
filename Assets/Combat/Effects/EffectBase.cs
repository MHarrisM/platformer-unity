using UnityEngine;

public abstract class EffectBase
{
    public float duration;

    public EffectBase(float duration)
    {
        this.duration = duration;
    }

    public abstract void Apply(GameObject target);
}
