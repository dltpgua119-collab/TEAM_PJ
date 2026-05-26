using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// RTS 방식의 유닛 선택 및 이동 명령 관리자
/// 좌클릭 드래그로 선택, 우클릭으로 이동
/// </summary>
public class UnitSelectionManager : MonoBehaviour
{
    [Header("카메라 설정")]
    [SerializeField] private Camera mainCamera;

    [Header("선택 박스 시각 효과")]
    [SerializeField] private RectTransform selectionBoxUI; // Canvas 위에 올릴 Image UI
    [SerializeField] private TextMeshProUGUI selectedCountText;

    [Header("이동 설정")]
    [SerializeField] private LayerMask groundLayer; // 지면 레이어
    [SerializeField] private float formationSpacing = 1.5f; // 유닛 간 간격

    private Vector2 startMousePosition;
    private List<Selectable> selectedUnits = new List<Selectable>();
    private bool isDragging = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (selectionBoxUI != null)
            selectionBoxUI.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleUnitSelection();
        HandleUnitMovement();
        HandleHotkeySelection();
    }

    #region 유닛 선택 로직

    /// <summary>
    /// 좌클릭 드래그 선택 처리
    /// </summary>
    void HandleUnitSelection()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            isDragging = true;

            // 기존 선택 해제
            DeselectAll();
        }

        // 드래그 중
        if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateSelectionBox();
        }

        // 드래그 종료
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            SelectUnitsInBox();
            isDragging = false;

            if (selectionBoxUI != null)
                selectionBoxUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 드래그 박스 UI 업데이트
    /// </summary>
    void UpdateSelectionBox()
    {
        if (selectionBoxUI == null) return;

        Vector2 currentMousePosition = Input.mousePosition;
        Vector2 boxStart = startMousePosition;
        Vector2 boxEnd = currentMousePosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2f;
        selectionBoxUI.position = boxCenter;

        Vector2 boxSize = new Vector2(
            Mathf.Abs(boxStart.x - boxEnd.x),
            Mathf.Abs(boxStart.y - boxEnd.y)
        );

        selectionBoxUI.sizeDelta = boxSize;
        selectionBoxUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// 드래그 박스 안의 유닛들 선택
    /// </summary>
    void SelectUnitsInBox()
    {
        Vector2 min = Vector2.Min(startMousePosition, Input.mousePosition);
        Vector2 max = Vector2.Max(startMousePosition, Input.mousePosition);

        Selectable[] allSelectables = FindObjectsOfType<Selectable>();

        foreach (Selectable selectable in allSelectables)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(selectable.transform.position);

            if (screenPos.x >= min.x && screenPos.x <= max.x &&
                screenPos.y >= min.y && screenPos.y <= max.y &&
                screenPos.z > 0) // 카메라 앞에 있는 오브젝트만
            {
                selectable.IsSelected = true;
                if (!selectedUnits.Contains(selectable))
                {
                    selectedUnits.Add(selectable);
                }
            }
        }

        Debug.Log($"{selectedUnits.Count}개 유닛 선택됨");
        UpdateSelectedCountUI();
    }

    /// <summary>
    /// 모든 유닛 선택 해제
    /// </summary>
    void DeselectAll()
    {
        foreach (Selectable selectable in selectedUnits)
        {
            if (selectable != null)
                selectable.IsSelected = false;
        }
        selectedUnits.Clear();
        UpdateSelectedCountUI();
    }

    #endregion

    #region 유닛 이동 로직

    /// <summary>
    /// 우클릭으로 선택된 유닛들 이동
    /// </summary>
    void HandleUnitMovement()
    {
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                Vector3 targetPosition = hit.point;
                MoveSelectedUnits(targetPosition);
                StartCoroutine(ShowClickIndicator(targetPosition));
            }
        }
    }

    /// <summary>
    /// 선택된 유닛들을 목표 지점으로 이동 (포메이션 적용)
    /// </summary>
    void MoveSelectedUnits(Vector3 targetPosition)
    {
        if (selectedUnits.Count == 0) return;

        // 한 유닛만 선택된 경우
        if (selectedUnits.Count == 1)
        {
            UnitMovement movement = selectedUnits[0].GetComponent<UnitMovement>();
            if (movement != null)
            {
                movement.MoveToPosition(targetPosition, Vector3.zero);
            }
            return;
        }

        // 여러 유닛 선택 시 원형 포메이션으로 배치
        int unitCount = selectedUnits.Count;
        float angleStep = 360f / unitCount;

        for (int i = 0; i < unitCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * formationSpacing,
                0f,
                Mathf.Sin(angle) * formationSpacing
            );

            UnitMovement movement = selectedUnits[i].GetComponent<UnitMovement>();
            if (movement != null)
            {
                movement.MoveToPosition(targetPosition, offset);
            }
        }

        Debug.Log($"{unitCount}개 유닛이 {targetPosition}로 이동 명령 받음");
    }

    #endregion
    /// <summary>
    /// 클릭 지점 표시
    /// </summary>
    System.Collections.IEnumerator ShowClickIndicator(Vector3 position)
    {
        GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicator.transform.position = position + Vector3.up * 0.1f;
        indicator.transform.localScale = Vector3.one * 0.5f;

        Renderer rend = indicator.GetComponent<Renderer>();
        rend.material.color = Color.yellow;

        yield return new WaitForSeconds(0.5f);

        Destroy(indicator);
    }
    /// <summary>
    /// 1,2,3 키로 역할별 유닛 선택
    /// </summary>
    void HandleHotkeySelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1키
        {
            SelectUnitsByName("Tanker");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2키
        {
            SelectUnitsByName("Dealer");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // 3키
        {
            SelectUnitsByName("Healer");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // 4키 ← 이거 추가
        {
            SelectUnitsByName("Player");
        }
    }

    void SelectUnitsByName(string unitName)
    {
        DeselectAll();

        Selectable[] allUnits = Object.FindObjectsByType<Selectable>(FindObjectsSortMode.None);

        foreach (Selectable unit in allUnits)
        {
            if (unit.gameObject.name.Contains(unitName))
            {
                unit.IsSelected = true;
                selectedUnits.Add(unit);
            }
        }

        Debug.Log($"{unitName} {selectedUnits.Count}개 선택됨");
    }
    void UpdateSelectedCountUI()
    {
        if (selectedCountText != null)
        {
            selectedCountText.text = $"Selected: {selectedUnits.Count}";
        }
    }
}