using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public PlayerInfo stat;

    private CharacterController controller;
    private Animator animator;

    private float _rotationY = 0f;
    private float _verticalVelocity = 0f;
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

    public void Move(Vector2 moveVector)
    {
        if (!isAttack)
        {
            Vector3 move = transform.forward * moveVector.y * (moveVector.y > 0 ? stat.moveSpeedForward : stat.moveSpeedBackward) + 
                transform.right * moveVector.x * stat.strafeSpeed;
            controller.Move(move * Time.deltaTime);
            animator.SetFloat("Speed", moveVector.y);
            animator.SetFloat("Strafe", moveVector.x);

            _verticalVelocity += stat.gravity * Time.deltaTime;
            if (_verticalVelocity > 0)
            {
                controller.Move(new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
                animator.SetTrigger("Jump");
            }
        }
    }

    public void Rotate(Vector2 rotateVector)
    {
        if (!isAttack)
        {
            _rotationY += rotateVector.x * stat.rotateSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, _rotationY, 0f);
        }
    }

    public void Jump()
    {
        if (!isAttack && controller.isGrounded)
        {
            _verticalVelocity = stat.jumpForce;
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
