using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemyNPC : EnemyNPC
{
    [Header("Enemy Stats")]
    [SerializeField] private float viewDistance = 10f;

    [Header("Flying Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float hoverHeight = 5f;
    [SerializeField] private float objectAvoidanceDistance = 5f;
    [SerializeField] private float moveForce = 5f;
    [SerializeField] private Transform[] raycastPoints;

    [Header("Patrol Area")]
    [SerializeField] private Vector3 patrolCenter = new Vector3(0,0,0);
    [SerializeField] private float maxDistance = 10f;

    private Rigidbody _rigidbody;

    private Vector3 _moveDirection;
    private Vector3 _targetPosition;

    private Vector3 GetPatrolPoint()
    {
        Vector3 randomPoint = patrolCenter + Random.insideUnitSphere * maxDistance;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, maxDistance, 1);
        return hit.position;
    }
}
