using System.Collections;
using System.Collections.Generic;
using UnityEngine;
////// summary
/// accessed by controller class, attached to controller and needs a TextMesh attached and positioned
/// when an object with the corresponding text information is interacted with, it's given details will appear on the controller using this script
//////
public class DesciptionTextController : MonoBehaviour {

    private string description = " ";

    private TextMesh Text;

    public void SetDescription(string value)
    {
        description = value; 
    }

    public void ClearDescription()
    {
        description = " ";
    }

    void OnEnable()
    {
        Text = GetComponent<TextMesh>();
    }

    void Update ()
    {
        Text.text = description;
    }
}
