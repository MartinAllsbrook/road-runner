using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using QFSW.QC;
using Unity.Netcode;

public class NavMeshEnemyNPC : EnemyNPC
{
    [SerializeField] private NavMeshAgent agent;

    private Vector3 patrolCenter = new Vector3(0, 0, 0);
    private float maxPatrolDistance = 32f;

    [Header("Vision")]
    [SerializeField] private float visionRange = 32f;
    [SerializeField] private Transform visionOrigin;
    [SerializeField] private float visionAngle = 90f;
    [Tooltip("Everything the enemy can see except the LocalPlayer layer")] [SerializeField] private LayerMask canSee;
    [Tooltip("The LocalPlayer layer")] [SerializeField] private LayerMask localPayer;

    protected bool _canSeeLocalPlayer = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
    }

    private void Start()
    {
        if (!IsServer)
        {
            Destroy(agent);
            return;
        }

        agent.enabled = true;
        GoToRandomPoint();
    }

    protected virtual void Update()
    {
        _canSeeLocalPlayer = CanSeeLocalPlayer();

        if (_canSeeLocalPlayer)
        {
            SetTargetToLocalPlayer();
        }

        if (!IsServer)
            return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToRandomPoint();
        }
    }

    protected bool CanSeeLocalPlayer()
    {
        if (Physics.CheckSphere(visionOrigin.position, visionRange, localPayer))
        {
            Vector3 vectorToPlayer = GetVectorToLocalPlayer();
            float angleBetweenEnemyAndPlayer = Vector3.Angle(visionOrigin.forward, vectorToPlayer);
            //Debug.Log("Player In Range, Angle: " + angleBetweenEnemyAndPlayer);
            
            if (angleBetweenEnemyAndPlayer < visionAngle)
            {
                float distanceToPlayer = vectorToPlayer.magnitude;
                Ray ray = new Ray(visionOrigin.position, vectorToPlayer);

                if (!Physics.Raycast(ray, out RaycastHit hit, distanceToPlayer, canSee))
                {
                    Debug.DrawRay(visionOrigin.position, vectorToPlayer, Color.red);
                    return true;
                }
                //Debug.DrawRay(visionOrigin.position, directionToPlayer, Color.yellow);
                //return false;
            }
            //Debug.DrawRay(visionOrigin.position, directionToPlayer, Color.green);
        }
        return false;
    }

    protected Vector3 GetVectorToLocalPlayer()
    {
        Vector3 localPlayerPosition = Player.LocalPlayerInstance.transform.position + Vector3.up * 0.5f;
        Vector3 vectorToPlayer = localPlayerPosition - transform.position;
        return vectorToPlayer;
    }

    public override void Spawn(SpawnZone parentSpawnZone, Vector3 sprinkleCenter, float sprinkleRadius)
    {
        base.Spawn(parentSpawnZone, sprinkleCenter, sprinkleRadius);

        patrolCenter = sprinkleCenter;
        maxPatrolDistance = sprinkleRadius;
    }

    #region Server Only
    private void GoToRandomPoint()
    {
        if (!IsServer)
        {
            Debug.LogError("GoToRandomPoint called on client");
            return;
        }

        Vector3 newPatrolPoint = GetPatrolPoint();

        agent.SetDestination(newPatrolPoint);
    }

    private Vector3 GetPatrolPoint()
    {
        if (!_freshlySpawned)
            return SprinkleGenerator.Instance.GetPointInSprinkleOnNavmesh();

        Vector3 randomPoint = patrolCenter + Random.insideUnitSphere * maxPatrolDistance;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, maxPatrolDistance, 1);
        return hit.position;
    }
    #endregion

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
