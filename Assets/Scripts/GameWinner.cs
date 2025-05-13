using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameWinner : MonoBehaviour
{
    public TextMeshProUGUI winnerText;

    public GameObject Winner;
    public GameObject Loser1;
    public GameObject Loser2;
    public GameObject Loser3;

    [System.Serializable]
    public class PlayerModel
    {
        public Transform root;   // The parent GameObject for rotation
        public Renderer head;    // Head material
        public Renderer body;    // Body material
    }

    public PlayerModel[] players;

    void Start()
    {
        setDisplays();
        setPlayers();
        winnerText.text = "PLAYER " + (Settings.Instance.playerPlacement[0] + 1) + " WINS!";
    }

    void setDisplays() {
        RectTransform WinnerRect = Winner.GetComponent<RectTransform>();
        RectTransform LoserRect1 = Loser1.GetComponent<RectTransform>();
        RectTransform LoserRect2 = Loser2.GetComponent<RectTransform>();
        RectTransform LoserRect3 = Loser3.GetComponent<RectTransform>();

        WinnerRect.anchoredPosition = new Vector3(0f, 421.2f, 0f);

        if (Settings.Instance.playerNumber == 4) {
            LoserRect1.anchoredPosition = new Vector3(-338f, -259.3f, 0f);
            LoserRect2.anchoredPosition = new Vector3(2.8f, -259.3f, 0f);
            LoserRect3.anchoredPosition = new Vector3(338f, -259.3f, 0f);
        }
        else if (Settings.Instance.playerNumber == 3) {
            LoserRect1.anchoredPosition = new Vector3(-170f, -259.3f, 0f);
            LoserRect2.anchoredPosition = new Vector3(170f, -259.3f, 0f);
            Loser3.SetActive(false);
        }
        else if (Settings.Instance.playerNumber == 2) {
            LoserRect1.anchoredPosition = new Vector3(2.8f, -259.3f, 0f);
            Loser2.SetActive(false);
            Loser3.SetActive(false);
        }
    }

    void setPlayers() {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].head.material = Settings.Instance.playerColors[Settings.Instance.playerPlacement[i]];
            players[i].body.material = Settings.Instance.playerColors[Settings.Instance.playerPlacement[i]];
        }
    }
}
