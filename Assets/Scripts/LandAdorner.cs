using System.Net;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using Unity.Mathematics;
using static UnityEngine.UI.Image;
using static UnityEngine.Rendering.VolumeComponent;
using System.Diagnostics;

//box collider için layermask ekle objelere

public class LandAdorner : MonoBehaviour
{
    private GameObject land;
    [SerializeField] private GameObject[] resources;
    [SerializeField] private GameObject allTrees;
    Vector3 currentPoint, endPoint;
    float meshScale, raycastHeight, raycastHeightStep, offset, waterHeight;
    readonly float stepSize = 10f;
    int stepCount;
    List<float4> spawnableRanges;
    Unity.Mathematics.Random rand;
    
    int index = 0;
    Stopwatch stopwatch;

    // Start is called before the first frame update
    void Start()
    {
        InitializeLand();
        InitializeRaycast();
        FindValidGrounds();

        stopwatch = new Stopwatch();
        stopwatch.Start();
        PlaceObjects();
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
    }

    // Update is called once per frame
    void Update()
    {
        //spawnableRange[i]->x:first point, y:last point, z:vertical order of the line, w: raycast height of the line
        
    }

    private void PlaceObjects()
    {
        while (index < spawnableRanges.Count)
        {
            int startStep = (int)spawnableRanges[index].x / (int)stepSize + 1;
            Vector3 startingPoint = new Vector3(startStep, spawnableRanges[index].w, spawnableRanges[index].z);

            while (startingPoint.x <= spawnableRanges[index].y)
            {
                float offSet_X = offset * (rand.NextFloat() * 2 - 1);
                float offSet_Z = offset * (rand.NextFloat() * 2 - 1);
                Vector3 shootPoint = startingPoint + new Vector3(offSet_X, 0, offSet_Z);

                RaycastHit hit;
                Ray ray = new Ray(shootPoint, Vector3.down);
                if (Physics.Raycast(ray, out hit, shootPoint.y - waterHeight + 0.01f))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        if (rand.NextFloat() > 0.5f)
                        {
                            //int selection = rand.NextInt(0, 3);
                            Vector3 spawnPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                            Instantiate(resources[0], spawnPoint, Quaternion.identity, allTrees.transform);
                        }
                    }
                }
                startingPoint.x += stepSize;
            }
            index++;
        }
    }

    private void InitializeLand()
    {
        land = GameObject.FindWithTag("Ground");
        Mesh mesh = land.GetComponent<MeshFilter>().mesh;
        meshScale = land.transform.localScale.x;
        endPoint = mesh.bounds.max * meshScale;
        stepCount = (int)(endPoint.z / stepSize);
        raycastHeight = endPoint.y + 0.01f;
        raycastHeightStep = raycastHeight / 10;
        offset = stepSize / 2;
    }

    private void InitializeRaycast()
    {
        Vector3 startPoint = new Vector3(0, raycastHeight, 0);
        Ray startRay = new Ray(startPoint, Vector3.down);
        RaycastHit firstHit;
        if (Physics.Raycast(startRay, out firstHit, raycastHeight))
        {
            currentPoint = firstHit.point;
            waterHeight = currentPoint.y;
            currentPoint.y += 0.001f;
        }
    }

    private void FindValidGrounds()
    {
        Physics.queriesHitBackfaces = true;
        spawnableRanges = new List<float4>();
        for (int i = 0; i < stepCount; i++)
        {
            RaycastHit[] groundHits = RaycastEverything(currentPoint, Vector3.right, endPoint.x);
            if (groundHits.Length > 0)
            {
                for (int j = 0; j < groundHits.Length; j += 2)
                {
                    float maxRayHeight = raycastHeight;
                    Vector3 firstPoint = new Vector3(groundHits[j].point.x - 0.01f, maxRayHeight, currentPoint.z);
                    Vector3 lastPoint = new Vector3(groundHits[j + 1].point.x + 0.01f, maxRayHeight, currentPoint.z);

                    float distance = lastPoint.x - firstPoint.x;
                    while (firstPoint.y > waterHeight && Physics.Raycast(firstPoint, Vector3.right, distance))
                    {
                        firstPoint.y -= raycastHeightStep;
                        lastPoint.y -= raycastHeightStep;

                    }
                    maxRayHeight = firstPoint.y + raycastHeightStep;

                    float4 validRange = new float4(groundHits[j].point.x, groundHits[j + 1].point.x, currentPoint.z, maxRayHeight);
                    spawnableRanges.Add(validRange);
                }

            }
            currentPoint.z += stepSize;
        }
        Physics.queriesHitBackfaces = false;

        uint seed = (uint)(UnityEngine.Random.value * uint.MaxValue);
        if (seed == 0) seed++;
        rand = new Unity.Mathematics.Random(seed);
    }

    RaycastHit[] RaycastEverything(Vector3 origin, Vector3 direction, float maxDistance)
    {
        direction = direction.normalized;
        float originalMaxDistance = maxDistance;

        List<RaycastHit> hits = new();

        while (true)
        {
            RaycastHit hit;

            if (Physics.Raycast(origin, direction, out hit, maxDistance))
            {
                origin = hit.point + direction / 100.0f;
                maxDistance -= hit.distance;

                hit.distance = originalMaxDistance - maxDistance;
                hits.Add(hit);

                maxDistance -= 0.01f;
            }
            else
            {
                return hits.ToArray();
            }
        }
    }
}
