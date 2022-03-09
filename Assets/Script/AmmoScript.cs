using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            Destroy(gameObject);
            other.GetComponent<PlayerShooting>().addAmmo(10);
        }
    }
}
