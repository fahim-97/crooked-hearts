using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackScript : MonoBehaviour
{
    [Header("Enemy Movement")]
    public float speed;
    public float knockbackAmount;
    public float socialDistance;
    public TrailRenderer trail;
    public Transform lookAtPlayerObj;

    [Header("Enemy Stats")]
    public float health;
    public float maxHealth;
    public GameObject bleedingEffect;
    public float weaponDamage;
    public GameObject ChoicePanel;
    public Animator choiceAnim;
    //crucial bool
    public bool savedCrookedOne = false;
    private Transform player;

  
    [Header("Detection and Attack")]
    public Animator animator;
    private float timeBtwChase;
    public float startTimeBtwChase;
    private float timeBtwAttack;
    public float startTimeBtwAttack;
    public DialogueTrigger finalDialogue;
    public DialogueTrigger gameoverDialogue;
    private bool startBattle = false;
    private bool firstBossDialogueEnd = false;

    [Header("Audio")]
    public AudioSource src;
    public AudioClip bossWeaponSFX;


    void Start()
    {
        health = maxHealth;
        player = FindObjectOfType<PlayerScript>().GetComponent<Transform>();

        src = GetComponent<AudioSource>();
    }

    public void StartBossBattle()
    {
        if (!startBattle)
        {
            startBattle = true;
        }
    }

    void Update()
    {
        lookTowardsPlayer();
        if (health <= 0)
        {
            Die();
        }

        if (startBattle && !FindObjectOfType<PlayerScript>().isTalking && !ChoicePanel.activeInHierarchy)
        {
            //chase
            if (Vector2.Distance(transform.position, player.position) > socialDistance && timeBtwChase <= 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, player.position) < socialDistance + 2f && !firstBossDialogueEnd) //only once
                {
                    GetComponent<DialogueTrigger>().TriggerDialogue();
                    firstBossDialogueEnd = true;
                }
            }
            else if (Vector2.Distance(transform.position, player.position) < socialDistance)
            {
                if (timeBtwAttack <= 0 && !ChoicePanel.activeInHierarchy)
                {
                    //then you attack
                    animator.SetTrigger("Attack");
                    //play Hit SFX
                    src.PlayOneShot(bossWeaponSFX);
                    player.GetComponent<PlayerScript>().TakeDamage(weaponDamage, this.transform);
                    timeBtwAttack = startTimeBtwAttack;
                    timeBtwChase = startTimeBtwChase;
                }
                else
                {
                    timeBtwAttack -= Time.deltaTime;
                }
            }

            timeBtwChase -= Time.deltaTime;
        }
        
    }

    private void Die()
    {
        ChoicePanel.SetActive(true);
    }

    public void SaveCrookedOne()
    {
        savedCrookedOne = true;
        choiceAnim.SetBool("doneChoosing", true);
        ChoicePanel.SetActive(true);
        finalDialogue.TriggerDialogue();
    }

    public void KillCrookedOne()
    {
        savedCrookedOne = false;
        choiceAnim.SetBool("doneChoosing", true);
        ChoicePanel.SetActive(true);
        gameoverDialogue.TriggerDialogue();
    }

    public void TakeDamage(float damage)
    {
        Instantiate(bleedingEffect, transform.position, Quaternion.identity);
        health -= damage;
        StartCoroutine("knockbackRoutine");

    }

    IEnumerator knockbackRoutine()
    {
        trail.emitting = true;
        transform.Translate((-lookAtPlayerObj.right) * knockbackAmount * Time.deltaTime);

        yield return new WaitForSeconds(10);
        trail.emitting = false;
        transform.Translate(Vector2.zero);

    }

    public void lookTowardsPlayer()
    {
        lookAtPlayerObj.right = player.position - transform.position;
    }
}
