using UnityEngine;
using System.Collections;

public class JumpingGuy : MonoBehaviour
{
    public float pause = 4.5f;
    public float unpause = 7.5f;
    public float jumpRate = .8f;
    public float jumpHeight = 1f;
    public float jumpDuration = .4f;
    public GameObject target;

    private float timer = 0f;
    private float timer2 = 0f;
    private float originalY;
    
    void Start() {
        originalY = target.transform.position.y;
    }

    void Update()
    {
        timer += Time.deltaTime;
        timer2 += Time.deltaTime;
        if (timer >= jumpRate && (timer2 < pause || timer2 > unpause)) {
            Jump();
            timer = 0;
        }
    }

    void Jump() {
        LeanTween.moveY(gameObject, originalY + jumpHeight, jumpDuration)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.moveY(gameObject, originalY, jumpDuration)
                    .setEaseInQuad();
            });
    }
}
