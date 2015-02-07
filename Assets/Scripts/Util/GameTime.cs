using UnityEngine;
using System.Collections;

public static class GameTime
{
    private static float multiplier = 1.0f;

    public static float Multiplier
    {
        get { return multiplier; }
        set
        {
            if (value >= 0)
                multiplier = value;
        }
    }
    
    public static bool Paused { get; set; }

    public static float deltaTime
    {
        get
        {
            if (Paused)
                return 0;
            return Time.deltaTime*multiplier;
        }
    }

    public static float fixedDeltaTime
    {
        get
        {
            if (Paused)
                return 0;
            return Time.fixedDeltaTime * multiplier;
        }
    }
}
