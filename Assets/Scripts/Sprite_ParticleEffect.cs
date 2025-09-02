using System.Collections;
using System.Linq;
using UnityEngine;

public class Sprite_ParticleEffect : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Clear());
    }

    private IEnumerator Clear()
    {
        Animator anim = GetComponent<Animator>();
        float delay = anim.runtimeAnimatorController.animationClips.FirstOrDefault().length;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);    
    }
}
