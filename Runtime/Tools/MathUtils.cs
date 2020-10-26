using System;

namespace Core.Runtime.Tools
{
    public class MathUtils
    {
        /// <summary>
        /// 比较Float
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool FloatEquals(float num1, float num2, float threshold = 0.0001f)
        {
            return Math.Abs(num1 - num2) < threshold;
        }
    }
}