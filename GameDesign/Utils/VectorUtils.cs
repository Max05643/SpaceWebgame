using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDesign.Utils
{
    public static class VectorUtils
    {
        /// <summary>
        /// Moves current towards target
        /// </summary>
        public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float maxDistanceDelta)
        {
            Vector2 vectorToTarget = target - current;
            float distance = vectorToTarget.Length();

            if (distance <= maxDistanceDelta || distance == 0f)
            {
                return target;
            }

            return current + vectorToTarget / distance * maxDistanceDelta;
        }
    }
}
