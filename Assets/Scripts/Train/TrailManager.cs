using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Train
{
    [Serializable]
    public class TrailManager
    {
        public int TrialLength { protected set;get; } = 10;
        private List<TrailPoint> _trailPoints = new ();

        public void ClearTrail(Transform transform)
        {
            _trailPoints.Clear();
            AddTrailPoint(new TrailPoint(transform));
        }

        public void SetTrailLength(int length)
        {
            if(length < 0) return;
            TrialLength = length;
        }
        public void AddTrailPoint(TrailPoint point)
        {
            _trailPoints.Insert(0,point);
            while (_trailPoints.Count > TrialLength) { _trailPoints.RemoveAt(_trailPoints.Count-1);}
        }

        public bool GetTrailPoint(int index, out TrailPoint point)
        {
            point = default;
            if (index < 0 || index >= _trailPoints.Count) return false;
            point =_trailPoints[index];
            return true;
        }
    }

    public struct TrailPoint
    {
        public TrailPoint(Vector3 worldPosition, Quaternion worldRotation)
        {
            WorldPosition = worldPosition;
            WorldRotation = worldRotation;
        }

        public TrailPoint(Transform transform)
        {
            WorldPosition = transform.position;
            WorldRotation = transform.rotation;
        }

        public Vector3 WorldPosition { get; }
        public Quaternion WorldRotation { get; }
    }
}