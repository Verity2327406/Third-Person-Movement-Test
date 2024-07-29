using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactor : MonoBehaviour
{
    #region Settings
    [Header("Settings")]
    public Transform interactorSource;
    public float interactRange;

    public GameObject interactionUI;
    public TMP_Text interactionText;
    #endregion

    private InputReader _ir;

    IInteractable _closeInteractable;

    HashSet<Collider> hitColliders = new HashSet<Collider>();
    HashSet<Collider> validColliders = new HashSet<Collider>();

    private void OnEnable()
    {
        _ir = GetComponent<InputReader>();
        _ir.interact += Interact;
    }

    private void OnDisable()
    {
        _ir.interact -= Interact;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactorSource.position, interactRange);
    }

    private void Update()
    {
        CheckForInteractions();
    }

    private void Interact()
    {

        _closeInteractable.Interact();

        //if(validColliders.Count > 0)
        //    if(validColliders[0].TryGetComponent(out IInteractable interactObj))
        //        interactObj.Interact();
    }

    private void CheckForInteractions()
    {
        // Setup
        if(validColliders.Count <= 0)
            _closeInteractable = null;

        interactionUI.SetActive(validColliders.Count > 0);

        // Get every collider within range
        Collider[] currentColliders = Physics.OverlapSphere(interactorSource.position, interactRange);
        // Temporary HashSet for the current frame's colliders
        HashSet<Collider> currentCollidersSet = new HashSet<Collider>(currentColliders);

        // Add new colliders
        foreach (var collider in currentCollidersSet)
        {
            hitColliders.Add(collider);
        }

        // Remove colliders that are no longer in range from both HashSets
        hitColliders.RemoveWhere(collider =>
        {
            if (!currentCollidersSet.Contains(collider))
            {
                validColliders.Remove(collider);
                return true;
            }

            return false;
        });

        // Add new colliders that have "IInteractable" to validColliders HashSet
        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
                validColliders.Add(collider);
        }

        // The closest collider with an "IInteractable" with have its interact text shown
        if (validColliders.Count <= 0) return;

        foreach (Collider col in validColliders)
        {
            _closeInteractable = col.GetComponent<IInteractable>();
            break;
        }

        interactionText.text = _closeInteractable.GetDescription();
    }
}
