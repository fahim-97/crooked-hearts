using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Movement")]
    public float movementSpeed;
    private Vector2 movement;
    public float knockbackAmount;
    public Transform door;

    [Header("Player Animation")]
    public Animator anim;
    public Animator heartAnim;
    public Animator cameraAnim;
    public int heartCount;
    public int maxHeartCount;
    public GameObject[] heartBar;
    public Animator notReady;
    public GameObject notReadyBox;

    [Header("Weapon System")]
    public int currentWeapon;
    public SpriteRenderer weaponSprite;

    [Header("Player Stats")]
    public float health;
    public float maxHealth;
    public GameObject bleedingEffect;

    [Header("Dialogue Elements")]
    public string storyline;
    public bool isTalking = false;
   
    [Header("Attack System")]
    public float timeBtwAttack;
    public float startTimeBtwAttack;
    public Transform attackPos;
    public float attackRadius;
    public LayerMask whatIsEnemies;
    public Animator animator;
    public TrailRenderer trail;
    public Transform lookAtEnemyObj;

    [Header("Component References")]
    public Rigidbody2D rb;
    public WeaponScript weaponScript;
    public AudioSource src;


    private void Awake()
    {
        //SINGLETON PATTERN
        int managerCount = FindObjectsOfType<PlayerScript>().Length;
        if (managerCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        weaponScript = GetComponent<WeaponScript>();
        rb = GetComponent<Rigidbody2D>();
        src = GetComponent<AudioSource>();
        health = maxHealth;
  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Heart"))
        {
            rb.bodyType = RigidbodyType2D.Static;
            collision.gameObject.GetComponent<DialogueTrigger>().TriggerDialogue();
            Destroy(collision.gameObject);
            heartCount += 1;
            heartAnim.SetTrigger("HeartBig");
            heartChangeBar();
        }
        else if (collision.CompareTag("door") && heartCount < maxHeartCount)
        {
            notReadyBox.SetActive(true);
            lookAtEnemyObj = collision.transform;
            StartCoroutine("knockbackRoutine");
            notReady.SetBool("done", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("door") && heartCount == maxHeartCount)
        {
            BossAttackScript boss = FindObjectOfType<BossAttackScript>();
            boss.StartBossBattle();
        }
    }

    void Update()
    {
        if (!isTalking)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            anim.SetFloat("Movement", (Mathf.Abs(movement.x + movement.y)));
            //check if alive
            if (health <= 0)
            {
                Die();
            }

            //move
            pickMovement();

            if (timeBtwAttack <= 0)
            {
                //then you attack
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Attack();
                    timeBtwAttack = startTimeBtwAttack;
                }
            }
            else
            {
                timeBtwAttack -= Time.deltaTime;
            }
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        //detect all the enemies hit
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, whatIsEnemies);

        if(enemiesToDamage.Length == 0)
        {
            //play Swoosh SFX
            src.PlayOneShot(weaponScript.weaponArray[currentWeapon].weaponSwooshSFX);
        }

        //damage them
        foreach(Collider2D enemy in enemiesToDamage)
        {
            if (enemy.GetComponent<EnemyScript>() != null)
            {
                //play Hit SFX
                src.PlayOneShot(weaponScript.weaponArray[currentWeapon].weaponHitSFX);
                enemy.GetComponent<EnemyScript>().TakeDamage(weaponScript.weaponArray[currentWeapon].weaponDamage);
            }
            else if (enemy.GetComponent<BossAttackScript>() != null)
            {
                //play Hit SFX
                src.PlayOneShot(weaponScript.weaponArray[currentWeapon].weaponHitSFX);
                enemy.GetComponent<BossAttackScript>().TakeDamage(weaponScript.weaponArray[currentWeapon].weaponDamage);
            }
        }
    }


    public void heartChangeBar()
    {
        for (int i = 0; i < heartCount; i++)
        {
            heartBar[i].SetActive(true);
        }
    }

    public void pickMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        //flip
        if (movement.x > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }

    private void FixedUpdate()
    {
        if (!isTalking)
        {
            rb.MovePosition(rb.position + movement * movementSpeed / (weaponScript.weaponArray[currentWeapon].weaponWeight) * Time.fixedDeltaTime);
        }  
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }


    public void WeaponUpgrade()
    {
        if (currentWeapon+1 < weaponScript.weaponArray.Length)
        {
            currentWeapon++;
            weaponSprite.sprite = weaponScript.weaponArray[currentWeapon].sprite;
        }
        //Debug.Log("Current Weapon: " + weaponScript.weaponArray[currentWeapon].weaponName);
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage, Transform enemyTransform)
    {
        Instantiate(bleedingEffect, transform.position, Quaternion.identity);
        health -= damage;
        lookAtEnemyObj.right = enemyTransform.position - transform.position;
        cameraAnim.SetTrigger("Shake");
        StartCoroutine("knockbackRoutine");
    }

    IEnumerator knockbackRoutine()
    {
        trail.emitting = true;
        transform.Translate((-lookAtEnemyObj.right) * knockbackAmount * Time.deltaTime);

        yield return new WaitForSeconds(10);
        trail.emitting = false;
        transform.Translate(Vector2.zero);
    }

    
}
