using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] int coinValue = 100;
    bool wasCollected = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().IncreaseScore(coinValue);
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, 1f);     
            gameObject.SetActive(false);
            Destroy(gameObject);
        }        
    }
}
