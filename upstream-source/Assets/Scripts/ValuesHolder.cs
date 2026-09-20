using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValuesHolder : MonoBehaviour
{
    public int id = 0;
    public List<string> Strings = new List<string>();
    public List<float> Floats = new List<float>();
    public List<int> Integers = new List<int>();
    public List<bool> Bools = new List<bool>();
    public List<Color> Colors = new List<Color>();
    public List<GameObject> Objects = new List<GameObject>();

    [Header("UI STUFF")]
    public List<Image> Ui_images;
    public List<Text> Ui_text;
    public List<Sprite> Images;

    //[Header("REWIRED")]
   // public Rewired.Controller controller;

    [Header("Extras")]
    public List<UnityEngine.Video.VideoClip> Video = new List<UnityEngine.Video.VideoClip>();
}
