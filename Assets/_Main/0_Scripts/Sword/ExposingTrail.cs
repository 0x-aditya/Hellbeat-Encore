using Drakkar.GameUtils;
using ScriptLibrary.Singletons;
using UnityEngine;

public class ExposingTrail : Singleton<ExposingTrail>
{
    public DrakkarTrail trail;
    void Start()
    {
        trail = GetComponent<DrakkarTrail>();
    }
}
