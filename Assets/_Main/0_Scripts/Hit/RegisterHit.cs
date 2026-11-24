using UnityEngine;
using ScriptLibrary.Singletons;

public class RegisterHit : Singleton<RegisterHit>
{
    [SerializeField] private Transform hitPoint;
    [SerializeField] private KeyCode hitKey = KeyCode.Space;
    [SerializeField] private float maxHitDistance = 200f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(hitKey))
        {
            RegisterHitEvent();
        }
    }

    public void RegisterHitEvent()
    {
        GameObject note = NoteTransporter.ClosestNoteToHitPoint;
        if (!note) return;

        float distance = Vector2.Distance(hitPoint.position, note.transform.position);
        
        if (distance >= maxHitDistance) return;

        Destroy(note);

        string result;
        if (distance < 50f)       result = "Perfect";
        else if (distance < 100f) result = "Good";
        else                      result = "Meh";

        GameManager.Instance.UpdateScore(result);
        HitFeedback.Instance?.Show(result);
    }
}