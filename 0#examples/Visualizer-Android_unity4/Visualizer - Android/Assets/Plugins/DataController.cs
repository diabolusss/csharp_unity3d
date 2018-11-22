using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class DataController : MonoBehaviour
{

    public ThinkGearData headsetData;
    public IBrainwaveDataPlayer dataPlayer;
    public IBrainwaveDataPlayer standbyPlayer;

    private long lastPacketReadTime;

    private ConnectionController connection;

    private Thread updateThread;
    private bool updateDataThreadActive = false;

    private double elapsedTime = 0.0;
    private bool isConnected = false;

    private const float TIMEOUT_INTERVAL = 3.5f;
    private const long TIMEOUT_INTERVAL_TICKS = (long)(TIMEOUT_INTERVAL * 10000000);

    private bool hasBlinked = false;
    private int blinkStrength = 0;

    public bool IsOffHead
    {
        get { return !isConnected || (isConnected && headsetData.poorSignalValue >= 200); }
    }

    public bool IsHeadsetInitialized
    {
        get { return isConnected; }
    }

    public bool IsDemo
    {
        get { return connection.currentConnectionType == ConnectionController.ConnectionTypes.Demo; }
    }

    public bool IsESenseReady
    {
        get { return !IsOffHead && (headsetData.attention != 0 && headsetData.meditation != 0); }
    }

    void Awake()
    {
        // dataPlayer = new FakeMindSetOutput();

        //updateThread = new Thread(UpdateDataValuesThread);

        connection = (ConnectionController)GetComponent(typeof(ConnectionController));
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (hasBlinked)
        {
            GameHelper.SendMessageToAll("OnBlinked", headsetData.blink, SendMessageOptions.DontRequireReceiver);
            hasBlinked = false;
        }

        if (updateDataThreadActive)
        {
            headsetData = dataPlayer.DataAt(elapsedTime);
        }
    }

    void OnHeadsetConnected()
    {
        isConnected = true;
        if (IsDemo)
        {
            dataPlayer = new FakeMindSetOutput();
        }
        else
        {
            dataPlayer = new AndroidOutput();
        }
        lastPacketReadTime = DateTime.Now.Ticks;
        Invoke("UpdateDataValues", 0.0f);
    }


    void OnHeadsetDisconnected()
    {
        isConnected = false;

        updateDataThreadActive = false;

        //dataPlayer = standbyPlayer;

        headsetData = new ThinkGearData(elapsedTime, 0, 0, 0, 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        if (updateThread != null && updateThread.IsAlive)
        {
            //updateThread.Abort();
        }
    }

    private void UpdateDataValues()
    {
        updateDataThreadActive = true;

        //if (updateThread == null || (updateThread != null && !updateThread.IsAlive))
        //{
        //    updateThread = new Thread(UpdateDataValuesThread);
        //    updateThread.Start();
        //}
    }

    private void UpdateDataValuesThread()
    {
        //while (updateThread.IsAlive && updateDataThreadActive)
        //{
        //    lock (this) { headsetData = dataPlayer.DataAt(elapsedTime); }
        //    lastPacketReadTime = DateTime.Now.Ticks;
        //    Thread.Sleep(80);
        //}
    }
}


