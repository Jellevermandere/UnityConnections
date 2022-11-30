using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // this enables the IO port namespace Needs .NET Framework to work
using UnityEngine.Events;

// ************* This script manages the Arduino Communication ******************* //
// tutorial I learned this from: https://playground.arduino.cc/Main/MPU-6050/#measurements //
// arduino script at the bottom //

namespace JelleVer.ConnectionTools
{
    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    public class SerialConnection : MonoBehaviour
    {
        [Header("Connection Settings")]
        [SerializeField]
        [Tooltip("Use this to automatically activate the serial port at the start")]
        public bool activateAtStart = true;

        [SerializeField]
        [Tooltip("Logs the received and transmitted value to the console")]
        public bool logValue = true;

        [Tooltip("Change this to whatever port your SPI device is connected to")]
        public string IOPort = "/dev/cu.HC05-SPPDev"; // Change this to whatever port your Arduino is connected to, this is the port for the specefic bluetooth adaptor used (HC-05 Wireless Bluetooth RF Transceiver)

        [Tooltip("this must match the bauderate of the Serial device")]
        public int baudeRate = 9600; //this must match the bauderate of the Arduino script

        [SerializeField]
        [Tooltip("The serial timeout between readings")]
        public int readTimeout = 25;

        [Header("Events")]
        [SerializeField]
        [Tooltip("This event is invoked when teh serial port recieves data")]
        private StringEvent OnValueReceived = new StringEvent();

        [HideInInspector]
        public SerialPort sp;

        public string recievedValue { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            if (activateAtStart) ActivateSP();
        }

        /// <summary>
        /// Activates the serial port with the set value in the object
        /// </summary>
        public void ActivateSP()
        {
            sp = new SerialPort(IOPort, baudeRate, Parity.None, 8, StopBits.One);

            sp.Open();
            sp.ReadTimeout = readTimeout;
            sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        /// <summary>
        /// Handkes the on data received event
        /// </summary>
        /// <param name="sender">the SerialPort object</param>
        /// <param name="e">extra SerialDataReceivedEventArgs</param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serial = (SerialPort)sender;
            recievedValue = serial.ReadExisting();
            if (logValue) Debug.Log(recievedValue);
            OnValueReceived.Invoke(recievedValue);
        }

        /// <summary>
        /// Sends a message to the activate serial port
        /// </summary>
        /// <param name="value">the value to send</param>
        public void SendBytes(byte[] value)
        {
            if (sp == null)
            {
                Debug.LogWarning("Cant send data, because th eport is not available");
                return;
            }

            if (sp.IsOpen)
            {
                sp.Write(value, 0, value.Length);
                if (logValue)
                {
                    string mess = "";

                    foreach (var item in value)
                    {
                        mess += item + ", ";
                    }
                    Debug.Log(mess);
                }
                
            }

        }

    }
}

/* Put this code on your arduino 

    void setup(){
      Serial.begin(9600);
    }
    void loop(){
      Serial.println("Hello World");
      Serial.flush();
      delay(25);
    }
*/