using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Slider HPSlider;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject YouWinPanel;

    public PlayerInfo stat;

    private CharacterController controller;
    private Animator animator;

    private float _rotationY = 0f;
    private float _verticalVelocity = 0f;
    private bool isAttack = false;
    private bool isHit = false;

    private float _hp;
    private bool isDead = false;

    private SpawnSkeleton spawner;
    private LayerMask enemyLayer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        spawner = FindFirstObjectByType<SpawnSkeleton>();
        enemyLayer = LayerMask.GetMask("Enemy");
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        isDead = false;
        isAttack = false;
        isHit = false;
        _hp = stat.maxHP;
        HPSlider.maxValue = stat.maxHP;
        GameOverPanel.SetActive(false);
        YouWinPanel.SetActive(false);
        UpdateHPValues();
    }

    public void Move(Vector2 moveVector)
    {
        if (!isAttack && !isHit && !isDead)
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
            }
        }
    }

    public void Rotate(Vector2 rotateVector)
    {
        if (!isAttack && !isHit && !isDead)
        {
            _rotationY += rotateVector.x * stat.rotateSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, _rotationY, 0f);
        }
    }

    public void Jump()
    {
        if (!isAttack && !isHit && !isDead && controller.isGrounded)
        {
            _verticalVelocity = stat.jumpForce;
            animator.SetTrigger("Jump");
        }
    }

    public void Attack(string name)
    {
        if (isAttack || isHit || isDead)
            return;
        Enemy[] targets = GetAttackObjects();
        if (name.Equals("Attack"))
        {
            StartCoroutine(SlashAttack());
            if (targets != null)
            {
                foreach (Enemy target in targets)
                {
                    target.GetDamage(stat.slashDamage);
                }
            }
        }
        else if (name.Equals("Kick"))
        {
            StartCoroutine(KickAttack());
            if (targets != null)
            {
                Enemy target = targets[0];
                target.GetDamage(stat.kickDamage);
            }
        }
        else
            return;
        
    }

    public void GetDamage(float amount)
    {
        _hp -= amount;
        UpdateHPValues();
        if (_hp <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(GameOver());
        } 
        else StartCoroutine(Damage());
    }

    private IEnumerator GameOver()
    {
        isDead = true;
        animator.SetTrigger("Death");
        foreach (GameObject item in spawner.GetSkeletonsList())
        {
            item.GetComponent<Enemy>().StopAllCoroutines();
            item.GetComponent<Animator>().StopPlayback();
        }
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0.0f;
        GameOverPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {
        Time.timeScale = 0.0f;
        YouWinPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator Damage()
    {
        isHit = true;
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(0.7f);
        isHit = false;
    }

    private void UpdateHPValues()
    {
        _hp = Mathf.Clamp(_hp, 0f, stat.maxHP);
        HPSlider.value = _hp;
        HPText.text = _hp.ToString();
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

    private Enemy[] GetAttackObjects()
    {
        int count = 0;
        Vector3 lookVector = transform.forward;
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, 0.3f, lookVector, 1.0f, enemyLayer);
        Enemy[] enemies = hits != null ? new Enemy[hits.Length] : null;
        foreach (RaycastHit hit in hits) 
        {
            Enemy enemy;
            if (hit.collider.TryGetComponent<Enemy>(out enemy))
            {
                enemies[count++] = enemy;
            }
        }
        return count > 0 ? enemies : null;
    }
}
