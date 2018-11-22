using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;

public enum PlayerState {
  PlayingFakeMindSetData,
  PlayingMindSetData,
  PlayingRecordedData
}

/**
 * This class listens for the following public events:
 *
 * OnRequestPortScan
 * OnRequestHeadsetDisconnect
 *
 * This class dispatches the following public events:
 *
 * OnPortScanStarted
 * OnPortScanSuccessful
 * OnPortScanFailed
 *
 * OnHeadsetConnected
 * OnHeadsetDisconnected
 *
 * The class also responds to the following events from PortScanHelper when
 * performing a port scan. These are not intended to be used externally:
 *
 * OnPortQuerySuccessful
 * OnPortQueryFailed
 */

//[RequireComponent(typeof(PortScanHelper))]
public class ConnectionController : MonoBehaviour
{

    private const string DemoMode = "DemoMode";

    private bool isConnected = false;

    //private UnityThinkGear unityThinkGear;

    public enum DisconnectStatuses
    {
        DisconnectRequested,
        TimedOut
    }

    public enum ConnectionTypes
    {
        AndroidBlueTooth,
        Demo
    }

    //public ConnectionTypes primaryConnectionType;

    [HideInInspector]
    public ConnectionTypes currentConnectionType = ConnectionTypes.Demo;

    public bool Connected
    {
        get { return isConnected; }
    }

    void OnRequestConnect(Boolean isDemo)
    {
        if (isDemo)
        {
            currentConnectionType = ConnectionTypes.Demo;
            GameHelper.SendMessageToAll("OnHeadsetConnected", null, SendMessageOptions.DontRequireReceiver);
            isConnected = true;
        }
        else
        {
            currentConnectionType = ConnectionTypes.AndroidBlueTooth;
            UnityThinkGear.connect();
            isConnected = true;
            GameHelper.SendMessageToAll("OnHeadsetConnected", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    /**
     * Call this method when you want to disconnect a headset. The response will consist of the following
     * event:
     *
     * OnHeadsetDisconnected - always dispatched. Indicates that a headset has disconnected.
     */
    void OnRequestDisconnect()
    {
        if (currentConnectionType != ConnectionTypes.Demo)
        {
            UnityThinkGear.disconnect();
        }
        isConnected = false;
        GameHelper.SendMessageToAll("OnHeadsetDisconnected", null, SendMessageOptions.DontRequireReceiver);
    }

    public void OnApplicationQuit()
    {
        GameHelper.SendMessageToAll("OnRequestDisconnect",
                                    DisconnectStatuses.DisconnectRequested,
                                    SendMessageOptions.DontRequireReceiver);
    }

}

