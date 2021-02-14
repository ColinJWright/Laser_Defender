using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMultiShoot : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float health = 10f;
    [SerializeField] int scoreValue = 150;

    [Header("Enemy Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3f;
    [SerializeField] GameObject enemyLaserPrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] AudioClip enemyShootSound;
    [SerializeField] [Range(0, 1)] float enemyShootSoundVolume = 0.25f;

    [Header("Enemy Death")]
    [SerializeField] GameObject explosionParticlesPrefab;
    [SerializeField] float durationOfExplosion = 1f;
    [SerializeField] AudioClip enemyDeathSound;
    [SerializeField] [Range(0, 1)] float enemyDeathSoundVolume = 0.75f;


    // Start is called before the first frame update
    void Start()
    {
        shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(-3, -projectileSpeed);
        GameObject laser2 = Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser2.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -projectileSpeed);
        GameObject laser3 = Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser3.GetComponent<Rigidbody2D>().velocity = new Vector2(3, -projectileSpeed);
        AudioSource.PlayClipAtPoint(enemyShootSound, Camera.main.transform.position, enemyShootSoundVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<GameSession>().AddToScore(scoreValue);
        Destroy(gameObject);
        GameObject explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, transform.rotation);
        Destroy(explosionParticles, durationOfExplosion);
        AudioSource.PlayClipAtPoint(enemyDeathSound, Camera.main.transform.position, enemyDeathSoundVolume);
    }
}