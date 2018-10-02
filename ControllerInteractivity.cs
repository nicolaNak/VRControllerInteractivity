using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
////// summary
/// this script relies onn gameObjects having suitable tags
/// tags: GeneralItem & CollectableItem. the controllers should also be tagged GameController
/// the class checks what the controller is touching via tags, and stores the item until it is either picked up or not touching
/// haptics are triggered to imitate the pressure of touching and releasing an object
//////
public class ControllerInteractivity : MonoBehaviour {

    public AudioManager AudioPlayer;

    public GameObject DataSetManager;

    private SteamVR_TrackedController controller;

    private DesciptionTextController HeldItemDescription;
    private MarkerController MarkerColor;

    public enum CarryingState
    {
        Empty,
        Touching,
        Holding
    }
    public CarryingState controllerCarryState;

    private GameObject currentObject;

    private InteractiveObject interactiveObject;
    private AdjustableObject pointCloudObjects; 
    private GeneralObjectInteractivity generalObject;

    private void OnEnable()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.TriggerClicked += HandleTriggerClicked;
        controller.TriggerUnclicked += HandleTriggerUnclicked;

        HeldItemDescription = GetComponentInChildren<DesciptionTextController>();
        MarkerColor = GetComponentInChildren<MarkerController>();

        controllerCarryState = CarryingState.Empty;

        Physics.IgnoreCollision(DataSetManager.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void OnDisable()
    {
        controller.TriggerClicked -= HandleTriggerClicked;
        controller.TriggerUnclicked -= HandleTriggerUnclicked;
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if(controllerCarryState == CarryingState.Touching)
        {
            //find which object it is
            if (currentObject.tag == "CollectableItem") { interactiveObject = currentObject.GetComponent<InteractiveObject>(); }
            if (currentObject.tag == "GeneralItem")     { generalObject = currentObject.GetComponent<GeneralObjectInteractivity>(); }

            controllerCarryState = CarryingState.Holding;
            //is held by other controller
            if (currentObject.transform.parent.tag == "GameController")
            {
               if(interactiveObject != null) { interactiveObject.ObjectTransferred(transform); }
               else if(generalObject != null) { generalObject.ObjectTransferred(transform); }  
            }
            else //can be picked up, TODO: see about playing each objects audio clip once if required
            {
               if(interactiveObject != null)
               {
                    interactiveObject.ObjectPickedUp(transform);
                    AudioPlayer.PlayAudioClip(interactiveObject.voiceOver);
               } 
               else if( generalObject != null)
               {
                    generalObject.ObjectPickedUp(transform);
                    AudioPlayer.PlayAudioClip(generalObject.voiceOver);
               }
            } 
        }
    }

    private void HandleTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        if(controllerCarryState == CarryingState.Holding)
        { 
			RemoveItem(true);             
        }
    }

    void OnTriggerEnter(Collider obj)
    {
        if(controllerCarryState != CarryingState.Holding)
        {
            StartCoroutine(HapticPulse(3000, 0.15f, true));
            currentObject = obj.gameObject;
            controllerCarryState = CarryingState.Touching;
            if(obj.GetComponent<ObjectDescriptionScript>() != null)  //null reference error for description when starts up
            {
                HeldItemDescription.SetDescription(obj.GetComponent<ObjectDescriptionScript>().description);
            }
        }
    }

    void OnTriggerExit(Collider obj)
    {
        if(controllerCarryState != CarryingState.Holding) { RemoveItem(); StartCoroutine(HapticPulse(1000, 0.15f, true, false)); }
        else
        {
            if(obj.name == currentObject.name) { RemoveItem(); }
        }     
    }

    private void RemoveItem(bool stillTouching = false)
    {
        if (stillTouching)
        {
            //when let got of adjustable object will still be touching it after
            controllerCarryState = CarryingState.Touching;
            pointCloudObjects.ReleaseObject();
            pointCloudObjects = null;
        }
        else
        {
            controllerCarryState = CarryingState.Empty;
            HeldItemDescription.ClearDescription();
            MarkerColor.ResetColor();
            currentObject = null;
            if (interactiveObject != null) { interactiveObject.ObjectReleased(); interactiveObject = null; }
            if(generalObject != null) { generalObject.ObjectReleased(); generalObject = null; }
        }
    }

    public void PassingHeldObject()
    {
        RemoveItem(true);
    }

    //TODO: too mmuch repeating code need to refactor, also this may have broken with the latest version of SteamVR?
    IEnumerator HapticPulse(ushort strength, float time, bool fade=false, bool fadeIn=true)
    {
        float timer = 0.0f;
        
        if (!fade)
        {
            while (timer < time)
            {
                controller.TriggerHapticPulse(strength);
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            if (fadeIn)
            {
                ushort startStrength = 0;
                float timeStep = time / Time.deltaTime; //deltaTime is not constant so not exact but close enough
                ushort strengthChange = (ushort)(strength / timeStep);
                while (timer < time)
                {
                    controller.TriggerHapticPulse(startStrength += strengthChange);
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                ushort startStrength = strength;
                float timeStep = time / Time.deltaTime;
                ushort strengthChange = (ushort)(strength / timeStep);
                while (timer < time)
                {
                    controller.TriggerHapticPulse(startStrength -= strengthChange);
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }
}
