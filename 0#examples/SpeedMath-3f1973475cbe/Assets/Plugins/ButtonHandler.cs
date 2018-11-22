using UnityEngine;
using System.Collections;

public abstract class ButtonHandler : MonoBehaviour {

  private Component controller;

  public abstract string[] labels {
    get;
  }

	// Use this for initialization
	void Start () {
	  controller = gameObject.GetComponent("MenuController");
	}
	
	public abstract void ClickEvent(int i);
}
