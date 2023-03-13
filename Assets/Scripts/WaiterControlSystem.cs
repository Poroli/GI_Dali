using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaiterControlSystem : MonoBehaviour
{
    [SerializeField] private List<Table> tablesToServe;
    [SerializeField] private GameObject parkingPosition;
    [SerializeField] private GameObject cooker;
    private NavMeshAgent navMeshAgent;
    private bool getDish;
    private bool setDish;

    private void SetNewDestination()
    {
        if (!navMeshAgent.hasPath)
        {
            if (tablesToServe.Count == 0)
            {
                navMeshAgent.SetDestination(parkingPosition.transform.position);
            }
            else
            {
                ServingDish();
            }

        }
    }

    private void ServingDish()
    {
        if (getDish)
        {
            if (!navMeshAgent.hasPath) 
            { 
                navMeshAgent.SetDestination(cooker.transform.position);
            }
            else if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                getDish = false;
                navMeshAgent.SetDestination(tablesToServe[0].TablePosition);
                setDish = true;
            }
        }
        if (setDish)
        {
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                tablesToServe.RemoveAt(0);
            }
        }
    }
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        gameObject.transform.position = parkingPosition.transform.position;
    }
}
