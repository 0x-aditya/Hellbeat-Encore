using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace _Main.Scripts.Rhythm
{
    public class AddSongToBeatMap : MonoBehaviour
    {
        [SerializeField] private float bpm = 144f;
        [SerializeField] private string beatMapName;
        private float _lastUpdateTime;
        private float _secondsPerBeat;
        private readonly List<bool> _beatList = new();
        private void Start()
        {
            _secondsPerBeat = 60f/bpm; 
            _lastUpdateTime = Time.time;
        }

        private void OnEnable()
        {
            BeatHandler.Instance.OnBeat += AddFalseBeat;
            BeatHandler.Instance.OnMarker += AddTrueBeat;
        }

        private void OnDisable()
        {
            BeatHandler.Instance.OnBeat -= AddFalseBeat;
            BeatHandler.Instance.OnMarker -= AddTrueBeat;
        }

        private void OnDestroy()
        {
            UpdateJsonFile();
        }

        private void AddFalseBeat()
        {
            if (Time.time - _lastUpdateTime < _secondsPerBeat)
            {
                return;
            }
            _lastUpdateTime = Time.time;
            _beatList.Add(false);
            print(false);
        }
        
        private void AddTrueBeat()
        {
            if (Time.time - _lastUpdateTime < _secondsPerBeat)
            {
                _beatList.RemoveAt(_beatList.Count - 1);
            }
            _lastUpdateTime = Time.time;
            _beatList.Add(true);
            print(true);
        }
        private void UpdateJsonFile()
        {
            // === display list ===
            string something = "[";
            for (int i = 0; i < _beatList.Count; i++)
            {
                something += _beatList[i] ? "1" : "0";
                if (i < _beatList.Count - 1)
                {
                    something += ",";
                }
            }
            something += "]";
            print(something);
            // ====================
            
            BeatPattern pattern = new BeatPattern
            {
                beats = _beatList.ToArray()
            };
            string json = JsonUtility.ToJson(pattern, true);
 
            string fileName = beatMapName.EndsWith(".json") ? beatMapName : beatMapName + ".json";
            string path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, json);
 
            Debug.Log("Saved beat map to: " + path);

        }
    }
}