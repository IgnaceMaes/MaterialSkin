namespace MaterialSkin.Animations
{
    /// <summary>
    /// Defines the AnimationDirection
    /// </summary>
    internal enum AnimationDirection
    {
        /// <summary>
        /// Defines the In
        /// </summary>
        In, //In. Stops if finished.

        /// <summary>
        /// Defines the Out
        /// </summary>
        Out, //Out. Stops if finished.

        /// <summary>
        /// Defines the InOutIn
        /// </summary>
        InOutIn, //Same as In, but changes to InOutOut if finished.

        /// <summary>
        /// Defines the InOutOut
        /// </summary>
        InOutOut, //Same as Out.

        /// <summary>
        /// Defines the InOutRepeatingIn
        /// </summary>
        InOutRepeatingIn, // Same as In, but changes to InOutRepeatingOut if finished.

        /// <summary>
        /// Defines the InOutRepeatingOut
        /// </summary>
        InOutRepeatingOut// Same as Out, but changes to InOutRepeatingIn if finished.   
    }
}
