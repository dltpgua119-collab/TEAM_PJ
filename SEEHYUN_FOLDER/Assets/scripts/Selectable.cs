using UnityEngine;

/// <summary>
/// 선택 가능한 유닛임을 표시하는 컴포넌트
/// 선택 시각 효과도 여기서 관리
/// </summary>
public class Selectable : MonoBehaviour
{
    [Header("선택 표시 설정")]
    [SerializeField] private GameObject selectionIndicator; // 선택 시 발 밑에 표시될 원형 이미지

    private bool isSelected = false;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(isSelected);
            }
        }
    }

    void Start()
    {
        // 시작 시 선택 표시 비활성화
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }
}