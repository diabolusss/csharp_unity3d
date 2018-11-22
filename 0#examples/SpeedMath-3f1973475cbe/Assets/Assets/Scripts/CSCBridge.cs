using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Jayrock;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Text;

public class CSCBridge : MonoBehaviour {

  public class TaskInfoTransmission {
    public string method;
    public string appKey;
    public string taskName;
  
    public TaskInfoTransmission(){
      method = "";
      appKey = "";
      taskName = "";
    }

    public TaskInfoTransmission(string method, string appKey, string taskName){
      this.method = method;
      this.appKey = appKey;
      this.taskName = taskName;
    }
  }
  
  public class AppInfoTransmission {
    public string method = "sendAppInfo";
    public string appKey;
    public string appName;
    public AchievementBase.Achievement[] tasks;

    public AppInfoTransmission(){
      method = "";
      appKey = "";
      appName = "";
      tasks = new AchievementBase.Achievement[0];
    }

    public AppInfoTransmission(string appName, string appKey, AchievementBase.Achievement[] tasks){
      this.tasks = tasks;
      this.appName = appName;
      this.appKey = appKey;
    }
  }

  public string appName = "My App";
  public string appKey;

  private Socket connector; 
  private const string HOST = "127.0.0.1";
  private const int PORT = 23854; 

  private System.Text.UTF8Encoding encoding;
  private byte[] buffer;
  private char[] delimiters;

  private Queue<string> transmitBuffer;

  public enum TransmitStates {
    ReadyToTransmit,
    WaitingForResponse
  }

  private TransmitStates state = TransmitStates.ReadyToTransmit;

  private GUISkin skin;
  private GUIStyle boxStyle;
  private GUIStyle disconnectedStyle;

  void Awake(){
    transmitBuffer = new Queue<string>();
  }

	// Use this for initialization
	void Start () {
    buffer = new byte[1024];
    encoding = new System.Text.UTF8Encoding();

    AttemptConnection();

    delimiters = new char[1]{'\r'};

    OnLanguageChanged();
	}

  void OnLanguageChanged(){
    skin = Localizer.Skins["nsskin"];
    boxStyle = skin.GetStyle("SignalBox");
    disconnectedStyle = skin.GetStyle("SignalError");
  }

  bool AttemptConnection(){
    try {
      connector = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      connector.NoDelay = true;
      connector.Connect(HOST, PORT);
      InvokeRepeating("ReadFromSocket", 0.0f, 0.2f);
    }
    catch(SocketException e){
      SendMessage("OnServerNotFound", null, SendMessageOptions.DontRequireReceiver);
      Debug.Log(e);
      return false;
    }

    return true;
  }

  void Update(){
    if(connector.Connected && state == TransmitStates.ReadyToTransmit && transmitBuffer.Count > 0){
      byte[] response = encoding.GetBytes(transmitBuffer.Dequeue());
      connector.Send(response, response.Length, SocketFlags.None);
      state = TransmitStates.WaitingForResponse;
    }
  }

  void ReadFromSocket(){
    string rawResponse = ReadStringFromSocket(connector);

    if(rawResponse != string.Empty){
      string[] responses = rawResponse.Split(delimiters); 
      
      foreach(string response in responses){
        if(!string.IsNullOrEmpty(response.Trim()) && response[0] != '\0'){

          IDictionary d = (IDictionary)JsonConvert.Import(typeof(IDictionary), response);
          state = TransmitStates.ReadyToTransmit;

          foreach(string key in d.Keys){
            if(key == "status"){
              ServerResponse s = (ServerResponse)JsonConvert.Import(typeof(ServerResponse), response);
              SendMessage("OnServerStatusResponse", s, SendMessageOptions.DontRequireReceiver);
              break;
            }
            else if(key == "taskName"){
              Task t = (Task)JsonConvert.Import(typeof(Task), response);
              SendMessage("OnServerTaskResponse", t, SendMessageOptions.DontRequireReceiver);
              break;
            }
            else if(key == "error"){
              ErrorResponse e = (ErrorResponse)JsonConvert.Import(typeof(ErrorResponse), response);
              SendMessage("OnServerError", e, SendMessageOptions.DontRequireReceiver);
              break;
            }
          }
        }
      }
    }
  }

  public void Identify(){
    string request = JsonConvert.ExportToString(new BaseClientRequest(BaseClientRequest.Methods.Identify,
                                                                       appKey));
    WriteStringToSocket(connector, request);
  }

  public void SendAppInfo(AchievementBase.Achievement[] tasks){
    string request = JsonConvert.ExportToString(new AppInfoTransmission(appName, appKey, tasks));
    WriteStringToSocket(connector, request);
  }

  public void GetTask(string taskName){
    string request = JsonConvert.ExportToString(new TaskInfoTransmission("getTask", appKey, taskName));
    WriteStringToSocket(connector, request);
  }

  public void CompleteTask(string taskName){
    string request = JsonConvert.ExportToString(new TaskInfoTransmission("completeTask", appKey, taskName));
    WriteStringToSocket(connector, request);
  }
	
  void OnApplicationQuit(){
    connector.Close();   
  }

  private string ReadStringFromSocket(Socket socket){
    if(socket.Available > 0){
      int bytesRead = socket.Receive(buffer);
      return encoding.GetString(buffer, 0, bytesRead);
    }
    else
      return string.Empty;
  }

  private void WriteStringToSocket(Socket socket, string s){
    transmitBuffer.Enqueue(s + '\r');
  }

  void OnGUI(){
    // HACK: disconnectedStyle is null for some reason
    if(disconnectedStyle == null)
      OnLanguageChanged();

    GUI.skin = skin;

    GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));

    GUILayout.Space(15);

    if(connector.Connected){
      GUILayout.BeginHorizontal(boxStyle);
      GUILayout.Label(Localizer.Content["cscpanel"]["connected"]);
      GUILayout.EndHorizontal();
    }
    else {
      GUILayout.BeginHorizontal(disconnectedStyle);
      GUILayout.Label(Localizer.Content["cscpanel"]["disconnected"]);

      if(GUILayout.Button(Localizer.Content["cscpanel"]["connect"]))
        AttemptConnection();

      GUILayout.EndHorizontal();
    }

    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
  }
}
