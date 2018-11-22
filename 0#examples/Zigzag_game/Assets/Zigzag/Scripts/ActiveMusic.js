#pragma strict

var Music:AudioClip[];
private var ThisLvl:int;


function Start(){
if(PlayerPrefs.HasKey("ThisLvl")){
ThisLvl=PlayerPrefs.GetInt("ThisLvl");
}else{ThisLvl=1;}
PlayerPrefs.SetInt("ThisLvl",ThisLvl);
gameObject.GetComponent.<AudioSource>().clip=Music[(ThisLvl-1)];
gameObject.GetComponent.<AudioSource>().Play();
}