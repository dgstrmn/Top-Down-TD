using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    private Vector3 spawnLocation = Vector3.zero;
    [SerializeField] private GameObject terrain;
    private int i, j;
    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(terrain, spawnLocation, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
