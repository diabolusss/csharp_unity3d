using UnityEngine;
using System.Collections;

public class ModeSwitchForDisplay : MonoBehaviour
{
    private bool startDetectTouch = false;

    void Start()
    {
        Invoke("detectTouch", 2.0f);
    }

    void detectTouch()
    {
        startDetectTouch = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (startDetectTouch && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            if (Input.GetTouch(0).deltaPosition.x < -10)
            {
                Application.LoadLevel("Visualizer");
                //Application.LoadLevelAsync("Visualizer");
            }
        }
    }
}
