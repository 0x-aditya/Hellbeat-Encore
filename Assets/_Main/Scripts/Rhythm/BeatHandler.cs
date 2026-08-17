using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

class BeatHandler : ScriptLibrary.Singletons.Singleton<BeatHandler>
{
    public event Action OnBeat;
    public event Action OnMarker;
    public FMOD.Studio.EventInstance MusicInstance;
    
    private TimelineInfo _timelineInfo;
    private GCHandle _timelineHandle;

    [SerializeField] public FMODUnity.EventReference eventName;

    private FMOD.Studio.EVENT_CALLBACK _beatCallback;

    public static int LastBeat = 0;
    public static string LastMarker = "";
    
    
    class TimelineInfo
    {
        public int CurrentBeat = 0;
        public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
    }
    
    void Start()
    {
        _timelineInfo = new TimelineInfo();

        _beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        MusicInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);

        _timelineHandle = GCHandle.Alloc(_timelineInfo);
        MusicInstance.setUserData(GCHandle.ToIntPtr(_timelineHandle));

        MusicInstance.setCallback(_beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        //MusicInstance.start();
    }

    void OnDestroy()
    {
        MusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        MusicInstance.release();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 120, 200, 100));
        GUILayout.Box($"Current Beat = {_timelineInfo.CurrentBeat}, Last Marker = {(string)_timelineInfo.LastMarker}");
        GUILayout.EndArea();
    }

    
    private void Update()
    {
        if (LastMarker != _timelineInfo.LastMarker)
        {
            LastMarker = _timelineInfo.LastMarker;
            OnMarker?.Invoke();
        }
        if (LastBeat != _timelineInfo.CurrentBeat)
        {
            LastBeat = _timelineInfo.CurrentBeat;
            OnBeat?.Invoke();
        }
    }
    
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        FMOD.RESULT result = instance.getUserData(out var timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.CurrentBeat = parameter.beat;
                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                    timelineInfo.LastMarker = parameter.name;
                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    timelineHandle.Free();
                    break;
                }
            }
        }
        return FMOD.RESULT.OK;
    }
}
