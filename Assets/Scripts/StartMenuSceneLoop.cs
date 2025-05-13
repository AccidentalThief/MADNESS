using UnityEngine;
using System.Collections;

public class StartMenuSceneLoop : MonoBehaviour
{
    public Animator animator;
    public AudioSource sound1;
    private Camera cam;

    void Start() {
        cam = Camera.main;
        cam.clearFlags = CameraClearFlags.Skybox;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sound1.PlayOneShot(sound1.clip);
            StartCoroutine(EndIt());
        }
    }

    IEnumerator EndIt()
    {
        GameObject nextMenu = null;
        PlayAnimation();
        yield return new WaitForSeconds(1.15f);

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(true); //Include inactive objects

        foreach (GameObject go in allObjects)
        {
            if (go.name == "PlayerNumber")
            {
                nextMenu = go;
                break;
            }
        }

        if (nextMenu != null)
        {
            nextMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Object with name 'PlayerNumber' not found.");
        }

        GameObject[] dudes = GameObject.FindGameObjectsWithTag("Dude");
        foreach(GameObject dude in dudes)
            Destroy(dude);
        yield return new WaitForSeconds(1f);
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