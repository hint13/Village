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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
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
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SlashAttack());
        } else if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(KickAttack());
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

}
