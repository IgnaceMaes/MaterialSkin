namespace MaterialSkin
{
    using System;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="Extensions" />
    /// These add functions on default C# types and classes
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
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Transforms every char on a string to this dot: ●
        /// </summary>
        /// <param name="plainString"></param>
        /// <returns></returns>
        public static string ToSecureString(this string plainString)
        {
            if (plainString == null)
                return null;

            string secureString = "";
            for (uint i = 0; i < plainString.Length; i++)
            {
                secureString += '\u25CF';
            }
            return secureString;
        }

        // Color extensions
        public static Color ToColor(this int argb)
        {
            return Color.FromArgb(
                (argb & 0xFF0000) >> 16,
                (argb & 0x00FF00) >> 8,
                 argb & 0x0000FF);
        }

        public static Color RemoveAlpha(this Color color)
        {
            return Color.FromArgb(color.R, color.G, color.B);
        }

        public static int PercentageToColorComponent(this int percentage)
        {
            return (int)((percentage / 100d) * 255d);
        }
    }
}
