using UnityEngine;

public class DedicateGameObject : MonoBehaviour
{
    public AudioClip dedicationSound;

    void Start()
    {
        
    }

    public void Dedicate(GameObject obj)
    {
        HighlightObject(obj);   
    }

    private void HighlightObject(GameObject obj)
    {
        // Example: Change the object's color to red
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }

        // Example: Scale up the object
        obj.transform.localScale *= 1.2f;

        // Example: Add a particle effect (ensure a ParticleSystem component is attached)
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            SoundManager.Instance.PlaySFXSound(dedicationSound);
        }
    }
}
