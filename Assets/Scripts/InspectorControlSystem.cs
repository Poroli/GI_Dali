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
    [SerializeField][Range(1, 50)] private float maxRaycastRange;
    [SerializeField] private LayerMask layersRaycastCanDetect;
    [SerializeField] private bool debugGizmosOn;
    private NavMeshAgent navMeshAgent;
    private Vector3[] pathCornerPositions;
    private int currentCheckListPosition = 0;

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

    private void InspectorWalkDirectionIdentiy()
    {
        pathCornerPositions = navMeshAgent.path.corners;

        if (pathCornerPositions.Length < 2)
        {
            // tritt eigl. nie ein... ist zur absicherung da
            return;
        }
        if (Mathf.Abs((gameObject.transform.position - pathCornerPositions[1]).x) < Mathf.Abs((gameObject.transform.position - pathCornerPositions[1]).z))
        {
            switch ((gameObject.transform.position - pathCornerPositions[1]).z)
            {
                case > 0:
                    Physics.Raycast(gameObject.transform.position, Vector3.forward, maxRaycastRange, layersRaycastCanDetect);
                    break;
                case < 0:
                    Physics.Raycast(gameObject.transform.position, Vector3.back, maxRaycastRange, layersRaycastCanDetect);
                    break;
            }
        }
        else
        {
            switch ((gameObject.transform.position - pathCornerPositions[1]).x)
            {
                case > 0:
                    Physics.Raycast(gameObject.transform.position, Vector3.right, maxRaycastRange, layersRaycastCanDetect);
                    break;
                case < 0:
                    Physics.Raycast(gameObject.transform.position, Vector3.left, maxRaycastRange, layersRaycastCanDetect);
                    break;
            }
        }
    }

    private void SetNewDestination()
    {
        if (!navMeshAgent.hasPath)
        {
            if ((currentCheckListPosition) > (placesToCheck.Length - 1))
            {
                navMeshAgent.SetDestination(parkingPosition.transform.position);
            }
            else
            {
                navMeshAgent.SetDestination(placesToCheck[currentCheckListPosition].transform.position);
                currentCheckListPosition++;
            }
        }
    }

    private void FixedUpdate()
    {
        SetNewDestination();
        InspectorWalkDirectionIdentiy();
    }
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        gameObject.transform.position = parkingPosition.transform.position;
        SetPlacesToCheckOrder();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying && debugGizmosOn)
        {
            if (pathCornerPositions.Length >= 2)
            {
                if (Mathf.Abs((pathCornerPositions[1] - gameObject.transform.position).x) < Mathf.Abs((pathCornerPositions[1] - gameObject.transform.position).z))
                {
                    switch ((pathCornerPositions[1] - gameObject.transform.position).z)
                    {
                        case > 0:
                            Gizmos.DrawLine(gameObject.transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + maxRaycastRange));
                            break;
                        case < 0:
                            Gizmos.DrawLine(gameObject.transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z - maxRaycastRange));
                            break;
                    }
                }
                else
                {
                    switch ((pathCornerPositions[1] - gameObject.transform.position).x)
                    {
                        case > 0:
                            Gizmos.DrawLine(gameObject.transform.position, new Vector3(transform.position.x + maxRaycastRange, transform.position.y, transform.position.z));
                            break;
                        case < 0:
                            Gizmos.DrawLine(gameObject.transform.position, new Vector3(transform.position.x - maxRaycastRange, transform.position.y, transform.position.z));
                            break;
                    }
                }
            }

            for (int i = 0; i < pathCornerPositions.Length; i++)
            {
                Gizmos.DrawWireSphere(pathCornerPositions[i], 0.5f);
            }
        }
    }
}