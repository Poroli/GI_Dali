using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerControlSystem : MonoBehaviour
{
    [SerializeField] private GameObject CustomerPrefab;
    [SerializeField][Range(0, 5)] private float timeToEatSoup;
    private List<Customer> customers = new();
    private Soup servedSoup;
    private GameObject customerGOsSpawnposition;

    private void InitializeCustomer()
    {
        GameObject tmpCustomerGO = Instantiate(CustomerPrefab, customerGOsSpawnposition.transform.position, Quaternion.identity);
        tmpCustomerGO.transform.parent = gameObject.transform;

        Customer tmpCustomer = new()
        {
            CustomerGO = tmpCustomerGO,
            navMeshAgent = tmpCustomerGO.GetComponent<NavMeshAgent>()
        };
        tmpCustomer.navMeshAgent.updateRotation = false;
        customers.Add(tmpCustomer);
    }

    private void CustomerOrganizer()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if ((customers[i].navMeshAgent.hasPath && customers[i].WalkToTable))
            {
                continue;
            }

            if (customers[i].WaitingForSoup)
            {
                if (SoupManager.Tables[customers[i].TableWhereHeSitsID].DishServed)
                {
                    StartCoroutine(TimeCutomerNeedsToEat(i));
                }
                continue;
            }
            if (customers[i].navMeshAgent.remainingDistance <= 0.1f && customers[i].WalkToTable)
            {
                print("Customer reached Table");
                WaiterControlSystem.TablesToServe.Add(SoupManager.Tables[customers[i].TableWhereHeSitsID]);
                customers[i].WalkToTable = false;
                customers[i].navMeshAgent.isStopped = true;
                customers[i].Sitting = true;
                customers[i].WaitingForSoup = true;
                continue;
            }
            for (int j = 0; j < SoupManager.Tables.Length; j++)
            {
                if (!SoupManager.Tables[j].HasCustomer)
                {
                    customers[i].navMeshAgent.destination = SoupManager.Tables[j].TablePosition;
                    SoupManager.Tables[j].HasCustomer = true;
                    customers[i].TableWhereHeSitsID = j;
                    customers[i].WalkToTable = true;
                    break;
                }
            }
        }
    }
    private void EatingSoup(int i)
    {
        print("ateSoup");
        if (SoupManager.Tables[customers[i].TableWhereHeSitsID].SoupOnTable.Poisoned)
        {
            print("Customer dies");
            return;
        }
        customers[i].Sitting = false;
        customers[i].WaitingForSoup = false;
        SoupManager.Tables[customers[i].TableWhereHeSitsID].DishServed = false;
        SoupManager.Tables[customers[i].TableWhereHeSitsID].HasCustomer = false;
    }

    private IEnumerator TimeCutomerNeedsToEat(int i)
    {
        print("eatSoup");
        yield return new WaitForSeconds(timeToEatSoup);
        EatingSoup(i);
    }

    private void Start()
    {
        customerGOsSpawnposition = GameObject.Find("CustomersSpawnPosition");

        // Test
        InitializeCustomer();
    }

    private void FixedUpdate()
    {
        CustomerOrganizer();
    }
}

[Serializable]
public class Customer
{
    public GameObject CustomerGO;
    public NavMeshAgent navMeshAgent;
    public int TableWhereHeSitsID;
    public bool WalkToTable;
    public bool Sitting;
    public bool WaitingForSoup;
}