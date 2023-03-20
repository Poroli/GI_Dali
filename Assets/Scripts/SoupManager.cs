using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoupManager : MonoBehaviour
{
    public static Table[] Tables;

    [SerializeField] private float timeToCook;
    private static Cooker[] Cookers;

    private void CookingAgent()
    {
        for (int i = 0; i < Cookers.Length; i++)
        {
            if (Cookers[i].SoupToCook.Ingredients && !Cookers[i].SoupToCook.IsCooking)
            {
                Cookers[i].SoupToCook.IsCooking = true;
                StartCoroutine(SoupReadyForServe(i));
            }
        }
    }

    IEnumerator SoupReadyForServe(int i)
    {
        WaiterControlSystem.CookersForServe.Add(Cookers[i]);
        yield return new WaitForSeconds(timeToCook);
    }

    private void PoisonSoup()
    {
        for (int i = 0; i < Cookers.Length; i++)
        {
            if (Cookers[i].SoupToCook.IsCooking && !Cookers[i].SoupToCook.Poisoned /* && Hier auf Topf geklickt einfügen */)
            {
                Cookers[i].SoupToCook.Poisoned = true;
            }
        }
    }

    public static void SetAvailableTables()
    {
        GameObject[] tempTableGOs = GameObject.FindGameObjectsWithTag("Table");
        Tables = new Table[tempTableGOs.Length];

        GameObject[] tempCookersGOs = GameObject.FindGameObjectsWithTag("Cooker");
        Tables = new Table[tempCookersGOs.Length];

        for (int i = 0; i < Tables.Length; i++)
        {
            Tables[i].TableID = i;
            Tables[i].TablePosition = tempTableGOs[i].transform.position;
        }
        for (int i = 0; i < Cookers.Length; i++)
        {
            Cookers[i].CookerID = i;
            Cookers[i].CookerPosition = tempCookersGOs[i].transform.position;
        }
    }

    private void Start()
    {
        SetAvailableTables();
    }

    private void Update()
    {
        CookingAgent();
    }
}

public class Soup
{
    public bool Ingredients;
    public bool Poisoned;
    public bool IsCooking;
}
public class Cooker
{
    public int CookerID;
    public Vector3 CookerPosition;
    public Soup SoupToCook;
}
public class Table
{
    public int TableID;
    public Vector3 TablePosition;
}
