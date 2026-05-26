using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSkillController : MonoBehaviour
{
    [Header("스킬 쿨다운 (초)")]
    [SerializeField] private float cooldown_A = 8f;
    [SerializeField] private float cooldown_S = 6f;
    [SerializeField] private float cooldown_D = 10f;
    [SerializeField] private float cooldown_F = 15f;

    [Header("F스킬 무적 지속시간 (초)")]
    [SerializeField] private float invincibleDuration = 3f;

    [Header("UI 쿨다운 이미지 (없으면 비워도 됨)")]
    [SerializeField] private Image cdImage_A;
    [SerializeField] private Image cdImage_S;
    [SerializeField] private Image cdImage_D;
    [SerializeField] private Image cdImage_F;

    private float timer_A;
    private float timer_S;
    private float timer_D;
    private float timer_F;

    private void Update()
    {
        TickCooldowns();
        UpdateCooldownUI();
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) TryActivate_A();
        if (Input.GetKeyDown(KeyCode.S)) TryActivate_S();
        if (Input.GetKeyDown(KeyCode.D)) TryActivate_D();
        if (Input.GetKeyDown(KeyCode.F)) TryActivate_F();
    }

    private void TryActivate_A()
    {
        if (timer_A > 0f)
        {
            Debug.Log($"[SkillController] A 쿨다운 중 ({timer_A:F1}초 남음)");
            return;
        }
        var dealers = FindAliveUnits<MeleeDealerUnit>();
        if (dealers.Count == 0) { Debug.Log("[SkillController] A: 활성 근거리 딜러 없음"); return; }
        foreach (var d in dealers) d.TryUseSkill();
        timer_A = cooldown_A;
        Debug.Log($"[SkillController] A Skill: 근거리 딜러 {dealers.Count}명 발동");
    }

    private void TryActivate_S()
    {
        if (timer_S > 0f)
        {
            Debug.Log($"[SkillController] S 쿨다운 중 ({timer_S:F1}초 남음)");
            return;
        }
        var healers = FindAliveUnits<HealerUnit>();
        if (healers.Count == 0) { Debug.Log("[SkillController] S: 활성 힐러 없음"); return; }
        foreach (var h in healers) h.TryUseSkill();
        timer_S = cooldown_S;
        Debug.Log($"[SkillController] S Skill: 힐러 {healers.Count}명 발동");
    }

    private void TryActivate_D()
    {
        if (timer_D > 0f)
        {
            Debug.Log($"[SkillController] D 쿨다운 중 ({timer_D:F1}초 남음)");
            return;
        }
        var tankers = FindAliveUnits<TankerUnit>();
        if (tankers.Count == 0) { Debug.Log("[SkillController] D: 활성 탱커 없음"); return; }
        foreach (var t in tankers) t.TryUseSkill();
        timer_D = cooldown_D;
        Debug.Log($"[SkillController] D Skill: 탱커 {tankers.Count}명 발동");
    }

    private void TryActivate_F()
    {
        if (timer_F > 0f)
        {
            Debug.Log($"[SkillController] F 쿨다운 중 ({timer_F:F1}초 남음)");
            return;
        }
        var allUnits = FindAliveUnits<UnitBase>();
        if (allUnits.Count == 0) { Debug.Log("[SkillController] F: 활성 아군 없음"); return; }
        foreach (var unit in allUnits) unit.ApplyInvincible(invincibleDuration);
        timer_F = cooldown_F;
        Debug.Log($"[SkillController] F Skill: 전체 {allUnits.Count}명 → {invincibleDuration}초 무적");
    }

    private List<T> FindAliveUnits<T>() where T : UnitBase
    {
        var result = new List<T>();
        foreach (var u in FindObjectsOfType<T>())
            if (!u.IsDead) result.Add(u);
        return result;
    }

    private void TickCooldowns()
    {
        timer_A = Mathf.Max(0f, timer_A - Time.deltaTime);
        timer_S = Mathf.Max(0f, timer_S - Time.deltaTime);
        timer_D = Mathf.Max(0f, timer_D - Time.deltaTime);
        timer_F = Mathf.Max(0f, timer_F - Time.deltaTime);
    }

    private void UpdateCooldownUI()
    {
        if (cdImage_A != null) cdImage_A.fillAmount = timer_A / cooldown_A;
        if (cdImage_S != null) cdImage_S.fillAmount = timer_S / cooldown_S;
        if (cdImage_D != null) cdImage_D.fillAmount = timer_D / cooldown_D;
        if (cdImage_F != null) cdImage_F.fillAmount = timer_F / cooldown_F;
    }

    public bool IsReady_A => timer_A <= 0f;
    public bool IsReady_S => timer_S <= 0f;
    public bool IsReady_D => timer_D <= 0f;
    public bool IsReady_F => timer_F <= 0f;
}