using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageScaleSelection : MonoBehaviour
{
    public AudioSource sound1;
    public Image[] selectableImages;
    public Camera[] cameras;
    public Animator animator;
    public float selectedScale = 1.2f;
    public float unselectedScale = 1f;
    public float scaleSpeed = 5f;
    public float zoomedZoom = 56f;
    public float normalZoom = 60f;
    public float zoomSpeed = 5f;
    private int selectedIndex = 1;

    void Start()
    {
        if (selectableImages.Length == 0)
        {
            Debug.LogError("ImageScaleSelection: Images not assigned!");
            return;
        }

        UpdateSelection();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // Corrected to LeftArrow
        {
            selectedIndex--;
            if (selectedIndex < 0)
            {
                selectedIndex = selectableImages.Length - 1;
            }
            sound1.PlayOneShot(sound1.clip);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // Corrected to RightArrow
        {
            selectedIndex++;
            if (selectedIndex >= selectableImages.Length)
            {
                selectedIndex = 0;
            }
            sound1.PlayOneShot(sound1.clip);
        }
        else if (Input.GetKeyDown(KeyCode.Space)) {
            sound1.PlayOneShot(sound1.clip);
            StartCoroutine(EndIt());
            Settings.Instance.playerNumber = selectedIndex + 2;
        } 
        else if (Input.inputString == "\b") {
            sound1.PlayOneShot(sound1.clip);
            GameObject start = GetObject("StartScreen");
            start.SetActive(true);
            transform.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < selectableImages.Length; i++)
        {
            float targetScale = (i == selectedIndex) ? selectedScale : unselectedScale;
            selectableImages[i].transform.localScale = Vector3.Lerp(
                selectableImages[i].transform.localScale,
                Vector3.one * targetScale,
                scaleSpeed * Time.deltaTime
            );
        }
        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            float targetScale = (i == selectedIndex) ? normalZoom : zoomedZoom;
            cameras[i].fieldOfView = Mathf.Lerp(
                cameras[i].fieldOfView,
                targetScale,
                scaleSpeed * Time.deltaTime
            );
            
        }
    }

    void OnEnable() {
        selectedIndex = 1;
    }

    GameObject GetObject(string name) {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true); //Include inactive objects

        foreach (GameObject go in allObjects)
        {
            if (go.name == name)
            {
                return go;
            }
        }
        return null;
    }

    IEnumerator EndIt()
    {
        GameObject nextMenu = GetObject("Colors");
        GameObject thisCanvas = GetObject("PlayerNumberCanvas");
        PlayAnimation();
        yield return new WaitForSeconds(1f);
        if (nextMenu != null) {
            nextMenu.SetActive(true);
            thisCanvas.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        thisCanvas.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }

    void PlayAnimation()
    {
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("End Of Scene");
        }
        else
        {
            Debug.LogError("Animator component not assigned!");
        }
    }
}