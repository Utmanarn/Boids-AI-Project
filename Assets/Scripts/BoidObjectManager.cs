using System.Collections.Generic;
using UnityEngine;

public class BoidObjectManager : MonoBehaviour
{
    private static List<SheepController> _sheepList;

    public static List<SheepController> SheepList => _sheepList;

    private void Awake()
    {
        _sheepList = new List<SheepController>();
    }
}
