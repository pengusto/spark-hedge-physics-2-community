using UnityEngine;

public class SudoRandomAnimatorStateVector : StateMachineBehaviour
{
    public string VectorX = "rand_vec_x";
    public string VectorY = "rand_vec_y";
    public string VectorZ = "rand_vec_z";
    public float VectorSize = 1;
    public int Seed = 1000;
    public int Progress;
    public Vector3 FinalVector;
    System.Random r = new System.Random();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        r = new System.Random(Seed + Progress);
        FinalVector = new Vector3();
        if (VectorX.Length == 0) { FinalVector.x = 0; } else { FinalVector.x = (float)r.Next(-1000, 1000) / 1000f; }
        if (VectorY.Length == 0) { FinalVector.y = 0; } else { FinalVector.y = (float)r.Next(-1000, 1000) / 1000f; }
        if (VectorZ.Length == 0) { FinalVector.z = 0; } else { FinalVector.z = (float)r.Next(-1000, 1000) / 1000f; }
        FinalVector = FinalVector.normalized * VectorSize;
        Debug.Log("ANIMATION VECTOR: " + FinalVector);
        if (animator.GetFloat(VectorX) != null) { animator.SetFloat(VectorX ,FinalVector.x); }
        if (animator.GetFloat(VectorY) != null) { animator.SetFloat(VectorY, FinalVector.y); }
        if (animator.GetFloat(VectorZ) != null) { animator.SetFloat(VectorZ, FinalVector.z); }
        Progress++;
    }

}
