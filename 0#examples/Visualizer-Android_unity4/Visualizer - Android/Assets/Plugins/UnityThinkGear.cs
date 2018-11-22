using UnityEngine;
using System.Collections;
using System;

public class UnityThinkGear : MonoBehaviour
{

    private AndroidJavaClass jc;
    private static AndroidJavaObject jo;

    //private static string connectStr = "Nothing!";

    // Use this for initialization
    void Start()
    {
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public static void connect()
    {/*
      * 连接，并且发送RawData
      */
        jo.Call("connectWithRaw");
        //connectStr = "connect!!!!";
    }
    public static void disconnect()
    {
        jo.Call("disconnect");
    }
    public static int getAttention()
    {
        return jo.Get<int>("attention");
    }
    public static int getMeditation()
    {
        return jo.Get<int>("meditation");
    }
    public static int getPoorSignalValue()
    {
        return jo.Get<int>("poorSignalValue");
    }
    public static float getDelta()
    {
        return jo.Get<float>("delta");
    }
    public static float getTheta()
    {
        return jo.Get<float>("theta");
    }
    public static float getLowAlpha()
    {
        return jo.Get<float>("lowAlpha");
    }
    public static float getHighAlpha()
    {
        return jo.Get<float>("highAlpha");
    }
    public static float getLowBeta()
    {
        return jo.Get<float>("lowBeta");
    }
    public static float getHighBeta()
    {
        return jo.Get<float>("highBeta");
    }
    public static float getLowGamma()
    {
        return jo.Get<float>("lowGamma");
    }
    public static float getHighGamma()
    {
        return jo.Get<float>("highGamma");
    }
    public static float getRaw()
    {
        return jo.Get<float>("raw");
    }
    public static float getBlink()
    {
        return jo.Get<float>("blink");
    }
    public static int getHeartRate()
    {
        return jo.Get<int>("heartRate");
    }
    public static int getRawCount()
    {
        return jo.Get<int>("rawCount");
    }
    public static int getConnectState()
    {/*	Jar包中对连接状态的定义
      * public static final int STATE_IDLE = 0;
      * public static final int STATE_CONNECTING = 1;
      * public static final int STATE_CONNECTED = 2;
      * public static final int STATE_NOT_FOUND = 3;
      * public static final int STATE_NOT_PAIRED = 4;
      * public static final int STATE_DISCONNECTED = 5;
      * public static final int LOW_BATTERY = 6;
      * public static final int BULETOOTH_ERROR = 7;
     */
        return jo.Get<int>("connectState");
    }

    //void OnGUI(){
    //    GUI.Label(new Rect(10,10,50,50),connectStr);
    //}
}
