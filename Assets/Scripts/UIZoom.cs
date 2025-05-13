using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UIZoom : MonoBehaviour
{
    public GameObject UIComponent;    
    public float minScale = 0f;
    public float maxScale = 1f;
    public float scaleSpeed = 1f;
    public float pause = 5f;
    public float zoomOut = 0f;

    public GameObject animation;

    private RectTransform UIRect;
    private float timer = 0f;
    private bool ignore = false;
    
    void Start()
    {
        UIRect = UIComponent.GetComponent<RectTransform>();
        UIRect.localScale = new Vector3(minScale, minScale, minScale);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && timer > 9.5) {
            ignore = true;
            timer = 0;
        }
        if (timer > 2 && ignore) {
            animation.SetActive(true);
        }
        if (timer > 2.75 && ignore) {
            animation.SetActive(false);
            SceneManager.LoadScene(sceneName:"MainMenu");
        }
        
        else if ((timer > zoomOut && zoomOut != 0) || ignore) {
            float currentScale = UIRect.localScale.x;
            float targetScale = minScale;
            float newScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * scaleSpeed);
            UIRect.localScale = new Vector3(newScale, newScale, newScale);
        }
        else if (timer > pause) {
            float currentScale = UIRect.localScale.x;
            float targetScale = maxScale;
            float newScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * scaleSpeed);
            UIRect.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}
