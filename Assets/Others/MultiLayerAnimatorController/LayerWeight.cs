using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that manages layer weights independently, respecting SRP
public class LayerWeight : ILayerWeight
{
    private readonly Animator _animator;

    public LayerWeight(Animator animator)
    {
        _animator = animator;

        SetLayerWeight(0, 1f); // Body layer
        SetLayerWeight(1, 1f); // Hand layer
        SetLayerWeight(2, 1f); // Leg layer
    }

    public void SetLayerWeight(int layerIndex, float weight)
    {
        _animator.SetLayerWeight(layerIndex, weight);
    }
}