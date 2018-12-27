namespace MaterialSkin
{
    using System;
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
            var type = obj.GetType();
            return IsAbstractOf<IMaterialControl>(type);
        }

        /// <summary>
        /// The IsAbstractOf
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="type">The type<see cref="Type"/></param>
        /// <returns>The <see cref="bool"/></returns>
        public static bool IsAbstractOf<TInterface>(this Type type)
        {
            try
            {
                var map = type.GetInterfaceMap(typeof(TInterface));
                foreach (var info in map.TargetMethods)
                {
                    if (!info.IsAbstract)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
