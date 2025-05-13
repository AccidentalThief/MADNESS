using UnityEngine;

public class DisableChildrenColors : MonoBehaviour
{
    public int index;
    public GameObject ui;

    void OnEnable()
    {
        //Debug.Log(Settings.Instance.playerNumber);
        if (index <= 2 || Settings.Instance.playerNumber >= 4) {
            EnableAll();
        }
        else if (index == 3 && Settings.Instance.playerNumber < 3) {
            DisableChildrenWithTag(); 
        }
        else if (index == 4) {
            DisableChildrenWithTag(); 
        } 
        else {
            EnableAll();
        }
        
    }

    void DisableChildrenWithTag()
    {
        ui.SetActive(false);
        foreach (Transform child in transform) 
        {
            if (child.CompareTag("Dude")) 
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void EnableAll() {
        ui.SetActive(true);
        foreach (Transform child in transform) 
        {
            child.gameObject.SetActive(true);
        }
    }
}
