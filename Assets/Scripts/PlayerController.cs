using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("Ground Plane")]
    public Transform planeOrigin;       
    public Vector3 planeNormal = Vector3.up;
    private Plane plane;

    [Header("Player options")]
    public float moveSpeed = 3f;
    public float slideSpeed = 3f;
    public float rotateSpeed = 106f;

    private float rotation = 0f;

    private bool inAttack = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        plane = new Plane(planeNormal, planeOrigin.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (inAttack)
        {
            return;
        }
        MovePlayer();
        RotatePlayer();

        if (Input.GetMouseButtonDown(0))
        {
            inAttack = true;
            StartCoroutine(Attack());
        }
    }

    private void MovePlayer()
    {
        float walk = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");
        animator.SetFloat("walk", walk);
        animator.SetFloat("strafe", strafe);
        if (walk != 0 || strafe != 0)
        {
            Vector3 move = transform.forward * moveSpeed * walk * Time.deltaTime +
                transform.right * slideSpeed * strafe * Time.deltaTime;
            controller.Move(move);
        }
    }

    private void RotatePlayer()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotation += -rotateSpeed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotation += rotateSpeed * Time.deltaTime;
        }
        if (Mathf.Abs(rotation) > 0.01f)
        {
            animator.SetFloat("rotate", rotation);
            rotation += rotation < 0 ? rotateSpeed * Time.deltaTime : -rotateSpeed * Time.deltaTime;
        } 
        else
        {
            animator.SetFloat("rotate", 0f);
        }
    }

    private IEnumerator Attack()
    {
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(1f);
        inAttack = false;
    }

    private void RotatePlayerByMouse()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                Vector3 newDir = Vector3.RotateTowards(transform.forward, (hitPoint - transform.position), rotateSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }


}
