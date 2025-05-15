using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnSkeleton : MonoBehaviour
{
    public List<SkeletonPrefabs> _skeletonPrefabs = new();
    public float spawnWidth = 5f;
    public float spawnHeigh = 0.5f;

    private List<int> spawnIntervals = new();
    private List<int> spawnCount = new();

    private bool _isSpawning = false;
    private int spawnIndex = 0;

    // For debug
    private Vector3[] points;

    void Start()
    {
        foreach (SkeletonPrefabs prefab in _skeletonPrefabs)
        {
            spawnIntervals.Add((int)prefab.spawnInterval);
            spawnCount.Add(prefab.spawnCount);
        }
        points = new Vector3[4]
        {
            new Vector3(transform.position.x-spawnWidth, 0.01f, transform.position.z-spawnHeigh),
            new Vector3(transform.position.x+spawnWidth, 0.01f, transform.position.z-spawnHeigh),
            new Vector3(transform.position.x+spawnWidth, 0.01f, transform.position.z+spawnHeigh),
            new Vector3(transform.position.x-spawnWidth, 0.01f, transform.position.z+spawnHeigh)
        }; 
    }

    private void FixedUpdate()
    {
        if (!_isSpawning)
            TrySpawn();
    }

    private void Update()
    {
    }

    private void TrySpawn()
    {
        if (spawnIndex >= spawnIntervals.Count)
        {
            return;
        }
        int time = (int)Mathf.Ceil(Time.time);
        if (time % spawnIntervals[spawnIndex] == 0)
        {
            //Debug.Log("Spawn new Skeleton " + spawnIndex + ", time: " + time);
            if (spawnCount[spawnIndex] != 0)
                StartCoroutine(Spawn(_skeletonPrefabs[spawnIndex]));
        }
    }

    private IEnumerator Spawn(SkeletonPrefabs skeleton)
    {
        _isSpawning = true;
        float animSpeed = 0; 
        switch (spawnIndex) {
            case 0: animSpeed = 1f; break;
            case 1: animSpeed = 0.8f; break;
            case 2: animSpeed = 0.4f; break;
        }
        spawnCount[spawnIndex]--;
        if (spawnCount[spawnIndex] == 0)
        {
            spawnIndex++;
        }
        Vector3 lookToCamera = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        var s = Instantiate(skeleton.prefab, GetSpawnPoint(), transform.rotation.normalized);
        Animator anim = s.GetComponentInChildren<Animator>();
        if (anim != null) {
            //Debug.Log("Go, go, go...");
            anim.SetFloat("Speed", animSpeed);
        }
        ///Destroy(s, 5.0f);
        yield return new WaitForSeconds(1.0f);
        _isSpawning = false;
    }

    private Vector3 GetSpawnPoint()
    {
        float x = Random.Range(-spawnWidth, spawnWidth) + transform.position.x;
        float z = Random.Range(-0.5f, 0.5f) + transform.position.z;
        return new Vector3(x, transform.position.y, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineStrip(points, true);
    }
}
