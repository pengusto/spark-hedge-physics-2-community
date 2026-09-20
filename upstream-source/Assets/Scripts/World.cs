using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World WorldInfo;
    public bool IsGlobal = true;

    [Header("World - Proprieties")]
    public float WorldScale = 0.25f;
    public AnimationCurve AtmospherePressureCurve;
    public float SeaLevelPressureInAtm = 1;
    public bool AtmosphereHasO2 = true;
    public AnimationCurve AtmosphereHeightTemperatureCurve;
    public float SeaLevelTemperatureInCelcius = 20;
    public AnimationCurve HumidityHeightCurve;
    public float BaseHumidity = 50;

    [Header("General - Universal")]
    public AnimationCurve HeatGainOverSpeed;

    [Header("World - Gravity")]
    public float WorldGravity = 9.8f;
    public float GravityMultiplier = 5;
    public Vector3 GravityDir = new Vector3(0, 1, 0);

    [Header("World - FX")]
    public float surfaceParticleTransparency = 1;
    public static float SurfaceParticleTransparency = 1;

    [Header("Local - Weather")]
    public bool Rain = false;
    [Range(0, 6)] public float RainStrenght = 1; //(1: Strong) (2: Hurricane) (3: Under a waterfall)
    public bool Snow = false;
    [Range(0, 1)] public float SnowStrenght = 0.1f;
    public bool Wind = false;
    public Vector3 WindDir = Vector3.zero;

    // CACHE
    //public RadiantGI.RadiantVolume LocalGI_Volume;
    float temp;

    private void Start()
    {
        SurfaceParticleTransparency = surfaceParticleTransparency;
    }

    private void OnEnable()
    {
        if (IsGlobal) { WorldInfo = this; }
    }

    public static float SetEnvoirimentDetails(float WorldYposition, /*CarPhysics CarData,*/ World world)
    {
        //// INITIAL
        //WorldYposition *= world.WorldScale;

        //// PRESSURE
        //CarData.CurrentAtmoPressure = world.GetAtmosphereAt(WorldYposition);
        //CarData.AtmoHasO2 = world.AtmosphereHasO2;

        //// GRAVITY
        //CarData.CurrentGravity = world.GetCurrentGravity(CarData.transform.position);

        //if (CarData.CurrentAtmoPressure > 0.001f)
        //{
        //    // TEMPERATURE
        //    world.temp = world.SeaLevelTemperatureInCelcius + world.AtmosphereHeightTemperatureCurve.Evaluate(WorldYposition); // ROOM TEMP
        //    // MAKE IT ROOM TEMP OVER TIME
        //    CarData.CarSkinTemperature = Mathf.Lerp(CarData.CarSkinTemperature, world.temp, CarData.WorldDataPullRate * CarData.CarHeatRadiatePower);
        //    // ADD TEMP, THEN CLAMP
        //    CarData.CarSkinTemperature +=
        //        world.HeatGainOverSpeed.Evaluate(CarData.rigid.linearDamping) * CarData.WorldDataPullRate * CarData.CarSkinHeatAbsorption /** CarData.CurrentAtmoPressure*/;
        //    CarData.CarSkinTemperature = Mathf.Clamp(CarData.CarSkinTemperature, -300, 700);
        //}
        //else
        //{
        //    if (CarData.CarSkinTemperature > -200)
        //    {
        //        CarData.CarSkinTemperature -= CarData.WorldDataPullRate;
        //        if(CarData.CarSkinTemperature > 20) 
        //        { 
        //            CarData.CarSkinTemperature = Mathf.Lerp(CarData.CarSkinTemperature, 20, CarData.WorldDataPullRate * 0.05f);
        //        }
        //    }
        //}

        //// HUMIDITY
        //CarData.CurrentHumidity = 
        //    Mathf.Lerp(CarData.CurrentHumidity, world.HumidityHeightCurve.Evaluate(WorldYposition) * world.BaseHumidity, CarData.WorldDataPullRate * 1f);

        return world.temp;
    }

    // FUNCTIONS
    public float GetAtmosphereAt(float WorldYposition)
    {
        return AtmospherePressureCurve.Evaluate(WorldYposition) * SeaLevelPressureInAtm;
    }

    public Vector3 GetCurrentGravity(Vector3 pos)
    {
        return Vector3.down * (WorldGravity * GravityMultiplier);
    }

}
