using UnityEngine;
using System.Collections;

public class ColorSelectorMenuSceneLoop : MonoBehaviour
{
    private Camera cam;
    public Color solidColor = Color.black;
    public Animator animator;
    public AudioSource sound1;

    void Start() {
        cam = Camera.main;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = solidColor;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            sound1.PlayOneShot(sound1.clip);
            StartCoroutine(EndIt());
        } 
        else if (Input.inputString == "\b") {
            sound1.PlayOneShot(sound1.clip);
            GameObject number = GetObject("PlayerNumber");
            number.SetActive(true);
            transform.gameObject.SetActive(false);
        }
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
        GameObject nextMenu = GetObject("Points");
        GameObject thisCanvas = GetObject("ColorsCanvas");
        PlayAnimation();
        yield return new WaitForSeconds(1f);
        if (nextMenu != null) {
            nextMenu.SetActive(true);
            thisCanvas.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        thisCanvas.SetActive(true);
        transform.gameObject.SetActive(false);
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
