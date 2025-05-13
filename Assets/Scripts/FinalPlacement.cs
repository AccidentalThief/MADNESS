using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FinalPlacement : MonoBehaviour
{
    public TextMeshProUGUI[] placements = new TextMeshProUGUI[4];
    public GameObject[] bars = new GameObject[4];
    public Image[] fills = new Image[4];

    void Start()
    {
        setBars();
        AssignColor();
    }

    void setBars() {
        if (Settings.Instance.playerNumber == 3)
            bars[3].SetActive(false);
        else if (Settings.Instance.playerNumber == 2) {
            bars[2].SetActive(false);
            bars[3].SetActive(false);
        }

        for (int i = 0; i < 4; i++) {
            placements[i].text = "Player " + (Settings.Instance.playerPlacement[i] + 1).ToString();
        }
    }

    void AssignColor() {
        for (int i = 0; i < 4; i++)
            fills[i].color = Settings.Instance.playerColors[Settings.Instance.playerPlacement[i]].color;
    }
}
