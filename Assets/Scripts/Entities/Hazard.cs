using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hazard : MonoBehaviour
{
    [Header("Hazard Metrics")]
    [SerializeField] private int spriteFrameCount;
    [SerializeField] private float scaleSet;
    [SerializeField] private int damageValue = 1;

    private CircleCollider2D cirCol;
    private Animator anim;
    private AnimationClip clipInfo;

    private List<GameObject> affectedEntities = new List<GameObject>();

    public int Damage
    {
        get => damageValue;
    }

    private void Awake()
    {
        cirCol   = GetComponent<CircleCollider2D>();
        anim     = GetComponent<Animator>();
        clipInfo = anim.runtimeAnimatorController.animationClips.FirstOrDefault();
    }

    public void CreateHazard()
    {
        StartCoroutine(ProcessHazard());
    }

    public bool MarkEntity(GameObject entity)
    {
        if (affectedEntities.Contains(entity))
            return false;
        else
        {
            affectedEntities.Add(entity);
            return true;
        }
    }

    private IEnumerator ProcessHazard()
    {
        cirCol.radius = scaleSet;

        float clipTime = clipInfo.length;

        // Create four steps
        float delta = clipTime / spriteFrameCount;
        float step = scaleSet / spriteFrameCount;

        int index = 0;

        while (index < spriteFrameCount)
        {
            cirCol.radius += step;
            ++index;
            yield return new WaitForSeconds(delta);
        }

        Pools.Instance.RemoveObject(gameObject);
    }
}
