using System.Collections;
using UnityEngine;

public class MeleeDealerUnit : UnitBase
{
    [Header("A키 스킬 - 돌진 & 스플래시")]
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private float splashRadius = 2f;
    [SerializeField] private float splashDamage = 80f;
    [SerializeField] private float arrivalThreshold = 0.4f;
    [SerializeField] private float skillDuration = 3f;

    [Header("탐색 레이어")]
    [SerializeField] private LayerMask enemyLayer;

    private bool isSkillActive = false;
    private Coroutine skillCoroutine;

    protected override void Awake()
    {
        maxHP = 120f;
        moveSpeed = 1.5f;
        base.Awake();
    }

    private void Update()
    {
        if (IsDead || !isSkillActive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist <= arrivalThreshold)
        {
            StopMove();
            ExecuteSplashAttack(transform.position);
        }
        else
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            dir.y = 0f;
            rb.linearVelocity = dir * dashSpeed;
            if (dir.x != 0f)
                transform.localScale = new Vector3(dir.x < 0f ? -1f : 1f, 1f, 1f);
        }
    }

    protected override void OnSkillActivated()
    {
        if (isSkillActive) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log("[MeleeDealer] A Skill: 'Player' 태그 오브젝트를 찾을 수 없음");
            return;
        }

        Debug.Log("[MeleeDealer] A Skill: 플레이어 위치 돌진 시작!");

        if (skillCoroutine != null) StopCoroutine(skillCoroutine);
        skillCoroutine = StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        isSkillActive = true;
        yield return new WaitForSeconds(skillDuration);
        isSkillActive = false;
        StopMove();
        Debug.Log("[MeleeDealer] A Skill: 종료");
    }

    private void ExecuteSplashAttack(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, splashRadius, enemyLayer);
        int hitCount = 0;

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;
            damageable.TakeDamage(splashDamage);
            hitCount++;
        }

        Debug.Log($"[MeleeDealer] 스플래시 폭발! 반경 {splashRadius} | {hitCount}명 적중 | 데미지 {splashDamage}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}

