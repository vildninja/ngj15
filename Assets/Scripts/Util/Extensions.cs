using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;


namespace UtilExtensions
{
    
    public static class AudioSourceExtension
    {
        public static float ExactLength(this AudioClip clip)
        {
            return clip.samples / (float)clip.frequency;
        }

        /// <summary>
        /// Returns how far along a source is with playing
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double ExactPosition(this AudioSource source)
        {
            return source.timeSamples / (double)source.clip.frequency;
        }
    }

    public static class ArrayExtension
    {
        public static List<Object> CombineToObjectList(params List<Object>[] lists)
        {
            List<Object> newList = new List<Object>();
            for (int i = 0; i < lists[i].Count; i++)
            {
                newList.AddRange(lists[i]);
            }
            return newList;
        }

        public static T[] TakeNonNulls<T>(this T[] arr) where T : Object
        {
            int nullCount = 0;
            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i] == null)
                {
                    nullCount += 1;
                }
            }

            if (nullCount > 0)
            {
                T[] nonNull = new T[arr.Length - nullCount];
                int lastIndex = 0;
                for (int i = 0; i < arr.Length; ++i)
                {
                    if (arr[i] != null)
                    {
                        nonNull[lastIndex] = arr[i];
                        lastIndex++;
                    }
                }
                return nonNull;
            }

            else
                return arr;
        }

        public static void ForEach<T>(this T[] source, Action<T> action)
        {
            for (int i = 0; i < source.Length; ++i)
            {
                action(source[i]);
            }
        }

        /// <summary>
        /// Returns a list of all objects that match a certain condition and is not nulls
        /// </summary>
        public static T[] FindAllNoNulls<T>(this IList<T> source, Func<T, bool> action)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < source.Count; ++i)
            {
                if (source[i] != null && action(source[i]))
                {
                    list.Add(source[i]);
                }
            }
            return list.ToArray();
        }

        public static void ForEachAssign<T>(this T[] source, Func<T, T> action)
        {
            for (int i = 0; i < source.Length; ++i)
            {
                source[i] = action(source[i]);
            }
        }

        //public static U[] Convert<T, U>(this T[] source, Func<T, U> action)
        //{
        //    U[] newArr = new U[source.Length];
        //    for (int i = 0; i < source.Length; ++i)
        //    {
        //        newArr[i] = action(source[i]);
        //    }
        //    return newArr;
        //}


        public static T[] Add<T>(this T[] source, params T[] objs)
        {
            var arr = new T[source.Length + objs.Length];
            int i = 0;
            for (; i < source.Length; i++)
            {
                arr[i] = source[i];
            }
            for (int j = 0; i < source.Length + objs.Length; )
            {
                arr[i] = objs[j];
                ++i;
                ++j;
            }
            return arr;
        }

        public static Object[] AddObj(this Object[] source, params Object[] objs)
        {
            List<Object> arr = new List<Object>();  

            for (int i = 0; i < source.Length; i++)
            {
                arr.Add(source[i]);
            }
            for (int i = 0; i < objs.Length; i++)
            {
                arr.Add(objs[i]);
            }
            return arr.ToArray();
        }
    }

    public static class IListExtension
    {
        public static bool TrueForAny<T>(this IList<T> list, Func<T, bool> trueForElement)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (trueForElement(list[i]))
                    return true;
            }
            return false;
        }
    }

    public static class ListExtension
    {
        public static void AddIfNotContains<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void SwapRemoveAt<T>(this IList<T> list, ref int i)
        {
            list[i] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            i -= 1;
        }

        public static void SafeRemoveAt<T>(this IList<T> list, ref int i)
        {
            list[i] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            i -= 1;
        }

        public static T Find<T>(this IList<T> list, Func<T, bool> func)
        {
            if (list == null || func == null)
                return default(T);
            for (int i = 0; i < list.Count; i++)
            {
                if (func(list[i]))
                {
                    return list[i];
                }
            }
            return default(T);
        }

        public static T FindMax<T>(this IList<T> list, Func<T, int> func)
        {
            if (list == null || func == null)
                return default(T);
            int maxScore = 0;
            T maxElem = default(T);
            for (int i = 0; i < list.Count; i++)
            {
                int score = func(list[i]);
                if (score > maxScore)
                {
                    maxElem = list[i];
                }
            }
            return maxElem;
        }

        public static int FindMaxIndex<T>(this IList<T> list, Func<T, int> func)
        {
            if (list == null || func == null)
                return -1;
            int maxScore = int.MinValue;
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                int score = func(list[i]);
                if (score > maxScore)
                {
                    index = i;
                }
            }
            return index;
        }

        public static T FindFirstNonNull<T>(this IList<T> list) where T : class 
        {
            if (list == null)
                return null;
            for (int i = 0; i < list.Count; i++)
            {
                if(list[i] != null)
                    return list[i];
            }
            return null;
        }

        public static int FindIndex<T>(this List<T> list, T toFind)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].Equals(toFind))
                    return i;
            }

            return -1;
        }

        public static void RemoveLast<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        public static bool RemoveFirst<T>(this List<T> list, Func<T, bool> shouldRemove)
        {
            if (list == null)
                return false;
            for (int i = 0; i < list.Count; i++)
            {
                if (shouldRemove(list[i]))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds an element in a list, removes it by swapping with the last element and decreases the size of the list
        /// </summary>
        /// <param name="list">The list to work on</param>
        /// <param name="toFind">The object to find in the list</param>
        /// <returns>Returns true if an element was removed</returns>
        public static bool FindSwapRemove<T>(this List<T> list, T toFind)
        {
            int index = FindIndex(list, toFind);
            if (index == -1)
                return false;
            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return true;
        }

        public static List<T> SwapAt<T>(this List<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
            return list;
        }

        public static T TryGet<T>(this List<T> list, int index)
        {
            if (index < list.Count)
                return list[index];
            return default(T);
        }

        //public static void ForEach<T>(this EventList<T> source, Action<T> action)
        //{
        //    for (int i = 0; i < source.Count; ++i)
        //    {
        //        action(source[i]);
        //    }
        //}


        public static void ThrowIfNull(this UnityEngine.Object obj)
        {
            if (obj == null)
                throw new NullReferenceException("");
        }

        public static int CountIf<T>(this IList<T> objs, Func<T, bool> predicate)
        {
            int count = 0;
            for (int i = 0; i < objs.Count; i++)
            {
                if (predicate(objs[i]))
                    count++;
            }
            return count;
        }

        public static void ThrowIfNull<T>(this T obj) where T : class
        {
            if (obj == null)
                throw new NullReferenceException(typeof(T).FullName);
        }

        public static List<U> ConvertList<T, U>(this List<T> toConvert)
            where T : UnityEngine.Object
            where U : class
        {
            List<U> newList = new List<U>(toConvert.Count);
            for (int i = 0; i < toConvert.Count; ++i)
            {
                newList.Add(toConvert[i] as U);
            }
            return newList;
        }
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static string FormatedName(this Enum someEnum)
        {
            return someEnum.ToString().AddSpacesToSentence();
        }
    }

    public static class StringUtil
    {
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = false)
        {
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                            i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public static class RectUtil
    {
        public static bool Intersect(this Rect a, Rect b)
        {
            FlipNegative(ref a);
            FlipNegative(ref b);
            bool c1 = a.xMin < b.xMax;
            bool c2 = a.xMax > b.xMin;
            bool c3 = a.yMin < b.yMax;
            bool c4 = a.yMax > b.yMin;
            return c1 && c2 && c3 && c4;
        }

        public static bool WithinWidth(this Rect a, Vector2 pos)
        {
            if (pos.x > a.x && pos.x < a.x + a.width)
                return true;
            return false;
        }

        public static void FlipNegative(ref Rect r)
        {
            if (r.width < 0)
                r.x -= (r.width *= -1);
            if (r.height < 0)
                r.y -= (r.height *= -1);
        }

        public static Rect Add(this Rect a, Rect b)
        {
            a.x += b.x;
            a.y += b.y;
            a.width += b.width;
            a.height += b.height;
            return a;
        }

        public static Rect Add(this Rect a, Vector2 b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static Rect Add(this Rect a, float x, float y)
        {
            a.x += x;
            a.y += y;
            return a;
        }
    }

    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
                return false;
            return go.GetComponent<T>() != null;

        }

        public static GameObject RootGO(this GameObject go)
        {
            return go.transform.root.gameObject;
        }
    }

    public static class Loudness
    {
        //http://wiki.unity3d.com/index.php?title=Loudness
        // A 20 dB increase sounds about 4x louder.
        // A signal needs an amplitude that is 10^(dB/20) greater, 
        // to be increased by 'dB' decibels.
        class Exponents
        {
            public static readonly float Set = Mathf.Log(10, 4);
            public static readonly float Get = 1 / Set;
        }

        public static float GetLoudness(this AudioSource audioSource)
        {
            return Mathf.Pow(audioSource.volume, Exponents.Get);
        }

        public static float SetLoudness(this AudioSource audioSource, float value)
        {
            return audioSource.volume = Mathf.Pow(Mathf.Clamp01(value), Exponents.Set);
        }

        public static float OfListener
        {
            get
            {
                return Mathf.Pow(AudioListener.volume, Exponents.Get);
            }
            set
            {
                AudioListener.volume = Mathf.Pow(Mathf.Clamp01(value), Exponents.Set);
            }
        }
    }

    


}

