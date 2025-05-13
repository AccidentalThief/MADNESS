using UnityEngine;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
    public static Settings Instance;
    public int playerNumber;
    public int pointTotal;
    public int games = 0;
    public int gamesTotal;
    public int gameIndex;
    public bool gamesOrPoints;
    public List<Material> playerColors = new List <Material> {null, null, null, null};
    public List<int> playerPoints = new List <int> {0, 0, 0, 0};
    //index is placement, value is player : playerPlacement[0] = player who got first 
    public List<int> playerPlacement = new List <int> {0, 1, 2, 3};
    public List<int> playerPointsToAdd = new List <int> {3, 5, 6, 10};
    public List<int> availableIndexes = new List<int> {0, 2, 3, 6};
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        //Debug.Log(playerNumber);
    }
}
