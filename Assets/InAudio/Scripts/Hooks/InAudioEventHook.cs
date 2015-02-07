using InAudioSystem;
using UnityEngine;

[AddComponentMenu("InAudio/Audio Event Hook")]
public class InAudioEventHook : MonoBehaviour
{
    public InAudioEvent onEnable;

    public InAudioEvent onStart;

    public InAudioEvent onDisable;

    public InAudioEvent onDestroy;

    public AudioEventCollisionList CollisionList; 

    void OnEnable()
    {
        if (onEnable != null)
            InAudio.PostEvent(gameObject, onEnable);
    }

    void Start()
    {
        InAudio.PostEvent(gameObject, onStart);
    }

    void OnDisable()
    {
        InAudio.PostEvent(gameObject, onDisable);
    }

    void OnDestroy()
    {
        InAudio.PostEvent(gameObject, onDestroy);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // Convert the object's layer to a bitfield for comparison
        int objLayerMask = (1 << obj.layer);
        if ((layerMask.value & objLayerMask) > 0)  // Extra round brackets required!
            return true;
        else
            return false;
    }

    #if !UNITY_4_1 && !UNITY_4_2
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }   
#endif
}