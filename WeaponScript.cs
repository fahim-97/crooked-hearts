using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WeaponScript : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string weaponName;
        public Sprite sprite;
        public int weaponDamage;
        public int type; //0 ranged 1 melee
        public int weaponWeight;

        public AudioClip weaponHitSFX;
        public AudioClip weaponDrawnSFX;
        public AudioClip weaponBlockSFX;
        public AudioClip weaponSwooshSFX;
    }

    [Header("Weapon System")]
    public Weapon[] weaponArray;
    public SpriteRenderer weaponGFX;
    public int weaponIndex;

    // Start is called before the first frame update
    void Start()
    {
        weaponGFX.sprite = weaponArray[weaponIndex].sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
