using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyShooting
{
    public Transform GetNearestPlayer(GameObject gameObject)
    {
        var players = UnityEngine.Object.FindObjectsOfType<PlayerAttributes>();
        Transform nearestPlayer;
        List<float> distances = new List<float>();
        
        for (int i = 0; i < players.Length; i++ )
        {
            distances.Add(Vector3.Distance(gameObject.transform.position, players[i].transform.position));
        }
        int index = distances.FindIndex(distance => distances.Min() == distance);// used Linq for getting the min distance value from distances list.
        nearestPlayer = players[index].transform;
        return nearestPlayer;
    }

    public float GetDistance(Transform playerTransform,GameObject gameObject)
    {
        return Vector3.Distance(gameObject.transform.position, playerTransform.position);
    }
}
