using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public float health = 100;

    // Sets it to use children as fragments
    [SerializeField]
    bool useChildrenAsFragments = false;
    [SerializeField]
    float childExplodeForce = 12;

    //Generate Cube Data
    [SerializeField]
    bool generateCubeFragments;
    [SerializeField]
    int numberOfCubeFragments;
    [SerializeField]
    float cubeExplodeForce = 6;
    [SerializeField]
    float cubeFragementSize = 1;

    // Spped fragements shrink
    [SerializeField]
    float fragementsShrinkSpeed = 7.5f;

    //Shrink on Kill Data
    [SerializeField]
    bool shrinkOnKill;
    [SerializeField]
    float shrinkSpeed = 7.5f;

    public AudioSource aud;

    public bool shrinkOnHit;
    public float shrinkFactor = 0.98f;


    public AudioClip hit1;
    public AudioClip hit2;
    public AudioClip hit3;
    public AudioClip explode;

    Rigidbody objectRB;
    Rigidbody childRD;

    int hitSound;

    bool canCollide = true;

    private void Awake()
    {
        objectRB = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canCollide)
        {
            canCollide = false;
            StartCoroutine(stopMultipleCollisionSounds());
            if (health > 0)
            {
                hitSound = (int)Random.Range(0, 3);

                switch (hitSound)
                {
                    case 0:
                        aud.PlayOneShot(hit1);
                        break;
                    case 1:
                        aud.PlayOneShot(hit2);
                        break;
                    case 2:
                        aud.PlayOneShot(hit3);
                        break;
                    default:
                        aud.PlayOneShot(hit3);
                        break;
                }
            }

            if (collision.gameObject.CompareTag("Bullet"))
            {
                health -= collision.gameObject.GetComponent<BulletScript>().damage;
                if (shrinkOnHit)
                {
                    transform.localScale *= shrinkFactor;
                }

                if (health <= 0)
                {
                    death(collision);
                }
            }
        }
    }

    IEnumerator stopMultipleCollisionSounds()
    {
        yield return null;
        canCollide = true;
    }

    void death(Collision collision)
    {
        //Play Sound
        aud.PlayOneShot(explode);
        
        // Stop Physics on Object
        if (objectRB != null && useChildrenAsFragments) Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(gameObject.GetComponent<Collider>());

        //Make Object Shrink if set
        if(shrinkOnKill && !useChildrenAsFragments) gameObject.AddComponent<ShrinkToDestroy>().speed = shrinkSpeed;

        // Code if set to use children as fragments
        if (useChildrenAsFragments)
        {
            foreach (Transform child in transform)
            {
                child.transform.SetParent(null);
                child.gameObject.AddComponent<ShrinkToDestroy>().speed = fragementsShrinkSpeed;
                childRD = child.gameObject.AddComponent<Rigidbody>();
                childRD.AddForce(collision.contacts[0].normal * childExplodeForce, ForceMode.Impulse);
                childRD.AddExplosionForce(childExplodeForce * 5, transform.position, 2, 10);
            }
            Destroy(gameObject);
        }


        // Code if set to generate cube fragments
        if (generateCubeFragments)
        {
            for (int i = 0; i < numberOfCubeFragments; i++)
            {
                // Create fragment
                GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // Randomly scale fragment
                part.transform.localScale = (Vector3.one * cubeFragementSize) * Random.Range(0.2f, 1.2f);
                part.layer = 6;
                part.transform.position = transform.position + (Vector3.one * Random.Range(0.05f, 1f));
                part.AddComponent<ShrinkToDestroy>().speed = fragementsShrinkSpeed;
                Rigidbody partRB = part.AddComponent<Rigidbody>();
                partRB.AddForce(collision.contacts[0].normal * cubeExplodeForce, ForceMode.Impulse);
                partRB.AddExplosionForce(1, transform.position, 2);
            }
        }
    }
}
