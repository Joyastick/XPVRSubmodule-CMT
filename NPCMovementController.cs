using System;
using System.Collections;
using MyBox;
using UnityEngine;

namespace ConversationMatrixTool
{
    //this script moves NPCs on a curve with constant speed therefore enabling its use for kinetic animations like running or walking
    public class NPCMovementController : MonoBehaviour
    {
        [Tooltip("parent transform of the character to be moved")]
        public Transform nPCToMove;
        [Tooltip("transforms of control points for the curve")]
        public Transform[] controlPoints;
        private Vector3 gizmoPos;
        private int numberOfPoints = 333;
        [Tooltip("this value determines the position of the NPC on the curve")]
        private float testT;
        private Vector3 lookAtPoint;
        [Tooltip("this distance is used to calculate the facing position of the NPC while moving on the curve")]
        [Range(0, 1)] public float lookAtDistance;
        private Quaternion quRot;
        private Coroutine cR_Move;
        private WaitForEndOfFrame wait;
        private bool preview;
        [Tooltip("duration of movement on the curve - preview DOES NOT work in editor, only at runtime")]
        public float duration;
        private float linearDistance;
        private Vector3 currentPosition;
        private float speedConstant;//distance per second to finish the travel on the curve within the duration
        private float curveLength;
        private int steps = 6666;//can go higher to make more precise calculation
        private float step;
/*       [ButtonMethod()]
        private void Preview()
        {
            testT = 0f;
            preview = true;
            CalculateCurveLength();
        }        
*/
        //[ButtonMethod()]
        private void CalculateCurveLength()
        {
            curveLength = 0f;
            step = 1f / steps;
            var t = 0f;
            var oldT = 0f;
            linearDistance = 0f;
            for (int i = 0; i < steps; i++)
            {
                oldT = t;
                t = step * i;
                linearDistance = Vector3.Distance(PositionOnCurve(t), PositionOnCurve(oldT));
                curveLength += linearDistance;
            }
            speedConstant = curveLength / duration;
            linearDistance = 0f;
        }

        private void OnValidate()
        {
            CalculateCurveLength();
        }

        private void OnDrawGizmos()
        {
            DrawPoints(numberOfPoints);
        }
        private void Update()
        {
            if (preview)
            {
                if (testT < 1f)
                {
                    var currentPos = PlaceOnCurve(testT, nPCToMove);
                    linearDistance = Vector3.Distance(currentPos, PositionOnCurve(testT + (Time.deltaTime / duration)));
                    testT += ((speedConstant * Time.deltaTime) / linearDistance) * (Time.deltaTime / duration);
                }
                else
                {
                    testT = 0f;
                    preview = false;
                }
            }
        }

        public void MoveNPC(float start, float end, float _duration)
        {
            if (nPCToMove == null) return;
            if (wait == null) wait = new WaitForEndOfFrame();
            CalculateCurveLength();
            speedConstant = curveLength / duration;
            if (cR_Move != null) StopCoroutine(cR_Move);
            cR_Move = StartCoroutine(CR_MoveNPC(start, end, _duration)); //duration is in seconds
        }

        IEnumerator CR_MoveNPC(float start, float end, float _duration)
        {
            CalculateCurveLength();
            var t = start;
            while (t < end)
            {
                var currentPos = PlaceOnCurve(t);
                linearDistance = Vector3.Distance(currentPos, PositionOnCurve(t + (Time.deltaTime / duration)));
                t += ((speedConstant * Time.deltaTime) / linearDistance) * (Time.deltaTime / duration);
                yield return wait;
            }
        }

        public void MoveNPC(float _duration)
        {
            MoveNPC(0f, 1f, _duration);
        }

        public void MoveNPC()
        {
            MoveNPC(0f, 1f, duration);
        }

        public void DrawPoints(int _numberOfPoints)
        {
            if (controlPoints == null) return;
            if (controlPoints.Length < 4) return;
            Gizmos.color = Color.white;
            var increment = 1f / _numberOfPoints;
            for (float t = 0f; t <= 1f; t += increment)
            {
                gizmoPos = PositionOnCurve(t);
                Gizmos.DrawSphere(gizmoPos, .015f);
            }

            //lines for control points
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);
            Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);

            //spheres for control point heads
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(controlPoints[0].position, .20f);
            Gizmos.DrawSphere(controlPoints[3].position, .20f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(controlPoints[1].position, .0666f);
            Gizmos.DrawSphere(controlPoints[2].position, .0666f);

            //POSITION AND ROTATION INDICATOR
            Gizmos.color = Color.magenta;
            var pos = PositionOnCurve(testT);
            Gizmos.DrawSphere(pos, .13f);
            DrawArrowTo(pos, PositionOnCurve(testT + lookAtDistance));
        }

        public Vector3 PositionOnCurve(float t)
        {

            var position = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
                   3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
                   3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
                   Mathf.Pow(t, 3) * controlPoints[3].position;
            return position;
        }

        public Vector3
            PlaceOnCurve(float t, Transform subject, bool doNotRotate,
                bool reverse) //defines the direction movement on the curve
        {
            subject.position = PositionOnCurve(t);
            if (!doNotRotate)
            {
                lookAtPoint = PositionOnCurve(Mathf.Clamp01(reverse ? t - lookAtDistance : t + lookAtDistance));
                subject.localRotation = Quaternion.LookRotation(
                    lookAtPoint - subject.position, Vector3.up
                );
            }
            return subject.position;
        }

        public Vector3
            PlaceOnCurve(float t, Transform subject) //override with less parameters keeps my code cleaner
        {
            return PlaceOnCurve(t, subject, false, false);
        }

        public Vector3
            PlaceOnCurve(float t)
        {
            if (nPCToMove == null) return Vector3.zero;
            return PlaceOnCurve(t, nPCToMove, false, false);
        }

        private const float arrowAngle = 16f;
        private float arrowHeadSize = .33f;

        private void DrawArrowTo(Vector3 from, Vector3 to)
        {
            var distance = Vector3.Distance(from, to);
            var tip = from + (to - from);
            Gizmos.DrawLine(from, tip);
            var currentAngle = AngleTo(
                new Vector2(from.x, from.z),
                new Vector2(tip.x, tip.z)
            );
            var angle = currentAngle + arrowAngle;
            var x = arrowHeadSize * Mathf.Cos(angle * Mathf.Deg2Rad);
            var z = arrowHeadSize * Mathf.Sin(angle * Mathf.Deg2Rad);
            var rArrowPoint = tip - new Vector3(x, 0f, z);

            Gizmos.DrawLine(tip, rArrowPoint);

            angle = currentAngle - arrowAngle;
            x = arrowHeadSize * Mathf.Cos(angle * Mathf.Deg2Rad);
            z = arrowHeadSize * Mathf.Sin(angle * Mathf.Deg2Rad);
            var lArrowPoint = tip - new Vector3(x, 0f, z);

            Gizmos.DrawLine(tip, lArrowPoint);
        }

        private float AngleTo(Vector2 pos, Vector2 target)
        {
            Vector2 difference = target - pos;
            float sign = (target.y < pos.y) ? -1.0f : 1.0f;
            return Vector2.Angle(Vector2.right, difference) * sign;
        }
    }
}