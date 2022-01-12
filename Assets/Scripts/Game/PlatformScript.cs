using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] pieces;
    private static COLOUR colour;
    [SerializeField]
    private Texture2D[] textures;
    private float velocity = 8.0f;
    private bool moving = true;
    private float scale;
    private Rigidbody2D platformBody;

    public float GetScale() { return scale; }

    public float GetVelocity() { return velocity; }

    public enum COLOUR
    {
        MAGENTA = 0,
        CYAN = 1,
        YELLOW = 2
    }

    public static COLOUR GetColour() { return colour; }

    public void SetMoving(bool moving) { this.moving = moving; }

    private void Awake()
    {
        platformBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) ScalePlatform(Random.Range(0, 15));
        if(moving)
        {
            platformBody.velocity = new Vector2(-velocity, 0);
        }
        else
        {
            platformBody.velocity = Vector2.zero;
        }
    }

    public void ScalePlatform(float scale)
    {
        this.scale = scale;

        //1 = Middle
        pieces[1].transform.localScale = new Vector3(scale * 100, 100, 100);
        //0 = Front
        pieces[0].transform.localPosition = new Vector3(1 * scale, 0, 0);
        //2 = End
        pieces[2].transform.localPosition = new Vector3(-1 * scale, 0, 0);

        //Scale the box collider
        GetComponent<BoxCollider2D>().size = new Vector2(scale * 2 + 3, 2);
    }

    public void ChangeColour(COLOUR newColour)
    {
        colour = newColour;

        for (int i = 0; i < 3; i++)
        {
            pieces[i].GetComponent<MeshRenderer>().material.mainTexture = textures[(i * 3) + ((int)colour)];
        }
    }
}
