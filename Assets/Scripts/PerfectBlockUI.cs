using UnityEngine;

public class PerfectBlockUI : MonoBehaviour
{
    public static PerfectBlockUI Instance;

    [SerializeField] private GameObject prompt;

    private void Awake()
    {
        Instance = this;

        HidePrompt();
    }

    public void ShowPrompt()
    {
        prompt.SetActive(true);
    }

    public void HidePrompt()
    {
        prompt.SetActive(false);
    }
}