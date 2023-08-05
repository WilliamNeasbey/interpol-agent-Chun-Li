using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    //AudioSource audioData; 
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 1;
    public AudioClip audioClip;
   // public CountDownTimer countDownTimer;


    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        //audioData = GetComponent<AudioSource>();
        // audioData.Play(0);
        //Debug.Log("started");
        AudioSource.PlayClipAtPoint(audioClip, transform.position, volume);
       // FindObjectOfType<CountDownTimer>().AddTime();



    }
}
