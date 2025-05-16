using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public SkeletonInfo skeleton;
    public float attackDistance = 1f;

    private Player player;
    private Animator animator;

    private Vector3 moveDirection = Vector3.zero;
    private float currentSpeed = 0f;

    private bool isAttacking = false;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        if (skeleton == null)
            skeleton = Resources.Load<SkeletonInfo>("Skeleton/NormalSkeleton");
        animator = GetComponentInChildren<Animator>();
        Debug.Log(skeleton.ToString());            
    }

    private void Update()
    {
        moveDirection = MoveDirection();
        currentSpeed = Speed();
        if (!isAttacking)
        {
            transform.position += currentSpeed * Time.deltaTime * moveDirection;
        }
        ShowAnimation();
    }

    private float Speed()
    {
        float speed = 0f;
        if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
            speed = skeleton.Speed;
        return speed;
    }

    private Vector3 MoveDirection()
    {
        transform.LookAt(player.transform.position);
        return transform.forward;
    }

    private void ShowAnimation()
    {
        if (currentSpeed == 0)
        {
            animator.SetFloat("Speed", 0.0f);
            if (!isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else if (!isAttacking)
        {
            animator.SetFloat("Speed", skeleton.animationSpeed);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        SkeletonAttack attack = ChooseAttack();
        animator.SetTrigger(attack.ToString());
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }

    private SkeletonAttack ChooseAttack()
    {
        float rnd = Random.value;
        if (rnd < 0.3f && skeleton.countAttacks > 2)
        {
            return SkeletonAttack.Slash;
        }
        if (rnd < 0.5f && skeleton.countAttacks > 1)
        {
            return SkeletonAttack.Kick;
        }
        return SkeletonAttack.Punch;
    }
}
