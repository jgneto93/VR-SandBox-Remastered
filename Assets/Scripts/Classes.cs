using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerTracking {
    public List<Person> people;
}
    
[Serializable]
public class Ball {
    public float ballPositionX;
    public float ballPositionY;
    public float ballPositionZ;
}

[Serializable]
public class Person {
    public List<float> centerOfMass;
    public float heightOfGround;
    public int jerseyNumber;
    public List<int> jointIds;
    public List<float> positions;
    public int roleId;
    public string prefabName;
    public int teamId;
    public int trackId;
    public Camera camera;
    public int newId;
}

[Serializable]
public class TouchData{
    public Vector3 playerPosition;
    public int eventType;
    public int frameId;
    public int playerId;
    public int playerTeamId;
    public int playerRoleId;
}

[Serializable]
public class JsonData {
    public float[] ballPosition;
    public PlayerTracking playerTracking;
}
