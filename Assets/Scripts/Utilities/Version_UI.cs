using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Version_UI : MonoBehaviour
{
    public Text ver;
    public GameObject updater;
    [HideInInspector]
    public string current_version = "";
    
    void Start()
    {
        current_version = updater.GetComponent<Updater>().current_version;
    }

    void Update()
    {
        ver.text = current_version;
    }
}
