using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CubeMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.4f;
    [SerializeField] private float mashTime = 0.4f;
    [SerializeField] private float winDistance = 2.5f;
    [SerializeField] private AudioClip whoosh;
    public TMP_Text winnerText;
    public GameObject backgroundPanel;
    public Animator animator;

    private bool won = false;
    private bool condition = true;
    private bool inputEnabled = false;

    private float leftPressCount = 0f;
    private float rightPressCount = 0f;
    private float mashTimer = 0f;
    private float timer2 = 0f;

    private int playerNumber = 0;
    private Vector3 direction = Vector3.zero;

    public List<int> teamPos;
    public int soloPlayer;

    void Start()
    {
        playerNumber = Settings.Instance.playerNumber;
    }

    void Update()
    {
        if (won) return;

        timer2 += Time.deltaTime;

        if (timer2 > 11f && condition)
        {
            condition = false;
            inputEnabled = true;
            backgroundPanel.SetActive(false);
            winnerText.gameObject.SetActive(false);
        }

        if (inputEnabled)
        {
            mashTimer += Time.deltaTime;

            if (mashTimer >= mashTime)
            {
                if (leftPressCount > rightPressCount)
                {
                    direction = Vector3.left;
                }
                else if (rightPressCount > leftPressCount)
                {
                    direction = Vector3.right;
                }
                else
                {
                    direction = Vector3.zero;
                }

                leftPressCount = 0f;
                rightPressCount = 0f;
                mashTimer = 0f;
            }

            // Player 1 - Q
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Now Im lost");
                RegisterPress(0);
            }

            // Player 2 - C
            if (Input.GetKeyDown(KeyCode.C))
            {
                RegisterPress(1);
            }

            // Player 3 - Y
            if (playerNumber >= 3 && Input.GetKeyDown(KeyCode.Y))
            {
                RegisterPress(2);
            }

            // Player 4 - M
            if (playerNumber == 4 && Input.GetKeyDown(KeyCode.M))
            {
                RegisterPress(3);
            }
        }

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (transform.position.x >= winDistance)
        {
            Win(false);
        }
        else if (transform.position.x <= -winDistance)
        {
            Win(true);
        }
    }

    void RegisterPress(int index)
    {
        if (teamPos == null || teamPos.Count <= index) return;

        bool isLeft = teamPos[index] < 0;
        if (soloPlayer == index + 1)
        {
            if (isLeft) leftPressCount++;
            else rightPressCount++;
        }
        if (isLeft) leftPressCount++;
        else rightPressCount++;
    }

    void Win(bool leftTeamWins)
    {
        AudioSource.PlayClipAtPoint(whoosh, transform.position, 1f);
        won = true;

        if (playerNumber == 2)
        {
            TwoPlayerWin(leftTeamWins);
        }
        else if (playerNumber == 3)
        {
            ThreePlayerWin(leftTeamWins);
        }
        else if (playerNumber == 4)
        {
            FourPlayerWin(leftTeamWins);
        }

        if (leftTeamWins)
        {
            winnerText.text = "Red Team Wins!";
            winnerText.colorGradient = new VertexGradient(
                HexToColor("#FF0000FF"),
                HexToColor("#FF3D3DFF"),
                HexToColor("#A4A4A4FF"),
                HexToColor("#D6D6D6FF")
            );
        }
        else
        {
            winnerText.text = "Blue Team Wins!";
            winnerText.colorGradient = new VertexGradient(
                HexToColor("#00FFEFFF"),
                HexToColor("#000BFFFF"),
                HexToColor("#FFFFFFFF"),
                HexToColor("#66676AFF")
            );
        }

        backgroundPanel.SetActive(true);
        winnerText.gameObject.SetActive(true);
        StartCoroutine(BigWin());
    }

    IEnumerator BigWin() {
        yield return new WaitForSeconds(2.5f);
        PlayAnimation();
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("WinScreen");
    }

    void PlayAnimation()
    {
        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("End");
        }
        else
        {
            Debug.LogError("Animator component not assigned!");
        }
    }

    void TwoPlayerWin(bool leftTeamWins)
    {
        if (Settings.Instance == null || teamPos == null || teamPos.Count < 2) return;

        int leftPlayer = -1;
        int rightPlayer = -1;

        // Identify which player is on which team
        for (int i = 0; i < 2; i++)
        {
            if (teamPos[i] < 0)
                leftPlayer = i;
            else if (teamPos[i] > 0)
                rightPlayer = i;
        }

        if (leftPlayer == -1 || rightPlayer == -1) return; // Defensive: both players must be assigned

        if (leftTeamWins)
        {
            Settings.Instance.playerPointsToAdd[leftPlayer] = 5;
            Settings.Instance.playerPointsToAdd[rightPlayer] = 0;
            Settings.Instance.playerPlacement[0] = leftPlayer;
            Settings.Instance.playerPlacement[1] = rightPlayer;
        }
        else
        {
            Settings.Instance.playerPointsToAdd[leftPlayer] = 0;
            Settings.Instance.playerPointsToAdd[rightPlayer] = 5;
            Settings.Instance.playerPlacement[0] = rightPlayer;
            Settings.Instance.playerPlacement[1] = leftPlayer;
        }
    }

    void ThreePlayerWin(bool leftTeamWins)
    {
        if (Settings.Instance == null || teamPos == null || teamPos.Count < soloPlayer) return;

        bool soloPlayerWin = (teamPos[soloPlayer - 1] < 0 && leftTeamWins) ||
                             (teamPos[soloPlayer - 1] > 0 && !leftTeamWins);

        if (soloPlayerWin)
        {
            Settings.Instance.playerPointsToAdd[soloPlayer - 1] = 5;
            Settings.Instance.playerPlacement[0] = soloPlayer - 1;

            int idx = 1;
            for (int i = 0; i < 3; i++)
            {
                if (i != (soloPlayer - 1))
                {
                    Settings.Instance.playerPointsToAdd[idx] = 0;
                    Settings.Instance.playerPlacement[idx++] = i;
                }
            }
        }
        else
        {
            Settings.Instance.playerPlacement[2] = soloPlayer - 1;

            int idx = 0;
            for (int i = 0; i < 3; i++)
            {
                if (i != (soloPlayer - 1))
                {
                    Settings.Instance.playerPointsToAdd[i] = 5;
                    Settings.Instance.playerPlacement[idx++] = i;
                }
                else
                {
                    Settings.Instance.playerPointsToAdd[i] = 0;
                }
            }
        }
    }

    void FourPlayerWin(bool leftTeamWins)
    {
        if (Settings.Instance == null || teamPos == null || teamPos.Count != 4) return;

        int winner = 0;
        int loser = 2;

        for (int i = 0; i < 4; i++)
        {
            if ((leftTeamWins && teamPos[i] < 0) || (!leftTeamWins && teamPos[i] > 0))
            {
                Settings.Instance.playerPointsToAdd[i] = 5;
                Settings.Instance.playerPlacement[winner++] = i;
            }
            else
            {
                Settings.Instance.playerPointsToAdd[i] = 0;
                Settings.Instance.playerPlacement[loser++] = i;
            }
        }
    }

    private Color HexToColor(string hex)
    {
        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
