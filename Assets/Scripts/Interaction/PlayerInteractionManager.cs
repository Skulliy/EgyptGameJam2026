using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField] private bool enableInteraction = false;

    public void CanInteract()
    {
        enableInteraction = true;
    }

    public void CannotInteract()
    {
        enableInteraction = false;
    }

    private void Update()
    {
        if (enableInteraction)
        {
            if (Input.GetButtonDown("Interact")) TriggerInteraction();
        }
    }
    private void TriggerInteraction()
    {

    }
}
