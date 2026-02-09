using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //Tooltip Object in Scene
    private TextMeshProUGUI tooltipObject;
    private PlayerInteractionManager playerInteractionManager;

    private void Start()
    {
        tooltipObject = GameObject.FindGameObjectWithTag("Tooltip").GetComponent<TextMeshProUGUI>();
        tooltipObject.gameObject.SetActive(false);
        playerInteractionManager = FindObjectOfType<PlayerInteractionManager>();
    }

    public void EnableTooltip(PickableObject pickable)
    {
        if (pickable.interactable)
        {
            tooltipObject.gameObject.SetActive(true);
            tooltipObject.text = pickable.tooltipText;
            playerInteractionManager.CanInteract();
        }
    }

    public void DisableTooltip(PickableObject pickable)
    {
        if (pickable.interactable)
        {
            tooltipObject.gameObject.SetActive(false);
            playerInteractionManager.CannotInteract();
        }

    }
}
