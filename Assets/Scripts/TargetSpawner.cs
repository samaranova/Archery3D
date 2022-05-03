using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    // Use our target prefab
    public GameObject target;

    void Start()
    {
        SpawnTargets();
    }

    // Spawn 20 targets around the map
    private void SpawnTargets()
    {
        for (int i = 0; i < 10; i++)
        {
            // Spawn a target object
            GameObject targetObject = Instantiate(target);
        }
    }
}
