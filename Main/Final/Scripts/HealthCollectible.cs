using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public GameObject pickupParticlesPrefab;

    public AudioClip collectedClip;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if(controller.health  < controller.maxHealth)
            {
                controller.ChangeHealth(1);
                GameObject pickupParticleObject = Instantiate(pickupParticlesPrefab, transform.position, Quaternion.identity);
                //ParticleSystem particleSystem = pickupParticleObject.GetComponent<ParticleSystem>();
                //particleSystem.Start();
                Destroy(gameObject);
                controller.PlaySound(collectedClip);
            }
        }
    }
}
