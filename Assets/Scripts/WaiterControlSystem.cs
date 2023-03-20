using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaiterControlSystem : MonoBehaviour
{
    public static List<Cooker> CookersForServe;


    [SerializeField] private List<Table> tablesToServe;
    [SerializeField] private GameObject parkingPosition;
    private NavMeshAgent navMeshAgent;
    private bool getDish;
    private bool setDish;

    private void CheckIfSoupReady()
    {
        if (CookersForServe.Count > 0 && !setDish)
        {
            getDish = true;
        }
    }

    private void ServingPath()
    {
        if (!navMeshAgent.hasPath)
        {
            if (tablesToServe.Count == 0 && CookersForServe.Count == 0)
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
                navMeshAgent.SetDestination(CookersForServe[0].CookerPosition);
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
                setDish = false;
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
