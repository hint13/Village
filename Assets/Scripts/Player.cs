using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Main stats")]
    public int maxHP = 100;
    public int slashDamage = 20;
    public int kickDamage = 15;

    [Header("Movement speed")]
    public float moveSpeedForward = 3f;
    public float moveSpeedBackward = 2f;
    public float strafeSpeed = 1f;
    public float jumpForce = 5f;

    [Header("Animations timing")]
    public float slashTime = 1.5f;
    public float kickTime = 1.2f;
    public float jumpTime = 0.967f;
    public float hitTime = 0.967f;
    public float deadTime = 2.3f;

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
        moveDirection = new Vector3(moveX * strafeSpeed, 0, moveZ > 0 ? moveZ * moveSpeedForward : moveZ * moveSpeedBackward);
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
        yield return new WaitForSeconds(slashTime);
        isAttack = false;
    }

    private IEnumerator KickAttack()
    {
        isAttack = true;
        animator.SetTrigger("Kick");
        yield return new WaitForSeconds(kickTime);
        isAttack = false;
    }

}
