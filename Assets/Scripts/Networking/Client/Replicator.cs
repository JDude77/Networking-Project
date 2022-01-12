using UnityEngine;
using NetworkingUtility;
using System.Collections.Generic;

public class Replicator : MonoBehaviour
{
    private Client client;

    [SerializeField]
    private GameObject playerReplicaPrefab, platformReplicaPrefab;
    [SerializeField]
    private Texture2D[] platformTextures;

    private GameObject playerObject;
    private Transform meshTransform;
    private PlayerData playerData;

    private Vector3 crossRotation = new Vector3(90, 0, 0);
    private Vector3 plusRotation = new Vector3(0, 0, 0);
    private Vector3 minusRotation = new Vector3(0, 90, 0);

    private Dictionary<int, GameObject> platformObjects = new Dictionary<int, GameObject>();
    private Dictionary<int, PlatformData> platformData;

    private void Awake()
    {
        //Retreive the client object
        client = FindObjectOfType<Client>();

        //Get the data from the client object
        playerData = client.GetPlayerData();
        platformData = client.GetPlatformsData();

        //Create the player
        playerObject = Instantiate(playerReplicaPrefab);
        meshTransform = playerObject.GetComponentInChildren<MeshRenderer>().gameObject.transform;
        //Player X and Z-position values never change once set, not needed in playerData
        playerObject.transform.position = new Vector3(-7.5f, playerData.yPosition, 0.0f);
        meshTransform.eulerAngles = crossRotation;

        //Create the platforms
        foreach(PlatformData platform in platformData.Values)
        {
            CreatePlatformFromData(platform);
        }
    }

    private void Update()
    {
        //Grab new data from client
        playerData = client.GetPlayerData();
        platformData = client.GetPlatformsData();

        //Update the player
        //- Check if dead and stop everything if so
        if(playerData.dead)
        {
            return;
        }
        //- Check if colliding
        //- Update transform
        //- Change colour
        if (playerData.colliding)
        {
            playerObject.transform.position = new Vector3(playerObject.transform.position.x, playerData.yPosition, 0);
            switch(playerData.gravityDirection)
            {
                case 1:
                    ChangeColour(1);
                    break;

                case -1:
                    ChangeColour(0);
                    break;
            }
        }
        else
        {
            ChangeColour(2);
            playerObject.transform.position = new Vector3(playerObject.transform.position.x, playerData.yPosition + playerData.yVelocity, 0);
        }


        //Update platforms

        //- Cull platforms not needed
        foreach(KeyValuePair<int, PlatformData> entry in platformData)
        {
            bool found = false;
            foreach(KeyValuePair<int, GameObject> gameObj in platformObjects)
            {
                if (entry.Key == gameObj.Key)
                {
                    found = true;

                    gameObj.Value.transform.position = new Vector3(entry.Value.xPosition, entry.Value.yPosition, 0.0f);

                    break;
                }
            }
            if(!found && entry.Value.xPosition < 0)
            {
                Destroy(platformObjects[entry.Key]);
                platformObjects.Remove(entry.Key);
            }
            else if(!found && entry.Value.xPosition >= 0)
            {
                CreatePlatformFromData(entry.Value);
            }
        }
        //- Update transforms of existing platforms
        //- Add new platforms
    }

    private void CreatePlatformFromData(PlatformData platformData)
    {
        GameObject newPlatform = Instantiate(platformReplicaPrefab);
        newPlatform.transform.position = new Vector3(platformData.xPosition, platformData.yPosition, 0.0f);
        ScalePlatform(ref newPlatform, platformData.xScale);
        platformObjects.Add(platformData.platformID, newPlatform);
    }

    public void ScalePlatform(ref GameObject platform, float scale)
    {
        MeshRenderer[] renderers = platform.GetComponentsInChildren<MeshRenderer>();

        for(int i = 0; i < renderers.Length; i++)
        {
            switch(renderers[i].gameObject.tag)
            {
                case "Middle":
                    renderers[i].transform.localScale = new Vector3(scale * 100, 100, 100);
                    break;

                case "Front":
                    renderers[i].transform.localPosition = new Vector3(1 * scale, 0, 0);
                    break;

                case "End":
                    renderers[i].transform.localPosition = new Vector3(-1 * scale, 0, 0);
                    break;
            }
        }

        //Scale the box collider
        platform.GetComponent<BoxCollider2D>().size = new Vector2(scale * 2 + 3, 2);
    }

    public void ChangeColour(int newColour)
    {
        switch(newColour)
        {
            case 0:
                meshTransform.eulerAngles = plusRotation;
                newColour = 1;
                break;
            case 1:
                meshTransform.eulerAngles = minusRotation;
                newColour = 0;
                break;
            case 2:
                meshTransform.eulerAngles = crossRotation;
                break;
        }

        foreach (GameObject platform in platformObjects.Values)
        {
            MeshRenderer[] renderers = platform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].GetComponent<MeshRenderer>().material.mainTexture = platformTextures[(i * 3) + newColour];
            }
        }
    }
}
