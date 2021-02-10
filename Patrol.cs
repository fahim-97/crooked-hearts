using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public Transform[] moveSpots;
    private int randomSpot;
    private Rigidbody2D rb;

    [Header("Detection and Attack")]
    public WeaponScript weaponScript;
    public int currentWeapon;
    public float detectionRadius;
    public LayerMask playerMask;
    public Animator animator;
    public Transform attackPos;
    public float startTimeBtwAttack;
    private float timeBtwAttack;

    [Header("Patrol")]
    public float socialDistance;
    public float startWaitTime;
    private float waitTime;
    private PlayerScript playerStat;
    //private float someScale;
    //private float posX;
    //private int direction;

    [Header("Audio")]
    public AudioSource src;

    void Start()
    {
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, moveSpots.Length);
        playerStat = FindObjectOfType<PlayerScript>();
        weaponScript = GetComponent<WeaponScript>();
        rb = GetComponent<Rigidbody2D>();
        currentWeapon = GetComponent<EnemyScript>().currentWeapon;
        src = GetComponent<AudioSource>();

        //someScale = transform.localScale.x;
        //posX = transform.position.x;
        //direction = 1; //right
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, detectionRadius);
    }

    void Update()
    {
        Flip();

        if (playerStat.isTalking)
        {
            FreezeEverything();
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            //Detection AI
            Collider2D player = Physics2D.OverlapCircle(attackPos.position, detectionRadius, playerMask);
            if (player != null)
            {

                Transform target = player.GetComponent<Transform>();
                if (Vector2.Distance(transform.position, target.position) > socialDistance)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                }
                else
                {
                    if (timeBtwAttack <= 0)
                    {
                        //then you attack
                        animator.SetTrigger("Attack");
                        //play Hit SFX
                        src.PlayOneShot(weaponScript.weaponArray[currentWeapon].weaponHitSFX);
                        player.GetComponent<PlayerScript>().TakeDamage(weaponScript.weaponArray[currentWeapon].weaponDamage, this.transform);
                        timeBtwAttack = startTimeBtwAttack;
                    }
                    else
                    {
                        timeBtwAttack -= Time.deltaTime;
                    }
                }
            }
            else
            {
                //patrol AI
                transform.position = Vector2.MoveTowards(transform.position, moveSpots[randomSpot].position, speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
                {
                    if (waitTime <= 0)
                    {
                        randomSpot = Random.Range(0, moveSpots.Length);
                        waitTime = startWaitTime;
                    }
                    else
                    {
                        waitTime -= Time.deltaTime;
                    }
                }
            }
        }
        
    }

    private void Flip()
    {
        if (transform.position.x < playerStat.GetComponent<Transform>().position.x)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        //if (transform.position.x < posX && direction == 1) //facing left
        //{
        //    transform.localScale = new Vector2(-someScale, transform.localScale.y);
        //    direction = -1;
        //}
        //else if (transform.position.x > posX && direction == -1) //facing right
        //{
        //    transform.localScale = new Vector2(someScale, transform.localScale.y);
        //    direction = 1;
        //}

        //posX = transform.position.x;
    }

    private void FreezeEverything()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }
}
