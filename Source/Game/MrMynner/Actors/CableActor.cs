using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner.Actors
{
    public class CableActor : Spline
    {
        [Serialize]
        public SplineModel model;

        [Serialize]
        public SplineCollider collider;

        public Actor target;

        [Serialize]
        public float maxCableLength = 300;

        #region Props

        public bool SplineHasTwoPoints{ get => SplinePointsCount > 1;}

        #endregion

        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            model = GetOrAddChild<SplineModel>();
            collider = GetOrAddChild<SplineCollider>();
        }

        public override void UpdateSpline()
        {
            base.UpdateSpline();
            SmoothSpline();
        }

        public bool IsIndexValid(int idx)
        {
            return idx >= 0 && idx < SplinePointsCount;
        }

        public float GetSplineLength(int sampleCount = 100)
        {
            if(SplinePointsCount < 2){return 0f;}

            float beginTime = GetSplineTime(0);
            float endTime = GetSplineTime(SplinePointsCount - 1);
            float splineTime = endTime - beginTime;

            float timeStep = splineTime / (float)sampleCount;
            float curTime = beginTime;

            float distance = 0f;

            Vector3 lastPointPos = GetSplinePoint(curTime);

            while(curTime < endTime)
            {
                Vector3 curPointPos = GetSplinePoint(curTime);
                distance += Vector3.Distance(lastPointPos, curPointPos);
                lastPointPos = curPointPos;
                curTime += timeStep;
            }

            return distance;
        }

        public void MovePoint(int pointIdx, Vector3 newPointPos)
        {
            if(!SplineHasTwoPoints || !IsIndexValid(pointIdx)){return;}

            Vector3 oldPointPos = GetSplinePoint(pointIdx);
            Vector3 moveDir = Vector3.Normalize(newPointPos - oldPointPos);
            float moveDist = Vector3.Distance(oldPointPos, newPointPos);
        
            float oldSplineLen = SplineLength;

            float cableRadius = 1f;

            int backPointIdx = pointIdx - 1;
            int nextPointIdx = pointIdx + 1;

            if(pointIdx == 0)
            {
                backPointIdx = pointIdx;
            }
            else if(pointIdx == SplinePointsCount - 1)
            {
                nextPointIdx = pointIdx;
            }

            Vector3[] oldIntervalPoints = GetSplineIntervalPoints(backPointIdx, nextPointIdx, cableRadius);

            newPointPos = PathSphereCast(oldPointPos, newPointPos, cableRadius);
            SetSplinePoint(pointIdx, newPointPos);
            float newSplineLen = SplineLength;

            Vector3[] newIntervalPoints = GetSplineIntervalPoints(backPointIdx, nextPointIdx, cableRadius);

            if(newSplineLen > maxCableLength || CheckIntervalMove(oldIntervalPoints, newIntervalPoints, cableRadius))
            {
                SetSplinePoint(pointIdx, oldPointPos);
            }
        }

        public float GetTimeStepUsingRadius(float radius)
        {
            if(SplinePointsCount < 2){return 0f;}

            float beginTime = GetSplineTime(0);
            float endTime = GetSplineTime(SplinePointsCount - 1);
            float time = endTime - beginTime;

            return radius * time / SplineLength;
        }

        public bool CheckIntervalFree(int aIdx, int bIdx)
        {
            return false;
        }
    
        public void SmoothSpline()
        {
            if(SplinePointsCount > 1)
            {
                for(int pointIdx = 0; pointIdx < SplinePointsCount; pointIdx++)
                {
                    SmoothPoint(pointIdx);
                }
            }
        }

        public void SmoothPoint(int pointIdx, float distIntensity = 0.25f)
        {
            if(SplinePointsCount < 1 || pointIdx >= SplinePointsCount){return;}

            if(pointIdx == 0)// Takes the init spline point
            {
                Transform nextInTan = GetSplineTangent(pointIdx + 1, true);
                SetSplineTangent(0, nextInTan, false);
            }
            else if(pointIdx == SplinePointsCount - 1)// Takes the end spline point
            {
                Transform backOutTan = GetSplineTangent(pointIdx - 1, false);
                SetSplineTangent(0, backOutTan, true);
            }
            else// takes mid spline points
            {
                Vector3 backPointPos = GetSplinePoint(pointIdx - 1);
                Vector3 nextPointPos = GetSplinePoint(pointIdx + 1);
                Vector3 curPointPos = GetSplinePoint(pointIdx);

                Vector3 backPointToCurPointDir = Vector3.Normalize(curPointPos - backPointPos);
                Vector3 nextPointToCurPointDir = Vector3.Normalize(curPointPos - nextPointPos);

                float backPointToCurPointDist = Vector3.Distance(backPointPos, curPointPos);
                float nextPointToCurPointDist = Vector3.Distance(nextPointPos, curPointPos);

                float backPointToNextPointAngle = Vector3.Angle(backPointPos - curPointPos, nextPointPos - curPointPos);
                float backPointToNextPointAngleNormalized = backPointToNextPointAngle / 180f;
                float invBackPointToNextPointAngleNormalized = 1f - backPointToCurPointDist;

                Vector3 inSmoothPoint = curPointPos + nextPointToCurPointDir * backPointToCurPointDist * distIntensity;
                Vector3 outSmoothPoint = curPointPos + backPointToCurPointDir * nextPointToCurPointDist * distIntensity;

                Vector3 rayCastedInSmoothPoint = TangentPathRayCast(curPointPos, inSmoothPoint);
                Vector3 rayCastedOutSmoothPoint = TangentPathRayCast(curPointPos, outSmoothPoint);

                SetSplineTangent(pointIdx, new Transform(rayCastedInSmoothPoint), true);
                SetSplineTangent(pointIdx, new Transform(rayCastedOutSmoothPoint), false);
            }
        }

        private Vector3 TangentPathRayCast(Vector3 origin, Vector3 point)
        {
            Vector3 dir = Vector3.Normalize(point - origin);
            float distance = Vector3.Distance(origin, point);

            RayCastHit hit;
            if(Physics.RayCast(origin, dir, out hit, distance, uint.MaxValue, false))
            {
                return hit.Point;
            }

            return point;
        }

        private Vector3 PathRayCast(Vector3 origin, Vector3 point)
        {
            Vector3 dir = Vector3.Normalize(point - origin);
            float distance = Vector3.Distance(origin, point);

            RayCastHit hit;
            if(Physics.RayCast(origin, dir, out hit, distance, uint.MaxValue, false))
            {
                return hit.Point;
            }

            return point;
        }

        private Vector3 PathSphereCast(Vector3 origin, Vector3 point, float radius, bool skipSelf = false)
        {
            Vector3 dir = Vector3.Normalize(point - origin);

            if(skipSelf)
            {
                origin += dir * radius;
            }

            float distance = Vector3.Distance(origin, point);

            RayCastHit hit;
            if(Physics.SphereCast(origin, radius, dir, out hit, distance, uint.MaxValue, false))
            {
                return hit.Point + (dir * -1f) * (radius * 0.5f);
            }

            return point;
        }

        public Vector3[] GetCollisionPointsBeetwenPoints(int aPointIdx, int bPointIdx, float cableRadius, int sampleCount = 100)
        {
            if(!SplineHasTwoPoints || !IsIndexValid(aPointIdx) || !IsIndexValid(bPointIdx)){return null;}

            float aTime = GetSplineTime(aPointIdx);
            float bTime = GetSplineTime(bPointIdx);

            float timeStep = GetTimeStepUsingRadius(cableRadius);
            List<Vector3> contactsPoints = new List<Vector3>(20);

            while(aTime < bTime)
            {
                Vector3 pos = GetSplinePoint(aTime);

                if(Physics.CheckSphere(pos, cableRadius, uint.MaxValue, false))
                {
                    contactsPoints.Add(pos);
                }

                aTime += timeStep;
            }

            return contactsPoints.ToArray();
        }

        public bool CheckCollisionBeetwenPoints(int aPointIdx, int bPointIdx, float cableRadius, int sampleCount = 100)
        {
            if(!SplineHasTwoPoints || !IsIndexValid(aPointIdx) || !IsIndexValid(bPointIdx)){return false;}

            float aTime = GetSplineTime(aPointIdx);
            float bTime = GetSplineTime(bPointIdx);

            float timeStep = GetTimeStepUsingRadius(cableRadius);

            while(aTime < bTime)
            {
                Vector3 pos = GetSplinePoint(aTime);

                if(Physics.CheckSphere(pos, cableRadius, uint.MaxValue, false))
                {
                    return true;
                }

                aTime += timeStep;
            }

            return false;
        }

        public bool CheckCollisionBeetwenPointsOnMove(int pointIdx, Vector3 newPointPos)
        {
            if(!SplineHasTwoPoints || !IsIndexValid(pointIdx)){return false;}

            float backPointTime = GetSplineTime(pointIdx - 1);
            float currPointTime = GetSplineTime(pointIdx);
            float nextPointTime = GetSplineTime(pointIdx + 1);

            Vector3 oldPointPos = GetSplinePoint(pointIdx);
            Vector3 moveDir = Vector3.Normalize(newPointPos - oldPointPos);
            float moveDist = Vector3.Distance(oldPointPos, newPointPos);

            return false;
        }

        public Vector3[] GetSplineIntervalPoints(int aPointIdx, int bPointIdx, float cableRadius)
        {
            if(!SplineHasTwoPoints || !IsIndexValid(aPointIdx) || !IsIndexValid(bPointIdx)){return null;}

            float aTime = GetSplineTime(aPointIdx);
            float bTime = GetSplineTime(bPointIdx);

            float majorTime = Mathf.Max(aTime, bTime);
            float minorTime = Mathf.Min(aTime, bTime);

            float totalTime = majorTime - minorTime;

            float timeStep = GetTimeStepUsingRadius(cableRadius);
            int intervalPointCount = Mathf.FloorToInt(totalTime / timeStep);

            Vector3[] points = new Vector3[intervalPointCount];

            for(int timeStepMult = 0; timeStepMult < intervalPointCount; timeStepMult++)
            {
                float pointTime = minorTime + timeStepMult * timeStep;
                Vector3 pointPos = GetSplinePoint(pointTime);
                points[timeStepMult] = pointPos;
            }

            return points;
        }
    
        public bool CheckIntervalMove(Vector3[] oldIntervalPoints, Vector3[] newIntervalPoints, float cableRadius)
        {
            if(oldIntervalPoints == null || newIntervalPoints == null){return false;}

            int minorLen = Mathf.Min(oldIntervalPoints.Length, newIntervalPoints.Length);

            for(int idx = 0; idx < minorLen; idx++)
            {
                Vector3 oldPointPos = oldIntervalPoints[idx];
                Vector3 newPointPos = newIntervalPoints[idx];

                Vector3 moveDir = Vector3.Normalize(newPointPos - oldPointPos);
                float moveDist = Vector3.Distance(oldPointPos, newPointPos);

                RayCastHit hit;
                if(Physics.SphereCast(oldPointPos, cableRadius, moveDir, out hit, moveDist, uint.MaxValue, false))
                {
                    if(hit.Distance < moveDist){return true;}
                }
            }

            return false;
        }
    }
}
