// 힐러 S스킬 대상 인터페이스
// 플레이어 담당 팀원 → PlayerController에 IHealable 추가 + "Player" 태그 설정
public interface IHealable
{
    float CurrentHP { get; }
    float MaxHP { get; }
    bool IsDead { get; }
    void Heal(float amount);
}

// 탱커 D스킬 봉쇄 대상 인터페이스
// 적 담당 팀원 → EnemyUnit에 ISkillSealable 추가 + Enemy 레이어 설정
public interface ISkillSealable
{
    bool IsSkillSealed { get; }
    void ApplySkillSeal(float duration);
}

// 근거리 딜러 스플래시 데미지 대상 인터페이스
// 적 담당 팀원 → EnemyUnit에 IDamageable 추가
public interface IDamageable
{
    void TakeDamage(float damage);
}
