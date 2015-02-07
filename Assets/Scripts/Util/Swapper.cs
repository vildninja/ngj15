namespace GameCore
{
    public static class Swapper
    {
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T temp = t1;
            t1 = t2;
            t2 = temp;
        }
    }

}