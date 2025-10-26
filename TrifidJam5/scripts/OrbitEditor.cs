using UnityEngine;

namespace TrifidJam5
{
    [ExecuteAlways]
    public class OrbitEditor : MonoBehaviour
    {
        [SerializeField]
        private bool liveEditing = false;
        [SerializeField]
        private LineRenderer line;

        public float LineWidth;
        public int LineSegments;
        public Color LineStartColor;
        public Color LineEndColor;
        public float SemiMajorAxis;
        [Range(0f, 1f)]
        public float Eccentricity;
        [Range(0f, 360f)]
        public float ArgumentOfPeriapsis;
        [Range(0f, 360f)]
        public float Inclination;
        [Range(0f, 360f)]
        public float LongitudeOfAscendingNode;
        [Range(0f, 360f)]
        public float MeanAnomaly;
        public Transform Parent;
        public Transform Satellite;
        public bool Binary = false;
        public OrbitEditor Secondary;
        public float Separation;
        public float PrimaryMass = 1;
        public float SecondaryMass = 1;

        [ContextMenu("Build Orbit")]
        private void BuildOrbit()
        {
            if (line == null) line = GetComponent<LineRenderer>();
            line.widthMultiplier = LineWidth;
            line.positionCount = LineSegments;
            line.startColor = LineStartColor;
            line.endColor = LineEndColor;

            if (Binary)
            {
                Secondary.liveEditing = liveEditing;
                SemiMajorAxis = Separation / (1 + PrimaryMass / SecondaryMass);
                Secondary.SemiMajorAxis = Separation - SemiMajorAxis;
                Secondary.Eccentricity = Eccentricity;
                Secondary.ArgumentOfPeriapsis = ArgumentOfPeriapsis + 180;
                Secondary.Inclination = Inclination;
                Secondary.LongitudeOfAscendingNode = LongitudeOfAscendingNode;
                Secondary.MeanAnomaly = MeanAnomaly;
            }

            Vector3[] points = new Vector3[LineSegments];
            for (int i = 0; i < LineSegments; i++)
            {
                points[i] = SampleOrbit(MeanAnomaly + 360 * i / LineSegments);
            }
            line.SetPositions(points);

            Vector3 axis = new Vector3(Mathf.Cos(LongitudeOfAscendingNode * Mathf.Deg2Rad), 0, Mathf.Sin(LongitudeOfAscendingNode * Mathf.Deg2Rad));
            if (Parent != null)
            {
                Parent.localPosition = Vector3.zero;
                Parent.localRotation = Quaternion.identity;
                Parent.RotateAround(Parent.position, -transform.TransformDirection(axis), Inclination);
            }
            if (Satellite != null)
            {
                Satellite.localPosition = SampleOrbit(MeanAnomaly);
                Satellite.localRotation = Quaternion.identity;
                Satellite.RotateAround(Satellite.position, -transform.TransformDirection(axis), Inclination);
                Satellite.RotateAround(Satellite.position, -Satellite.up, MeanAnomaly + ArgumentOfPeriapsis + LongitudeOfAscendingNode + 90); // z+ inward
            }
        }

        public Vector3 SampleOrbit(float theta)
        {
            Vector3 point = Vector3.one * SemiMajorAxis * (1 - (Eccentricity * Eccentricity)) / (1 + Eccentricity * Mathf.Cos(theta * Mathf.Deg2Rad));
            point.x *= Mathf.Cos(LongitudeOfAscendingNode * Mathf.Deg2Rad) * Mathf.Cos((theta + ArgumentOfPeriapsis) * Mathf.Deg2Rad) - Mathf.Sin(LongitudeOfAscendingNode * Mathf.Deg2Rad) * Mathf.Sin((theta + ArgumentOfPeriapsis) * Mathf.Deg2Rad) * Mathf.Cos(Inclination * Mathf.Deg2Rad);
            point.z *= Mathf.Sin(LongitudeOfAscendingNode * Mathf.Deg2Rad) * Mathf.Cos((theta + ArgumentOfPeriapsis) * Mathf.Deg2Rad) + Mathf.Cos(LongitudeOfAscendingNode * Mathf.Deg2Rad) * Mathf.Sin((theta + ArgumentOfPeriapsis) * Mathf.Deg2Rad) * Mathf.Cos(Inclination * Mathf.Deg2Rad);
            point.y *= Mathf.Sin(Inclination * Mathf.Deg2Rad) * Mathf.Sin((theta + ArgumentOfPeriapsis) * Mathf.Deg2Rad);
            return point;
        }

        private void Awake()
        {
            liveEditing = false;
        }

        private void Update()
        {
            if (!Application.isPlaying && liveEditing)
            {
                BuildOrbit();
            }
        }
    }
}
