using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public SkeletonInfo skeleton;
    public float attackDistance = 1f;

    private PlayerController player;
    private Animator animator;
    

    private Vector3 moveDirection = Vector3.zero;
    private float currentSpeed = 0f;

    private bool isAttacking = false;

    private bool isDead = false;
    private bool isHit = false;

    private float _currentHP;
    private Slider _hpSlider;
    private Canvas _canvas;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        SetupEnemy();
    }

    private void SetupEnemy()
    {
        _currentHP = skeleton.HP;
        _hpSlider = gameObject.GetComponentInChildren<Slider>();
        _canvas = gameObject.GetComponentInChildren<Canvas>();
        if (_hpSlider != null)
        {
            _hpSlider.maxValue = skeleton.HP;
            _hpSlider.value = skeleton.HP;
        }
    }

    public bool IsDead()
    {
        return isDead;
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
        if (isDead)
            return;
        moveDirection = MoveDirection();
        currentSpeed = Speed();
        if (!isAttacking)
        {
            transform.position += currentSpeed * Time.deltaTime * moveDirection;
        }
        if (_canvas != null && _currentHP > 0)
        {
            _canvas.gameObject.transform.LookAt(Camera.main.transform.position);
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
        transform.LookAt(new Vector3(player.transform.position.x, 0f, player.transform.position.z));
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
        else if (!isAttacking && !isDead && !isHit)
        {
            animator.SetFloat("Speed", skeleton.animationSpeed);
        }
    }

    public void GetDamage(int amount)
    {
        if (isDead) return;
        _currentHP -= amount;
        Debug.Log(gameObject.name + " HP: " + _currentHP);
        SetHpSlider(_currentHP);
        ResetState();
        if (_currentHP <= 0.01f)
        {
            StartCoroutine(Death());
        } else
        {
            StartCoroutine(Damage());
        }
    }

    private void ResetState()
    { 
        StopAllCoroutines();
        isAttacking = false;
        isHit = false;
    }

    private void SetHpSlider(float value)
    {
        if (_hpSlider == null) return;
        if (value <= 0)
            _hpSlider.gameObject.SetActive(false);
        _hpSlider.value = value;
    }

    private IEnumerator Damage()
    {
        isHit = true;
        animator.SetTrigger("Damage");
        yield return new WaitForSeconds(0.7f);
        isHit = false; 
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        SkeletonAttack attack = ChooseAttack();
        float attackDamage = GetAttackDamage(attack);
        animator.SetTrigger(attack.ToString());
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
        if (!isHit && Vector3.Distance(gameObject.transform.position, player.transform.position) < skeleton.agroRadius)
        {
            player.GetDamage(attackDamage);
        }
    }

    private float GetAttackDamage(SkeletonAttack attack)
    {
        switch (attack)
        {
            case SkeletonAttack.Punch:
                return 5f;
            case SkeletonAttack.Kick:
                return 7f;
            case SkeletonAttack.Slash:
                return 10f;
        }
        return 0f;
    }

    private IEnumerator Death()
    {
        isDead = true;
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(4.6f);
        GetComponent<Collider>().enabled = false;
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
