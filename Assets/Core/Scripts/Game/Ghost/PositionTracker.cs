using System;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    [SerializeField] private float _recordInterval = 0.1f; // Time between position records
    [SerializeField] private RouteRecord _routeBuffer;

    private float _timeElapsed;

    [ContextMenu("Start Tracking")]
    public void StartTracking()
    {
        _timeElapsed = 0f;
        ClearRouteBuffer();
        InvokeRepeating(nameof(RecordPosition), 0f, _recordInterval);
    }

    private void ClearRouteBuffer()
    {
        _routeBuffer.RoutePoints.Clear();
    }

    [ContextMenu("Stop Tracking")]
    public void StopTracking()
    {
        CancelInvoke(nameof(RecordPosition));
    }

    private void RecordPosition()
    {
        _routeBuffer.RoutePoints.Add(new RoutePoint
        {
            Position = new Vector3(transform.position.x, transform.position.y, transform.position.z),
            EulerRotation = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
            Time = _timeElapsed
        });

        Debug.Log($"Recorded position: {transform.position}, rotation: {transform.rotation.eulerAngles}, time: {_timeElapsed}");

        _timeElapsed += _recordInterval;
    }
}
