using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnSkeleton : MonoBehaviour
{
    [Header("Skeletons list")]
    public List<GameObject> _skeletonPrefabs = new();

    [Header("Spawn area size")]
    public float spawnAreaWidth = 5f;
    public float spawnAreaHeigh = 0.5f;

    private List<int> unitSpawnIntervals = new();
    private List<int> availableForSpawn = new();

    private bool _isSpawning = false;
    private int currentUnit = 0;

    private List<GameObject> skeletons = new();

    // For debug
    private Vector3[] points;

    void Start()
    {
        foreach (GameObject prefab in _skeletonPrefabs)
        {
            SkeletonInfo skeleton = prefab.GetComponent<Enemy>().skeleton;
            unitSpawnIntervals.Add((int)skeleton.spawnInterval);
            availableForSpawn.Add(skeleton.spawnCount);
        }
        Vector3 pos = transform.position;
        points = new Vector3[4]
        {
            new Vector3(pos.x-spawnAreaWidth, 0.01f, pos.z-spawnAreaHeigh),
            new Vector3(pos.x+spawnAreaWidth, 0.01f, pos.z-spawnAreaHeigh),
            new Vector3(pos.x+spawnAreaWidth, 0.01f, pos.z+spawnAreaHeigh),
            new Vector3(pos.x-spawnAreaWidth, 0.01f, pos.z+spawnAreaHeigh)
        }; 
        skeletons.Clear();
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
        if (currentUnit >= unitSpawnIntervals.Count)
        {
            return;
        }
        int time = (int)Mathf.Ceil(Time.time);
        if (time % unitSpawnIntervals[currentUnit] == 0)
        {
            if (availableForSpawn[currentUnit] != 0)
                StartCoroutine(Spawn(_skeletonPrefabs[currentUnit]));
        }
    }

    private IEnumerator Spawn(GameObject skeleton)
    {
        _isSpawning = true;
        availableForSpawn[currentUnit]--;
        if (availableForSpawn[currentUnit] == 0)
        {
            currentUnit++;
        }
        
        var s = Instantiate(skeleton, GetSpawnPoint(), transform.rotation.normalized);
        s.layer = LayerMask.NameToLayer("Enemy");
        skeletons.Add(s);

        yield return new WaitForSeconds(1f);
        _isSpawning = false;
    }

    private Vector3 GetSpawnPoint()
    {
        float x = Random.Range(-spawnAreaWidth, spawnAreaWidth) + transform.position.x;
        float z = Random.Range(-spawnAreaHeigh, spawnAreaHeigh) + transform.position.z;
        return new Vector3(x, transform.position.y, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineStrip(points, true);
    }

    public List<GameObject> GetSkeletonsList()
    {
        return skeletons;
    }
}
