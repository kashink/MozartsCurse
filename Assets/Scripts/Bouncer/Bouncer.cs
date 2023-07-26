using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] float bounceVelocity = 0.5f;
    [SerializeField] float bounceCharge = 0;
    [SerializeField] float maxBounceCharge;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color color0;
    [SerializeField] Color color1;
    [SerializeField] Color color2;
    [SerializeField] Color color3;

    public AudioClip keySound;
    bool onTheFloor = true;

    void Start()
    {
        bounceCharge = Random.Range(0f, maxBounceCharge*0.5f);
    }

    void Update()
    {
        if (bounceCharge < maxBounceCharge)
        {
            bounceCharge += 1 * Time.deltaTime; 
        } else if (bounceCharge >= maxBounceCharge)
        {
            rigidbody2D.velocity = new Vector2(0, bounceVelocity);
                bounceCharge = Random.Range(0f,maxBounceCharge*0.5f);
        }
        if (bounceCharge < maxBounceCharge*0.25)
        {
            spriteRenderer.color = color0;
        } else if (bounceCharge < maxBounceCharge*0.5)
        {
            spriteRenderer.color = Color.Lerp(color0, color1,(bounceCharge-(maxBounceCharge*0.25f))*0.5f);
        } else if (bounceCharge < maxBounceCharge*0.75)
        {
            spriteRenderer.color = Color.Lerp(color1, color2, (bounceCharge -(maxBounceCharge*0.5f)) * 0.5f);
        } else if (bounceCharge <= maxBounceCharge)
        {
            spriteRenderer.color = Color.Lerp(color2, color3, (bounceCharge -(maxBounceCharge*0.75f)) * 0.5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
        {
            GetComponent<AudioSource>().clip = this.keySound;
            GetComponent<AudioSource>().Play();
            return;
        }

        if (collision.gameObject.tag == "Floor")
        {
            this.onTheFloor = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            this.onTheFloor = false;
        }
    }
}
