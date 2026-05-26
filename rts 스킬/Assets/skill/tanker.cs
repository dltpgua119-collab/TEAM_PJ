using UnityEngine;

public class TankerUnit : UnitBase
{
    [Header("D키 스킬 - 스킬 봉쇄")]
    [SerializeField] private float skillSealRange = 5f;
    [SerializeField] private float skillSealDuration = 3f;

    [Header("탐색 레이어")]
    [SerializeField] private LayerMask enemyLayer;

    protected override void Awake()
    {
        maxHP = 300f;
        moveSpeed = 1.0f;
        base.Awake();
    }

    protected override void OnSkillActivated()
    {
        ISkillSealable target = FindNearestSealableEnemy();

        if (target == null)
        {
            Debug.Log("[Tanker] D Skill: 범위 내 봉쇄 가능한 적 없음");
            return;
        }

        target.ApplySkillSeal(skillSealDuration);
        Debug.Log($"[Tanker] D Skill: {((MonoBehaviour)target).name} 스킬 봉쇄 → {skillSealDuration}초");
    }

    private ISkillSealable FindNearestSealableEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, skillSealRange, enemyLayer);

        ISkillSealable nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            var sealable = hit.GetComponent<ISkillSealable>();
            if (sealable == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = sealable;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.6f, 0f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, skillSealRange);
    }
}

