using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    public void ApplyEffect(EffectBase effect)
    {
        effect.Apply(gameObject);
    }
}
