using UnityEngine;

[CreateAssetMenu(menuName = "Game/Skeleton Info", fileName = "Skeleton")]
public class SkeletonInfo: ScriptableObject
{
    public SkeletonType type;
    public GameObject prefab;

    public float HP;
    public float Speed;

    public int countAttacks;

    public int spawnCount;
    public float spawnInterval;

    public float agroRadius;
    public float animationSpeed;

    public override string ToString()
    {
        return string.Format("{0} [HP: {1}, Speed: {2}, Agro: {3}]", type,  HP, Speed, agroRadius);
    }
}
