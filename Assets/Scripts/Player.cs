using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] int maxHealth = 100;
    public int currentHealth;

    [Header("Screen Padding")]
    [SerializeField] float xPadding = 0.5f;
//    [SerializeField] float yTopPadding = 10f;
//    [SerializeField] float yBottomPadding = 2f;

    [Header("SFX and VFX")]
    [SerializeField] AudioClip playerDeathSound;
    [SerializeField] [Range(0, 1)] float playerDeathSoundVolume = 0.75f;
    [SerializeField] AudioClip playerShootSound;
    [SerializeField] [Range(0, 1)] float playerShootSoundVolume = 0.1f;
    [SerializeField] GameObject explosionParticlesPrefab;
    [SerializeField] float durationOfExplosion = 2f;
    [SerializeField] GameObject hitParticlesPrefab;

    [Header("Player Firing")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float projectileSpeed = 25f;
    [SerializeField] float projectileFiringPeriod = 0.25f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
//    float yMin;
//    float yMax;

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        currentHealth -= damageDealer.GetDamage();
        damageDealer.Hit();
        GameObject hitParticles = Instantiate(hitParticlesPrefab, transform.position, transform.rotation);
        Destroy(hitParticles, 0.1f);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        AudioSource.PlayClipAtPoint(playerDeathSound, Camera.main.transform.position, playerDeathSoundVolume);
        GameObject explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, transform.rotation);
        Destroy(explosionParticles, durationOfExplosion);
        FindObjectOfType<Level>().LoadGameOver();
    }

    public int GetHealth()
    {
        return currentHealth;
    }
    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
            AudioSource.PlayClipAtPoint(playerShootSound, Camera.main.transform.position, playerShootSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
//        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
//        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, transform.position.y);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + xPadding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPadding;
//        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + yBottomPadding;
//        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yTopPadding;
    }
}
