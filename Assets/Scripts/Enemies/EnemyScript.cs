using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject gameObject;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] float gravMax = 1.3f;
    [SerializeField] float gravMin = 0.7f;
    [SerializeField] float moveTimer = 0;
    [SerializeField] float chargeTimeMax = 1.8f;
    [SerializeField] bool onGroundCheck;
    private float directionX;
    public EnemyManager enemyManager;

    void Start()
    {
        rigidbody2D.gravityScale = Random.Range(gravMin, gravMax);
        playerScript = FindAnyObjectByType<PlayerScript>();
    }

    void Update()
    {
        if (boxCollider2D.GetContacts(new ContactPoint2D[1]) > 0)
        {
            if (moveTimer < chargeTimeMax)
            {
                moveTimer += 1 * Time.deltaTime;
            }
            else if (moveTimer > chargeTimeMax)
            {
                moveTimer = 0;
                directionX = playerScript.gameObject.transform.position.x - gameObject.transform.position.x;
                if (directionX > 0)
                {
                    rigidbody2D.velocity = new Vector2(5, 1f);
                }
                else
                {
                    rigidbody2D.velocity = new Vector2(-5, 1f);
                }

            }
        }
    }

    public void Kill()
    {
        enemyManager.KillEnemy(this);
    }
}
