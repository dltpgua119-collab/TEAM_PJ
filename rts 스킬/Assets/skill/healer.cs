using System.Collections.Generic;
using UnityEngine;

public class HealerUnit : UnitBase
{
    [Header("S키 스킬 - 버스트 힐")]
    [SerializeField] private float burstHealAmount = 60f;
    [SerializeField] private float healRange = 5f;

    [Header("힐 대상 레이어")]
    [SerializeField] private LayerMask allyLayer;

    protected override void Awake()
    {
        maxHP = 80f;
        moveSpeed = 1.3f;
        base.Awake();
    }

    protected override void OnSkillActivated()
    {
        List<IHealable> targets = CollectHealTargets();

        if (targets.Count == 0)
        {
            Debug.Log("[Healer] S Skill: 범위 내 힐 대상 없음");
            return;
        }

        foreach (var target in targets)
            target.Heal(burstHealAmount);

        Debug.Log($"[Healer] S Skill: 버스트 힐 +{burstHealAmount} HP × {targets.Count}명");
    }

    private List<IHealable> CollectHealTargets()
    {
        var targets = new List<IHealable>();

        Collider[] allyHits = Physics.OverlapSphere(transform.position, healRange, allyLayer);
        foreach (var hit in allyHits)
        {
            var healable = hit.GetComponent<IHealable>();
            if (healable == null || healable.IsDead) continue;
            if (healable.CurrentHP >= healable.MaxHP) continue;
            targets.Add(healable);
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, playerObj.transform.position);
            if (distToPlayer <= healRange)
            {
                var playerHealable = playerObj.GetComponent<IHealable>();
                if (playerHealable != null && !playerHealable.IsDead && playerHealable.CurrentHP < playerHealable.MaxHP)
                    targets.Add(playerHealable);
            }
        }

        return targets;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.3f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
