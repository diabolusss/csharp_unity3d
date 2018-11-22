using UnityEngine;
using System.Collections;
using System.Threading;
using System;

public class DataController : MonoBehaviour {

  public ThinkGearData headsetData;
  public IBrainwaveDataPlayer dataPlayer;
  public IBrainwaveDataPlayer standbyPlayer;

  private long lastPacketReadTime;

  private System.Net.Sockets.TcpClient client;

  private ConnectionController connection;

  private Thread updateThread;
  private bool updateDataThreadActive = true;

  private double elapsedTime = 0.0;
  private bool isConnected = false;

  private const float TIMEOUT_INTERVAL = 3.5f;
  private const long TIMEOUT_INTERVAL_TICKS = (long)(TIMEOUT_INTERVAL * 10000000);

  private bool hasBlinked = false;
  private int blinkStrength = 0;

  public bool IsOffHead {
    get { return !isConnected || (isConnected && headsetData.poorSignalValue >= 200); }
  }

  public bool IsHeadsetInitialized {
    get { return isConnected; }
  }

  public bool IsDemo {
    get { return connection.currentConnectionType == ConnectionController.ConnectionTypes.Demo; }
  }

  public bool IsESenseReady {
    get { return !IsOffHead && (headsetData.attention != 0 && headsetData.meditation != 0); }
  }

  void Awake(){
    dataPlayer = new FakeMindSetOutput();
    updateThread = new Thread(UpdateDataValuesThread);

    connection = (ConnectionController)GetComponent(typeof(ConnectionController));
  }

  void Update(){
    elapsedTime += Time.deltaTime;

    if(hasBlinked){
      GameHelper.SendMessageToAll("OnBlinked", headsetData.blink, SendMessageOptions.DontRequireReceiver);
      hasBlinked = false;
    }
  }

  void OnHeadsetConnected(string portName){
    isConnected = true;
    
    switch(connection.currentConnectionType){
      case ConnectionController.ConnectionTypes.Serial:
        standbyPlayer = dataPlayer;
        dataPlayer = (IBrainwaveDataPlayer)new MindSetOutput(MindSetVersions.ASIC);
        break;
      case ConnectionController.ConnectionTypes.Socket:
        break;
      case ConnectionController.ConnectionTypes.Demo:
      default:
        standbyPlayer = dataPlayer;
        dataPlayer = (IBrainwaveDataPlayer)new FakeMindSetOutput();
        break;
    }

    lastPacketReadTime = DateTime.Now.Ticks;
    Invoke("UpdateDataValues", 0.0f);
  }

  void OnServerConnected(System.Net.Sockets.TcpClient client){
    this.client = client;
    standbyPlayer = dataPlayer;
    dataPlayer = (IBrainwaveDataPlayer)new SocketHeadsetOutput(client.GetStream());
  }

  void OnHeadsetDisconnected(ConnectionController.DisconnectStatuses d){
    isConnected = false;

    updateDataThreadActive = false;

    dataPlayer = standbyPlayer;

    if(updateThread != null && updateThread.IsAlive)
      updateThread.Abort();

    // clean up the different connection types differently
    switch(connection.currentConnectionType){
      case ConnectionController.ConnectionTypes.Serial:
        ThinkGear.FreeConnection();
        break;
      case ConnectionController.ConnectionTypes.Socket:
        if(client != null)
          client.Close();

        break;
      default:
        break;
    }
  }

  private void UpdateDataValues(){
    updateDataThreadActive = true;

    if(updateThread == null || (updateThread != null && !updateThread.IsAlive)){
      updateThread = new Thread(UpdateDataValuesThread);
      updateThread.Start();
    }
  }

  private void UpdateDataValuesThread(){
    while(updateThread.IsAlive && updateDataThreadActive){
      switch(connection.currentConnectionType){
        case ConnectionController.ConnectionTypes.Serial:
          int readResult = ThinkGear.ReadPackets(-1);

          if(readResult < 0 && (DateTime.Now.Ticks - lastPacketReadTime > TIMEOUT_INTERVAL_TICKS)){
            // if we haven't seen a valid packet in a while, then the headset was probably
            // disconnected. do a cleanup.
            Debug.Log("Headset data receipt timed out.");
            GameHelper.SendMessageToAll("OnRequestHeadsetDisconnect",
                                        ConnectionController.DisconnectStatuses.TimedOut, 
                                        SendMessageOptions.DontRequireReceiver);

            continue;
          }
          
          break;
        case ConnectionController.ConnectionTypes.Socket:
          break;
        case ConnectionController.ConnectionTypes.Demo:
        default:
          break;
      }

      lock(this){ headsetData = dataPlayer.DataAt(elapsedTime); }
      lastPacketReadTime = DateTime.Now.Ticks;
      Thread.Sleep(20);
    }
  }
}


