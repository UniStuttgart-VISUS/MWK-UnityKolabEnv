using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutostartScript : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadAutostart() {
        GameObject autostart = GameObject.Instantiate(Resources.Load("Autostart")) as GameObject;
        GameObject.DontDestroyOnLoad(autostart);
    }
}
