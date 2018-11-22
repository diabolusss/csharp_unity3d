#pragma strict

var Rooms_1and3:GameObject;
var Rooms_2and4:GameObject;
var Lvl:Sprite[];
private var Black:GameObject;
var Fon:SpriteRenderer;
var Player:SpriteRenderer;
private var LightL:GameObject;


private var MLevel:GameObject;
private var ThisLvl:int;
private var MaxLvl:int;
private var NoLoop:boolean=false;


function Start(){
if(PlayerPrefs.HasKey("ThisLvl")){
ThisLvl=PlayerPrefs.GetInt("ThisLvl");
}else{ThisLvl=1;}

if(PlayerPrefs.HasKey("MaxLvl")){
MaxLvl=PlayerPrefs.GetInt("MaxLvl");
}else{MaxLvl=1;}

PlayerPrefs.SetInt("MaxLvl",MaxLvl);
PlayerPrefs.SetInt("ThisLvl",ThisLvl);

LightL=GameObject.Find("Directional Light");
Black=GameObject.Find("BlackFon");
MLevel=GameObject.Find("Level Max");
ActiveWhat();
}

function ActiveWhat(){
if(ThisLvl<=1){
MLevel.GetComponent.<UI.Image>().sprite=Lvl[0];
Rooms_1and3.SetActive(true);
Destroy(Rooms_2and4);
Fon.color=Color32(0,137,255,255);
Player.color=Color32(0,137,255,255);
LightL.GetComponent.<Light>().color=Color32(0,100,230,255);
LightL.GetComponent.<Light>().intensity=5.77;
}
if(ThisLvl==2){
MLevel.GetComponent.<UI.Image>().sprite=Lvl[1];
Rooms_2and4.SetActive(true);
Destroy(Rooms_1and3);
Fon.color=Color32(157,0,167,255);
Player.color=Color32(157,0,187,255);
LightL.GetComponent.<Light>().color=Color32(157,0,197,255);
LightL.GetComponent.<Light>().intensity=3.77;
}
if(ThisLvl==3){
MLevel.GetComponent.<UI.Image>().sprite=Lvl[2];
Rooms_1and3.SetActive(true);
Destroy(Rooms_2and4);
Fon.color=Color32(0,157,27,255);
Player.color=Color32(0,187,37,255);
LightL.GetComponent.<Light>().color=Color32(0,217,27,255);
LightL.GetComponent.<Light>().intensity=2.77;
}
if(ThisLvl==4){
MLevel.GetComponent.<UI.Image>().sprite=Lvl[3];
Rooms_2and4.SetActive(true);
Destroy(Rooms_1and3);
Fon.color=Color32(255,137,0,255);
Player.color=Color32(255,107,0,255);
LightL.GetComponent.<Light>().color=Color32(255,77,0,255);
LightL.GetComponent.<Light>().intensity=5.77;
}
}


function NextLvl(){TapOn();}
function TapOn(){
if(MaxLvl!=1){
if(ThisLvl<MaxLvl){ThisLvl++;}
else{ThisLvl=1;}
var Point:int=0;
Black.SetActive(true);
while(Point==0){
if(Black.GetComponent.<UI.Image>().color.a<1){
Black.GetComponent.<UI.Image>().color.a=Black.GetComponent.<UI.Image>().color.a+0.037;
}else{PlayerPrefs.SetInt("ThisLvl",ThisLvl);Point=1;Application.LoadLevel(0);}
yield WaitForSeconds(0.007);
}
}
}
