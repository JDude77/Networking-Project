using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System;
using NetworkingUtility;
using TMPro;

public class Server : UDPBase
{
    private static IPEndPoint ipEndPoint;

    private UdpClient udpClient;

    private static Server instance;

    private PlatformManager platformManager;

    private PlayerData playerData;

    private List<PlatformData> platformData;

    private List<PlatformScript> platforms;

    private Player player;

    protected new void Awake()
    {
        base.Awake();

        //Singleton stuff
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        ipEndPoint = new IPEndPoint(ipAddress, clientPortNumber);
    }

    private void Start()
    {
        try
        {
            udpClient = new UdpClient(serverPortNumber);
            udpClient.Connect(ipEndPoint);
        }
        catch (SocketException se)
        {
            Debug.LogError(se.Message);
        }

        platformManager = FindObjectOfType<PlatformManager>();

        playerData = new PlayerData();

        StartCoroutine(SendMessages());
    }

    private void Update()
    {
        if (platformManager == null) platformManager = FindObjectOfType<PlatformManager>();
        if (player == null) player = FindObjectOfType<Player>();
        GetPlatformData();
        GetPlayerData();
    }

    private void GetPlayerData()
    {
        playerData.colliding = player.GetColliding();
        playerData.dead = player.GetDead();
        playerData.faceShowing = player.GetFaceShowing();
        playerData.gravityDirection = player.GetGravityDirection();
        playerData.yPosition = player.transform.position.y;
        playerData.yVelocity = player.GetComponent<Rigidbody2D>().velocity.y;
    }

    private void GetPlatformData()
    {
        List<GameObject> platformObjects = platformManager.GetPlatforms();
        platforms = new List<PlatformScript>();
        platformData = new List<PlatformData>();
        for(int i = 0; i < platformObjects.Count; i++)
        {
            platforms.Add(platformObjects[i].GetComponent<PlatformScript>());
            PlatformData newPlatformData = new PlatformData();
            newPlatformData.colour = PlatformScript.GetColour();
            newPlatformData.destroyed = false;
            newPlatformData.platformID = platforms[i].GetInstanceID();
            newPlatformData.xPosition = platforms[i].transform.position.x;
            newPlatformData.xScale = platforms[i].GetScale();
            newPlatformData.xVelocity = platforms[i].GetComponentInChildren<Rigidbody2D>().velocity.x;
            newPlatformData.yPosition = platforms[i].transform.position.y;
            platformData.Add(newPlatformData);
        }
    }

    private IEnumerator SendMessages()
    {
        int cycle = -1;
        while(true)
        {
            if (udpClient != null)
            {
                try
                {
                    Debug.Log("Server connected: " + udpClient.Client.Connected);

                    byte[] dataToSend;

                    if(cycle == -1)
                    {
                        dataToSend = NetUtil.Serialise(this.playerData);
                    }
                    else
                    {
                        dataToSend = NetUtil.Serialise(this.platformData[cycle]);
                    }

                    udpClient.Send(dataToSend, dataToSend.Length);

                    if (cycle == platformData.Count - 1) cycle = -1;
                    else cycle++;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}