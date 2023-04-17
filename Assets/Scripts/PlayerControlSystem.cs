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
    private RaycastHit hit;
    private bool carryCorpse;
    private bool carryMeat;
    private bool fridging;
    private bool tryToCook;
    private bool counterMeatProduction;
    private bool pathCheckCooldown;



    private void HitObjectTagCheck()
    {
        pathCheckCooldown = true;
        StartCoroutine(PathCheckCooldown());
        if (hit.collider.gameObject.CompareTag("Cooker"))
        {
            fridging = false;
            counterMeatProduction = false;
            targetposition = hit.collider.gameObject.transform.position;
            tryToCook = true;
        }
        if (hit.collider.gameObject.CompareTag("Counter"))
        {
            fridging = false;
            tryToCook = false;
            if (carryCorpse)
            {
                targetposition = hit.collider.gameObject.transform.position;
                counterMeatProduction = true;
            }
        }
        if (hit.collider.gameObject.CompareTag("Fridge"))
        {
            counterMeatProduction = false;
            tryToCook = false;
            targetposition = hit.collider.gameObject.transform.position;
            fridging = true;
        }
        if (hit.collider.gameObject.CompareTag("Ground"))
        {
            counterMeatProduction = false;
            fridging = false;
            tryToCook = false;
            targetposition = hit.point;
        }
        if (NavMesh.SamplePosition(targetposition,out NavMeshHit navHit, 5f, NavMesh.AllAreas))
        {
            print(navHit.position);
            navMeshAgent.destination = navHit.position;
        }
    }

    private void CounterMeatProduction()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && !pathCheckCooldown)
        {
            SoupManager.MeatList[0].Corpse = false;
            SoupManager.MeatList[0].Hamburger = true;
        }
    }
    private void Fridging()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && !pathCheckCooldown)
        {
            print("asshole");
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
    }
    private void TryToCook()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && !pathCheckCooldown)
        {
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

    private IEnumerator PathCheckCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        pathCheckCooldown = false;
    }
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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                HitObjectTagCheck();
            }
        }
        if (tryToCook)
        {
            //TryToCook();
        }
        if (counterMeatProduction)
        {
            //CounterMeatProduction();
        }
        if (fridging)
        {
            //Fridging();
        }
        print (navMeshAgent.pathStatus);
    }
}