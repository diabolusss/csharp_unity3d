#pragma strict

private var LevelMax:GameObject;
private var LineNext:GameObject;
private var ThisLvl:int;
var ScoreInLevels:float[];
var NumLevel:Sprite[];
private var Text_Score:GameObject;
private var Sound:GameObject;
private var BestScoreRect:GameObject;
private var ZIGZAG:GameObject;
private var NumberLevel:GameObject;
private var Exit:GameObject;
var Music:GameObject;



function Start(){
Text_Score=GameObject.Find("Score Game");
Sound=GameObject.Find("Sound");
BestScoreRect=GameObject.Find("Score Best");
ZIGZAG=GameObject.Find("Zigzag");
NumberLevel=GameObject.Find("Number Level");
LevelMax=GameObject.Find("Level Max");
LineNext=GameObject.Find("Line Up");
Exit=GameObject.Find("Exit");

if(PlayerPrefs.HasKey("ThisLvl")){
ThisLvl=PlayerPrefs.GetInt("ThisLvl");
}else{ThisLvl=1;}
PlayerPrefs.SetInt("ThisLvl",ThisLvl);
BestScoreRect.GetComponent.<UI.Text>().text = "BEST : " + PlayerPrefs.GetInt("BestScore"+ThisLvl.ToString()).ToString();
LvlsControl();
}

function LvlsControl(){
var Point:int=0;
var ScoreSearch:int=0;
while(Point==0){
if(ScoreSearch<ScoreInLevels.Length){
if(PlayerPrefs.GetInt("ScoreAll")<ScoreInLevels[ScoreSearch]){
PlayerPrefs.SetInt("MaxLvl",(ScoreSearch+1));
if(ScoreSearch>=1){
LineNext.GetComponent.<UI.Image>().fillAmount=(PlayerPrefs.GetInt("ScoreAll")+0.001-ScoreInLevels[(ScoreSearch-1)])/(ScoreInLevels[ScoreSearch]-ScoreInLevels[(ScoreSearch-1)]);
}else{LineNext.GetComponent.<UI.Image>().fillAmount=(PlayerPrefs.GetInt("ScoreAll")+0.001)/(ScoreInLevels[ScoreSearch]);}
Point=1;
}else{ScoreSearch++;}
}else{Point=1;PlayerPrefs.SetInt("MaxLvl",4);LineNext.GetComponent.<UI.Image>().fillAmount=1;}
}
NumberLevel.GetComponent.<UI.Image>().sprite=NumLevel[(PlayerPrefs.GetInt("MaxLvl")-1)];
}


function TapToPlay(){
OnMouseDown();
}

function OnMouseDown(){
var Point:int=0;
var AlphaColor:float;
while(Point==0){
AlphaColor=ZIGZAG.GetComponent.<UI.Text>().color.a;
if(AlphaColor<1&&AlphaColor>0){
AlphaColor=AlphaColor-0.1;
BestScoreRect.GetComponent.<UI.Text>().color.a=AlphaColor;
ZIGZAG.GetComponent.<UI.Text>().color.a=AlphaColor;
Text_Score.GetComponent.<UI.Text>().color.a=1-AlphaColor;
Sound.GetComponent.<UI.Image>().color.a=AlphaColor;
NumberLevel.GetComponent.<UI.Image>().color.a=AlphaColor;
LineNext.GetComponent.<UI.Image>().color.a=AlphaColor;
LevelMax.GetComponent.<UI.Image>().color.a=AlphaColor;
gameObject.GetComponent.<UI.Image>().color.a=AlphaColor;
Exit.GetComponent.<UI.Image>().color.a=AlphaColor;
}else{
Point=1;
GameObject.Find("Tap").GetComponent.<PlayerDoing>().enabled=true;
Destroy(GameObject.Find("Canvas Panel Start"));
GameObject.Find("EventSystem").SetActive(false);
Music.SetActive(true);
}
yield WaitForSeconds(0.007);
}

}