using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffect : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;

    public static IEnumerator RandomSpawnEffect(GameObject toSpawn, AnimationClip animation, Vector3 center, float radius, float delay, int count){

        for(int i = 0; i < count; i++){
            GameObject newSpawned = Instantiate(toSpawn, center, Quaternion.identity);
            newSpawned.transform.position += Random.insideUnitSphere * radius;
            BattleEffect thisEffect = newSpawned.GetComponent<BattleEffect>();
            thisEffect.SetEffect(animation);
            yield return new WaitForSeconds(delay);
        }

        yield return null;
    }

    void Awake(){
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetEffect(AnimationClip animation){
        string animationName = animation.name;
        animator.Play(animationName);
        Destroy(gameObject, animation.length);
    }

    public void SetColor(Color color){
        spriteRenderer.color = color;
    }
}
