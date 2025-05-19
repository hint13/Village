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

    private bool isAttack = false;
    private SpawnSkeleton spawner;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        spawner = FindFirstObjectByType<SpawnSkeleton>();
    }

    private void FixedUpdate()
    {
        if (isAttack)
            return;
        Vector3 mouse = Input.mousePosition;
        //Debug.Log("Mouse position: " + mouse);
    }

    void Update()
    {
        if (!isAttack)
        {
            Move();
            Attack();
        }
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveDirection = new Vector3(moveX * stat.strafeSpeed, 0, moveZ > 0 ? moveZ * stat.moveSpeedForward : moveZ * stat.moveSpeedBackward);
        controller.Move(moveDirection * Time.deltaTime);
        animator.SetFloat("Speed", moveZ);
        animator.SetFloat("Strafe", moveX);
    }

    private void Attack() 
    {
        Vector3 target = Vector3.zero;
        if (Input.GetMouseButtonDown(0))
        {
            target = GetAttackPoint();
            StartCoroutine(SlashAttack());
        } else if (Input.GetMouseButtonDown(1))
        {
            target = GetAttackPoint();
            StartCoroutine(KickAttack());
        }
        if (!target.Equals(Vector3.zero))
        {
            transform.rotation.SetLookRotation(target);
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

    private Vector3 GetAttackPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = LayerMask.GetMask("Enemy");
        RaycastHit hit;
        // Проверяем, пересекается ли луч с чем-либо на сцене  
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            // Если пересечение есть, получаем информацию о нём  
            Transform objectHit = hit.transform;
            Debug.Log("Попал в объект: " + objectHit.name);
            return objectHit.position;
        }
        return Vector3.zero;
    }
}
