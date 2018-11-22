using UnityEngine;
using System.Collections;

public class AndroidOutput : IBrainwaveDataPlayer {

  public AndroidOutput() { }

  public ThinkGearData DataAt(double secondsFromBeginning){
      return new ThinkGearData(secondsFromBeginning,
                               UnityThinkGear.getMeditation(),
                               UnityThinkGear.getAttention(),
                               0,
                               UnityThinkGear.getPoorSignalValue(),
                               UnityThinkGear.getDelta(),
                               UnityThinkGear.getTheta(),
                               UnityThinkGear.getLowAlpha(),
                               UnityThinkGear.getHighAlpha(),
                               UnityThinkGear.getLowBeta(),
                               UnityThinkGear.getHighBeta(),
                               UnityThinkGear.getLowGamma(),
                               UnityThinkGear.getHighGamma(),
                               UnityThinkGear.getRaw(),
                               0.0f);
  }

  public int dataPoints {
    get { return -1; }
  }

  public double duration {
    get { return 0.0; }
  }
}

