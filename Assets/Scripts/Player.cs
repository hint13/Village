using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public PlayerInfo stat;

    private CharacterController controller;
    private Animator animator;

    private Vector3 moveDirection = Vector3.zero;
    private float rotation = 0f;

    private bool isAttack = false;
    private SpawnSkeleton spawner;

    private LayerMask enemyLayer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        spawner = FindFirstObjectByType<SpawnSkeleton>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        if (!isAttack)
        {
            Move();
            Rotate();
            Attack();
        }
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = moveX * stat.strafeSpeed * transform.right + (moveZ > 0 ? moveZ * stat.moveSpeedForward : moveZ * stat.moveSpeedBackward) * transform.forward;
        controller.Move(moveDirection * Time.deltaTime);
        animator.SetFloat("Speed", moveZ);
        animator.SetFloat("Strafe", moveX);
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotation += -stat.rotateSpeed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotation += stat.rotateSpeed * Time.deltaTime;
        }
        if (Mathf.Abs(rotation) > 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, rotation, 0f), 15f);
        }
    }

    private void Attack() 
    {
        Transform target = null;
        if (Input.GetMouseButtonDown(0))
        {
            target = GetAttackObject();
            StartCoroutine(SlashAttack());
        } else if (Input.GetMouseButtonDown(1))
        {
            target = GetAttackObject();
            StartCoroutine(KickAttack());
        }
        if (target != null)
        {
            Debug.Log("Distance to target = " + Vector3.Distance(transform.position, target.position));
            transform.LookAt(target.position);
        }
    }

    private IEnumerator SlashAttack()
    {
        isAttack = true;
        animator.SetTrigger("Slash");
        yield return new WaitForSeconds(stat.slashTime);
        isAttack = false;
    }

    private IEnumerator KickAttack()
    {
        isAttack = true;
        animator.SetTrigger("Kick");
        yield return new WaitForSeconds(stat.kickTime);
        isAttack = false;
    }

    private Transform GetAttackObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 5.0f, enemyLayer))
        {
            Transform objectHit = hit.transform;
            Debug.Log("Попал в объект: " + objectHit.name);
            return objectHit;
        }
        return null;
    }
}
