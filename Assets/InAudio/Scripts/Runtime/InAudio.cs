using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Runtime;
using UnityEngine;

[AddComponentMenu(FolderSettings.ComponentPathPrefabsManager + "InAudio")]
public class InAudio : MonoBehaviour
{
    /******************/
    /*Public interface*/
    /******************/
    #region Public fields
    /// <summary>
    /// The active listener in the scene. Some features will not work if not set, like the spline system.
    /// </summary>
    public AudioListener activeAudioListener;

    public static AudioListener ActiveListener
    {
        get
        {
            if (instance != null)
                return instance.activeAudioListener;
            return null;
        }
        set
        {
            if (instance != null)
                instance.activeAudioListener = ActiveListener;
        }
    }
    #endregion

    #region Audio player

    /// <summary>
    /// Play an audio node directly
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer Play(GameObject gameObject, InAudioNode audioNode)
    {
        if(instance != null && gameObject != null && audioNode != null)
            return instance._inAudioEventWorker.PlayAttachedTo(gameObject, audioNode, gameObject);
        return null;
    }


    /// <summary>
    /// Play an audio node directly, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="attachedTo">The object to be attached to</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAttachedTo(GameObject gameObject, InAudioNode audioNode, GameObject attachedTo)
    {
        if (instance != null && gameObject != null && audioNode != null)
            return instance._inAudioEventWorker.PlayAttachedTo(gameObject, audioNode, attachedTo);
        return null;
    }


    /// <summary>
    /// Play an audio node directly, at this position in world space
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="position">The world position to play at</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAtPosition(GameObject gameObject, InAudioNode audioNode, Vector3 position)
    {
        if (instance != null && gameObject != null && audioNode != null)
            return instance._inAudioEventWorker.PlayAtPosition(gameObject, audioNode, position);
        return null;
    }


    /// <summary>
    /// Play an audio node directly with fade in time
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer Play(GameObject gameObject, InAudioNode audioNode, float fadeTime, LeanTweenType tweeenType)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
            return null;

        InPlayer player = instance._inAudioEventWorker.PlayAttachedTo(gameObject, audioNode, gameObject);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0, 1.0f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with fade in time, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="attachedTo">The game object to attach to</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAttachedTo(GameObject gameObject, InAudioNode audioNode, GameObject attachedTo, float fadeTime, LeanTweenType tweeenType)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
            return null;

        InPlayer player = instance._inAudioEventWorker.PlayAttachedTo(gameObject, audioNode, attachedTo);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0, 1.0f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with fade in time, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="position">The position in world space to play the audio</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAtPosition(GameObject gameObject, InAudioNode audioNode, Vector3 position, float fadeTime, LeanTweenType tweeenType)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
            return null;

