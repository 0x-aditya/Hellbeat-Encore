using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteTransporter : MonoBehaviour
{
    public static GameObject ClosestNoteToHitPoint { get; private set; }
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform finalPoint;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform canvasTransform;
    
    // Tracks in-flight notes; List so we don't have to worry about fixed size / index math.
    private List<GameObject> _currentNotes = new List<GameObject>();

    public void OnEnable()
    {
        print("Enabling NoteTransporter");
        RhythmHandler.Instance.OnSpawn += SpawnNote;
    }

    public void OnDisable()
    {
        RhythmHandler.Instance.OnSpawn -= SpawnNote;
    }
    
    public void Update()
    {
        UpdateClosestNote();
    }
    
    private void UpdateClosestNote()
    {
        float closestDistance = float.MaxValue;
        GameObject closestNote = null;

        foreach (GameObject note in _currentNotes)
        {
            if (!note) continue;

            float distance = Vector2.Distance(hitPoint.position, note.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNote = note;
            }
        }
        ClosestNoteToHitPoint = closestNote;
    }

    private void SpawnNote()
    {
        GameObject newNote = Instantiate(notePrefab, spawnPoint.position, spawnPoint.rotation, canvasTransform);
        _currentNotes.Add(newNote);

        int noteIndex = _currentNotes.Count - 1;
        TransportNote(noteIndex, hitPoint, RhythmHandler.Instance.travelTime);
    }

    public void TransportNote(int noteIndex, Transform target, float travelTime)
    {
        GameObject note = _currentNotes[noteIndex];
        StartCoroutine(MoveNoteToTarget(note, target.position, finalPoint.position, travelTime));
    }

    private IEnumerator MoveNoteToTarget(GameObject note, Vector2 targetPosition, Vector3 finalDestination, float travelTime)
    {
        float exitDuration = travelTime;
        Vector2 startPosition = note.transform.position;
 
        double startDspTime = Time.time;
        double arrivalDspTime = startDspTime + travelTime;
 
        while (Time.time < arrivalDspTime)
        {
            float t = (float)((Time.time - startDspTime) / travelTime);
            note.transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
 
        note.transform.position = targetPosition;
 
        Vector3 legTwoStart = note.transform.position;
        double legTwoStartDspTime = Time.time;
        double legTwoArrivalDspTime = legTwoStartDspTime + exitDuration;
 
        while (Time.time < legTwoArrivalDspTime)
        {
            float t = (float)((Time.time - legTwoStartDspTime) / exitDuration);
            note.transform.position = Vector3.Lerp(legTwoStart, finalDestination, t);
            yield return null;
        }
 
        note.transform.position = finalDestination;
    }
}