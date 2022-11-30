using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPClient : MonoBehaviour
{
    // public
    public string IP = "127.0.0.1";
    public int port = 8081; // define > init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // start from unity3d
    public void Start()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    // sendData
    [ContextMenu("Send TestString")]
    public void SendTestMessage()
    {
        SendString("test");
    }


    public void SendString(string message)
    {
        try
        {
            //if (message != "")
            //{

            // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
            byte[] data = Encoding.UTF8.GetBytes(message);

            // Den message zum Remote-Client senden.
            client.Send(data, data.Length, remoteEndPoint);
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

}