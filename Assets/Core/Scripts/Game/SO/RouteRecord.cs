using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RouteRecord", menuName = "Scriptable Objects/RouteRecord")]
public class RouteRecord : ScriptableObject
{
    public List<RoutePoint> RoutePoints;
}
