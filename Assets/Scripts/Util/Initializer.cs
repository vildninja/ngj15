using UnityEngine;
using System.Collections;
namespace GameCore
{
    
    public class Initializer : MonoBehaviour
    {
        [Tooltip("If it is to bind all fields after awake. Set to true for objects in the scene and simple prefabs. Set to false if they are constructed later, for example in a factory")]
        public bool PrimaryInitializer = true;

        public void Awake()
        {
            BroadcastMessage("BindFields", !PrimaryInitializer, SendMessageOptions.RequireReceiver);
        }
    }

}