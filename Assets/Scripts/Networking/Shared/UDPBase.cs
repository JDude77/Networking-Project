using UnityEngine;
using System.Net;
using TMPro;

public class UDPBase : MonoBehaviour
{
    protected static int serverPortNumber = 15001;
    protected static int clientPortNumber = 15000;

    protected static string ipAddressString = "127.0.0.1";
    protected static IPAddress ipAddress;

    protected void Awake()
    {
        DontDestroyOnLoad(this);

        ipAddressString = GameObject.FindGameObjectWithTag("IP IF").GetComponent<TMP_InputField>().text.Length > 0 ? GameObject.FindGameObjectWithTag("IP IF").GetComponent<TMP_InputField>().text : "127.0.0.1";
        if(!IPAddress.TryParse(ipAddressString, out ipAddress))
        {
            ipAddressString = "127.0.0.1";
            ipAddress = IPAddress.Parse(ipAddressString);
        }
        serverPortNumber = GameObject.FindGameObjectWithTag("SP IF").GetComponent<TMP_InputField>().text.Length > 0 ? int.Parse(GameObject.FindGameObjectWithTag("SP IF").GetComponent<TMP_InputField>().text) : 15001;
        clientPortNumber = GameObject.FindGameObjectWithTag("CP IF").GetComponent<TMP_InputField>().text.Length > 0 ? int.Parse(GameObject.FindGameObjectWithTag("CP IF").GetComponent<TMP_InputField>().text) : 15000;
    }

    public void setServerPortNumber(int pn)
    {
        serverPortNumber = pn;
    }

    public void setClientPortNumber(int pn)
    {
        clientPortNumber = pn;
    }

    public void setIPAddress(string ip)
    {
        ipAddressString = ip;
    }
}
