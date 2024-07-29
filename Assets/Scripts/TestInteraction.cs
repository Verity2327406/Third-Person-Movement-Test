using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    public string GetDescription()
    {
        return "Generate random number";
    }
    public void Interact()
    {
        float randm = UnityEngine.Random.Range(0f, 100f);
        Debug.Log((int)randm);
    }
}
