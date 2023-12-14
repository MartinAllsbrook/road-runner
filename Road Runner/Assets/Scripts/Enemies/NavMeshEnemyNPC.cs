using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using QFSW.QC;

public class NavMeshEnemyNPC : EnemyNPC
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Patrol Area")]
    [SerializeField] private Vector3 patrolCenter = new Vector3(0, 0, 0);
    [SerializeField] private float maxPatrolDistance = 32f;

    // Patroling
    //private Vector3 _currentPatrolPoint = Vector3.zero;

    private void Start()
    {
        GoToRandomPoint();
    }

    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToRandomPoint();
        }
    }

    private void GoToRandomPoint()
    {
        Vector3 newPatrolPoint = GetPatrolPoint();

        agent.SetDestination(newPatrolPoint);
    }

    private Vector3 GetPatrolPoint()
    {
        Vector3 randomPoint = patrolCenter + Random.insideUnitSphere * maxPatrolDistance;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, maxPatrolDistance, 1);
        return hit.position;
    }

    [Command ("BotsFollowMe", MonoTargetType.All)]
    private void SetDestinationHere()
    {
        Vector3 destination = Player.Instance.transform.position;
        agent.SetDestination(destination);
    }
}
