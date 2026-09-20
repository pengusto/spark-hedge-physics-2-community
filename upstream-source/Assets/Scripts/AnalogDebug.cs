using UnityEngine;
using UnityEngine.UI;

public class AnalogDebug : MonoBehaviour
{
    public CharacterInput Inp;
    public CharacterActions actions;
    public GameObject SpawnRef;
    public float SampleFrequency = 0.0166666f;
    public Transform RawCenter;
    public Transform ChangedCenter;
    public float CenterMultiply = 80;

    // CACHE
    GameObject o;
    Vector3 p;

    private void Start()
    {
        InvokeRepeating("FakeUpdate", 0.1f, SampleFrequency);
    }

    void FakeUpdate()
    {
        // RAW
        o = GameObject.Instantiate(SpawnRef);
        o.transform.parent = RawCenter;
        o.SetActive(true);
        p = new Vector3(
            RawCenter.position.x + (Inp.LeftAnalogInput.x * CenterMultiply), 
            RawCenter.position.y + (Inp.LeftAnalogInput.z * CenterMultiply), 
            RawCenter.position.z
            );
        o.transform.position = p;

        // NEW
        o = GameObject.Instantiate(SpawnRef);
        o.transform.parent = ChangedCenter;
        o.SetActive(true);
        o.GetComponent<Image>().color = Color.green;
        p = new Vector3(
            ChangedCenter.position.x + (Inp.CamRelativeInput.x * CenterMultiply),
            ChangedCenter.position.y + (Inp.CamRelativeInput.z * CenterMultiply),
            ChangedCenter.position.z
            );
        o.transform.position = p;

        // NEW
        o = GameObject.Instantiate(SpawnRef);
        o.transform.parent = ChangedCenter;
        o.SetActive(true);
        o.GetComponent<Image>().color = new Color(1, 0.5f, 0.5f, 1);
        Vector3 v = Inp.CharCam.Proxy.transform.InverseTransformDirection(Inp.LeftAnalogInput);
        p = new Vector3(
            ChangedCenter.position.x + (v.x * CenterMultiply),
            ChangedCenter.position.y + (v.z * CenterMultiply),
            ChangedCenter.position.z
            );
        o.transform.position = p;

        // NEW
        o = GameObject.Instantiate(SpawnRef);
        o.transform.parent = ChangedCenter;
        o.SetActive(true);
        o.GetComponent<Image>().color = Color.blue;
        p = new Vector3(
            ChangedCenter.position.x + (actions.Basic.HS_Input.x * CenterMultiply),
            ChangedCenter.position.y + (actions.Basic.HS_Input.z * CenterMultiply),
            ChangedCenter.position.z
            );
        o.transform.position = p;

    }
}
