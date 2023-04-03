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
    private bool carryCorpse;
    private bool carryMeat;

    private void Start()
    {
        cam = Camera.main;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseScreenPosition = Input.mousePosition;
            Ray ray = cam.ScreenPointToRay(mouseScreenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                HitObjectTagCheck(hit);
            }
        }
    }

    private void HitObjectTagCheck(RaycastHit hit)
    {
        if (hit.collider.gameObject.CompareTag("Cooker"))
        {
            for (int i = 0; i < SoupManager.Cookers.Length; i++)
            {
                if (SoupManager.Cookers[i].CookerPosition.Equals(hit.collider.gameObject.transform.position))
                {
                    targetposition = SoupManager.Cookers[i].CookerPosition;
                }
                
            }
        }
        if (hit.collider.gameObject.CompareTag("Counter"))
        {
            if (carryCorpse)
            {
                targetposition = hit.collider.gameObject.transform.position;
                StartCoroutine("CounterMeatProduction");
            }
        }
        if (hit.collider.gameObject.CompareTag("Ground"))
        {
            targetposition = hit.point;
        }
        if (hit.collider.gameObject.CompareTag("Cooker"))
        {
            targetposition = hit.collider.gameObject.transform.position;
            StartCoroutine("TryToCook", hit);
        }
        if (hit.collider.gameObject.CompareTag("Fridge"))
        {
            targetposition = hit.collider.gameObject.transform.position;
            StartCoroutine("Fridging", hit);
        }
        navMeshAgent.destination = targetposition;
    }

    private IEnumerator CounterMeatProduction()
    {
        yield return navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
        SoupManager.MeatList[0].Corpse = false;
        SoupManager.MeatList[0].Hamburger = true;
    }
    private IEnumerator Fridging(RaycastHit hit)
    {
        yield return navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
        if (carryMeat)
        {
            for (int i = 0; i < SoupManager.Fridges.Length; i++)
            {
                if (hit.collider.gameObject.Equals(SoupManager.Fridges[i]))
                {
                    SoupManager.Fridges[i].StoresMeat = true;
                    carryMeat = false;
                }
            }
        }
        if (!carryMeat)
        {
            for (int i = 0; i < SoupManager.Fridges.Length; i++)
            {
                if (hit.collider.gameObject.Equals(SoupManager.Fridges[i]) && SoupManager.Fridges[i].StoresMeat)
                {
                    SoupManager.Fridges[i].StoresMeat = true;
                    carryMeat = true;
                }
            }
        }
    }
    private IEnumerator TryToCook(RaycastHit hit)
    {
        yield return navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
        if (carryMeat)
        {
            for (int i = 0; i < SoupManager.Cookers.Length; i++)
            {
                if (!SoupManager.Cookers[i].SoupToCook.Ingredients && hit.collider.gameObject.transform.position.Equals(SoupManager.Cookers[i].CookerPosition))
                {
                    SoupManager.Cookers[i].SoupToCook.Ingredients = true;
                }
            }
        }
        else
        {
            for (int i = 0; i < SoupManager.Cookers.Length; i++)
            {
                if (!SoupManager.Cookers[i].SoupToCook.Poisoned && SoupManager.Cookers[i].SoupToCook.IsCooking && hit.collider.gameObject.transform.position.Equals(SoupManager.Cookers[i].CookerPosition))
                {
                    SoupManager.Cookers[i].SoupToCook.Poisoned = true;
                }
            }
        }
    }
}