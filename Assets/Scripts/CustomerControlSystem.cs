using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerControlSystem : MonoBehaviour
{
    [SerializeField] private GameObject CustomerPrefab;
    private List<Customer> customers = new List<Customer>();
    private Table table;
    private Soup servedSoup;


    private void CustomerOrganizer()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (customers[i].Sitting || customers[i].navMeshAgent.hasPath)
            {
                continue;
            }
            else
            {
                for (int j = 0; j < SoupManager.Tables.Length; j++)
                {
                    if (!SoupManager.Tables[j].HasCustomer)
                    {
                        customers[i].navMeshAgent.destination = SoupManager.Tables[j].TablePosition;
                        SoupManager.Tables[j].HasCustomer = true;
                        table = SoupManager.Tables[j];
                        break;
                    }
                }
                StartCoroutine(CustomerReachedTable(i,table));
            }
        }
    }
    private void EatingSoup(int i, Table table)
    {
        print("ateSoup");
        if (table.SoupOnTable.Poisoned)
        {

        }
        customers[i].Sitting = false;
        table.DishServed = false;
        table.HasCustomer = false;
    }

    private IEnumerator CustomerReachedTable(int i, Table table)
    {
        yield return customers[i].navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
        customers[i].Sitting = true;
        StartCoroutine(WaitingForSoup(i, table));
    }

    private IEnumerator WaitingForSoup(int i,Table table)
    {
        yield return table.DishServed;
        EatingSoup(i,table);
    }

}

public class Customer : ScriptableObject
{
    public GameObject CustomerGO;
    public NavMeshAgent navMeshAgent;
    public bool Sitting;
}