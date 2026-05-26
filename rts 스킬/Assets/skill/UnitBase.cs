using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class UnitBase : MonoBehaviour, IHealable
{
    [Header("기본 스탯")]
    [SerializeField] protected float maxHP = 100f;
    [SerializeField] protected float moveSpeed = 1f;

    public float CurrentHP { get; protected set; }
    public float MaxHP => maxHP;
    public bool IsDead { get; protected set; }
    public bool IsInvincible { get; private set; }
    public bool IsSkillSealed { get; private set; }

    protected Rigidbody rb;
    protected SpriteRenderer spriteRenderer;
    private Color originalColor;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        CurrentHP = maxHP;
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsInvincible || IsDead) return;
        CurrentHP = Mathf.Max(0f, CurrentHP - damage);
        if (CurrentHP <= 0f) Die();
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);
    }

    protected virtual void Die()
    {
        IsDead = true;
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void TryUseSkill()
    {
        if (IsDead || IsSkillSealed) return;
        OnSkillActivated();
    }

    protected abstract void OnSkillActivated();

    public void ApplyInvincible(float duration)
    {
        if (IsDead) return;
        StartCoroutine(InvincibleRoutine(duration));
    }

    private IEnumerator InvincibleRoutine(float duration)
    {
        IsInvincible = true;
        spriteRenderer.color = new Color(1f, 0.85f, 0f, 0.85f);
        yield return new WaitForSeconds(duration);
        IsInvincible = false;
        spriteRenderer.color = originalColor;
    }

    protected void MoveTo(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0f;
        rb.linearVelocity = dir * moveSpeed;
        if (dir.x != 0f)
            transform.localScale = new Vector3(dir.x < 0f ? -1f : 1f, 1f, 1f);
    }

    protected void StopMove() => rb.linearVelocity = Vector3.zero;

    protected bool IsInRange(Vector3 targetPos, float range)
        => Vector3.Distance(transform.position, targetPos) <= range;
}


