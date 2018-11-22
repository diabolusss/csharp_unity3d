using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class SocketHeadsetOutput : IBrainwaveDataPlayer {
  private TcpClient client; 
  private Stream stream;
  private byte[] buffer;
  private ThinkGearData d;

  public SocketHeadsetOutput(Stream s){
    stream = s;
    buffer = new byte[1024];
    byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
    stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
    
    d = new ThinkGearData();
  }

  public ThinkGearData DataAt(double secondsFromBeginning){
    if(stream.CanRead){
      try { 
        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        string[] packets = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split('\r');

        foreach(string packet in packets){
          // handle the case where there is a \r terminating character
          if(packet.Length == 0)
            continue;

          IDictionary primary = (IDictionary)JsonConvert.Import(typeof(IDictionary), packet);

          if(primary.Contains("poorSignalLevel")){
            d.poorSignalValue = int.Parse(primary["poorSignalLevel"].ToString());

            // check to see whether the packet contains eSense as well
            // if so, then grab all the data
            if(primary.Contains("eSense")){
              IDictionary eSense = (IDictionary)primary["eSense"];
              d.attention = int.Parse(eSense["attention"].ToString());
              d.meditation = int.Parse(eSense["meditation"].ToString());
            }

            if(primary.Contains("eegPower")){
              IDictionary eegPowers = (IDictionary)primary["eegPower"];

              d.delta = float.Parse(eegPowers["delta"].ToString());
              d.theta = float.Parse(eegPowers["theta"].ToString());
              d.lowAlpha = float.Parse(eegPowers["lowAlpha"].ToString());
              d.highAlpha = float.Parse(eegPowers["highAlpha"].ToString());
              d.lowBeta = float.Parse(eegPowers["lowBeta"].ToString());
              d.highBeta = float.Parse(eegPowers["highBeta"].ToString());
              d.lowGamma = float.Parse(eegPowers["lowGamma"].ToString());
              d.highGamma = float.Parse(eegPowers["highGamma"].ToString());
            }
          }
          else if(primary.Contains("rawEeg")){
            d.raw = int.Parse(primary["rawEeg"].ToString());
          }
          else if(primary.Contains("blinkStrength")){
            d.blink = int.Parse(primary["blinkStrength"].ToString());
          }
        }
      }
      catch(IOException e){ Debug.Log("IOException " + e); }
      catch(System.Exception e){ Debug.Log("Exception " + e); }
    }

    return new ThinkGearData(0.0f, d.meditation, d.attention, 0, d.poorSignalValue, d.delta, d.theta, d.lowAlpha, d.highAlpha, d.lowBeta, d.highBeta, d.lowGamma, d.highGamma, d.raw, d.blink);
  }

  public int dataPoints {
    get { return -1; }
  }

  public double duration {
    get { return 0.0; }
  }
}
