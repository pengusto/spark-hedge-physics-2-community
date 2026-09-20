using UnityEngine;

public class SetDotToTarget : StateMachineBehaviour
{
    [Header("References")]
    public string Param = "dot";
    public int Mode = 0;
    public float Speed = 3;
    public float GeneralMultiplier = -1;
    public bool ResetValue = true;

    [Header("Cache")]
    public float TargetValue;
    public float FinalValue = 0;
    public CharacterActions Actions;
    public CharacterInput Inp;
    public Vector3 Pos = Vector3.forward;
    public Vector3 TargetPos = Vector3.forward;
    public Vector3 FinalDir = Vector3.forward;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ResetValue) { FinalValue = 0; }
        if (Actions == null)
        {
            Actions = animator.GetComponent<RootMotionTransfer>().Actions;
            Inp = Actions.Inp;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Inp != null)
        {
            if (Inp.CurrentTarget != null)
            {
                Pos = Inp.transform.position;
                Pos = Vector3.ProjectOnPlane(Pos, Inp.transform.up);
                Pos = Vector3.ProjectOnPlane(Pos, -Inp.transform.up);
                TargetPos = Inp.CurrentTarget.transform.position;
                TargetPos = Vector3.ProjectOnPlane(TargetPos, Inp.transform.up);
                TargetPos = Vector3.ProjectOnPlane(TargetPos, -Inp.transform.up);

                FinalDir = (Pos - TargetPos).normalized;
                TargetValue = Vector3.Dot(Inp.anim.transform.right, FinalDir) * GeneralMultiplier;
            }
            else
            {
                TargetValue = 0;
            }

            FinalValue = Mathf.Lerp(FinalValue, TargetValue, Time.deltaTime * Speed);
            animator.SetFloat(Param, FinalValue);
        }

        //Debug.Log(Actions + "/" + Inp + "/" + TargetValue + " / " + FinalValue);
    }
}


