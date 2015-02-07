using UnityEngine;
using System.Collections;

public class AutoStart : MonoBehaviour
{
    public GameObject[] Objects;

    public void Awake()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            if (Objects[i] != null)
            {
                (Instantiate(Objects[i]) as GameObject).transform.parent = transform;
            }
        }
    }
}
