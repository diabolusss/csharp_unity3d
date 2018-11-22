#pragma strict

private var ThisLvl:int;
private var EventS:GameObject;
var Music:GameObject;
var SoundGameOver:AudioClip;
private var NoLoop:boolean=false;
var GameOverElements:GameObject[]; 
//0 - Text (Score in Game); 1 - Image; Other - Text; Last - Score; (Last-1) - BestScore;

function Start(){
EventS=GameObject.Find("EventSystem");


if(PlayerPrefs.HasKey("ThisLvl")){
ThisLvl=PlayerPrefs.GetInt("ThisLvl");
}else{ThisLvl=1;}
PlayerPrefs.SetInt("ThisLvl",ThisLvl);
}


function OnTriggerStay(other:Collider){
if(!NoLoop){
if(other.gameObject.tag=="GameOver"){
NoLoop=true;
var ScoreSave:int;
var BestScoreSave:int;
ScoreSave=GameObject.Find("Tap").GetComponent.<PlayerDoing>().ScoreTrans;
BestScoreSave=PlayerPrefs.GetInt("BestScore"+ThisLvl.ToString());
PlayerPrefs.SetInt("ScoreAll",(PlayerPrefs.GetInt("ScoreAll")+ScoreSave));
GameObject.Find("Sound").SetActive(false);
if(ScoreSave>BestScoreSave){
PlayerPrefs.SetInt("BestScore"+ThisLvl.ToString(),ScoreSave);
BestScoreSave=ScoreSave;
}
GameObject.Find("Tap").GetComponent.<PlayerDoing>().enabled=false;
GameObject.Find("Tap").GetComponent.<AudioSource>().enabled=false;
EventS.SetActive(true);
gameObject.GetComponent.<AudioSource>().PlayOneShot(SoundGameOver);


GameOverElements[(GameOverElements.Length-2)].GetComponent.<UI.Text>().text=ScoreSave.ToString();
GameOverElements[(GameOverElements.Length-1)].GetComponent.<UI.Text>().text=BestScoreSave.ToString();


var Point:int=0;
var ColorA:float=0;
var Objects:int=0;

while (Point==0){
if(ColorA<GameOverElements.Length){
GameOverElements[ColorA].SetActive(true);
ColorA++;
}else{Point=1;ColorA=0;}
}

while (Point==1){
if(ColorA<1){
ColorA=ColorA+0.05;
Objects=0;
while(Objects<GameOverElements.Length){
if(Objects==0){
GameOverElements[Objects].GetComponent.<UI.Text>().color.a=1-ColorA;
}else{
if(Objects==1){
GameOverElements[Objects].GetComponent.<UI.Image>().color.a=ColorA;
}else{
GameOverElements[Objects].GetComponent.<UI.Text>().color.a=ColorA;
}
}
Objects++;
}
yield WaitForSeconds(0.007);
}else{Point=2;}
}
}
}
}