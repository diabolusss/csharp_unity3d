[System.Serializable]
public class ThinkGearEvent {
  public int meditation;
  public int attention;
  public float eventTime;
  
  public ThinkGearEvent(int meditation, int attention, float eventTime){
    this.meditation = meditation;
    this.attention = attention;
    this.eventTime = eventTime;
  }
  
  public ThinkGearEvent() : this(0, 0, 0.0f){}
}