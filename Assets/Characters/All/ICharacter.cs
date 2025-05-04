
using System.Collections;
using UnityEngine;

public interface ICharacter 
{
    Rigidbody2D GetRigidbody();
    void SetInvincible(bool value);
    void DisableControl();
    void EnableControl();
    Coroutine StartCoroutine(IEnumerator routine);
}
