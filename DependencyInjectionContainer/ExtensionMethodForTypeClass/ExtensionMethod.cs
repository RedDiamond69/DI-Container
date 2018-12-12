using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjectionContainer.ExtensionMethodForTypeClass
{
    public static class ExtensionMethod
    {
        public static bool IsAssignableFromGeneric(this Type type, Type t)
        {
            if (!type.IsGenericTypeDefinition || !t.IsGenericTypeDefinition)
                throw new ArgumentException("Specified types should be generic");
            Type comparedType, baseType;
            Queue<Type> baseTypes = new Queue<Type>();
            baseTypes.Enqueue(t);
            bool result = false;
            do
            {
                comparedType = baseTypes.Dequeue();
                baseType = comparedType.BaseType;
                if ((baseType != null) && (baseType.IsGenericType || baseType.IsGenericTypeDefinition))
                    baseTypes.Enqueue(baseType.GetGenericTypeDefinition());
                foreach (Type baseInterface in 
                    comparedType.GetInterfaces()
                    .Where((intf) => intf.IsGenericType || intf.IsGenericTypeDefinition))
                    baseTypes.Enqueue(baseInterface.GetGenericTypeDefinition());
                result = comparedType == type;
            } while (!result && (baseTypes.Count > 0));
            return result;
        }
    }
}
