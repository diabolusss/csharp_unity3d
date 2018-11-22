#pragma strict

private var Rooms:GameObject;
private var Speed:float;
private var PlayerObj:GameObject;
private var Text_ScoreGame:GameObject;

private var OnePlayer:boolean=false;
private var DontJobs:boolean=false;
private var TransformGo:boolean=false;
var ScoreTrans:float;
var Click:AudioClip;


function Start(){
var ThisLevel:int;
if(PlayerPrefs.HasKey("ThisLvl")){
ThisLevel=PlayerPrefs.GetInt("ThisLvl");
}else{ThisLevel=1;}
PlayerPrefs.SetInt("ThisLvl",ThisLevel);

if(ThisLevel==1||ThisLevel==2){Speed=0.097;}
if(ThisLevel==3||ThisLevel==4){Speed=0.107;}

Rooms=GameObject.Find("allRooms");
PlayerObj=GameObject.Find("Player");
Text_ScoreGame=GameObject.Find("Score Game");

yield WaitForSeconds(0.57);
DontJobs=true;
}



function FixedUpdate(){
if(!OnePlayer){
if(PlayerObj.transform.position.x>0){
PlayerObj.transform.position.x=PlayerObj.transform.position.x-Speed;
}else{OnePlayer=true;}}

if(TransformGo){Rooms.transform.position.y=Rooms.transform.position.y-Speed;}
else{Rooms.transform.position.x=Rooms.transform.position.x-Speed;}

if(DontJobs){
ScoreTrans=ScoreTrans+Speed;
Text_ScoreGame.GetComponent.<UI.Text>().text = parseInt(ScoreTrans).ToString();
}
}

function OnMouseDown(){
if(DontJobs){
GetComponent.<AudioSource>().PlayOneShot(Click);
if(TransformGo){TransformGo=false;}
else{TransformGo=true;}
}
}