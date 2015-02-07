using UnityEngine;
using System.Collections;

namespace GameCore
{

    public static class GameDebug
    {
        public static void Log(string s, Object o)
        {
            Debug.Log(s, o);
        }

        public static void Log<T>(T s)
        {
            Debug.Log(s);
        }


        public static void Log<A, B>(A a, B b)
        {
            Debug.Log(a + " " + b);
        }

        public static void Log<A, B, C>(A a, B b, C c)
        {
            Debug.Log(a + " " + b + " " + c);
        }

        public static void Log<A, B, C, D>(A a, B b, C c, D d)
        {
            Debug.Log(a + " " + b + " " + c + " " + d);
        }

        public static void Log<A, B, C, D, E>(A a, B b, C c, D d, E e)
        {
            Debug.Log(a + " " + b + " " + c + " " + d + " " + e);
        }

        public static void LogWarning<A, B>(A a, B b)
        {
            Debug.LogWarning(a + " " + b);
        }

        public static void LogWarning<A, B, C>(A a, B b, C c)
        {
            Debug.LogWarning(a + " " + b + " " + c);
        }

        public static void LogWarning<A, B, C, D>(A a, B b, C c, D d)
        {
            Debug.LogWarning(a + " " + b + " " + c + " " + d);
        }

        public static void LogWarning<A, B, C, D, E>(A a, B b, C c, D d, E e)
        {
            Debug.LogWarning(a + " " + b + " " + c + " " + d + " " + e);
        }

        public static void LogError<A, B>(A a, B b)
        {
            Debug.LogError(a + " " + b);
        }

        public static void LogError<A, B, C>(A a, B b, C c)
        {
            Debug.LogError(a + " " + b + " " + c);
        }

        public static void LogError<A, B, C, D>(A a, B b, C c, D d)
        {
            Debug.LogError(a + " " + b + " " + c + " " + d);
        }

        public static void LogError<A, B, C, D, E>(A a, B b, C c, D d, E e)
        {
            Debug.LogError(a + " " + b + " " + c + " " + d + " " + e);
        }
    }

}