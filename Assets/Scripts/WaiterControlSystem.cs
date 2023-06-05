using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaiterControlSystem : MonoBehaviour
{
    public static List<Cooker> CookersForServe = new();

    public static List<Table> TablesToServe = new();

    [SerializeField] [Range (0, 5)]private float timeNeededForServe;
    [SerializeField] private GameObject parkingPosition;
    private NavMeshAgent navMeshAgent;
    private bool getDish;
    private bool setDish;
    private bool parked;

    private void ServingPath()
    {
        if (TablesToServe.Count == 0 && !parked)
        {
            parked = true;
            navMeshAgent.SetDestination(parkingPosition.transform.position);
        }
        else if (navMeshAgent.hasPath && parked)
        {
            parked = false;
        }
        ServingDish();
    }

    private void ServingDish()
    {
        if (setDish &&  navMeshAgent.remainingDistance <= 2.5f)
        {
            print("Table to Serve reached");
            setDish = false;
            navMeshAgent.isStopped = true;
            StartCoroutine(Serve());
            TablesToServe[0].DishServed = true;
            TablesToServe[0].SoupOnTable.Poisoned = CookersForServe[0].SoupToCook.Poisoned;
            return;
        }
        if (navMeshAgent.remainingDistance <= 0.1f && getDish)
        {
            for (int i = 0; i < SoupManager.Cookers.Length; i++)
            {
                if (SoupManager.Cookers[i].CookerPosition == CookersForServe[0].CookerPosition)
                {
                    print("Waiter brings Soup to Customer");
                    navMeshAgent.SetDestination(TablesToServe[0].TablePosition);
                    getDish = false;
                    setDish = true;
                    break;
                }
            }
            return;
        }
        if (CookersForServe.Count > 0 && !setDish && TablesToServe.Count > 0 && !navMeshAgent.hasPath)
        {
            print("Waiter goes to Cooker");
            navMeshAgent.SetDestination(CookersForServe[0].CookerPosition);
            getDish = true;
            return;
        }
    }

    private IEnumerator Serve()
    {
        yield return new WaitForSeconds(timeNeededForServe);
        navMeshAgent.isStopped = false;
        navMeshAgent.ResetPath();
        print("Soup served");
    }

    private void FixedUpdate()
    {
        ServingPath();
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        gameObject.transform.position = parkingPosition.transform.position;
    }
}
