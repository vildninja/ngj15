using UnityEngine;
using System.Collections;

namespace GameCore
{
    public static class GMath
    {

        public static Vector3 AngleToSpherePosition(Vector3 eulerAngles, float radius)
        {
            eulerAngles = eulerAngles.ToRadians();
            var angles = new Vector3();
            angles.x = radius*Mathf.Cos(eulerAngles.y)*Mathf.Cos(eulerAngles.x);
            angles.y = radius*Mathf.Sin(eulerAngles.y);
            angles.z = radius*Mathf.Cos(eulerAngles.y)*Mathf.Sin(eulerAngles.x);

            return angles;
        }

        public static Vector3 Rotate(Vector3 v, Vector3 axis, float degrees)
        {
            var quat = Quaternion.AngleAxis(degrees, axis.normalized);
            return quat*v;
        }

        public static float Normalize(float x, float y)
        {
            return Mathf.Clamp(Mathf.Abs(x) + Mathf.Abs(y), 0, 1)/2f;
        }


        public static void RotateTowardsFlat(Transform Root, Vector3 targetDir, float maxEulerAngle)
        {
            Vector3 newDir = Vector3.RotateTowards(Root.forward.normalized, targetDir.normalized,
                maxEulerAngle*Mathf.Deg2Rad*GameTime.deltaTime, 10000);
            Root.rotation = Quaternion.LookRotation(newDir, Vector3.up);
            Root.rotation = Quaternion.Euler(new Vector3(0, Root.rotation.eulerAngles.y, 0));
        }

        public static Vector2 Angle(float angle)
        {
            float X1 = Mathf.Cos(angle) - Mathf.Sin(angle);
            float Y1 = Mathf.Sin(angle) + Mathf.Cos(angle);
            return new Vector2(X1, Y1);
        }

        public static float Atan2ToDeg(Vector2 v)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(-v.x, v.y);
        }

        public static float Atan2(Vector2 v)
        {
            return Mathf.Atan2(-v.x, v.y);
        }

        public static float Atan2ToDeg(Vector3 v)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(-v.x, v.z);
        }

        public static float Atan2(Vector3 v)
        {
            return Mathf.Atan2(-v.x, v.z);
        }

        public static Vector3 FromTo(Vector3 from, Vector3 to)
        {
            return to - from;
        }

        public static Vector3 FromTo(Vector3 from, Vector3 to, float distance)
        {
            return (to - from).normalized*distance;
        }

        public static int MaskOut(int remove)
        {
            int mask = ~(1 << remove);
            return mask;
        }

        public static int MaskOut(this int number, int remove)
        {
            int layerMask = ~(1 << remove);
            number = number & layerMask;
            return number;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle*(point - pivot) + pivot;
        }

        public static float PointToSegmentDistance(Vector3 P, Vector3 S0, Vector3 S1)
        {
            Vector3 unused;
            return PointToSegmentDistance(P, S0, S1, out unused);
        }

        public static float PointToSegmentDistance(Vector3 P, Vector3 S0, Vector3 S1, out Vector3 point)
        {
            Vector3 v = S1 - S0;
            Vector3 w = P - S0;


            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
            {
                point = S0;
                return Vector3.Distance(P, S0);
            }

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
            {
                point = S1;
                return Vector3.Distance(P, S1);
            }

            float b = c1/c2;
            point = S0 + b*v;
            return Vector3.Distance(P, point);
        }

        public static Vector3 Abs(Vector3 v)
        {
            return new Vector3(
                Mathf.Abs(v.x),
                Mathf.Abs(v.y),
                Mathf.Abs(v.z));
        }

        public static float AbsoluteAngle(Vector3 vectorA, Vector3 vectorB)
        {
            float angle = Vector3.Angle(vectorA, vectorB);
            Vector3 cross = Vector3.Cross(vectorA, vectorB);
            if (cross.y < 0) 
                angle = -angle;
            return angle;
        }
    }


    public static class MathExtensions
    {
        public static Vector3 LocalRadianAngles(this Transform t)
        {
            Vector3 angle = t.localEulerAngles;
            return new Vector3(
                Mathf.Deg2Rad*angle.x,
                Mathf.Deg2Rad*angle.y,
                Mathf.Deg2Rad*angle.z
                );
        }

        public static Vector3 RadianAngles(this Transform t)
        {
            Vector3 angle = t.eulerAngles;
            return new Vector3(
                Mathf.Deg2Rad*angle.x,
                Mathf.Deg2Rad*angle.y,
                Mathf.Deg2Rad*angle.z
                );
        }

        public static Vector3 ToRadians(this Vector3 angle)
        {
            return new Vector3(
                Mathf.Deg2Rad*angle.x,
                Mathf.Deg2Rad*angle.y,
                Mathf.Deg2Rad*angle.z
                );
        }

        public static Vector3 ToDegrees(this Vector3 angle)
        {
            return new Vector3(
                Mathf.Rad2Deg*angle.x,
                Mathf.Rad2Deg*angle.y,
                Mathf.Rad2Deg*angle.z
                );
        }

        
    }
}