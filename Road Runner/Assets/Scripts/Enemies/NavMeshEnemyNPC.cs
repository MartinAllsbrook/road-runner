using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using QFSW.QC;

public class NavMeshEnemyNPC : EnemyNPC
{
    [SerializeField] private NavMeshAgent agent;

    [Command]
    private void SetDestinationHere()
    {
        Vector3 destination = Player.Instance.transform.position;
        agent.SetDestination(destination);
    }
}
