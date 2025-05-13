/*using UnityEngine;
using TMPro;

public class WinnerDisplay : MonoBehaviour
{
    public TextMeshProGUI winnerText;               // Assign in Inspector
    public ParticleSystem confettiEffect;     // Assign confetti ParticleSystem
    public float displayDelay = 0.5f;

    void Start()
    {
        winnerText.gameObject.SetActive(false); // Hide by default
        StartCoroutine(DisplayWinnerText(1));
    }

    private IEnumerator DisplayWinnerText(int playerNumber)
    {
        yield return new WaitForSeconds(displayDelay);

        winnerText.text = "Player " + playerNumber + " Wins!";
        winnerText.gameObject.SetActive(true);
        winnerText.transform.localScale = Vector3.zero;

        // Smooth pop-in animation
        LeanTween.scale(winnerText.gameObject, Vector3.one, 0.5f).setEaseOutBack();

        if (confettiEffect != null)
            confettiEffect.Play();
    }
}
*/