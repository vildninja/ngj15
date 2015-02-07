using System.Collections.Generic;

namespace GameCore.Util
{

    public class TupleList<T1> : List<Tuple<T1>>
    {
        public void Add(T1 item)
        {
            Add(new Tuple<T1>(item));
        }
    }

    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public Tuple<T1, T2> Add(T1 item, T2 item2)
        {
            var t = new Tuple<T1, T2>(item, item2);
            Add(t);
            return t;
        }
    }

    public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
    {
        public void Add(T1 item, T2 item2, T3 item3)
        {
            Add(new Tuple<T1, T2, T3>(item, item2, item3));
        }
    }

    public class TupleList<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>>
    {
        public void Add(T1 item, T2 item2, T3 item3, T4 item4)
        {
            Add(new Tuple<T1, T2, T3, T4>(item, item2, item3, item4));
        }
    }

    public class TupleList<T1, T2, T3, T4, T5> : List<Tuple<T1, T2, T3, T4, T5>>
    {
        public void Add(T1 item, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Add(new Tuple<T1, T2, T3, T4, T5>(item, item2, item3, item4, item5));
        }
    }
}