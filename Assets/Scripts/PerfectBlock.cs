using UnityEngine;

public class PerfectBlock : MonoBehaviour
{
    [Header("Perfect Block Settings")]
    [SerializeField] private float blockWindow = 0.5f;

    private bool blockAvailable = false;
    private float blockTimer = 0f;

    public void OpenBlockWindow()
    {
        blockAvailable = true;
        blockTimer = blockWindow;

        if (PerfectBlockUI.Instance != null)
        {
            PerfectBlockUI.Instance.ShowPrompt();
        }

        Debug.Log("PERFECT BLOCK READY!");
    }

    private void Update()
    {
        if (!blockAvailable)
            return;

        blockTimer -= Time.deltaTime;

        if (blockTimer <= 0f)
        {
            CloseBlockWindow();
            Debug.Log("BLOCK MISSED!");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformPerfectBlock();
        }
    }

    private void PerformPerfectBlock()
    {
        blockAvailable = false;

        if (PerfectBlockUI.Instance != null)
        {
            PerfectBlockUI.Instance.HidePrompt();
        }

        Debug.Log("PERFECT BLOCK!");
    }

    public void CloseBlockWindow()
    {
        blockAvailable = false;
        blockTimer = 0f;

        if (PerfectBlockUI.Instance != null)
        {
            PerfectBlockUI.Instance.HidePrompt();
        }
    }

    public bool IsBlockAvailable()
    {
        return blockAvailable;
    }
}