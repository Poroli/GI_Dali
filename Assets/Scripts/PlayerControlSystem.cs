using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControlSystem : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private NavMeshAgent navMeshAgent;
    private Camera cam;
    private Vector3 targetposition;
    private Vector2 mouseScreenPosition;

    private void Start()
    {
        cam = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseScreenPosition = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mouseScreenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                targetposition = hit.point;
                navMeshAgent.destination = targetposition;
            }
        }
    }
}