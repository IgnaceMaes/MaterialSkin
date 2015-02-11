using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialSkin.Animations
{
    enum AnimationType
    {
        Linear,
        EaseInOut,
    }

    static class AnimationLinear
    {
        public static double CalculateProgress(double progress)
        {
            return progress;
        }
    }

    static class AnimationEaseInOut
    {
        public static double PI = Math.PI;
        public static double PI_HALF = Math.PI / 2;

        public static double CalculateProgress(double progress)
        {
            return EaseInOut(progress);
        }

        private static double EaseInOut(double s)
        {
            return (Math.Sin(s * PI - PI_HALF) + 1) / 2;
        }
    }
}
