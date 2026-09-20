using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceDataContainer : MonoBehaviour
{
    public enum RaceType { Circuit, Sprint, Misc };
    public RaceData Race;
    public static RaceData StoredRaceData;

    [System.Serializable]
    public class Racer : System.IComparable<Racer>
    {
        [Header("BasicParameter")]
        public int PlayerID = 0;
        public int RacerInitialID = 0;
        public GameObject CarPrefab;
        public bool Is_AI = true;
        public string PilotName = "(unnamed)";
        public string CarName = "(unnamed car)";
        public bool Skilled = false;
        public bool ChangeAgression = false;
        public bool IsAgressive = false;
        public bool AgroPlayerOnly = false;
        [Range(0, 2)] public float SkillMultiplier = 1;
        [Range(0, 2)] public float LearningMultiplier = 1;
        [Range(0, 10)] public float WanderMutliplier = 1;
        [Range(0, 10)] public float AccelMultiplier = 1;
        [Range(0, 10)] public float BoostMultiplier = 1;
        [Range(0, 40)] public float TurningMultiplier = 1;
        public float TournamentPoints = 0;

        public int CompareTo(Racer other)
        {
            return TournamentPoints.CompareTo(other.TournamentPoints);
        }
    }

    [System.Serializable]
    public class RaceData
    {
        public RaceType Type = RaceType.Circuit;

        public string PlanetName = "Earth?";
        public string CountryName = "USA?";
        public string RegionName = "Texas?";
        public string RaceName = "Drive Drive";

        public int Laps = 3;
        public float CombatEnableTime = 20;
        public bool EnableCombatLater = true;

        [Range(0, 1)] public float Race_SkillMultiplier = 1;
        [Range(0, 1)] public float Race_LearningMultiplier = 1;
        [Range(0, 10)] public float Race_WanderMutliplier = 1;

        [Range(0, 2)] public float Race_Dificulty = 1;
        [Range(0, 2)] public float Race_AccelMultiplier = 1;
        [Range(0, 2)] public float Race_BoostMultiplier = 1;
        [Range(0, 40)] public float Race_TurningMultiplier = 1;
        public List<Racer> Racers = new List<Racer>();

        [Header("Optimizations")]
        public float SplineCheckMultiplier = 1;
    }
}
