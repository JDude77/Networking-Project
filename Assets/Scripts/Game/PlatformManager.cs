using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField]
    private GameObject platformPrefab;
    private List<GameObject> activePlatforms;
    private Player player;
    private float timeToNextSpawn;
    private float maxSize = 14.0f;
    private float platformSize, yPosition;
    private int lastYIndex;
    private readonly float[] yPositions = new float[4] { 8.5f, 3.0f, -3.0f, -8.5f };
    private bool gameStarted = false;


    public List<GameObject> GetPlatforms() { return activePlatforms; }

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        activePlatforms = new List<GameObject>();

        // Initial top and bottom platforms for grace period at game start
        GameObject topPlatform = Instantiate(platformPrefab, new Vector3(0, 8.5f, 0), Quaternion.identity);
        topPlatform.GetComponent<PlatformScript>().ScalePlatform(24);
        activePlatforms.Add(topPlatform);

        GameObject bottomPlatform = Instantiate(platformPrefab, new Vector3(0, -8.5f, 0), Quaternion.identity);
        bottomPlatform.GetComponent<PlatformScript>().ScalePlatform(24);
        activePlatforms.Add(bottomPlatform);

        if (SceneManager.GetActiveScene().name.Equals("Game"))
        {
            gameStarted = true;

            timeToNextSpawn = 0.0f;
        }
        else
        {
            gameStarted = false;

            StopAllPlatforms();
        }
    }

    private void Update()
    {
        if (gameStarted)
        {
            if (!player.GetDead())
            {
                maxSize -= 1 / 5.0f * Time.deltaTime;

                DestroyOutOfBoundsPlatform();

                timeToNextSpawn -= Time.deltaTime;

                if (timeToNextSpawn <= 0)
                {
                    SpawnNewPlatform();
                }//End if
            }//End if
            else
            {
                StopAllPlatforms();
            }//End else
        }//End if
        ChangePlatformColours();
    }

    private void OnDrawGizmos()
    {
        if(gameStarted)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(16 + (platformSize * 2), yPosition), new Vector3(platformSize * 2 + 6, 2));
        }
    }

    private void SpawnNewPlatform()
    {
        int yIndex = lastYIndex;

        platformSize = Random.Range(0, maxSize);

        while(yIndex == lastYIndex) yIndex = Random.Range(0, yPositions.Length);
        yPosition = yPositions[yIndex];

        Collider2D[] colliders = Physics2D.OverlapBoxAll(new Vector2(16 + (platformSize * 2), yPosition), new Vector2(platformSize * 2 + 6, 2), 0);

        while (colliders.Length > 0 || yIndex == lastYIndex)
        {
            yIndex = Random.Range(0, yPositions.Length);
            yPosition = yPositions[yIndex];
            platformSize = Random.Range(0, maxSize);
            colliders = Physics2D.OverlapBoxAll(new Vector2(16 + (platformSize * 2), yPosition), new Vector2(platformSize * 2 + 6, 2), 0);
        }
        lastYIndex = yIndex;

        //Spawn new platform
        GameObject newPlatform = Instantiate(platformPrefab, new Vector3(16 + (platformSize * 2), yPosition, 0), Quaternion.identity);
        newPlatform.GetComponent<PlatformScript>().ScalePlatform(platformSize);
        activePlatforms.Add(newPlatform);

        timeToNextSpawn = (platformSize * 1.75f) / (newPlatform.GetComponent<PlatformScript>().GetVelocity() * 1.5f);
    }

    private void DestroyOutOfBoundsPlatform()
    {
        if (activePlatforms[0].transform.position.x < -20 - activePlatforms[0].GetComponent<PlatformScript>().GetScale())
        {
            GameObject platformToDelete = activePlatforms[0];
            activePlatforms.Remove(platformToDelete);
            Destroy(platformToDelete);
        }//End if
    }

    private void ChangePlatformColours()
    {
        if (!player.GetColliding())
        {
            foreach (GameObject platform in activePlatforms)
            {
                platform.GetComponent<PlatformScript>().ChangeColour(PlatformScript.COLOUR.YELLOW);
            }
        }
        else
        {
            if (player.GetGravityDirection() > 0)
            {
                foreach (GameObject platform in activePlatforms)
                {
                    platform.GetComponent<PlatformScript>().ChangeColour(PlatformScript.COLOUR.CYAN);
                }
            }
            else
            {
                foreach (GameObject platform in activePlatforms)
                {
                    platform.GetComponent<PlatformScript>().ChangeColour(PlatformScript.COLOUR.MAGENTA);
                }
            }
        }
    }

    private void StopAllPlatforms()
    {
        foreach (GameObject platform in activePlatforms)
        {
            platform.GetComponent<PlatformScript>().SetMoving(false);
        }
    }
}
