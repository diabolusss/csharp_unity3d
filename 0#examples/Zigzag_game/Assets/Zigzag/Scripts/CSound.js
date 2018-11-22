@script ExecuteInEditMode;
var SoundsOn:Sprite;
var SoundsOff:Sprite;
private var SaveSound:float;

function Start () {
if (PlayerPrefs.HasKey("SaveSound")){
SaveSound=(PlayerPrefs.GetFloat('SaveSound') );
}else {SaveSound=1;}
ControlSound();
}
function SoundR(){
if (SaveSound==0) {SaveSound=1;}
else {SaveSound=0;}
ControlSound();
PlayerPrefs.SetFloat("SaveSound",SaveSound);
}
function ControlSound() {
if (SaveSound==0){
AudioListener.pause=true;
gameObject.GetComponent.<UI.Image>().sprite=SoundsOff;}
else {
AudioListener.pause=false;
gameObject.GetComponent.<UI.Image>().sprite=SoundsOn;}
}
function ExitFunction(){
Application.Quit();
}