using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform player;
    public PlayerMovement pm;

    // Update is called once per frame
    public void SpawnPlayer()
    {
        player.position = spawnPoint.position;
        pm.StopDying();
    }
}
