using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace GameCore
{

    public static class Services
    {
        private static Dictionary<Type, Object> services = new Dictionary<Type, Object>();

        public static T Get<T>() where T : class
        {
            Object obj;
            services.TryGetValue(typeof (T), out obj);
            return obj as T;
        }

        public static Object Get(Type type)
        {
            Object obj;
            services.TryGetValue(type, out obj);
            return obj;
        }

        public static bool Contains<T>() where T : class
        {
            Object obj;
            services.TryGetValue(typeof (T), out obj);
            return obj as T != null;
        }

        public static void Register(Type type, Object obj)
        {
            services.Add(type, obj);
        }


        public static bool RegisterIfNew(Type type, Object obj)
        {
            if (!services.ContainsKey(type))
            {
                services.Add(type, obj);
                return true;
            }
            return false;
        }

        public static void Override(Type type, Object obj)
        {
            services[type] = obj;
        }


        public static Callback Callback
        {
            get { return Get<Callback>(); }
        }

    }

}