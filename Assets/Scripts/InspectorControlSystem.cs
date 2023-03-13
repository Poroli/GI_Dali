using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class InspectorControlSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] placesToCheck;
    [SerializeField] private GameObject parkingPosition;
    [SerializeField] private int numOfPlacesToCheck;
    private NavMeshAgent navMeshAgent;
    private int currentCheckListPostion = 0;

    private void SetPlacesToCheckOrder()
    {
        List<GameObject> possiblePlacesToCheck = GameObject.FindGameObjectsWithTag("PlaceToCheck").ToList();
        if (possiblePlacesToCheck.Count < numOfPlacesToCheck)
        {
            Debug.Log("Not enough possible Places to Check or number of Places to Check to large");
            return;
        }
        else
        {
            placesToCheck = new GameObject[numOfPlacesToCheck]; 
            for (int i = 0; i < placesToCheck.Length; i++)
            {
                int tempint = Random.Range(0, possiblePlacesToCheck.Count);
                placesToCheck[i] = possiblePlacesToCheck[tempint];
                possiblePlacesToCheck.Remove(placesToCheck[i]);
            }
        }
    }

    private void SetNewDestination()
    {
        if (!navMeshAgent.hasPath)
        {
            if ((currentCheckListPostion) > (placesToCheck.Length -1))
            {
                navMeshAgent.SetDestination(parkingPosition.transform.position);
            }
            else
            {
                navMeshAgent.SetDestination(placesToCheck[currentCheckListPostion].transform.position);
                currentCheckListPostion++;
            }
        }
    }

    private void FixedUpdate()
    {
        SetNewDestination();
    }
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        gameObject.transform.position = parkingPosition.transform.position;
        SetPlacesToCheckOrder();
    }
}
