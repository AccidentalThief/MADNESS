using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TOWGameManager : MonoBehaviour
{
    private Vector3 spawnPos = new Vector3(2.5f, 1.0f, 0f); // Starting spawn position
    private float playerSpacing = 1.5f; // Distance to move players further out
    private List<Vector3> usedPositions = new List<Vector3>(); // List to track used spawn positions
    public List<int> teamPos = new List<int> { 0, 0, 0, 0 }; // each value is either a -1 (team 1 on the left) or 1 (team 2 on the right) each value corresponds to one player
    private int soloPlayer = 0;
    public List<GameObject> playerHead = new List<GameObject>();
    public List<GameObject> playerBody = new List<GameObject>();
    public List<GameObject> players = new List<GameObject>();
    int playerNumber;

    void Start()
    {
        playerNumber = Settings.Instance.playerNumber;
        List<Material> playerColor = Settings.Instance.playerColors;

        CubeMovement cubeMovement = FindFirstObjectByType<CubeMovement>();
        // Sets all of the player textures

        playerHead[0].GetComponent<Renderer>().material = playerColor[0];
        playerBody[0].GetComponent<Renderer>().material = playerColor[0];

        playerHead[1].GetComponent<Renderer>().material = playerColor[1];
        playerBody[1].GetComponent<Renderer>().material = playerColor[1];

        playerHead[2].GetComponent<Renderer>().material = playerColor[2];
        playerBody[2].GetComponent<Renderer>().material = playerColor[2];

        playerHead[3].GetComponent<Renderer>().material = playerColor[3];
        playerBody[3].GetComponent<Renderer>().material = playerColor[3];

        if (playerNumber == 2)
        {
            Spawn2Player();
        }
        else if (playerNumber == 3)
        {
            Spawn3Player();
        }
        else if (playerNumber == 4)
        {
            Spawn4Player();
        }

        if (cubeMovement != null)
        {
            cubeMovement.teamPos = teamPos;
            cubeMovement.soloPlayer = soloPlayer;
        }
        else
        {
            Debug.LogError("CubeMovement script not found in the scene.");
        }
    }

    void Spawn2Player()
    {
        int spawnSide = Random.Range(0, 2) * 2 - 1;  // Randomly return -1 or 1 spawns player1
        for (int i = 0; i < playerNumber; i++)
        {
            Vector3 currentSpawnPosition = new Vector3(spawnPos.x * -spawnSide, spawnPos.y, spawnPos.z);
            players[i].transform.position = currentSpawnPosition;
            if (currentSpawnPosition.x < 0)
            {
                players[i].transform.rotation = Quaternion.Euler(0, 90, 0);
                teamPos[i] = -1;
            }
            else
            {
                players[i].transform.rotation = Quaternion.Euler(0, -90, 0);
                teamPos[i] = 1;
            }
            spawnSide *= -1;
        }
    }

    void Spawn3Player()
    {
        int spawnSide = Random.Range(0, 2) * 2 - 1; // determines solo player side
        soloPlayer = Random.Range(1, 4); // determines solo player;
        int secondPlayer = Random.Range(0, 2);
        int soloPlayerSide;
        int thirdPlayer = 2;
        if (secondPlayer == 0)
        {
            thirdPlayer = 1;
        }
        else
        {
            thirdPlayer = 0;
        }
        players[(soloPlayer - 1)].transform.position = new Vector3(spawnPos.x * -spawnSide, spawnPos.y, spawnPos.z);
        if (spawnSide > 0)
        {
            players[soloPlayer - 1].transform.rotation = Quaternion.Euler(0, 90, 0);
            teamPos[soloPlayer - 1] = -1;
            soloPlayerSide = -1;
        }
        else
        {
            players[soloPlayer - 1].transform.rotation = Quaternion.Euler(0, -90, 0);
            teamPos[soloPlayer - 1] = 1;
            soloPlayerSide = 1;
        }
        spawnSide *= -1;
        players[(soloPlayer - 1)].transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        if (soloPlayer != 1)
        {
            players[0].transform.position = new Vector3((spawnPos.x + (playerSpacing * secondPlayer)) * -spawnSide, spawnPos.y, spawnPos.z);
            teamPos[0] = soloPlayerSide * -1;
            if (soloPlayerSide < 0)
            {
                players[0].transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                players[0].transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        if (soloPlayer != 2)
        {
            if (soloPlayer == 1)
            {
                players[1].transform.position = new Vector3((spawnPos.x + (playerSpacing * secondPlayer)) * -spawnSide, spawnPos.y, spawnPos.z);
                teamPos[1] = soloPlayerSide * -1;
            }
            else
            {
                players[1].transform.position = new Vector3((spawnPos.x + (playerSpacing * thirdPlayer)) * -spawnSide, spawnPos.y, spawnPos.z);
                teamPos[1] = soloPlayerSide * -1;
            }
            if (soloPlayerSide < 0)
            {
                players[1].transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                players[1].transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        if (soloPlayer != 3)
        {
            players[2].transform.position = new Vector3((spawnPos.x + (playerSpacing * thirdPlayer)) * -spawnSide, spawnPos.y, spawnPos.z);
            teamPos[2] = soloPlayerSide * -1;
            if (soloPlayerSide < 0)
            {
                players[2].transform.rotation = Quaternion.Euler(0, -90, 0);
            }
            else
            {
                players[2].transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    void Spawn4Player()
    {
        usedPositions.Clear(); // Ensure the used positions list is clear for a new spawn

        // Iterate for the number of players
        for (int i = 0; i < playerNumber; i++)
        {
            Vector3 finalSpawnPos = GetUniqueSpawnPos(i);

            players[i].transform.position = finalSpawnPos;
            if (finalSpawnPos.x < 0)
            {
                players[i].transform.rotation = Quaternion.Euler(0, 90, 0);
                teamPos[i] = (-1);
            }
            else
            {
                players[i].transform.rotation = Quaternion.Euler(0, -90, 0);
                teamPos[i] = 1;
            }
        }
    }

    Vector3 GetUniqueSpawnPos(int playerIndex)
    {
        List<Vector3> possibleSpawns = new List<Vector3>
        {
            new Vector3(-(spawnPos.x), spawnPos.y, spawnPos.z),
            new Vector3(-(spawnPos.x + playerSpacing), spawnPos.y, spawnPos.z),
            new Vector3(spawnPos.x, spawnPos.y, spawnPos.z),
            new Vector3(spawnPos.x + playerSpacing, spawnPos.y, spawnPos.z)
        };

        ShuffleList(possibleSpawns); // optional for randomness

        foreach (var pos in possibleSpawns)
        {
            if (!usedPositions.Contains(pos))
            {
                usedPositions.Add(pos);
                return pos;
            }
        }
        return spawnPos; // fallback
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}