using InAudioSystem;
using InAudioSystem.RuntimeHelperClass;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
#endif
[AddComponentMenu(FolderSettings.ComponentPathPrefabsManager + "InAudio Instance Finder")]
public class InAudioInstanceFinder : MonoBehaviour
{
    private static InAudioInstanceFinder instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    public static bool IsValid
    {
        get { return instance != null; }
    }

    public static InAudioInstanceFinder Instance
    {
        get { return instance; }
    }

    private static InCommonDataManager _dataManager;
    public static InCommonDataManager DataManager
    {
        get
        {
            if (_dataManager == null)
            {
                _dataManager = FindObjectOfType(typeof(InCommonDataManager)) as InCommonDataManager;
                if (_dataManager != null)
                    _dataManager.Load();
            }
            return _dataManager;
        }
    }


    private static InRuntimeAudioData _runtimeAudioData;
    public static InRuntimeAudioData RuntimeAudioData
    {
        get
        {
            if (_runtimeAudioData == null)
            {
                _runtimeAudioData = instance.GetComponent<InRuntimeAudioData>();
            }
            return _runtimeAudioData;
        }
    }

    private static InAudioEventWorker _inAudioEventWorker;
    public static InAudioEventWorker InAudioEventWorker
    {
        get
        {
            if (_inAudioEventWorker == null)
            {
                _inAudioEventWorker = instance.GetComponent<InAudioEventWorker>();
            }
            return _inAudioEventWorker;
        }
    }

    private static InRuntimeInfoPool _runtimeInfoPool;
    public static InRuntimeInfoPool RuntimeInfoPool
    {
        get
        {
            if (_runtimeInfoPool == null && instance != null)
            {
                _runtimeInfoPool = instance.GetComponent<InRuntimeInfoPool>();
            }
            return _runtimeInfoPool;
        }
    }

    private static InDSPTimePool _dspTimePool;
    public static InDSPTimePool DSPTimePool
    {
        get
        {
            if (_dspTimePool == null)
            {
                _dspTimePool = instance.GetComponent<InDSPTimePool>();
            }
            return _dspTimePool;
        }
    }

    private static ArrayPool<DSPTime> _dspArrayPool;
    public static ArrayPool<DSPTime> DSPArrayPool
    {
        get
        {
            if(_dspArrayPool == null)
                _dspArrayPool = new ArrayPool<DSPTime>(4, 1, 1);
            return _dspArrayPool;
        }
    }

    private static ArrayPool<Coroutine> _coroutinePool;
    public static ArrayPool<Coroutine> CoroutinePool
    {
        get
        {
            if (_coroutinePool == null)
                _coroutinePool = new ArrayPool<Coroutine>(4, 1, 1);
            return _coroutinePool;
        }
    }

    private static InRuntimePlayerPool _inRuntimePlayerPool;

    public static InRuntimePlayerPool InRuntimePlayerPool
    {
        get
        {
            if (_inRuntimePlayerPool == null && instance != null)
                _inRuntimePlayerPool = instance.GetComponent<InRuntimePlayerPool>();
            return _inRuntimePlayerPool;
        }
    }

    private static InRuntimePlayerControllerPool _runtimePlayerControllerPool;
    public static InRuntimePlayerControllerPool RuntimePlayerControllerPool
    {
        get
        {
            if (_runtimePlayerControllerPool == null && instance != null)
                _runtimePlayerControllerPool = instance.GetComponent<InRuntimePlayerControllerPool>();
            return _runtimePlayerControllerPool;
        }
    }

#if UNITY_EDITOR
    //private static AudioSource _editorAudioSource;
    //public static AudioSource EditorAudioSource
    //{
    //    get
    //    {
    //        if (_editorAudioSource == null)
    //        {
    //            var guide = Object.FindObjectOfType(typeof(InAudioGuide)) as InAudioGuide;
    //            if (guide != null)
    //            {
    //                var source = guide.GetComponentInChildren<AudioSource>();

    //                if (source != null)
    //                    _editorAudioSource = source;
    //                else
    //                    _editorAudioSource = guide.transform.GetChild(0).gameObject.AddComponent<AudioSource>();

                    
    //            }

    //        }
    //        if (_editorAudioSource != null)
    //            _editorAudioSource.playOnAwake = false;

    //        return _editorAudioSource;
    //    }
    //}

    private static InAudioGUIUserPrefs _inAudioGuiUserPref;
    public static InAudioGUIUserPrefs InAudioGuiUserPrefs
    {
        get
        {
            if (_inAudioGuiUserPref == null)
            {
                var prefGO = AssetDatabase.LoadAssetAtPath(FolderSettings.GUIUserPrefs, typeof(GameObject)) as GameObject;
                if (prefGO != null)
                {
                    _inAudioGuiUserPref = prefGO.GetComponent<InAudioGUIUserPrefs>();
                    if (_inAudioGuiUserPref == null)
                    {
                        _inAudioGuiUserPref = prefGO.AddComponent<InAudioGUIUserPrefs>();
                    }
                }

            }
            return _inAudioGuiUserPref;
        }
    }
#endif
}
