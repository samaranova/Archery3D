using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject hitEffect;
    public AudioSource targetAudio;

    void Start()
    {
        // Spawn the target in a random place on our map, with the target at a random angle
        transform.position = new Vector3(Random.Range(-21.5f, 22.5f), Random.Range(1.5f, 3.5f), Random.Range(-17.5f, 21.5f));
        transform.rotation = Quaternion.Euler(90.0f, Random.Range(0.0f, 360.0f), 0.0f);

        // Used whenever a target is hit
        targetAudio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check whether the target was hit by an arrow
        if (collision.gameObject.tag == "Arrow")
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            // Play a hit sound
            targetAudio.Play();

            // Decrease the number of targets we have left to the user
            var playerScore = GameObject.Find("PlayerScore").GetComponent(typeof(PlayerScore)) as PlayerScore;
            playerScore.DecreaseTargetCount();

            // Destroy the target 0.5s after being hit by arrow (enough time for target hit audioclip to play)
            Destroy(this.gameObject, 0.5f);
        }
    }
}
