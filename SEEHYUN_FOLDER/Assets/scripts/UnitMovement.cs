using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NavMesh를 사용한 유닛 이동 제어
/// 우클릭한 위치로 이동하는 기능 담당
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class UnitMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Selectable selectable;

    [Header("이동 설정")]
    [SerializeField] private float spreadRadius = 1f; // 여러 유닛 동시 이동 시 퍼지는 반경

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        selectable = GetComponent<Selectable>();
    }

    /// <summary>
    /// 지정된 위치로 이동 명령
    /// </summary>
    /// <param name="destination">목표 지점</param>
    /// <param name="offset">다른 유닛과 겹치지 않도록 추가할 오프셋</param>
    public void MoveToPosition(Vector3 destination, Vector3 offset)
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(destination + offset);
        }
    }

    /// <summary>
    /// 현재 이동 중인지 확인
    /// </summary>
    public bool IsMoving()
    {
        return agent != null && agent.remainingDistance > agent.stoppingDistance;
    }

    /// <summary>
    /// 이동 중지
    /// </summary>
    public void Stop()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }
}