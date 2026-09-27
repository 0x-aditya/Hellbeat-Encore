using Drakkar.GameUtils;
using UnityEngine;

public class DrakkarTrailCreator : MonoBehaviour
{
    [SerializeField] private DrakkarTrail trail;
    void Start()
    {
        trail = GetComponent<DrakkarTrail>();
        trail.Begin();
    }
}
