using UnityEngine;
using System.Collections;

public class SwapToScene : MonoBehaviour
{
    public GameObject Scoreboard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        StartCoroutine(Swap());
    }

    IEnumerator Swap() {
        yield return new WaitForSeconds(5.5f);
        Scoreboard.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}
