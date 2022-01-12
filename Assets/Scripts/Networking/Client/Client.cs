using UnityEngine;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System;
using NetworkingUtility;
using System.Collections.Generic;

public class Client : UDPBase
{
    private static int lastPlayerPacketID = -1;
    private static PlayerData playerData = new PlayerData();
    public PlayerData GetPlayerData() { return playerData; }

    private static Dictionary<int, PlatformData> platformsData = new Dictionary<int, PlatformData>();
    public Dictionary<int, PlatformData> GetPlatformsData() { return platformsData; }

    private static IPEndPoint ipEndPoint;

    private static Client instance;

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

        ipEndPoint = new IPEndPoint(ipAddress, serverPortNumber);
    }

    private void Start()
    {
        UDPListener();
    }

    private static void UDPListener()
    {
        Task.Run(async () =>
        {
            using (var udpClient = new UdpClient(clientPortNumber))
            {
                udpClient.Connect(ipEndPoint);
                while (true)
                {
                    try
                    {
                        UdpReceiveResult receivedResults = await udpClient.ReceiveAsync();
                        Packet p = new Packet();
                        NetUtil.Deserialise(receivedResults.Buffer, out p);
                        if(p.type == 1 && p.id > lastPlayerPacketID)
                        {
                            NetUtil.Deserialise(receivedResults.Buffer, out playerData);
                            lastPlayerPacketID = p.id;
                        }
                        else if(p.type == 2)
                        {
                            PlatformData pd = new PlatformData();
                            NetUtil.Deserialise(receivedResults.Buffer, out pd);

                            if (platformsData.ContainsKey(pd.platformID))
                            {
                                if(platformsData[pd.platformID].xPosition < pd.xPosition)
                                {
                                    if (pd.destroyed)
                                    {
                                        platformsData.Remove(pd.platformID);
                                    }
                                    else
                                    {
                                        platformsData[pd.platformID].xPosition = pd.xPosition;
                                        platformsData[pd.platformID].colour = pd.colour;
                                        platformsData[pd.platformID].xVelocity = pd.xVelocity;
                                    }
                                }
                            }
                            else platformsData.Add(pd.platformID, pd);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }//End while
            }//End using
        }//End lambda
        );//End Task
    }//End UDP Listener
}