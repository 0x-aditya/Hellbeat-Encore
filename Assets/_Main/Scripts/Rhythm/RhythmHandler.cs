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

    [SerializeField] private EventReference musicEvent;

    private EventInstance _musicInstance;
    private float _songStartTime;
    private float _nextBeatTime;
    private float _secondsPerBeat;
    private bool _songStarted = false;
    private float _songEndTime;

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
        float songLength = lengthMs / 1000f;

        _songStartTime = Time.time;
        _nextBeatTime = _songStartTime;
        _songEndTime = _songStartTime + songLength;
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

        if (Time.time >= _nextBeatTime - travelTime)
        {
            if (Time.time < _songEndTime - 1.0f)
            {
                bool shouldPlayBeat = _beatQueue.Count <= 0 || _beatQueue.Dequeue();

                print(shouldPlayBeat);
                if (shouldPlayBeat)
                {
                    OnSpawn?.Invoke();
                }

                _nextBeatTime += _secondsPerBeat;
            }
        }

        if (Time.time >= _songEndTime)
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