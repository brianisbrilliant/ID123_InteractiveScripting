using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Rigidbody bulletPrefab;
    public Transform shootPoint;
    public float bulletForce;
    public AudioClip fire;
    public AudioClip fire1;
    public AudioClip fire2;
    public AudioClip EmptyAmmo;
    public AudioClip AddAmmo;


    bool canShoot = true;
    bool reloadSound = true;
    int shotSound;
    public int totalAmmo = 10;
    public int maxAmmo = 20;
    Rigidbody playRB;

    private AudioSource aud;

    private void Awake()
    {
        aud = gameObject.GetComponent<AudioSource>();
        playRB = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Check for keyboard
        Keyboard keyboard = Keyboard.current;

        //If there is a keyboard check for input
        if (keyboard != null)
        {
            // Check if fire key are pressed
            if (keyboard.numpad0Key.isPressed && !keyboard.leftArrowKey.isPressed)
            {
                if (canShoot)
                {
                    if (totalAmmo > 0)
                    {
                        Shoot();
                    }
                    else
                    {
                        if (reloadSound)
                        {
                            StartCoroutine(pauseReloadSound());
                        }
                    }
                }
            }
        }
    }

    public void addAmmo(int amount)
    {
        aud.PlayOneShot(AddAmmo);
        totalAmmo += amount;
        if(totalAmmo > maxAmmo)
        {
            totalAmmo = maxAmmo;
        }
    }

    void Shoot()
    {
        // Automatically Set Can Shoot to false to prevent doubleshot
        canShoot = false;

        //Reduce Ammo
        totalAmmo--;

        //Randomize Sound
        shotSound = (int)Random.Range(0, 3);

        switch(shotSound)
        {
            case 0:
                aud.PlayOneShot(fire);
                break;
            case 1:
                aud.PlayOneShot(fire1);
                break;
            case 2:
                aud.PlayOneShot(fire2);
                break;
            default:
                aud.PlayOneShot(fire);
                break;
        }

        if (aud.pitch < 3)
        {
            aud.pitch *= 1.25f;
        }
        else
        {
            aud.pitch = 1;
        }

        //Create a bullet
        Rigidbody bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        bulletScript.shotBy = transform;
        //Move the 
        Vector3 shootDir = shootPoint.forward * (bullet.velocity.magnitude + 1);
        shootDir.y = 0.25f;


        bullet.AddForce(shootDir * Random.Range(bulletForce * 0.8f, bulletForce * 1.2f), ForceMode.Impulse);

        //Set Cooldown
        StartCoroutine(ShootCooldown());
    }

    IEnumerator pauseReloadSound()
    {
        reloadSound = false;
        aud.PlayOneShot(EmptyAmmo);
        yield return new WaitForSeconds(0.1f);
        reloadSound = true;
    }


    IEnumerator ShootCooldown()
    {
        //Wait for time, the set canshoot to true
        yield return new WaitForSeconds(0.1f);
        canShoot = true;
    }

}
