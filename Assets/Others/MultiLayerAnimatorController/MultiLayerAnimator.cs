using UnityEngine;

public class MultiLayerAnimator
{
    private IAnimationLayer _animationLayerManager;

    public MultiLayerAnimator(Animator animator)
    {
        // Create an instance of AnimationData
        AnimationData animationData = new AnimationData();

        // Inject AnimationData into the AnimationLayerManager
        _animationLayerManager = new AnimationLayer(animator, animationData);
        ILayerWeight _layerWeightManager = new LayerWeight(animator);

        _animationLayerManager.InitializeLayers();
    }



    public void PlayAnimations(int bodyLayerAnimationNumber, int handLayerAnimationNumber, int legLayerAnimationNumber)
    {
        _animationLayerManager.PlayAnimation(0, bodyLayerAnimationNumber); // Body layer
        _animationLayerManager.PlayAnimation(1, handLayerAnimationNumber); // Hand layer
        _animationLayerManager.PlayAnimation(2, legLayerAnimationNumber); // Leg layer
    }
}
