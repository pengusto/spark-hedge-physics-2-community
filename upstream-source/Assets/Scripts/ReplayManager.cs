using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ReplayManager : MonoBehaviour
{
    //[Header("Setup")]
    //public List<CarInput> Vehicles = new List<CarInput>();
    //public float time = 0;
    //public bool RecordMode = true;

    //// CACHE
    //Replay r = new Replay();
    //FrameInputs f = new FrameInputs();
    //CarData d = new CarData();
    //bool ReplayLoaded = false;
    //int frame;

    //private void FixedUpdate()
    //{
    //    if (RecordMode)
    //    {
    //        Record(Time.fixedDeltaTime);
    //    }
    //    else
    //    {
    //        r = LoadReplay(Application.dataPath + "/Replays" + "/test.replay");
    //        if (r != null) { Play(); }
    //    }
    //}

    //void Record(float deltaTime)
    //{
    //    time += deltaTime;
    //    for (int i = 0; i < Vehicles.Count; i++)
    //    {
    //        if(r.Sequences.Count < Vehicles.Count)
    //        {
    //            r.Sequences.Add(new ReplaySequence());
    //        }
    //    }

    //    for (int i = 0; i < r.Sequences.Count; i++)
    //    {
    //        // SETUP
    //        r.Sequences[i].IsAi = Vehicles[i].IsAi;

    //        // RECORD AI
    //        if (r.Sequences[i].IsAi)
    //        {
    //            f.Time = time;
    //            f.Accel = Vehicles[i].Ai_Accel;
    //            f.Steer = Vehicles[i].Ai_Steer;
    //            f.UpDown = Vehicles[i].Ai_UpDown;
    //            f.Strafe = 0;
    //            f.Break = false;
    //            f.Boost = Vehicles[i].Ai_Boosting;
    //            f.Spin = Vehicles[i].Ai_Spin;
    //            r.Sequences[i].Inputs.Add(f);
    //            d.Speed = Vehicles[i].Car.rigid.velocity;
    //            d.Pos = Vehicles[i].transform.position;
    //            d.Rot = Vehicles[i].transform.rotation;
    //            r.Sequences[i].Data.Add(d);
    //        }
    //    }
    //}

    //void Play()
    //{
    //    for (int i = 0; i < Vehicles.Count; i++)
    //    {
    //        /* So... I stopped here, thinking if this is really nescessary... 
    //         * I mean it may or it may not work... who knows, its a ton of debugging and testing
    //         * and I'm only a solo dev who's game has grown waaaaay out of proportion.
    //         * I'm already dreading implementing online so I should probably prioritize that instead.
    //         * Dear steam reviews please have mercy, maybe I should stick to single player games. :(
    //         */
            
    //    }

    //    frame++;
    //}

    
    //// SAVE AND LOAD ===============

    //public void SaveReplay(Replay replay)
    //{
    //    if (Directory.Exists(Application.dataPath + "/Replays") == false)
    //    { Directory.CreateDirectory(Application.dataPath + "/Replays"); }

    //    string path = Application.dataPath + "/Replays" + "/test.replay";

    //    BinaryFormatter bf = new BinaryFormatter();
    //    FileStream file = File.Create(path);
    //    bf.Serialize(file, replay);
    //    file.Close();

    //    //System.IO.File.WriteAllText(Application.dataPath + "/Replays/Test.replay", JsonUtility.ToJson(r, true));
    //}
    //public Replay LoadReplay(string path)
    //{
    //    if (File.Exists(path))
    //    {
    //        try
    //        {
    //            BinaryFormatter bf = new BinaryFormatter();
    //            FileStream file = File.Open(path, FileMode.Open);
    //            Replay replay = (Replay)bf.Deserialize(file);
    //            file.Close();

    //            return replay;
    //        }
    //        catch (SerializationException)
    //        {
    //            Debug.LogError("Failed to load at: " + path);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Replay file does not exist at: " + path);
    //    }

    //    return null;
    //}

    //// SYSTEM CLASSES ===============

    //[System.Serializable]
    //public class Replay
    //{
    //    public List<ReplaySequence> Sequences = new List<ReplaySequence>();
    //}

    //[System.Serializable]
    //public class ReplaySequence
    //{
    //    public bool IsAi = false;
    //    public List<FrameInputs> Inputs = new List<FrameInputs>();
    //    public List<CarData> Data = new List<CarData>();
    //}

    //[System.Serializable]
    //public class FrameInputs
    //{
    //    public float Time = 0;
    //    public float Accel = 0;
    //    public float Steer = 0;
    //    public float UpDown = 0;
    //    public float Strafe = 0;
    //    public bool Break = false;
    //    public bool Boost = false;
    //    public bool Spin = false;
    //}

    //[System.Serializable]
    //public class CarData
    //{
    //    public float Time;
    //    public Vector3 Speed;
    //    public Vector3 Pos;
    //    public Quaternion Rot;
    //}
}
