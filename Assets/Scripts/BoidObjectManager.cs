using System.Collections.Generic;
using UnityEngine;

public class BoidObjectManager : MonoBehaviour
{
    public static List<SheepController> _sheepList;

    public List<SheepController> SheepList => _sheepList;

    private void Awake()
    {
        _sheepList = new List<SheepController>();
    }
/*
    private void Start()
    {
        SheepController[] sheep = FindObjectsOfType<SheepController>();

        for (int i = 0; i < sheep.Length; i++)
        {
            _sheepList.Add(sheep[i]);
        }
    }
*/
    private void Update()
    {
        Debug.Log(_sheepList.Count);
    }
}
