using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupManager : MonoBehaviour
{

    [SerializeField] private float timeToCook;
    private Table[] Tables;
    private Cooker[] Cookers;

    private void CookingAgent()
    {
        for (int i = 0; i < Cookers.Length; i++)
        {
            if (Cookers[i].SoupToCook.Ingredients && !Cookers[i].SoupToCook.IsCooking && !Cookers[i].SoupToCook.ReadyForServe)
            {
                Cookers[i].SoupToCook.IsCooking = true;
                StartCoroutine(SoupReadyForServe(i));
            }
        }
    }

    IEnumerator SoupReadyForServe(int i)
    {
        yield return new WaitForSeconds(timeToCook);
        WaiterControlSystem.CookersForServe.Add(Cookers[i]);
        Cookers[i].SoupToCook.IsCooking = false;
        Cookers[i].SoupToCook.ReadyForServe = true;
        StopCoroutine(SoupReadyForServe(i));
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

    private void CreateTableAndCookerList()
    {
        GameObject[] tempTableGOs = GameObject.FindGameObjectsWithTag("Table");
        GameObject[] tempCookersGOs = GameObject.FindGameObjectsWithTag("Cooker");
        Tables = new Table[tempTableGOs.Length];
        Cookers = new Cooker[tempCookersGOs.Length];

        for (int i = 0; i < Tables.Length; i++)
        {
            Tables[i] = ScriptableObject.CreateInstance<Table>();

            Tables[i].TableID = i;
            Tables[i].TablePosition = tempTableGOs[i].transform.position;
        }
        for (int i = 0; i < Cookers.Length; i++)
        {
            Cookers[i] = ScriptableObject.CreateInstance<Cooker>();

            Cookers[i].CookerID = i;
            Cookers[i].CookerPosition = tempCookersGOs[i].transform.position;
            Cookers[i].SoupToCook = ScriptableObject.CreateInstance<Soup>();
        }
    }

    private void Start()
    {
        CreateTableAndCookerList();
    }

    private void Update()
    {
        CookingAgent();
        PoisonSoup();
    }
}

public class Soup : ScriptableObject
{
    public bool Ingredients;
    public bool Poisoned;
    public bool IsCooking;
    public bool ReadyForServe;
}
public class Cooker : ScriptableObject
{
    public int CookerID;
    public Vector3 CookerPosition;
    public Soup SoupToCook;
}
public class Table : ScriptableObject
{
    public int TableID;
    public Vector3 TablePosition;
}
