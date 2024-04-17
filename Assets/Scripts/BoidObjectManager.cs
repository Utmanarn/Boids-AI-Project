using System.Collections.Generic;
using UnityEngine;

public class BoidObjectManager : MonoBehaviour
{
    private List<SheepController> _sheepList;
    
    private void Start()
    {
        SheepController[] sheep = FindObjectsOfType<SheepController>();

        for (int i = 0; i < sheep.Length; i++)
        {
            _sheepList.Add(sheep[i]);
        }
    }
}
