using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallLoot : MonoBehaviour, IInteractable, IGrabable
{
    public string itemName;

    [SerializeField] private Transform handIKTarget;
    [SerializeField] private Animator animator;
    [SerializeField] private Interactor interactor;

    public string GetDescription()
    {
        return itemName;
    }

    public void Interact()
    {
        handIKTarget.position = transform.position;
        animator.SetTrigger("GrabItem");
        Pickup();
    }

    public void Pickup()
    {
        interactor.SetupAnim(transform);
    }
}
