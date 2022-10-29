using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviourGraph.Runtime.Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviourGraph.Runtime.Utilities
{
    public static class TypeLookup
    {
        public static FieldInfo[] GetFieldInfos(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        public static List<FieldInfo> GetVariables(Type type)
        {
            return GeVariablesBaseType(type)
                // remove duplicate variable taken from base class and derive class
                .DistinctBy(x => x.Name)
                // order fields according to FieldOrder attribute
                .OrderBy(x => x.GetCustomAttribute<FieldOrder>()?.Order ?? int.MaxValue)
                // change to list
                .ToList();
        }

        private static List<FieldInfo> GeVariablesBaseType(Type type)
        {
            var list = new List<FieldInfo>();

            if (type != null)
            {
                var fields = GetFieldInfos(type);

                list.AddRange(fields.Where(x => (x.HasAttribute<SerializeField>() || (x.IsPublic && !x.IsInitOnly)) && !x.HasAttribute<HideInInspector>()).ToList());

                list.AddRange(GeVariablesBaseType(type.BaseType));
            }

            return list;
        }
    }
}
