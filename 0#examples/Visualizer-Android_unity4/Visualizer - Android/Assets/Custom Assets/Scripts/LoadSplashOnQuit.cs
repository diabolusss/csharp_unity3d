using UnityEngine;
using System.Collections;

public class LoadSplashOnQuit : MonoBehaviour {

  void OnApplicationQuit(){
    if(Application.loadedLevelName != "ExitSplash"){
      Application.CancelQuit();
      Destroy(gameObject);
      Application.LoadLevel("ExitSplash");
    }
  }
}

