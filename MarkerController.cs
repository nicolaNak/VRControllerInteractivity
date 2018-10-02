using System.Collections;
using System.Collections.Generic;
using UnityEngine;
////// summary
/// allows for a marker 2D UI object on the controller to change color
/// set up so will change color depending on what it is touching, controller script does this
//////
public class MarkerController : MonoBehaviour {

    private MeshRenderer rend;
    private Material rendMat;
    private Color currentColor;

    public Color inactiveColor;

	void Start ()
    {
        rend = GetComponent<MeshRenderer>();
        rendMat = rend.sharedMaterial;
        currentColor = inactiveColor;
        rendMat.SetColor("_Color", currentColor);
	}
	
    public void ChangeColor(Color col)
    {
        StartCoroutine(LerpColor(currentColor, col)); 
    }

    public void ResetColor()
    {
        ChangeColor(inactiveColor);
    }

    IEnumerator LerpColor(Color start, Color finish)
    {
        float maxTime = 0.3f;
        float timer = 0.0f;
        float step = 0.0f;
        while(timer <= maxTime)
        {
            rendMat.SetColor("_Color", Color.Lerp(start, finish, step));
            timer += Time.deltaTime;
            step = timer / maxTime;
            yield return null;
        }
        currentColor = finish;
    }
}
