using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//////summary
/// this script is the base interactivity script. 
/// the gameObject needs: an audioSource, the gameobject it is sitting on as the parent
//////
public class GeneralObjectInteractivity : MonoBehaviour {

    public AudioClip SoundEffect;
	public bool playSoundEffect = true;

    public float originReturnDuration = 0.75f;

    public Transform restingSurface;
    protected Transform parent;
    protected Vector3 restingOrigin;
      
    protected Vector3 originPosition;
    protected Quaternion originRotation;
    protected Vector3 originScale;

    protected Vector3 distanceBetween; // distance from the restingSurface vector to the object vector

    void Start ()
    {
        SetObjectValues();
    }

    protected virtual void SetObjectValues(bool halo = false)
    {
        parent = transform.parent;
        restingOrigin = restingSurface.position;
        originPosition = transform.position;
        originRotation = transform.rotation;
        originScale = transform.localScale;

        distanceBetween = new Vector3(0, (restingOrigin.y - transform.position.y), 0);
    }

    //used for touchableItems
    public virtual void ObjectTouched(Transform controller, bool playSoundEffect = false)
    {
        if (playSoundEffect)
       {
           audioSource.Play();
       }
    }

    public virtual void ObjectPickedUp(Transform controller, bool playSoundEffect = false)
    {
        transform.parent = controller;
        if (playSoundEffect)
        {
            audioSource.Play();
        } 
    }

    public virtual void ObjectTransferred(Transform newController)
    {
        transform.parent.GetComponent<ControllerInteractivity>().PassingHeldObject();
        ObjectPickedUp(newController);
    }

    public virtual void ObjectReleased()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        transform.parent = parent;
        transform.localScale = originScale;
        StartCoroutine(ReturnToOrigin(originReturnDuration));
    }

    IEnumerator ReturnToOrigin(float duration)
    {
        float timer = 0.0f;
        while(timer <= 1.0f) //allows object to be thrown for a bit before moving back to table
        {
            timer += Time.deltaTime;
            yield return null;
        }

        float counter = 0;
        float travelTime = 0;
        while (counter < duration)
        {
            transform.position = Vector3.Lerp(transform.position, originPosition, travelTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, originRotation, travelTime);
            counter += Time.deltaTime;
            travelTime = counter / duration; //gives value between 0 and 1
            yield return null;
        }
    }


    void Update ()
    {
        if(restingSurface.parent.tag == "GameController")
        {
            originPosition = new Vector3(originPosition.x, (restingSurface.position.y - distanceBetween.y), originPosition.z);
            transform.position = originPosition;
        }
    }
}
