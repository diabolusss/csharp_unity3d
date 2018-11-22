using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

/**
 * Response objects (i.e. data that the server sends to the client
 */

public class ServerResponse {
  public enum StatusTypes {
    Ok,
    NeedInfo
  }

  public StatusTypes status;

  public ServerResponse(){
  }

  public ServerResponse(StatusTypes status){
    this.status = status;
  }
}

public class ErrorResponse {
  public enum ErrorTypes {
    None,
    BadRequestFormat,
    UnknownMethod,
    InvalidAppKey,
    InvalidTaskName
  }

  public ErrorTypes error;

  public ErrorResponse(){
    error = ErrorTypes.None;
  }

  public ErrorResponse(ErrorTypes e){
    error = e;
  }
}

/**
 * Request objects (i.e. data that the client sents to the server)
 */

public class BaseClientRequest {
  public enum Methods {
    Invalid,
    Identify,
    SendAppInfo,
    CompleteTask,
    GetTask 
  }

  public Methods method;
  public string appKey;

  public BaseClientRequest() {
    method = Methods.Invalid;
    appKey = "";
  }

  public BaseClientRequest(Methods m, string a){
    method = m;
    appKey = a;
  }
}

public class AppInfoRequest : BaseClientRequest {
  public string appName;

  public Task[] tasks;

  public AppInfoRequest() : base() {
    appName = "";
    tasks = new Task[0];
  }
}

public class AchievementRequest : BaseClientRequest {
  public string taskName;

  public AchievementRequest() : base() {
    taskName = "";
  }
}

/**
 * An object representing a WitFit task
 */

[System.Serializable]
[XmlRoot("task")]
public class Task {
  public string taskName;
  public string description;
  public int value;
  public bool completed;

  public Task() : this("", "", 0) {
    completed = false;
  }

  public Task(string name, string description, int value){
    this.taskName = name;
    this.description = description;
    this.value = value;
  }

  public override string ToString(){
    return "Task name: " + taskName + ", " +
            "Description: " + description + ", " + 
            "Points: " + value + ", " + 
            "Completed: " + completed;
  }
}
