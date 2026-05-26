using UnityEngine;
using TMPro;

public class UnitSpawner : MonoBehaviour
{
    [Header("유닛 프리팹")]
    [SerializeField] private GameObject tankerPrefab;
    [SerializeField] private GameObject dealerPrefab;
    [SerializeField] private GameObject healerPrefab;

    [Header("스폰 위치")]
    [SerializeField] private Transform spawnPoint;

    [Header("유닛 제한")]
    [SerializeField] private int maxUnits = 10;
    [SerializeField] private TextMeshProUGUI unitCountText;

    private int currentUnitCount = 0;

    void Start()
    {
        UpdateUnitCountUI();
    }

    public void SpawnTanker()
    {
        SpawnUnit(tankerPrefab);
    }

    public void SpawnDealer()
    {
        SpawnUnit(dealerPrefab);
    }

    public void SpawnHealer()
    {
        SpawnUnit(healerPrefab);
    }

    void SpawnUnit(GameObject unitPrefab)
    {
        if (currentUnitCount >= maxUnits)
        {
            Debug.Log("최대 유닛 수 도달!");
            return;
        }

        if (unitPrefab != null && spawnPoint != null)
        {
            // 고정 위치, 높이만 +2
            Vector3 spawnPos = spawnPoint.position + Vector3.up * 1f;

            GameObject newUnit = Instantiate(unitPrefab, spawnPos, Quaternion.identity);

            // Rigidbody 추가
            Rigidbody rb = newUnit.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = newUnit.AddComponent<Rigidbody>();
            }
            rb.mass = 3f; // 무겁게
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            currentUnitCount++;
            UpdateUnitCountUI();
        }
    }

    void UpdateUnitCountUI()
    {
        if (unitCountText != null)
        {
            unitCountText.text = $"{currentUnitCount}/{maxUnits}";
        }
    }
}