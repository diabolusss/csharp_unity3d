using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainerDatabase : MonoBehaviour {
  public List<int> sessionIDs;

  private string sessionPath;
  private XmlSerializer serializer;

  private const string EXTENSION = ".tdb1";

  void Awake(){
    sessionIDs = new List<int>();

    sessionPath = Application.dataPath + "/sessions";

    // create the sessions store dir if it doesn't exist
    if(!Directory.Exists(sessionPath))
      Directory.CreateDirectory(sessionPath);
      
    // now let's enumerate the sessions
    string[] sessionFiles = Directory.GetFiles(sessionPath);
    
    foreach(string sessionFile in sessionFiles){
      string fileName = (new FileInfo(sessionFile)).Name;

      if(Path.GetExtension(fileName) == EXTENSION)
        sessionIDs.Add(System.Convert.ToInt32(fileName.Remove(fileName.Length - EXTENSION.Length)));
    }

    serializer = new XmlSerializer(typeof(TrainerSession));
  }

  public int MostRecentSessionID(){
    return sessionIDs.Count > 0 ? sessionIDs[sessionIDs.Count - 1] : 0;
  }

  public void DeleteSession(int sessionID){
    string sessionFile = sessionPath + "/" + sessionID + EXTENSION;
  
    // remove the sessionID from the internal list of sessions
    sessionIDs.Remove(sessionID);
    
    // and delete the file
    File.Delete(sessionFile);
  }

  public TrainerSession GetSession(int sessionID) {
    string sessionFile = sessionPath + "/" + sessionID + EXTENSION;
    TrainerSession session = null;
    
    // make sure the file exists
    if(!File.Exists(sessionFile)){
      Debug.Log(sessionFile + " doesn't exist!");
      return null;
    }

    using(FileStream fs = new FileStream(sessionFile, FileMode.Open)){
      try {
        session = (TrainerSession)serializer.Deserialize(fs);
      }
      catch(SerializationException e){
        Debug.Log("Failed to deserialize. " + e.Message);
      }
    }

    return session;
  }

  public void WriteSession(TrainerSession session){
    using(FileStream fs = new FileStream(sessionPath + "/" + session.id + EXTENSION, FileMode.Create)){
      try {
        serializer.Serialize(fs, session);
      }
      catch (SerializationException e){
        Debug.Log("Failed to serialize. " + e.Message);
      }
    }
    
    sessionIDs.Add(session.id);
  }
}
