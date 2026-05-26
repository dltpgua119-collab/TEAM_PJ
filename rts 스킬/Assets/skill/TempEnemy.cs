using System.Collections;
using UnityEngine;

public class TempEnemy : MonoBehaviour, ISkillSealable, IDamageable
{
    public bool IsSkillSealed { get; private set; }

    public void ApplySkillSeal(float duration)
    {
        StartCoroutine(SealRoutine(duration));
    }

    private IEnumerator SealRoutine(float duration)
    {
        IsSkillSealed = true;
        Debug.Log($"[TempEnemy] 스킬 봉쇄 {duration}초");
        yield return new WaitForSeconds(duration);
        IsSkillSealed = false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"[TempEnemy] 데미지 -{damage}");
    }
}