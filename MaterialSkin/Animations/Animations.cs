namespace MaterialSkin.Animations
{
    using System;

    /// <summary>
    /// Defines the AnimationType
    /// </summary>
    internal enum AnimationType
    {
        /// <summary>
        /// Defines the Linear
        /// </summary>
        Linear,

        /// <summary>
        /// Defines the EaseInOut
        /// </summary>
        EaseInOut,

        /// <summary>
        /// Defines the EaseOut
        /// </summary>
        EaseOut,

        /// <summary>
        /// Defines the CustomQuadratic
        /// </summary>
        CustomQuadratic
    }

    /// <summary>
    /// Defines the <see cref="AnimationLinear" />
    /// </summary>
    internal static class AnimationLinear
    {
        /// <summary>
        /// The CalculateProgress
        /// </summary>
        /// <param name="progress">The progress<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateProgress(double progress)
        {
            return progress;
        }
    }

    /// <summary>
    /// Defines the <see cref="AnimationEaseInOut" />
    /// </summary>
    internal static class AnimationEaseInOut
    {
        /// <summary>
        /// Defines the PI
        /// </summary>
        public static double PI = Math.PI;

        /// <summary>
        /// Defines the PI_HALF
        /// </summary>
        public static double PI_HALF = Math.PI / 2;

        /// <summary>
        /// The CalculateProgress
        /// </summary>
        /// <param name="progress">The progress<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateProgress(double progress)
        {
            return EaseInOut(progress);
        }

        /// <summary>
        /// The EaseInOut
        /// </summary>
        /// <param name="s">The s<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        private static double EaseInOut(double s)
        {
            return s - Math.Sin(s * 2 * PI) / (2 * PI);
        }
    }

    /// <summary>
    /// Defines the <see cref="AnimationEaseOut" />
    /// </summary>
    public static class AnimationEaseOut
    {
        /// <summary>
        /// The CalculateProgress
        /// </summary>
        /// <param name="progress">The progress<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateProgress(double progress)
        {
            return -1 * progress * (progress - 2);
        }
    }

    /// <summary>
    /// Defines the <see cref="AnimationCustomQuadratic" />
    /// </summary>
    public static class AnimationCustomQuadratic
    {
        /// <summary>
        /// The CalculateProgress
        /// </summary>
        /// <param name="progress">The progress<see cref="double"/></param>
        /// <returns>The <see cref="double"/></returns>
        public static double CalculateProgress(double progress)
        {
            var kickoff = 0.6;
            return 1 - Math.Cos((Math.Max(progress, kickoff) - kickoff) * Math.PI / (2 - (2 * kickoff)));
        }
    }
}
