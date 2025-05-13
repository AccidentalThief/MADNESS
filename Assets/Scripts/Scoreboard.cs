using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq; 
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public float fillSpeed;
    public GameObject[] bars = new GameObject[4];
    public Image[] fills = new Image[4];
    public Texture[] textures;
    public GameObject[] images = new GameObject[4];
    public TextMeshProUGUI[] points = new TextMeshProUGUI[4];

    private bool[] canFill = {false, false, false, false};
    private float maxValue = 100;

    void OnEnable()
    {
        setMax();
        setBars();
        AssignColor();
        updatePoints();
        StartCoroutine(LetsGo());
    }

    void setBars() {
        RectTransform Player1 = bars[0].GetComponent<RectTransform>();
        RectTransform Player2 = bars[1].GetComponent<RectTransform>();
        RectTransform Player3 = bars[2].GetComponent<RectTransform>();
        RectTransform Player4 = bars[3].GetComponent<RectTransform>();

        if (Settings.Instance.playerNumber == 4) {
            Player1.anchoredPosition = new Vector3(-322f, 125f, 0f);
            Player2.anchoredPosition = new Vector3(-322f, 25f, 0f);
            Player3.anchoredPosition = new Vector3(-322f, -75f, 0f);
            Player4.anchoredPosition = new Vector3(-322f, -175f, 0f);
        }
        else if (Settings.Instance.playerNumber == 3) {
            Player1.anchoredPosition = new Vector3(-322f, 110f, 0f);
            Player2.anchoredPosition = new Vector3(-322f, 0f, 0f);
            Player3.anchoredPosition = new Vector3(-322f, -110f, 0f);
            bars[3].SetActive(false);
        }
        else if (Settings.Instance.playerNumber == 2) {
            Player1.anchoredPosition = new Vector3(-322f, 62f, 0f);
            Player2.anchoredPosition = new Vector3(-322f, -62f, 0f);
            bars[2].SetActive(false);
            bars[3].SetActive(false);
        }
        //set bars to the initial values
        for (int i = 0; i < 4; i++) {
            fills[i].fillAmount = Mathf.Clamp01(Settings.Instance.playerPoints[i] / maxValue);
        }
        //set points to their initial values
        for (int i = 0; i < 4; i++) {
            points[i].text = Settings.Instance.playerPoints[i].ToString();
        }
    }

    void updatePoints() {
        for (int i = 0; i < 4; i++) {
            Settings.Instance.playerPoints[i] += Settings.Instance.playerPointsToAdd[i];
        }
    }

    IEnumerator LetsGo() {
        yield return new WaitForSeconds(1f);
        StartCoroutine(CountUp());
    }

    void setMax() {
        if (Settings.Instance.gamesOrPoints) {
            maxValue = Settings.Instance.games * 5;
        }
        else {
            maxValue = Settings.Instance.pointTotal;
        }
    }

    IEnumerator CountUp() {
        for (int i = 0; i < 4; i++) {
            canFill[i] = true;
            for (int j = 1; j < Settings.Instance.playerPointsToAdd[i] + 1; j++) {
                int pointV = Settings.Instance.playerPoints[i] - Settings.Instance.playerPointsToAdd[i] + j;
                points[i].text = pointV.ToString();
                yield return new WaitForSeconds(.2f);
            }
        }
    }
    
    void Update() {
        for (int i = 0; i < 4; i++) {
            if (canFill[i]) {
                float currentFill = fills[i].fillAmount;
                float targetFill = Mathf.Clamp01(Settings.Instance.playerPoints[i] / maxValue);
                fills[i].fillAmount = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * fillSpeed);
            }
        }
    }

    void AssignColor() {
        //I'm sorry...
        for (int i = 0; i < 4; i++) {
            fills[i].color = Settings.Instance.playerColors[i].color;
            string s = Settings.Instance.playerColors[i].name;
            RawImage img = images[i].GetComponent<RawImage>();
            if (s == "PlayerBlack") {
                img.texture = textures[0];
            }
            else if (s == "PlayerBlue") {
                img.texture = textures[1];
            }
            else if (s == "PlayerBrown") {
                img.texture = textures[2];
            }
            else if (s == "PlayerByan") {
                img.texture = textures[3];
            }
            else if (s == "PlayerDarkBlue") {
                img.texture = textures[4];
            }
            else if (s == "PlayerGreen") {
                img.texture = textures[5];
            }
            else if (s == "PlayerGrey") {
                img.texture = textures[6];
            }
            else if (s == "PlayerLightOrange") {
                img.texture = textures[7];
            }
            else if (s == "PlayerLime") {
                img.texture = textures[8];
            }
            else if (s == "PlayerMagenta") {
                img.texture = textures[9];
            }
            else if (s == "PlayerOrange") {
                img.texture = textures[10];
            }
            else if (s == "PlayerPurple") {
                img.texture = textures[11];
            }
            else if (s == "PlayerRed") {
                img.texture = textures[12];
            }
            else if (s == "PlayerRedOrange") {
                img.texture = textures[13];
            }
            else if (s == "PlayerWhite") {
                img.texture = textures[14];
            }
            else if (s == "PlayerYellow") {
                img.texture = textures[15];
            }
            else {
                Debug.Log("idk what to say man :(");
            }
        }
    }
}
