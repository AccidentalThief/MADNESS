using UnityEngine;
using TMPro;

public class GuyAndTitleSetter : MonoBehaviour
{
    public Renderer head;    // Head material
    public Renderer body;    // Body material
    public TextMeshProUGUI title;
    public AudioSource winSound;

    void Start()
    {
        head.material = Settings.Instance.playerColors[Settings.Instance.playerPlacement[0]];
        body.material = Settings.Instance.playerColors[Settings.Instance.playerPlacement[0]];
        title.text = "Player " + (Settings.Instance.playerPlacement[0] + 1).ToString() + " Wins!";
        StartCoroutine(StartSequence());
    }

    private System.Collections.IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(4f); // Wait for 2 seconds
        if (winSound != null)
        {
            winSound.Play();
        }
    }
}
