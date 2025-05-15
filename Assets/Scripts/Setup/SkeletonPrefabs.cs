using UnityEngine;

[CreateAssetMenu(menuName = "hint13/Skeleton/New Prefab", fileName = "Skeleton")]
public class SkeletonPrefabs : ScriptableObject
{
    public SkeletonType type;
    public GameObject prefab;

    public float HP;
    public float Speed;

    public int countAttacks;

    public int spawnCount;
    public float spawnInterval;

    public float agroRadius;

    public override string ToString()
    {
        return string.Format("{0} [HP: {1}, Speed: {2}, Agro: {3}]", type,  HP, Speed, agroRadius);
    }
}
