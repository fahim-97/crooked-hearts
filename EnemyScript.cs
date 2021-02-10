using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Movement")]
    public float enemyMovementSpeed;
    private Vector2 movement;
    public float knockbackAmount;
   

    [Header("Enemy Stats")]
    public float health;
    public float maxHealth = 100;
    public GameObject bleedingEffect;

    [Header("Weapon System")]
    public int currentWeapon;
    public SpriteRenderer weaponSprite;

    [Header("Component References")]
    public Rigidbody2D rb;
    public WeaponScript weaponScript;
    public PlayerScript player;
    public TrailRenderer trail;
    public Transform lookAtPlayerObj;
    private Transform playerTransform;
    

    void Start()
    {
        health = maxHealth;
        weaponScript = GetComponent<WeaponScript>();
        player = FindObjectOfType<PlayerScript>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        lookTowardsPlayer();

  
        if (health <= 0)
        {
            Die();
        }

        if (player.currentWeapon != this.currentWeapon)
        {
            this.currentWeapon = player.currentWeapon;
            weaponSprite.sprite = weaponScript.weaponArray[currentWeapon].sprite;
        }
    }

    private void Die()
    {
        player.WeaponUpgrade();
        Destroy(gameObject);
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
        lookAtPlayerObj.right = playerTransform.position - transform.position;
    }
}
