using UnityEngine;
using System.Collections;

namespace MoreFun
{
    public class QuaternionUtil
    {
        /// <summary>
        /// get the angle from dirFrom to dir dirTo, around axis.
        /// </summary>
        /// <returns>The angle from dirFrom to dirTo, around axis.</returns>
        /// <param name="dirFrom">Dir From.</param>
        /// <param name="dirTo">Dir To.</param>
        /// <param name="axis">Axis.</param>
        public static float AngleAroundAxis(Vector3 dirFrom, Vector3 dirTo, Vector3 axis)
        {
            // A和B需要投影到和axis垂直的平面进行角度比较。

            dirFrom = dirFrom - Vector3.Project(dirFrom, axis);
            dirTo = dirTo - Vector3.Project(dirTo, axis);

            float angle = Vector3.Angle(dirFrom, dirTo);

            if (Vector3.Dot(axis, Vector3.Cross(dirFrom, dirTo)) < 0)
            {
                return -angle;
            }
            else
            {
                return angle;
            }
        }
    }

}