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

[RequireComponent(typeof(PortScanHelper))]
public class ConnectionController : MonoBehaviour {
  public const string MAC_PORT_FORMAT = "/dev/tty.MindSet-DevB-{0}";
  public const string PC_PORT_FORMAT = "\\\\.\\COM{0}";
  public const string SL_INITIAL_PORT = "/dev/tty.MindSet-DevB";
  public const string MINDWAVE_PORT = "/dev/tty.MindWave";

  public const int MAX_COM_PORT = 100;

  private List<string> globalPortList;
  private int portListIndex = 0;

  private const string DemoMode = "DemoMode";

  private bool isConnected = false;

  public enum DisconnectStatuses {
    DisconnectRequested,
    TimedOut
  }

  public enum ConnectionTypes {
    Serial,
    Socket,
    Demo
  }
  
  public ConnectionTypes primaryConnectionType;
  
  [HideInInspector]
  public ConnectionTypes currentConnectionType = ConnectionTypes.Demo;

  public bool Connected {
    get { return isConnected; }
  }

  /**
   * Call this method when you want to start a port scan. The response will consist of the following
   * events:
   *
   * OnPortScanStarted - always dispatched. Indicates that a port scan has started.
   *
   * And either of the following, depending on success or failure of the port scan:
   *
   * OnPortScanSuccessful - dispatched if a connection was successfully established to a port.
   * OnPortScanFailed - dispatched if no connection was able to be established. 
   */
  void OnRequestPortScan(string portName){
    GameHelper.SendMessageToAll("OnPortScanStarted", null, SendMessageOptions.DontRequireReceiver);

    currentConnectionType = portName == DemoMode ? ConnectionTypes.Demo : primaryConnectionType;

    switch(currentConnectionType){
      case ConnectionTypes.Serial:
        // generate the list of ports that we need to scan over
        globalPortList = GeneratePortList(portName);
        portListIndex = 0;

        // if we know there is more than one port in the list, then start the scan process
        if(globalPortList.Count > 0)
          SendMessage("OnRequestPortQuery", globalPortList[portListIndex], SendMessageOptions.DontRequireReceiver);

        break;
      case ConnectionTypes.Socket:
        SendMessage("OnRequestPortQuery", portName, SendMessageOptions.DontRequireReceiver);
        break;
      case ConnectionTypes.Demo:
        SendMessage("OnRequestPortQuery", portName, SendMessageOptions.DontRequireReceiver);
        break;
    }
  }

  void OnRequestPortScanTerminate(){
    portListIndex = globalPortList.Count;
  }

  /**
   * Call this method when you want to disconnect a headset. The response will consist of the following
   * event:
   *
   * OnHeadsetDisconnected - always dispatched. Indicates that a headset has disconnected.
   */
  void OnRequestHeadsetDisconnect(DisconnectStatuses d){
    GameHelper.SendMessageToAll("OnHeadsetDisconnected", d, SendMessageOptions.DontRequireReceiver);
    isConnected = false;
  }

  void OnSocketQuerySuccessful(System.Net.Sockets.TcpClient c){
    GameHelper.SendMessageToAll("OnServerConnected", c, SendMessageOptions.DontRequireReceiver);
  }

  /*
   * PORT SCAN HELPER EVENTS
   */

  void OnPortQuerySuccessful(string portName){
    isConnected = true;
    GameHelper.SendMessageToAll("OnPortScanSuccessful", null, SendMessageOptions.DontRequireReceiver);
    GameHelper.SendMessageToAll("OnHeadsetConnected", portName, SendMessageOptions.DontRequireReceiver);

    if(portName != DemoMode)
      PlayerPrefs.SetString("Serial Port", portName);
  }

  void OnPortQueryFailed(string portName){
    switch(currentConnectionType){
      case ConnectionTypes.Serial:
        if(++portListIndex < globalPortList.Count)
          SendMessage("OnRequestPortQuery", globalPortList[portListIndex], SendMessageOptions.DontRequireReceiver);
        else
          GameHelper.SendMessageToAll("OnPortScanFailed", null, SendMessageOptions.DontRequireReceiver);

        break;
      case ConnectionTypes.Socket:
        GameHelper.SendMessageToAll("OnPortScanFailed", null, SendMessageOptions.DontRequireReceiver);
        break;
      case ConnectionTypes.Demo:
        GameHelper.SendMessageToAll("OnPortScanFailed", null, SendMessageOptions.DontRequireReceiver);
        break;
    }
  }

  public void OnApplicationQuit(){
    GameHelper.SendMessageToAll("OnRequestHeadsetDisconnect", 
                                DisconnectStatuses.DisconnectRequested, 
                                SendMessageOptions.DontRequireReceiver);
  }

  public void OnTransmitByte(byte b){
    Send(b);
  }

  /**
   * This method generates a list of all port names to be scanned over when performing
   * the initial connection port scan upon startup.
   *
   * It makes sure not to insert duplicates into the collection.
   *
   * This method may eventually be expanded to allow delegate methods for custom sorting.
   */
  private List<string> GeneratePortList(string initialPort){
    List<string> portList = new List<string>(); 

    // always add the requested initial port
    if(initialPort != null && initialPort.Length != 0)
      portList.Add(initialPort);

    string previousPort = PlayerPrefs.GetString("Serial Port");

    // also add the port that the user successfully connected to previously
    if(previousPort != null && previousPort.Length != 0)
      portList.Add(previousPort);

    // use the appropriate portname formatter
    string portNameFormat = Application.platform == RuntimePlatform.WindowsPlayer ?
                              PC_PORT_FORMAT :
                              MAC_PORT_FORMAT;

    // add the default MindSet port used in Snow Leopard. querying
    // this port will always fail in pre-10.6 OS X, but that's OK since
    // it'll fail quickly
    if(Application.platform != RuntimePlatform.WindowsPlayer && !portList.Contains(SL_INITIAL_PORT))
      portList.Add(SL_INITIAL_PORT);

    // add the default MindWave port
    if(Application.platform != RuntimePlatform.WindowsPlayer && !portList.Contains(MINDWAVE_PORT))
      portList.Add(MINDWAVE_PORT);

    // iterate over the port numbers, making sure not to add duplicates
    for(int i = 1; i <= MAX_COM_PORT; i++){
      string portName = System.String.Format(portNameFormat, i.ToString());

      if(!portList.Contains(portName)){
        portList.Add(portName);
      }
    }

    return portList;
  }

  /**
   * List of commonly-used bytes
   *
   * MODULE (1.7+)
   * 0x02 - Turn on 57.6k, raw, EEG, normal
   * 0x41 - Set/unset error flags
   *
   * ASIC
   * 0x02 - Turn on 57.6k, raw, EEG, normal
   */
  private void Send(byte b){
    ThinkGear.SendByte(b);
    Debug.Log(string.Format("Sent 0x{0:x2}", b)); 
  }
}

