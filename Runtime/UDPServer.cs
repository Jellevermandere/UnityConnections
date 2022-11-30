using UnityEngine;
using System.Collections;
using UnityEngine.Events;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPServer : MonoBehaviour
{
    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    // public
    public string IP = "127.0.0.1";
    public int port = 8081; // define > init

    // infos
    public string receivedString = "";

    public StringEvent OnMessageReceived = new StringEvent();

    // receiving Thread
    Thread receiveThread;
    // udpclient object
    UdpClient client;

    bool valueReceived = false;

    // start from unity3d
    public void Start()
    {
        // set up new thread
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void Update()
    {
        if (valueReceived)
        {
            OnMessageReceived.Invoke(receivedString);
            valueReceived = false;
        }
    }


    // receive thread
    private void ReceiveData()
    {

        // set up new UDP client reciever
        client = new UdpClient(port);
        Debug.Log("New UDP server started");

        while (true)
        {

            try
            {
                // Receiving bytes.
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                // convert to UTF
                string text = Encoding.UTF8.GetString(data);

                // latest UDPpacket
                receivedString = text;
                valueReceived = true;
                Debug.Log("Message received: \n" + text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        return receivedString;
    }

    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        client.Close();
    }
}