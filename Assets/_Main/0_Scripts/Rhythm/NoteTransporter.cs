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
    [SerializeField] private float missDistance = 200f;
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
        _currentNotes.RemoveAll(n => n == null);
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
        StartCoroutine(MoveNoteToTarget(newNote, hitPoint.position, finalPoint.position, RhythmHandler.Instance.travelTime));
    }

    [SerializeField] private float missGraceTime = 0.15f;

    private IEnumerator MoveNoteToTarget(GameObject note, Vector2 targetPosition, Vector3 finalDestination, float travelTime)
    {
        Vector2 startPosition = note.transform.position;
        float startTime = Time.time;

        while (Time.time < startTime + travelTime)
        {
            if (!note) yield break;
            float t = (Time.time - startTime) / travelTime;
            note.transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        if (!note) yield break;
        note.transform.position = targetPosition;

        Vector3 legTwoStart = note.transform.position;
        float legTwoStartTime = Time.time;
        bool missRegistered = false;

        while (Time.time < legTwoStartTime + travelTime)
        {
            if (!note) yield break;

            if (!missRegistered && Time.time - legTwoStartTime > missGraceTime)
            {
                missRegistered = true;
                _currentNotes.Remove(note);
                RegisterMiss();
            }

            float t = (Time.time - legTwoStartTime) / travelTime;
            note.transform.position = Vector3.Lerp(legTwoStart, finalDestination, t);
            yield return null;
        }

        if (note) Destroy(note);
    }

    private void RegisterMiss()
    {
        if (GameManager.Instance) GameManager.Instance.UpdateScore("Miss");
        if (HitFeedback.Instance) HitFeedback.Instance.Show("Miss");
    }
}