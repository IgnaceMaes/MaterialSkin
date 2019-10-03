namespace MaterialSkin
{
    using System;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="Extensions" />
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// The HasProperty
        /// </summary>
        /// <param name="objectToCheck">The objectToCheck<see cref="object"/></param>
        /// <param name="propertyName">The propertyName<see cref="string"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool HasProperty(this object objectToCheck, string propertyName)
        {
            try
            {
                var type = objectToCheck.GetType();

                return type.GetProperty(propertyName) != null;
            }
            catch (AmbiguousMatchException)
            {
                // ambiguous means there is more than one result,
                // which means: a method with that name does exist
                return true;
            }
        }

        /// <summary>
        /// The IsMaterialControl
        /// </summary>
        /// <param name="obj">The obj<see cref="Object"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsMaterialControl(this Object obj)
        {
            if (obj is IMaterialControl)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
