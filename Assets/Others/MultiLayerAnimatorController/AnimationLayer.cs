using System.Collections.Generic;
using UnityEngine;

public class AnimationLayer : IAnimationLayer
{
    private readonly Animator _animator;
    private readonly AnimationData _animationData;

    public AnimationLayer(Animator animator, AnimationData animationData)
    {
        _animator = animator;
        _animationData = animationData;
    }

    public void InitializeLayers()
    {
        // No need to initialize layers here, as they are handled by the AnimationData class
    }

    public void PlayAnimation(int layerIndex, int animationIndex)
    {
        List<string> animations = _animationData.GetAnimationsForLayer(layerIndex);

        if (animations != null && animationIndex >= 0 && animationIndex < animations.Count)
        {
            string animationName = animations[animationIndex];
            _animator.Play(animationName, layerIndex);
        }
        else
        {
            Debug.LogWarning($"Invalid layer or animation index: Layer {layerIndex}, Animation {animationIndex}");
        }
    }

}
