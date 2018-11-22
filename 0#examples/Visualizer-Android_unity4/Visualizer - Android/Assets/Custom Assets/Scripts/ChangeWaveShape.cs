using UnityEngine;
using System.Collections;

public class ChangeWaveShape : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        int i = 0;
        while (i < Input.touchCount)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                GameHelper.SendMessageToAll("NextVisualization", null, SendMessageOptions.DontRequireReceiver);
                //Debug.Log("aaX:" + Input.GetTouch(i).position.x + "aaY:" + Input.GetTouch(i).position.y);
            }
            ++i;
        }
	}
}
