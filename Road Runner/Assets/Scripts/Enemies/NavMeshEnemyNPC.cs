using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using QFSW.QC;
using Unity.Netcode;

public class NavMeshEnemyNPC : EnemyNPC
{
    [SerializeField] private NavMeshAgent agent;

    [Header("Patrol Area")]
    [SerializeField] private Vector3 patrolCenter = new Vector3(0, 0, 0);
    [SerializeField] private float maxPatrolDistance = 32f;

    [Header("Vision")]
    [SerializeField] private float visionRange = 32f;
    [SerializeField] private Transform visionOrigin;
    [SerializeField] private float visionAngle = 90f;
    [Tooltip("Everything the enemy can see except the LocalPlayer layer")] [SerializeField] private LayerMask canSee;
    [Tooltip("The LocalPlayer layer")] [SerializeField] private LayerMask localPayer;

    private void Start()
    {
        GoToRandomPoint();
    }

    private void Update()
    {
        if (CanSeeLocalPlayer())
        {
            SetTargetToLocalPlayer();
        }
        
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToRandomPoint();
        }
    }

    private bool CanSeeLocalPlayer()
    {
        if (Physics.CheckSphere(visionOrigin.position, visionRange, localPayer))
        {
            Vector3 localPlayerPosition = Player.LocalPlayerInstance.transform.position + Vector3.up * 0.4f;
            Vector3 directionToPlayer = localPlayerPosition - visionOrigin.position;
            float angleBetweenEnemyAndPlayer = Vector3.Angle(visionOrigin.forward, directionToPlayer);
            //Debug.Log("Player In Range, Angle: " + angleBetweenEnemyAndPlayer);
            
            if (angleBetweenEnemyAndPlayer < visionAngle)
            {
                float distanceToPlayer = directionToPlayer.magnitude;
                Ray ray = new Ray(visionOrigin.position, directionToPlayer);

                if (!Physics.Raycast(ray, out RaycastHit hit, distanceToPlayer, canSee))
                {
                    Debug.DrawRay(visionOrigin.position, directionToPlayer, Color.red);
                    return true;
                }
                //Debug.DrawRay(visionOrigin.position, directionToPlayer, Color.yellow);
                //return false;
            }
            //Debug.DrawRay(visionOrigin.position, directionToPlayer, Color.green);
        }
        return false;
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

    [Command ("BotsTargetMe", MonoTargetType.All)]
    private void SetTargetToLocalPlayer()
    {
        Vector3 destination = Player.LocalPlayerInstance.transform.position;
        SetTargetPositionServerRpc(destination);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetTargetPositionServerRpc(Vector3 targetPosition)
    {
        NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, maxPatrolDistance, 1);
        agent.SetDestination(hit.position);
    }
}
