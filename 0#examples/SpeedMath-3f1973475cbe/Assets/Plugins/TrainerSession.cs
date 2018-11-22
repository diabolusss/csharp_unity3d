[System.Serializable]
public class TrainerSession {
  public int id;
  public ProblemEvent[] problemEvents;
  public ThinkGearEvent[] thinkGearEvents;
  public double startedAt;
  public double endedAt;
  
  public int numProblems;
  public int numCorrect;
  public int numIncorrect;
  
  public TrainerSession(int id, double startedAt) : this(id, new ProblemEvent[0], new ThinkGearEvent[0], startedAt, 0, 0, 0, 0){}
  
  public TrainerSession(int id, ProblemEvent[] problemEvents, ThinkGearEvent[] thinkGearEvents,
                        double startedAt, double endedAt, int numProblems, int numCorrect, int numIncorrect){
    this.id = id;
    this.problemEvents = problemEvents;
    this.thinkGearEvents = thinkGearEvents;
    this.startedAt = startedAt;
    this.endedAt = endedAt;
    this.numProblems = numProblems;
    this.numCorrect = numCorrect;
    this.numIncorrect = numIncorrect;
  }
  
  public TrainerSession() : this(0, new ProblemEvent[0], new ThinkGearEvent[0], 0, 0, 0, 0, 0){}
  
  public override string ToString(){
    string output = "";
    
    output += "id: " + id + "\n" +
              "startedAt: " + startedAt + "\n" +
              "endedAt: " + endedAt + "\n" + 
              "numProblems: " + numProblems + "\n" +
              "numCorrect: " + numCorrect + "\n" +
              "numIncorrect " + numIncorrect + "\n";
              
    return output;
  }
}