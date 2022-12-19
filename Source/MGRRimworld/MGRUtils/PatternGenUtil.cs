using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MGRRimworld.MGRUtils
{
    internal class PatternGenUtil
    {

        public static float CalcSlope(float x, Vector2 line)
        {
            return line.x * x + line.y;
        }

        public static Vector2 GetLine(float x1, float x2, float y1, float y2)
        {
            float m = (y2 - y1) / (x2 - x1);
            float b = y1 - (m * x1);
            return new Vector2(m, b);
        }

        public static Vector2 GetLine(Vector3 target1, Vector3 target2)
        {
            return GetLine(target1.x, target2.x, target1.z, target2.z);
        }

        public static IEnumerable<Vector2> SubPoint(Vector3 startPoint, Vector3 endPoint, int segment, int totalSegments)
        {
            if (segment == totalSegments)
                yield break;
            float midX = (startPoint.x + (int)((double)(endPoint.x - startPoint.x) / (double)totalSegments) * segment);
            float midZ = (startPoint.z + (int)((double)(endPoint.z - startPoint.y) / (double)totalSegments) * segment);
            Vector3 vec3 = new Vector3(midX, 0, midZ);
            yield return GetLine(startPoint, vec3);
            foreach (Vector2 v in SubPoint(vec3, endPoint, segment++, totalSegments))
                yield return v;

        }

        public static double CalcStandingAngle(Thing Caster, IntVec3 target)
        {
            return Math.Atan2(target.z - Caster.Position.z, target.x - Caster.Position.x) * (180 / Math.PI);
        }

        public static double CalcStandingAngle(IntVec3 Caster, IntVec3 target)
        {
            return Math.Atan2(target.z - Caster.z, target.x - Caster.x) * (180 / Math.PI);
        }

        public static double ConvertDegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        public static IEnumerable<IntVec3> FractalTree(IntVec3 target, double angle, int spread, int depth)
        {
            if (!target.IsValid || depth <= 1)
                yield break;
            yield return new IntVec3(target.x, 0, target.z);
            foreach (IntVec3 loc in FractalTree(target.x, target.z, angle, spread, depth - 1))
                yield return loc;
        }

        public static IEnumerable<IntVec3> FractalTree(int x1, int y1, double angle, int spread, int depth)
        {
            if (depth <= 1)
            {
                yield break;
            }
            else
            {
                int x2 = x1 + (int)(Math.Cos(ConvertDegreesToRadians(angle)) * depth);
                int y2 = y1 + (int)(Math.Sin(ConvertDegreesToRadians(angle)) * depth);
                yield return new IntVec3(x2, 0, y2);
                foreach (IntVec3 loc1 in FractalTree(x2, y2, angle - spread, spread, depth - 1))
                    yield return loc1;
                foreach (IntVec3 loc2 in FractalTree(x2, y2, angle + spread, spread, depth - 1))
                    yield return loc2;
            }
        }
    }
}
