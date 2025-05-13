using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class WinScreenTransition : MonoBehaviour
{
    private float timer = 0f;

    void Start()
    {
        Settings.Instance.games++;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 14f)
            Transition();
    }

    void Transition() {
        Debug.Log("hi");
        // Points
        if (!Settings.Instance.gamesOrPoints) {
            Debug.Log("hi2");
            for (int i = 0; i < 4; i++) {
                if ((Settings.Instance.playerPoints[i]) >= Settings.Instance.pointTotal) {
                    Debug.Log(Settings.Instance.playerPoints[i] + " " + Settings.Instance.pointTotal);
                    SortPlayers();
                    SceneManager.LoadScene(sceneName:"Final");
                    return;
                }
            }
            SceneManager.LoadScene(sceneName:"Wheel");
            return;
        }
        // Games
        else {
            if (Settings.Instance.games >= Settings.Instance.gamesTotal) {
                SortPlayers();
                SceneManager.LoadScene(sceneName:"Final");
                return;
            }
            else {
                Debug.Log("wrong");
                SceneManager.LoadScene(sceneName:"Wheel");
                return;
            }
        }
        
    }

    void SortPlayers() {
        int[] sortedIndices = Enumerable.Range(0, Settings.Instance.playerPoints.Count)
                .OrderByDescending(i => Settings.Instance.playerPoints[i])
                .ToArray();
        int[] sortedPlacement = new int[Settings.Instance.playerPlacement.Count];
        // Populate the sorted placement array based on the sorted points' original indices
        for (int i = 0; i < sortedIndices.Length; i++) {
                sortedPlacement[i] = Settings.Instance.playerPlacement[sortedIndices[i]];
            }

        for (int i = 0; i < 4; i++) {
            Settings.Instance.playerPlacement[i] = sortedPlacement[i];
        }
    }
}
