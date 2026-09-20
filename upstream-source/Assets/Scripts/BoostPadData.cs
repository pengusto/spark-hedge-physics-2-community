using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPadData : MonoBehaviour
{
    public float BoostForce = 1;
    public float BoostContinuousForce = 0.1f;
    public float BoostDuration = 1.5f;
    public float UsesCooldown = 1;

    [Header ("CHARGE")]
    public float Charge = 0;
    public float MaxCharge = 3;
    public float ChargeToAddOnUse = 0.1f;
    public float ChargeDecayTime = 0.1f;
    public AnimationCurve IncreaceOverCharge;
    public Gradient ColorChangeOverCharge;
    public float ColorPower = 4.5f;
    public MeshRenderer GlowArrowMesh;

    private void Update()
    {
        if(Charge > 0)
        {
            Charge -= Time.deltaTime * ChargeDecayTime;
            Charge = Mathf.Clamp(Charge, 0.0f, MaxCharge);
            GlowArrowMesh.material.SetColor("_MainColor", ColorChangeOverCharge.Evaluate(Charge / MaxCharge) * ColorPower);      
        }
    }

}
