using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaterialSkin
{
   static class Extensions
    {
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


        public static bool IsMaterialControl(this Object obj)
        {
            var type = obj.GetType();
            return IsAbstractOf<IMaterialControl>(type);
        }

        public static bool IsAbstractOf<TInterface>(this Type type)
        {
            try { 
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
