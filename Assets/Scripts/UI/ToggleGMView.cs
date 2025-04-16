using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGMView : MonoBehaviour
{
    [SerializeField] GameObject GMAnzeige;
    public void OnToggleGMView()
    {
        if (GMAnzeige.activeSelf)
        {
            GMAnzeige.SetActive(false);
        } else {
            GMAnzeige.SetActive(true);
        }
    }
}
