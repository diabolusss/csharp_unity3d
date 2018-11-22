[System.Serializable]
public class ProblemEvent {
  public enum Operands: int {
    Subtract = 0,
    Add = 1,
    Multiply = 2,
    Divide = 3
  }
  
  public int operand_1;
  public int operand_2;
  public Operands op;
  public int answer;        // value is 0 when it's a problemevent
  public float eventTime;
  
  public bool isCorrect {
    get {
      if(answer == 0)
        return false;
        
      switch(op){
        case Operands.Subtract:
          return answer == (operand_1 - operand_2);
        case Operands.Add:
          return answer == (operand_1 + operand_2);
        case Operands.Multiply:
          return answer == (operand_1 * operand_2);
        case Operands.Divide:
          return answer == (operand_1 / operand_2);
        default:
          return false;
      }
    }
  }
  
  public ProblemEvent() : this(0, 0, Operands.Subtract, 0.0f){}
  
  // This constructor is used in the case of constructing a "problem"
  public ProblemEvent(int op1, int op2, Operands op, float time) : this(op1, op2, op, 0, time){}
  
  // This constructor is used in the case of constructing an "answer"
  public ProblemEvent(int op1, int op2, Operands op, int answer, float time){
    this.operand_1 = op1;
    this.operand_2 = op2;
    this.op = op;
    this.answer = answer;
    this.eventTime = time;
  }
}
