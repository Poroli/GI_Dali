using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoupManager : MonoBehaviour
{
    public static Table[] Tables;

    private void Start()
    {
        GameObject[] tempTableGOs = GameObject.FindGameObjectsWithTag("Table");
        Tables = new Table[tempTableGOs.Length];

        for (int i = 0; i < Tables.Length; i++)
        {
            Tables[i].TableID = i;
            Tables[i].TablePosition = tempTableGOs[i].transform.position;
        }
    }
}

public class Table
{
    public int TableID;
    public Vector3 TablePosition;
}
