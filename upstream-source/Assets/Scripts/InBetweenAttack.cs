using UnityEngine;

public class InBetweenAttack : StateMachineBehaviour
{
    [Header("Parameters")]
    public int Seed = 123456;
    public Vector2 AttackRange = new Vector2(0,1);
    public string AttackTriggerName = "en_atk_";
    public bool ResetAllBeforeStarting = false;
    public int Mode = 0;

    [Header("Debug")]
    public bool LogValue = false;
    public int Progress = 0;
    System.Random r;
    public int value;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // RESET
        if (ResetAllBeforeStarting)
        {
            for (int i = (int)AttackRange.x; i < AttackRange.y; i++)
            {
                animator.ResetTrigger(AttackTriggerName + i);
            }
        }

        // SET
        if (Mode == 0) // RANDOM
        {
            r = new System.Random((animator.transform.name.Length * 99) + Seed + Progress);
            Progress++;
            value = r.Next((int)AttackRange.x, (int)AttackRange.y);
            animator.SetTrigger(AttackTriggerName + value);
        }
        else if(Mode == 1) // SEQUENCE
        {
            value = Progress;
            animator.SetTrigger(AttackTriggerName + value);
            Progress++;
            if (Progress > AttackRange.y) { Progress = 0; }
        }

        if (LogValue)
        {
            Debug.Log("InterAction: " + value);
            //BIGTEST
            //for (int i = 0; i < 1000; i++)
            //{
            //    Debug.Log("TEST:" + r.Next((int)AttackRange.x, (int)AttackRange.y));
            //}
        }
    }
}
