using UnityEngine;
using TMPro; // Important!
using System.Collections;

public class Opacity : MonoBehaviour
{
    public float cycleSpeed = 1.0f;
    public float minOpacity = 0.0f;
    public float maxOpacity = 1.0f;
    private float lastOpacity = 0;
    public TextMeshProUGUI textComponent; // Or TextMeshPro for 3D Text.

    private float timeElapsed = 0.0f;
    private bool okStart = false;
    private bool fade = false;
    private bool hasStarted = false;

    

    void Start()
    {
        hasStarted = true;
        StartCoroutine(StartWithDelay());
    }

    void OnEnable() {
        if (!hasStarted) {
            //If start has not run, then do nothing.
            return;
        }
        StartCoroutine(StartWithDelay());
    }

    IEnumerator StartWithDelay() {
        // reset variables
        timeElapsed = 0;
        okStart = false;
        fade = false;
        textComponent.color = new Color(1f, 1f, 1f, 0f);
        textComponent = GetComponent<TextMeshProUGUI>(); //Or TextMeshPro for 3D Text
        yield return StartCoroutine("Wait");
    }

    void Update()
    {
        float opacity;
        if(okStart) {
            timeElapsed += Time.deltaTime * cycleSpeed;
        }
        if(!fade) {
            opacity = Mathf.Sin(timeElapsed);
            if(opacity < 0) {
                opacity *= -1;
            }
        }
        else {
            opacity = lastOpacity - 2.0f * Time.deltaTime;
        }
        opacity = Mathf.Lerp(minOpacity, maxOpacity, opacity);
        Color currentColor = textComponent.color;
        currentColor.a = opacity;
        textComponent.color = currentColor;
        lastOpacity = opacity;
        if (Input.GetKeyDown(KeyCode.Space)) {
            fade = true;
        }
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(2);
        okStart = true;
    }
}