        InPlayer player = instance._inAudioEventWorker.PlayAtPosition(gameObject, audioNode, position);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0, 1.0f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }


    /// <summary>
    /// Play an audio node directly with custom fading
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer Play(GameObject gameObject, InAudioNode audioNode, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
            return null;

        InPlayer player = instance._inAudioEventWorker.PlayAttachedTo(gameObject, audioNode, gameObject);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with a custom fade, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="attachedTo">The game object to attach to</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAttachedTo(GameObject gameObject, InAudioNode audioNode, GameObject attachedTo, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
            return null;

        InPlayer player = instance._inAudioEventWorker.PlayAttachedTo(gameObject, audioNode, attachedTo);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node in world space with a custom fade, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="position">The world position to play at</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAtPosition(GameObject gameObject, InAudioNode audioNode, Vector3 position, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
            return null;

        InPlayer player = instance._inAudioEventWorker.PlayAtPosition(gameObject, audioNode, position);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Stop all instances of the this audio node on the game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    public static void Stop(GameObject gameObject, InAudioNode audioNode)
    {
        if (instance != null && gameObject != null && audioNode != null)
            instance._inAudioEventWorker.StopByNode(gameObject, audioNode);
    }

    /// <summary>
    /// Stop all instances of the this audio node on the game object with a fade out time
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    /// <param name="fadeOutTime"></param>
    public static void Stop(GameObject gameObject, InAudioNode audioNode, float fadeOutTime)
    {
        if(instance != null && gameObject != null && audioNode != null)
            instance._inAudioEventWorker.StopByNode(gameObject, audioNode, fadeOutTime);
    }

    /// <summary>
    /// Breaks all looping instances of this node on the game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    public static void Break(GameObject gameObject, InAudioNode audioNode)
    {
        instance._inAudioEventWorker.Break(gameObject, audioNode);
    }

    /// <summary>
    /// Stop all sounds playing on this game object
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAll(GameObject gameObject)
    {
        instance._inAudioEventWorker.StopAll(gameObject, 0, LeanTweenType.notUsed);
    }

    /// <summary>
    /// Stop all sounds playing on this game object
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAll()
    {
        instance._inAudioEventWorker.StopAll(0, LeanTweenType.notUsed);
    }

    /// <summary>
    /// Stop all sounds playing on this game object with fade out time
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="fadeOut">Time to fade out</param>
    /// <param name="fadeType">Fade type</param>
    public static void StopAll(GameObject gameObject, float fadeOut, LeanTweenType fadeType)
    {
        instance._inAudioEventWorker.StopAll(gameObject, fadeOut, fadeType);
    }


    /// <summary>
    /// Get a list of all players attached to this game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static InPlayer[] PlayersOnGO(GameObject gameObject)
    {
        if (instance != null && gameObject != null)
        {
            return instance._inAudioEventWorker.GetPlayers(gameObject);
        }
        return null;
    }

    /// <summary>
    /// Copy the list of playing sounds on this game object to a preallocated array
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="copyToList">If the list is too short, the partial list will be copied</param>
    public static void PlayersOnGO(GameObject gameObject, IList<InPlayer> copyToList)
    {
        if (instance != null && gameObject != null)
        {
            instance._inAudioEventWorker.GetPlayers(gameObject, copyToList);
        }
    }

    #endregion

    #region Set/Get Parameters

    /// <summary>
    /// Sets the volume for all instances of this audio node on the object. 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    /// <param name="volume"></param>
    public static void SetVolumeForNode(GameObject gameObject, InAudioNode audioNode, float volume)
    {
        if (instance != null)
        {
            instance._inAudioEventWorker.SetVolumeForNode(gameObject, audioNode, volume);
        }
    }

    /// <summary>
    /// Sets the volume for all audio playing on this game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="volume"></param>
    public static void SetVolumeForAll(GameObject gameObject, float volume)
    {
        if (instance != null)
        {
            instance._inAudioEventWorker.SetVolumeForGameObject(gameObject, volume);
        }
    }

    /// <summary>
    /// Set the runtime volume of the specified bus
    /// </summary>
    /// <param name="bus">The bus to set the volume</param>
    /// <param name="newVolume">The volume to set</param>
    /// <returns>The new volume. -1.0f if the bus is null</returns>
    public static float SetBusVolume(InAudioBus bus, float newVolume)
    {
        if (bus != null)
        {
            bus.Dirty = true;
            return bus.RuntimeSelfVolume = newVolume;
        }
        return -1.0f;
    }

    /// <summary>
    /// Get the runtime volume of this bus
    /// </summary>
    /// <param name="bus"></param>
    /// <returns>The volume. Will return -1.0f if the bus is invalid</returns>
    public static float BusVolume(InAudioBus bus)
    {
        if (bus != null)
        {
            return bus.RuntimeSelfVolume;
        }
        return -1.0f;
    }

    /// <summary>
    /// Set if a bus is muted
    /// </summary>
    /// <param name="bus">The bus</param>
    /// <param name="mute">to mute or unmute</param>
    public static void MuteBus(InAudioBus bus, bool mute)
    {
        if (bus != null)
        {
            bus.Dirty = true;
            bus.RuntimeMute = mute;
        }
        else
        {
            InDebug.LogWarning("Bus instance not set to an instance in mute");
        }
    }

    /// <summary>
    /// Check if a bus is muted
    /// </summary>
    /// <param name="bus">The bus</param>
    /// <param name="mute">to mute or unmute</param>
    public static void IsBusMuted(InAudioBus bus, bool mute)
    {
        if (bus != null)
        {
            bus.Dirty = true;
            bus.RuntimeMute = mute;
        }
        else
        {
            InDebug.LogWarning("Bus instance not set to an instance in mute");
        }
    }
    #endregion

    #region Post Event by reference
    /// <summary>
    /// Post all actions in this event attached to this gameobject
    /// </summary>
    /// <param name="controllingObject">The controlling object and the future parent the played audio files</param>
    /// <param name="postEvent">The event to post the actions of</param>
    public static void PostEvent(GameObject controllingObject, InAudioEventNode postEvent)
    {
        if (instance != null && controllingObject != null && postEvent != null)
            instance.OnPostEvent(controllingObject, postEvent, controllingObject);
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (postEvent == null)
            {
                InDebug.MissingEvent(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event attached to this another game object than the one controlling it
    /// </summary>
    /// <param name="controllingObject">The controlling object of the played sources</param>
    /// <param name="postEvent">The event to post the actions of</param>
    /// <param name="attachedToOther">The audio source to attach any audio sources to</param>
    public static void PostEventAttachedTo(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        if (instance != null && controllingObject != null && postEvent != null)
            instance.OnPostEvent(controllingObject, postEvent, attachedToOther);
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (postEvent == null)
            {
                InDebug.MissingEvent(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event at this position with this game object as the controller
    /// </summary>
    /// <param name="controllingObject">The controlling object of the played audio files</param>
    /// <param name="postEvent">The event to post the actions of</param>
    /// <param name="position">The position in world space of the sound</param>
    public static void PostEventAtPosition(GameObject controllingObject, InAudioEventNode postEvent, Vector3 position)
    {
        if (instance != null && controllingObject != null && postEvent != null)
            instance.OnPostEventAtPosition(controllingObject, postEvent, position);
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (postEvent == null)
            {
                InDebug.MissingEvent(controllingObject);
            }
        }
    }

    #endregion

    #region Post Event lists (inspector lists)

    /// <summary>
    /// Post all actions in this event in accordance to the data specified in the inspector, but overrides which object is it attached to.
    /// </summary>
    /// <param name="controllingObject">The controlling game object and the future parent of the audio files</param>
    /// <param name="eventList">All the events to post as added in the inspector</param>
    public static void PostEvent(GameObject controllingObject, InAudioEvent eventList)
    {
        if (instance != null && controllingObject != null && eventList != null && eventList.Events != null)
        {
            int count = eventList.Events.Count;
            Vector3 position = controllingObject.transform.position;
            for (int i = 0; i < count; i++)
            {
                EventHookListData eventData = eventList.Events[i];
                if (eventData != null && eventData.Event != null)
                {
                    if (eventData.PostAt == EventHookListData.PostEventAt.AttachedTo)
                        instance.OnPostEvent(controllingObject, eventData.Event, controllingObject);
                    else //if post at position
                        instance.OnPostEvent(controllingObject, eventData.Event, position);
                }
            }
        }
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (eventList == null || eventList.Events == null)
            {
                InDebug.MissingEventList(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event in accordance to the data specified in the inspector, but overrides which object is it attached to.
    /// </summary>
    /// <param name="controllingObject">The controlling game object and the future parent of the audio files</param>
    /// <param name="eventList">All the events to post as added in the inspector</param>
    /// <param name="attachedToOther">The object to attach the events to</param>
    public static void PostEventAttachedTo(GameObject controllingObject, InAudioEvent eventList, GameObject attachedToOther)
    {
        if (instance != null && controllingObject != null && eventList != null && eventList.Events != null)
        {
            int count = eventList.Events.Count;
            Vector3 position = controllingObject.transform.position;
            for (int i = 0; i < count; i++)
            {
                EventHookListData eventData = eventList.Events[i];
                if (eventData != null && eventData.Event != null)
                {
                    if (eventData.PostAt == EventHookListData.PostEventAt.AttachedTo)
                        instance.OnPostEvent(controllingObject, eventData.Event, attachedToOther);
                    else //if post at position
                        instance.OnPostEvent(controllingObject, eventData.Event, position);
                }
            }

        }
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (eventList == null || eventList.Events == null)
            {
                InDebug.MissingEventList(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event in accordance to the data specified in the inspector, but overrides another place to fire the sound from
    /// </summary>
    /// <param name="controllingObject">The controlling game object and the future parent of the audio files</param>
    /// <param name="eventList">All the events to post as added in the inspector</param>
    /// <param name="postAt">The new position to play at</param>
    public static void PostEventAtPosition(GameObject controllingObject, InAudioEvent eventList, Vector3 postAt)
    {
        if (instance != null && controllingObject != null && eventList != null && eventList.Events != null)
        {
            int count = eventList.Events.Count;
            for (int i = 0; i < count; i++)
            {
                EventHookListData eventData = eventList.Events[i];
                if (eventData != null && eventData.Event != null)
                {
                    instance.OnPostEvent(controllingObject, eventData.Event, postAt);
                }
            }
        }
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (eventList == null || eventList.Events == null)
            {
                InDebug.MissingEventList(controllingObject);
            }
        }
    }

    #endregion

    #region Find Event by ID

    /// <summary>
    /// Find an Audio Event by id so it can be posted directly
    /// </summary>
    /// <param name="id">The ID of the event to post. The ID is found in the InAudio Event window</param>
    /// <returns>The found audio event. Returns null if not found</returns>
    public static InAudioEventNode FindEventByID(int id)
    {
        InAudioEventNode postEvent = null;
        if (instance != null)
        {
            instance.runtimeData.Events.TryGetValue(id, out postEvent);
        }
        else
        {
            InDebug.LogWarning("InAudio: Could not try to find event with id " + id + " as no InAudio instance was found");
        }
        return postEvent;
    }
    #endregion

    #region Find bus & audio node by ID

    /// <summary>
    /// Finds an audio bus based on the ID specified
    /// </summary>
    /// <param name="id">The id to search for</param>
    /// <returns>The found bus, null if not found</returns>
    public static InAudioBus FindBusById(int id)
    {
        if (instance != null)
        {
            return TreeWalker.FindById(InAudioInstanceFinder.DataManager.BusTree, id);
        }
        else
        {
            InDebug.LogWarning("InAudio: Could not bus with id " + id);
        }
        return null;
    }

    /// <summary>
    /// Finds an audio node based on the ID specified
    /// </summary>
    /// <param name="id">The id to search for</param>
    /// <returns>The found bus, null if not found</returns>
    public static InAudioNode FindAudioNodeById(int id)
    {
        if (instance != null)
        {
            return TreeWalker.FindById(InAudioInstanceFinder.DataManager.AudioTree, id);
        }
        else
        {
            InDebug.LogWarning("InAudio: Could not bus with id " + id);
        }
        return null;
    }
    #endregion

    #region Banks
    /// <summary>
    /// Load all audio clips in this bank
    /// </summary>
    /// <param name="bank">The reference to the bank to load</param>
    public static void LoadBank(InAudioBankLink bank)
    {
        if (bank != null)
            BankLoader.Load(bank);
        else
        {
            InDebug.BankLoadMissing();
        }
    }

    /// <summary>
    /// Unload all audio clips in this banks. Also calls Resources.UnloadUnusedAssets(). Clips will autoreimport if any audio source still referencs this clip
    /// </summary>
    /// <param name="bank">The reference to the bank to unload</param>
    public static void UnloadBank(InAudioBankLink bank)
    {
        if (bank != null)
            BankLoader.Unload(bank);
        else
        {
            InDebug.BankUnloadMissing();
        }
    }

    #endregion

    #region Cleanup
    /// <summary>
    /// This will clean up any unused runtime memory and release objects back to their pools
    /// Does not release audio clips.
    /// Best called in loading screens 
    /// </summary>
    public static void Cleanup()
    {
        if (instance != null && instance._inAudioEventWorker != null)
            instance._inAudioEventWorker.Cleanup();
        else
        {
            InDebug.CleanupInstance();
        }
    }
    #endregion


    /*Internal systems*/
    #region Internal system

    private void HandleEventAction(GameObject controllingObject, AudioEventAction eventData, GameObject attachedTo, Vector3 playAt = new Vector3())
    {
        InAudioNode audioNode; //Because we can't create variables in the scope of the switch with the same name
        InEventBankLoadingAction bankLoadingData;

        if (eventData.Target == null && eventData.EventActionType != EventActionTypes.StopAll)
        {
            InDebug.MissingActionTarget(controllingObject, eventData);
            return;
        }

        switch (eventData.EventActionType)
        {
            case EventActionTypes.Play:
                var audioPlayData = ((InEventAudioAction) eventData);
                audioNode = audioPlayData.Node;
                if (audioNode != null)
                {
                    if (attachedTo != null)
                        _inAudioEventWorker.PlayAttachedTo(controllingObject, audioNode, attachedTo, audioPlayData.Fadetime, audioPlayData.TweenType);
                    else
                        _inAudioEventWorker.PlayAtPosition(controllingObject, audioNode, playAt, audioPlayData.Fadetime, audioPlayData.TweenType);
                }
                break;
            case EventActionTypes.Stop:
                var data = ((InEventAudioAction) eventData);
                audioNode = data.Node;
                _inAudioEventWorker.StopByNode(controllingObject, audioNode, data.Fadetime, data.TweenType);
                break;
            case EventActionTypes.StopAll:
                var stopAlLData = ((InEventAudioAction) eventData);
                _inAudioEventWorker.StopAll(controllingObject, stopAlLData.Fadetime, stopAlLData.TweenType);
                break;
            case EventActionTypes.Break:
                audioNode = ((InEventAudioAction)eventData).Node;
                _inAudioEventWorker.Break(controllingObject, audioNode);
                break;
            case EventActionTypes.SetBusVolume:
                var busData = eventData as InEventBusAction;
                if (busData != null && busData.Bus != null)
                {
                    AudioBusVolumeHelper.SetTargetVolume(busData.Bus, busData.Volume, busData.VolumeMode, busData.Duration, busData.FadeCurve);
                }
                break;
            case EventActionTypes.BankLoading:
                bankLoadingData = eventData as InEventBankLoadingAction;
                if (bankLoadingData != null)
                {
                    if (bankLoadingData.LoadingAction == BankHookActionType.Load)
                        BankLoader.Load(bankLoadingData.BankLink);
                    else
                    {
                        BankLoader.Unload(bankLoadingData.BankLink);
                    }
                }
                break;
            case EventActionTypes.StopAllInBus:
                busData = eventData as InEventBusAction;
                if (busData != null && busData.Bus != null)
                {
                    StopAllNodeInBus(busData.Bus);
                }
                break;
            case EventActionTypes.SetBusMute:
                var busMuteData = eventData as InEventBusMuteAction;
                if (busMuteData != null && busMuteData.Bus != null)
                {
                    AudioBusVolumeHelper.MuteAction(busMuteData.Bus, busMuteData.Action);
                }
                break;
            default:
                InDebug.UnusedActionType(gameObject, eventData);
                break;
        }

    }

    #region Debug

    public static InDebug InDebug = new InDebug();
    #endregion

    #region Internal event handling


    #region Post attached to
    private void OnPostEvent(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        bool areAnyDelayed = false;
        if (postEvent.Delay > 0)
        {
            StartCoroutine(DelayedEvent(controllingObject, postEvent, attachedToOther));
        }
        else
        {
            areAnyDelayed = PostUndelayedActions(controllingObject, postEvent, attachedToOther);
        }

        if (areAnyDelayed)
        {
            for (int i = 0; i < postEvent.ActionList.Count; ++i)
            {
                var eventData = postEvent.ActionList[i];
                if (eventData != null && eventData.Delay > 0)
                    StartCoroutine(PostDelayedActions(controllingObject, eventData, attachedToOther));
            }
        }
    }

    private bool PostUndelayedActions(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        bool areAnyDelayed = false;
        for (int i = 0; i < postEvent.ActionList.Count; ++i)
        {
            var eventData = postEvent.ActionList[i];
            if (eventData == null)
                continue;
            if (eventData.Delay > 0)
            {
                areAnyDelayed = true;
            }
            else
                HandleEventAction(controllingObject, eventData, attachedToOther);
        }
        return areAnyDelayed;
    }

    private IEnumerator DelayedEvent(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        yield return new WaitForSeconds(postEvent.Delay);
        PostUndelayedActions(controllingObject, postEvent, attachedToOther);
    }

    private IEnumerator PostDelayedActions(GameObject controllingObject, AudioEventAction eventData, GameObject attachedToOther)
    {
        yield return new WaitForSeconds(eventData.Delay);
        HandleEventAction(controllingObject, eventData, attachedToOther);
    }
    #endregion
    #region Post at position
    private void OnPostEventAtPosition(GameObject controllingObject, InAudioEventNode audioEvent, Vector3 position)
    {
        if (instance != null && controllingObject != null && audioEvent != null)
            instance.OnPostEvent(controllingObject, audioEvent, position);
    }

    private void OnPostEvent(GameObject controllingObject, InAudioEventNode postEvent, Vector3 postAt)
    {
        bool areAnyDelayed = false;
        if (postEvent.Delay > 0)
        {
            StartCoroutine(DelayedEvent(controllingObject, postEvent, postAt));
        }
        else
        {
            areAnyDelayed = PostUndelayedActions(controllingObject, postEvent, postAt);
        }

        if (areAnyDelayed)
        {
            for (int i = 0; i < postEvent.ActionList.Count; ++i)
            {
                var eventData = postEvent.ActionList[i];
                if (eventData != null && eventData.Delay > 0)
                    StartCoroutine(PostDelayedActions(controllingObject, eventData, postAt));
            }
        }
    }

    private bool PostUndelayedActions(GameObject controllingObject, InAudioEventNode postEvent, Vector3 postAt)
    {
        bool areAnyDelayed = false;
        for (int i = 0; i < postEvent.ActionList.Count; ++i)
        {
            var eventData = postEvent.ActionList[i];
            if (eventData == null)
                continue;
            if (eventData.Delay > 0)
            {
                areAnyDelayed = true;
            }
            else
                HandleEventAction(controllingObject, eventData, null, postAt);
        }
        return areAnyDelayed;
    }

    private IEnumerator DelayedEvent(GameObject controllingObject, InAudioEventNode postEvent, Vector3 postAt)
    {
        yield return new WaitForSeconds(postEvent.Delay);
        PostUndelayedActions(controllingObject, postEvent, postAt);
    }

    private IEnumerator PostDelayedActions(GameObject controllingObject, AudioEventAction eventData, Vector3 postAt)
    {
        yield return new WaitForSeconds(eventData.Delay);
        HandleEventAction(controllingObject, eventData, null, postAt);
    }
    #endregion
    #endregion

    #region Internal data

    private InAudioEventWorker _inAudioEventWorker;

    private InRuntimeAudioData runtimeData;

    private static InAudio instance;


    //TODO Move this to another class
    private static void StopAllNodeInBus(InAudioBus bus)
    {
        var players = bus.RuntimePlayers;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Stop();
        }
        for (int i = 0; i < bus.Children.Count; i++)
        {
            StopAllNodeInBus(bus.Children[i]);
        }
    }

    #endregion

    #region Unity functions
    void Update()
    {
        AudioBusVolumeHelper.UpdateDucking(InAudioInstanceFinder.DataManager.BusTree);
        AudioBusVolumeHelper.UpdateVolumes(InAudioInstanceFinder.DataManager.BusTree);

        var controllerPool = InAudioInstanceFinder.RuntimePlayerControllerPool;
        if (controllerPool != null)
        {
            controllerPool.DelayedRelease();
        }

        var playerPool = InAudioInstanceFinder.InRuntimePlayerPool;
        if (playerPool != null)
        {
            playerPool.DelayedRelease();
        }
    }

    void OnLevelWasLoaded()
    {
        var root = InAudioInstanceFinder.DataManager.BusTree;
        root.Dirty = true;
        AudioBusVolumeHelper.UpdateVolumes(root);
        if (activeAudioListener == null)
            activeAudioListener = FindActiveListener();
    }

    private static AudioListener FindActiveListener()
    {
        return Object.FindObjectsOfType(typeof(AudioListener)).FindFirst(activeListener => (activeListener as AudioListener).gameObject.activeInHierarchy) as AudioListener;
    }

    void OnEnable()
    {
        if (instance == null || instance == this)
        {
            if (activeAudioListener == null)
                activeAudioListener = FindActiveListener();

            instance = this;
            _inAudioEventWorker = GetComponentInChildren<InAudioEventWorker>();
            runtimeData = GetComponentInChildren<InRuntimeAudioData>();
            BankLoader.LoadAutoLoadedBanks();
            runtimeData.UpdateEvents(InAudioInstanceFinder.DataManager.EventTree);
            //AudioBusVolumeHelper.UpdateCombinedVolume(InAudioInstanceFinder.DataManager.BusTree);
            DontDestroyOnLoad(transform.gameObject);

            if (InAudioInstanceFinder.DataManager != null && InAudioInstanceFinder.DataManager.BusTree != null)
            {
                var busRoot = InAudioInstanceFinder.DataManager.BusTree;
                TreeWalker.ForEach(busRoot, b => b.NodesInBus = new List<InPlayer>());
                busRoot.Dirty = true;
                AudioBusVolumeHelper.InitVolumes(busRoot);
                AudioBusVolumeHelper.UpdateVolumes(InAudioInstanceFinder.DataManager.BusTree);
            }
        }
        else
        {
            Object.Destroy(transform.gameObject);
        }
    }


#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        InDebug.DoLog = false;
    }
#endif

    #endregion

    #endregion
}