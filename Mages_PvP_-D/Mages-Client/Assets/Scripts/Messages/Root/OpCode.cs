public enum OpCode
{
    TestMessage = 1,
    Confirmation = 2,
    Time = 3,
    S_PFhandshake = 4,
    S_ServerDetails = 5,
    S_Spawn = 6,
    S_SpawnOthers = 7,
    R_Input = 8,
    R_SnapShot = 9,
    R_Align = 10,
}

//MAX VALUE = 128
//S_ -> syncronisation
//R_ -> running
//E_ -> end