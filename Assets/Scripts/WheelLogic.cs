using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WheelLogic : MonoBehaviour
{
    public TextMeshPro[] texts;
    public AudioSource audioSource;
    public AudioClip soundEffect;

    //parrallel arrays
    private string[] gameNames = {"Apples or Oranges", "Boulder Run", "Climbing", "Tug of War", "Connect 4", "Tightrope", "Cup Shuffle", "Fruit Collector"};
    private string[] sceneNames = {"ApplesOOranges", "", "RockWallRace", "TugOfWar", "Connect4", "Tightrope", "CupGame", ""};
    //shuffle these instead of the actual list
    private int[] indexes = {0, 1, 2, 3, 4, 5, 6, 7};

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        StartCoroutine(PlaySoundAfterDelay());
        if (Settings.Instance.playerNumber == 2) {
            Settings.Instance.availableIndexes = new List<int> {0, 2, 3, 4, 5, 6};
        }
        else
            Settings.Instance.availableIndexes = new List<int> {0, 2, 3, 5, 6};

        int theGame = Settings.Instance.availableIndexes[Random.Range(0, Settings.Instance.availableIndexes.Count)];

        ShuffleIndexes(theGame); // Pass theGame to the shuffle function
        UpdateWheel();
        StartCoroutine(GoToScene(sceneNames[theGame]));
    }

    void UpdateWheel() {
        for (int i = 0; i < texts.Length; i++) {
            TextMeshPro t = texts[i];
            t.text = gameNames[indexes[i]];
        }
    }

    void ShuffleIndexes(int theGame)
    {
        // Store the original value at index 4
        int originalFourthIndexValue = indexes[4];

        // Perform Fisher-Yates shuffle
        for (int i = indexes.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = indexes[i];
            indexes[i] = indexes[randomIndex];
            indexes[randomIndex] = temp;
        }

        int gameIndex = 0;
        for (int i = 0; i < indexes.Length; i++) {
            if (theGame == indexes[i]) {
                gameIndex = i;
                break;
            }
        }

        //perform swap
        indexes[gameIndex] = indexes[3];
        indexes[3] = theGame;
    }

    IEnumerator GoToScene(string scene)
    {
        yield return new WaitForSeconds(14f);
        SceneManager.LoadScene(sceneName:scene);
    }

    IEnumerator PlaySoundAfterDelay()
    {
        yield return new WaitForSeconds(4.5f);
        if (audioSource != null && soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }
}
