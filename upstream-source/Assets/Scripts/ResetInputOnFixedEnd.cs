using UnityEngine;
using UnityEngine.InputSystem;

public class ResetInputOnFixedEnd : MonoBehaviour
{
    [Header("Make sure this executes last.")]
    public CharacterInput Inp;

    private void FixedUpdate()
    {
        Inp.Jump = false;
        Inp.LightAttack = false;
        Inp.HeavyAttack = false;
        Inp.JDash = false;
        Inp.Shot = false;
        Inp.Dash = false;
        Inp.Special = false;
        Inp.Block = false;
        Inp.RightStickPress = false;

    }
}
