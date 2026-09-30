using Drakkar.GameUtils;
using UnityEngine;

public class SlashBehaviour : StateMachineBehaviour
{
    private DrakkarTrail _drakkarTrail => ExposingTrail.Instance.trail;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter");
        _drakkarTrail.End();
        _drakkarTrail.Begin();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Exit");
        var next = animator.GetNextAnimatorStateInfo(layerIndex);
        if (animator.IsInTransition(layerIndex) && next.fullPathHash == stateInfo.fullPathHash)
            return;

        _drakkarTrail.End();
    }
}