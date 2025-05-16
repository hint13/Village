using UnityEngine;

[CreateAssetMenu(menuName="Game/Player Info", fileName="Player")]
public class PlayerInfo : ScriptableObject
{
    [Header("Main stats")]
    public int maxHP = 100;
    public int slashDamage = 20;
    public int kickDamage = 15;

    [Header("Movement speed")]
    public float moveSpeedForward = 3.5f;
    public float moveSpeedBackward = 2.3f;
    public float strafeSpeed = 1f;
    public float jumpForce = 5f;

    [Header("Animations timing")]
    public float slashTime = 1.4f;
    public float kickTime = 1.1f;
    public float jumpTime = 0.9f;
    public float hitTime = 0.9f;
    public float deadTime = 2.2f;

}
