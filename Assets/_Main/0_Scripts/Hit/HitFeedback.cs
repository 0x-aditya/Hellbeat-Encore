using UnityEngine;
using TMPro;
using ScriptLibrary.Singletons;

public class HitFeedback : Singleton<HitFeedback>
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float showTime = 0.5f;

    private float _hideAt;

    public void Show(string text)
    {
        label.text = text;
        label.gameObject.SetActive(true);
        _hideAt = Time.time + showTime;
    }


    private void Update()
    {
        if (label.gameObject.activeSelf && Time.time >= _hideAt)
            label.gameObject.SetActive(false);
    }
}