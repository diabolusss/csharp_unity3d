#pragma strict

var Rooms_1:GameObject[];
private var Room_Naw:int;
private var RoomInSave:int;
var Room_Naw_Save:int;
private var GamaDestroy:boolean=false;
private var LengthMax:int=0;
private var LengthNaw:int=0;

function Start () {
LengthMax=Rooms_1.Length;
if(PlayerPrefs.HasKey(Room_Naw_Save.ToString())){
Room_Naw=PlayerPrefs.GetInt(Room_Naw_Save.ToString());
}else{
Room_Naw=0;
}
Search();
}

function Search(){
if(Rooms_1.Length<=Room_Naw){PlayerPrefs.SetInt(Room_Naw_Save.ToString(),0);
Room_Naw=0;}
else{RoomInSave=Room_Naw;RoomInSave++;
PlayerPrefs.SetInt(Room_Naw_Save.ToString(),RoomInSave);
}
Rooms_1[Room_Naw].SetActive(true);
GamaDestroy=true;
}

function FixedUpdate(){
if(GamaDestroy){
if(LengthNaw<LengthMax){
if(LengthNaw!=Room_Naw){
Destroy(Rooms_1[LengthNaw]);
}
LengthNaw++;
}else{
GamaDestroy=false;
}
}
}