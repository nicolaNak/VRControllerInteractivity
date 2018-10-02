using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//////summary
///Gameobject needs to be tagged InteractiveObject to be picked up by controller
///class activates a particle system and halo when activated/picked up.
/// the particles will flow to a linked Point of Interest (POI), and any linked objects will appear as well.
/// there is also an object fade function that can be called if needed
/// game object needs: audioSource set to not play on awake, Renderer, another object with a particle system, 
///                    a POI gameobject, a Halo Behaviour component, a material that allows for fading.
//////
public class InteractiveObject : GeneralObjectInteractivity {

    public float fadingDuration = 1.0f;

    public List<GameObject> LinkedHiddenObjects;
    public GameObject LinkedPOI;

    private Behaviour halo;

    public GameObject particleEffect;
    public float POIRevealDelay;
	public bool playSoundEffect = false;

    void OnEnable()
    {
        SetObjectValues(true);

        Renderer renderer = transform.GetComponent<Renderer>();
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0);
        StartCoroutine(FadeObject(1, false, fadingDuration));
    }

    protected override void SetObjectValues(bool haloEnabled = false)
    {
        parent = transform.parent;
        tableOrigin = table.position;
        originPosition = transform.position;
        originRotation = transform.rotation;
        originScale = transform.localScale;

        distanceBetween = new Vector3(0, (tableOrigin.y - transform.position.y), 0);

        halo = (Behaviour)GetComponent("Halo");
        halo.enabled = haloEnabled;
    }

    private void ActivateLinkedHiddenObjects()
    {
        foreach(GameObject obj in LinkedHiddenObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.GetComponent<InteractiveObject>().ActivateHalo();
            }
        }
    }

    private void DeactivateLinkedObjects()
    {
        foreach (GameObject obj in LinkedHiddenObjects)
        {
            obj.GetComponent<InteractiveObject>().DeactivateHalo();
        }
    }

    //called by object this is linked to
    public void FadeOutObject() //not being used currently but does work
    {
        StartCoroutine(FadeObject(0, true, fadingDuration));
    }

    public void ActivateHalo()
    {
        halo.enabled = true;
    }

    public void DeactivateHalo()
    {
        halo.enabled = false;
    }
	
    //called from controller script
    public override void ObjectPickedUp(Transform controller, bool playSoundEffect = false)
    {
        transform.parent = controller;
        if (playSoundEffect)
        {
            audioSource.Play();
        }
        particleEffect.SetActive(true);
        StartCoroutine(TrackParticles());
        ActivateLinkedHiddenObjects();
    }

    IEnumerator TrackParticles()
    {
        while (particleEffect.GetComponent<ParticleSystem>().isEmitting)
        {
            yield return null;
        }
        float timer = 0.0f; 
        while(timer < POIRevealDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        LinkedPOI.SetActive(true);
    }

    IEnumerator FadeObject(float targetAlpha, bool disable, float duration)
    {
        Renderer meshRenderer = transform.GetComponent<Renderer>();
        float diffAlpha = (targetAlpha - meshRenderer.material.color.a);

        float counter = 0;
        while(counter < duration)
        {
            float alphaAmount = meshRenderer.material.color.a + (Time.deltaTime * diffAlpha) / duration;
            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g,
                meshRenderer.material.color.b, alphaAmount);

            counter += Time.deltaTime;
            yield return null;
        }
        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g,
                meshRenderer.material.color.b, targetAlpha);
        if (disable)
        {
            gameObject.SetActive(false);
        }
    }

   
}
