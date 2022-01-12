using UnityEngine;

public class Player : MonoBehaviour
{
    private bool colliding = false;
    private int gravityDirection = -1;
    private float gravityForce = 75.0f;
    private Rigidbody2D playerRigidbody;
    private bool dead = false;

    [SerializeField]
    private Transform meshTransform;

    private Vector3 crossRotation = new Vector3(90, 0, 0);
    private Vector3 plusRotation = new Vector3(0, 0, 0);
    private Vector3 minusRotation = new Vector3(0, 90, 0);

    public int GetGravityDirection() { return gravityDirection; }
    public bool GetColliding() { return colliding; }
    public bool GetDead() { return dead; }
    public int GetFaceShowing()
    {
        if (!colliding) return 2;
        else if (colliding && gravityDirection == -1) return 1;
        else return 0;
    }

    private void Awake()
    {
        playerRigidbody = GetComponentInChildren<Rigidbody2D>();
        meshTransform.eulerAngles = crossRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        colliding = true;

        if(collision.transform.position.y - transform.position.y < 0)
        {
            meshTransform.eulerAngles = minusRotation;
        }
        else
        {
            meshTransform.eulerAngles = plusRotation;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        colliding = true;

        if (collision.transform.position.y - transform.position.y < 0)
        {
            meshTransform.eulerAngles = minusRotation;
        }
        else
        {
            meshTransform.eulerAngles = plusRotation;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        colliding = false;

        meshTransform.eulerAngles = crossRotation;
    }

    private void Update()
    {
        if(!dead)
        {
            if (Input.GetKeyDown(KeyCode.Space) && colliding)
            {
                gravityDirection *= -1;
            }

            playerRigidbody.AddForce(new Vector2(0.0f, gravityDirection * gravityForce));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Entering trigger = death
        if(collision.CompareTag("Death Zone") || collision.CompareTag("Platform"))
        {
            playerRigidbody.Sleep();
            dead = true;
        }
    }
}
