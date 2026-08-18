using UnityEngine;
using ScriptLibrary.Singletons;
public class RegisterHit : Singleton<RegisterHit>
{
    [SerializeField] private Transform hitPoint;

    public void RegisterHitEvent()
    {
        if (!NoteTransporter.ClosestNoteToHitPoint)
        {
            return;
        }
        Transform noteTransform = NoteTransporter.ClosestNoteToHitPoint.transform;
        float distance = Vector2.Distance(hitPoint.position, noteTransform.position);
        Destroy(NoteTransporter.ClosestNoteToHitPoint);
        
        print("Distance to hit point: " + distance);
        if (distance < 50f)
        {
            GameManager.Instance.UpdateScore("Perfect");
        }
        else if (distance < 100f)
        {
            GameManager.Instance.UpdateScore("Good");
        }
        else if (distance < 200f)
        {
            GameManager.Instance.UpdateScore("Meh");
        }
        else
        {
            GameManager.Instance.UpdateScore("Miss");
        }
    }
}
