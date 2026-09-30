using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class RhythmHandler : ScriptLibrary.Singletons.Singleton<RhythmHandler>
{
    public event Action OnSpawn;
    public float bpm = 124f;
    public float startDelay = 2.0f;
    public float travelTime = 2.438f;
    public Transform hitPoint;
    public Transform spawnPoint;

    [Tooltip("Seconds to compensate for audio/output latency.")]
    public float latencyOffset = 0f;

    [SerializeField] private EventReference musicEvent;

    private EventInstance _musicInstance;
    private float _nextBeatTime;   // in song-position seconds, not Time.time
    private float _secondsPerBeat;
    private float _songLength;
    private bool _songStarted = false;

    private Queue<bool> _beatQueue = new Queue<bool>();

    void Start()
    {
        _secondsPerBeat = 60f / bpm;

        LoadBeatsFromJSON("BeatMap");

        _musicInstance = RuntimeManager.CreateInstance(musicEvent);
        StartCoroutine(StartSongAfterDelay(startDelay));
    }

    private IEnumerator StartSongAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _musicInstance.start();

        EventDescription eventDescription;
        _musicInstance.getDescription(out eventDescription);
        int lengthMs;
        eventDescription.getLength(out lengthMs);
        _songLength = lengthMs / 1000f;

        _nextBeatTime = 0f;
        _songStarted = true;
    }

    private void LoadBeatsFromJSON(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);

        if (jsonFile == null)
        {
            Debug.LogError($"Couldn't load {fileName}");
            return;
        }

        try
        {
            BeatPattern pattern = JsonUtility.FromJson<BeatPattern>(jsonFile.text);

            if (pattern?.beats == null || pattern.beats.Length == 0)
            {
                Debug.LogError("Beat pattern is empty!");
                return;
            }

            for (int i = 0; i < pattern.beats.Length; i++)
            {
                _beatQueue.Enqueue(pattern.beats[i]);
            }

            Debug.Log($"loaded {_beatQueue.Count} beats from {fileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"parsing error: {e.Message}");
        }
    }

    void Update()
    {
        if (!_songStarted) return;

        PLAYBACK_STATE playbackState;
        _musicInstance.getPlaybackState(out playbackState);
        if (playbackState == PLAYBACK_STATE.STOPPED) return;

        int positionMs;
        _musicInstance.getTimelinePosition(out positionMs);
        float songPosition = (positionMs / 1000f) + latencyOffset;

        if (songPosition >= _nextBeatTime - travelTime)
        {
            if (songPosition < _songLength - 1.0f)
            {
                bool shouldPlayBeat = _beatQueue.Count <= 0 || _beatQueue.Dequeue();

                if (shouldPlayBeat)
                {
                    OnSpawn?.Invoke();
                }

                _nextBeatTime += _secondsPerBeat;
            }
        }

        if (songPosition >= _songLength)
        {
            _songStarted = false;
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.release();
            GameManager.Instance.EndGame();
        }
    }

    public float GetNoteSpeed(Vector3 initialPosition, Vector3 finalPosition)
    {
        float distance = Mathf.Abs(initialPosition.z - finalPosition.z);
        return distance / travelTime;
    }
}

[Serializable]
public class BeatPattern
{
    public bool[] beats;
}