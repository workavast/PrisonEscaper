using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoomData : RoomData
{
    [SerializeField] private Transform playerSpawnPosition;
    public Vector3 PlayerSpawnPosition => playerSpawnPosition.position;
}