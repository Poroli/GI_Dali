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
            navMeshAgent.destination = navHit.position;
        }
    }

    private void CounterMeatProduction()
    {
        if (navMeshAgent.remainingDistance <= 0.1f && !pathCheckCooldown)
        {
            SoupManager.MeatList[0].Corpse = false;
            SoupManager.MeatList[0].Hamburger = true;
        }
    }
    private void Fridging()
    {
        if (navMeshAgent.remainingDistance <= 0.1f && !pathCheckCooldown)
        {
            if (carryMeat)
            {
                for (int i = 0; i < SoupManager.Fridges.Length; i++)
                {
                    if (hit.collider.gameObject.Equals(SoupManager.Fridges[i].FridgeIdentify))
                    {
                        print("putdown Meat");
                        SoupManager.Fridges[i].StoresMeat = true;
                        carryMeat = false;
                        fridging = false;
                        break;
                    }
                }
                return;
            }
            if (!carryMeat)
            {
                for (int i = 0; i < SoupManager.Fridges.Length; i++)
                {
                    if (hit.collider.gameObject.Equals(SoupManager.Fridges[i].FridgeIdentify) && SoupManager.Fridges[i].StoresMeat)
                    {
                        print("pickup Meat");
                        SoupManager.Fridges[i].StoresMeat = false;
                        carryMeat = true;
                        fridging = false;
                        break;
                    }
                }
                return;
            }
        }
    }
    private void TryToCook()
    {
        if (navMeshAgent.remainingDistance <= 0.1f && !pathCheckCooldown)
        {
            if (carryMeat)
            {
                for (int i = 0; i < SoupManager.Cookers.Length; i++)
                {
                    if (!SoupManager.Cookers[i].SoupToCook.Ingredients && hit.collider.gameObject.transform.position.Equals(SoupManager.Cookers[i].CookerPosition))
                    {
                        print("added Ingrediants");
                        SoupManager.Cookers[i].SoupToCook.Ingredients = true;
                        carryMeat = false;
                        tryToCook = false; 
                        break;
                    }
                }
                return;
            }
            else
            {
                for (int i = 0; i < SoupManager.Cookers.Length; i++)
                {
                    if (!SoupManager.Cookers[i].SoupToCook.Poisoned && SoupManager.Cookers[i].SoupToCook.IsCooking && hit.collider.gameObject.transform.position.Equals(SoupManager.Cookers[i].CookerPosition))
                    {
                        print("poisoned Soup");
                        SoupManager.Cookers[i].SoupToCook.Poisoned = true;
                        tryToCook = false;
                        break;
                    }
                }
                return;
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
            TryToCook();
        }
        if (counterMeatProduction)
        {
            CounterMeatProduction();
        }
        if (fridging)
        {
            Fridging();
        }
    }
}