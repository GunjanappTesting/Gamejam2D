using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Examples : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator1;
    [SerializeField] private Animator characterAnimator2;
    [SerializeField] private Animator characterAnimator3;

    private MultiLayerAnimator multiLayerAnimator1;
    private MultiLayerAnimator multiLayerAnimator2;
    private MultiLayerAnimator multiLayerAnimator3;


    private void Start()
    {
        multiLayerAnimator1 = new MultiLayerAnimator(characterAnimator1);
        multiLayerAnimator2 = new MultiLayerAnimator(characterAnimator2);
        multiLayerAnimator3 = new MultiLayerAnimator(characterAnimator3);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int num = Random.Range(0, 11);
            multiLayerAnimator1.PlayAnimations(num, num, num);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            int num = Random.Range(0, 11);
            multiLayerAnimator2.PlayAnimations(num, num, num);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            int num = Random.Range(0, 11);
            multiLayerAnimator3.PlayAnimations(num, num, num);
        }
    }
}
