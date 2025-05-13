using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PointsGame : MonoBehaviour
{
    public Slider slider;
    public Animator animator;
    public Button gamesButton;
    public Button pointsButton;
    public TextMeshProUGUI displayText;
    public AudioSource sound1;
    public TMP_Text winnerText;
    public GameObject backgroundPanel;

    private int pointsValue = 5;
    private int gamesValue = 8;
    private bool points = true;
    

    void Start() {
        gamesButton.onClick.AddListener(() => EditSlider(false));
        pointsButton.onClick.AddListener(() => EditSlider(true));
        
    }

    void EditSlider(bool switchToPoints) {
        if (switchToPoints) {
            slider.minValue = 3;
            slider.maxValue = 20;
            slider.value = pointsValue/5;
            points = true;
        }
        else {
            slider.minValue = 3;
            slider.maxValue = 20;
            slider.value = gamesValue;
            points = false;
        }
        sound1.PlayOneShot(sound1.clip);
    }

    void Update() {
        if (points) {
            pointsValue = (int) slider.value * 5;
            Settings.Instance.pointTotal = pointsValue;
            Settings.Instance.gamesOrPoints = false;
            displayText.text = "Points to Win: " + pointsValue;
        }
        else {
            gamesValue = (int) slider.value;
            Settings.Instance.gamesTotal = gamesValue;
            Settings.Instance.gamesOrPoints = true;
            displayText.text = "Games to Play: " + gamesValue;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            sound1.PlayOneShot(sound1.clip);
            StartCoroutine(EndIt());
        } 
        else if (Input.inputString == "\b") {
            sound1.PlayOneShot(sound1.clip);
            GameObject colors = GetObject("Colors");
            colors.SetActive(true);
            transform.gameObject.SetActive(false);
        }
    }
    
    IEnumerator EndIt()
    {
        PlayAnimation();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName:"Wheel");
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
