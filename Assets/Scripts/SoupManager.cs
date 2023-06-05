using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupManager : MonoBehaviour
{
    public static List<Meat> MeatList = new List<Meat>();
    public static Fridge[] Fridges;
    public static Cooker[] Cookers;
    public static Table[] Tables;

    [SerializeField] private float timeToCook;

    private void CookingAgent()
    {
        for (int i = 0; i < Cookers.Length; i++)
        {
            if (Cookers[i].SoupToCook.Ingredients && !Cookers[i].SoupToCook.IsCooking && !Cookers[i].SoupToCook.ReadyForServe)
            {
                Cookers[i].SoupToCook.IsCooking = true;
                print("cooking Soup");
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
        print ("Soup cooked");
    }

    private void CreateTableCookerAndFridgeList()
    {
        GameObject[] tempTableGOs = GameObject.FindGameObjectsWithTag("Table");
        GameObject[] tempCookersGOs = GameObject.FindGameObjectsWithTag("Cooker");
        GameObject[] tempFridgeGOs = GameObject.FindGameObjectsWithTag("Fridge");
        Tables = new Table[tempTableGOs.Length];
        Cookers = new Cooker[tempCookersGOs.Length];
        Fridges = new Fridge[tempFridgeGOs.Length];

        for (int i = 0; i < Tables.Length; i++)
        {
            Tables[i] = ScriptableObject.CreateInstance<Table>();

            Tables[i].TableID = i;
            Tables[i].TablePosition = tempTableGOs[i].transform.position;
            Tables[i].SoupOnTable = ScriptableObject.CreateInstance<Soup>();
        }
        for (int i = 0; i < Cookers.Length; i++)
        {
            Cookers[i] = ScriptableObject.CreateInstance<Cooker>();

            Cookers[i].CookerID = i;
            Cookers[i].CookerPosition = tempCookersGOs[i].transform.position;
            Cookers[i].SoupToCook = ScriptableObject.CreateInstance<Soup>();
        }
        for (int i = 0; i < Fridges.Length; i++)
        {
            Fridges[i] = ScriptableObject.CreateInstance<Fridge>();
            Fridges[i].FridgeIdentify = tempFridgeGOs[i];
        }
    }

    private void Start()
    {
        CreateTableCookerAndFridgeList();
    }

    private void Update()
    {
        CookingAgent();
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
    public bool HasCustomer;
    public bool DishServed;
    public Vector3 TablePosition;
    public Soup SoupOnTable;
}
public class Meat : ScriptableObject
{
    public bool Corpse = true;
    public bool Hamburger;
}
public class Fridge : ScriptableObject
{
    public bool StoresMeat = true;
    public GameObject FridgeIdentify;
}
