using UnityEngine;

public class Enemy : MonoBehaviour
{

    public SkeletonPrefabs prefab;

    void Start()
    {
        if (prefab == null)
            prefab = Resources.Load<SkeletonPrefabs>("Skeleton/NormalSkeleton");
        Debug.Log(prefab.ToString());            
    }

    void Update()
    {
        transform.position += prefab.Speed * Time.deltaTime * transform.forward;
        if (transform.position.z < -20)
        {
            Destroy(gameObject);
        }
    }
}
