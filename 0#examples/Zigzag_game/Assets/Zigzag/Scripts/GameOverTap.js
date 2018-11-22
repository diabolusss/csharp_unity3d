#pragma strict

var RateGameLink:String;
private var Black:GameObject;

function Start(){
Black=GameObject.Find("BlackFon");
Black.SetActive(false);
}

function InRetry(){Retry();}
function Retry(){
Black.SetActive(true);

var Point:int=0;
while(Point==0){
if(Black.GetComponent.<UI.Image>().color.a<1){
Black.GetComponent.<UI.Image>().color.a=Black.GetComponent.<UI.Image>().color.a+0.037;
}else{Point=1;Application.LoadLevel(0);}
yield WaitForSeconds(0.007);
}
}

function RateGame(){
Application.OpenURL(RateGameLink);
}