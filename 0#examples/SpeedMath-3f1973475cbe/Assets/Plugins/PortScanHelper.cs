using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;

public class PortScanHelper : MonoBehaviour {

  private string portName = "";

  private enum Status {
    Idle,
    Scanning,
    Successful,
    Cancelled,
    Failed
  };

  public const float TIMEOUT_INTERVAL = 3.5f;
  
  private Status status = Status.Idle;
  private Thread scanThread;

  private TcpClient client;

  private ConnectionController connection;

  void Awake(){
    connection = (ConnectionController)GetComponent(typeof(ConnectionController));
  }

	// Update is called once per frame
	void Update () {
    switch(status){
      case Status.Successful:
        switch(connection.currentConnectionType){
          case ConnectionController.ConnectionTypes.Socket:
            GameHelper.SendMessageToAll("OnSocketQuerySuccessful", client, SendMessageOptions.DontRequireReceiver);
            break;
          case ConnectionController.ConnectionTypes.Serial:
          case ConnectionController.ConnectionTypes.Demo:
          default:
            break;
        }

        GameHelper.SendMessageToAll("OnPortQuerySuccessful", portName, SendMessageOptions.DontRequireReceiver);
        status = Status.Idle;
        break;

      case Status.Failed:
        GameHelper.SendMessageToAll("OnPortQueryFailed", portName, SendMessageOptions.DontRequireReceiver);
        status = Status.Idle;
        break;

      case Status.Cancelled:
        scanThread.Abort();
        scanThread = null;

        switch(connection.currentConnectionType){
          case ConnectionController.ConnectionTypes.Socket:
            break;
          case ConnectionController.ConnectionTypes.Serial:
            ThinkGear.FreeConnection();
            break;
          case ConnectionController.ConnectionTypes.Demo:
          default:
            break;
        }

        GameHelper.SendMessageToAll("OnPortQueryFailed", portName, SendMessageOptions.DontRequireReceiver);
        status = Status.Idle;
        break;
    }
	}

  void OnRequestPortQuery(string portName){
    this.portName = portName;
    status = Status.Scanning;
    GameHelper.SendMessageToAll("OnPortQueryStarted", portName, SendMessageOptions.DontRequireReceiver);

    if(connection.currentConnectionType == ConnectionController.ConnectionTypes.Demo){
      status = Status.Successful;
      return;
    }

    if(scanThread == null || (scanThread != null && !scanThread.IsAlive)){
      // fire off the thread to scan the port
      scanThread = new Thread(ScanPort);	
      scanThread.Start(); 
    }
  }

  private void ScanPort(){
    switch(connection.currentConnectionType){
      case ConnectionController.ConnectionTypes.Socket:
        try {
          client = new TcpClient("127.0.0.1", 13854);

          status = Status.Successful;
          return;
        }
        catch(SocketException se){
          Debug.Log("Could not connect " + se);
        }
        catch(System.Exception e){
          Debug.Log("Exception in port scan " + e);
        }

        status = Status.Failed;

        break;
      case ConnectionController.ConnectionTypes.Serial:
        // connect at 9600 baud and using the "packets" stream type
        ThinkGear.GetNewConnectionID();

        Debug.Log("Attempting to connect to " + portName + "...");
        int connectStatus = ThinkGear.Connect(portName, 9600, 0);    
        Debug.Log("Connect() returned status " + connectStatus);

        if(connectStatus >= 0){
          Debug.Log("Sleeping thread...");
          Thread.Sleep((int)(TIMEOUT_INTERVAL * 1000.0f));

          Debug.Log("Attempting to read data from the serial port stream...");

          int packetCount = ThinkGear.ReadPackets(-1);

          if(packetCount >= 0){
            Debug.Log("Success!");
            status = Status.Successful;
            break;
          }

          Debug.Log("Connection successful, but headset data read timed out.");
        }
        else {
          // artificially slow down the port scanning process
          // otherwise we get weird behavior when it goes too fast
          Thread.Sleep(5);
        }
        
        ThinkGear.FreeConnection();
        status = Status.Failed;

        break;
      case ConnectionController.ConnectionTypes.Demo:
        break;
    }
  }

  // Cleanup a port scan in progress
  void OnRequestPortQueryCancel(){
    status = Status.Cancelled;
  }
}
