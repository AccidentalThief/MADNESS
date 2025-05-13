using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AOOManager : MonoBehaviour
{
    public GameObject apple;
    public GameObject orange;
    public GameObject shyGuy;
    public List<GameObject> AOOplayers=new List<GameObject>();
    public List<GameObject> AOOplayerapples=new List<GameObject>();
    public List<GameObject> AOOplayeroranges=new List<GameObject>();
    private Vector3 spawnPostion=new Vector3(-2.5f,0f,-10f);
    private Vector3 appleOffset=new Vector3(0.5f,1.2f,0f);
    private Vector3 orangeOffset=new Vector3(-0.5f,1.2f,0f);
    public List<GameObject> playerHead=new List<GameObject>();
    public List<GameObject> playerBody=new List<GameObject>();
    int playerNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerNumber = Settings.Instance.playerNumber;
        List<Material> playerColor =Settings.Instance.playerColors;

        playerHead[0].GetComponent<Renderer>().material=playerColor[0];
        playerBody[0].GetComponent<Renderer>().material=playerColor[0];

        playerHead[1].GetComponent<Renderer>().material=playerColor[1];
        playerBody[1].GetComponent<Renderer>().material=playerColor[1];
        
        playerHead[2].GetComponent<Renderer>().material=playerColor[2];
        playerBody[2].GetComponent<Renderer>().material=playerColor[2];

        playerHead[3].GetComponent<Renderer>().material=playerColor[3];
        playerBody[3].GetComponent<Renderer>().material=playerColor[3];

        spawnPlayerandFruit();

        AOOGamePlayer gamePlayer=FindFirstObjectByType<AOOGamePlayer>();
        gamePlayer.AOOplayers=AOOplayers;
        gamePlayer.AOOplayerapples=AOOplayerapples;
        gamePlayer.AOOplayeroranges=AOOplayeroranges;
    }

    // Update is called once per frame
    void spawnPlayerandFruit(){
    for (int i = 0; i < playerNumber; i++)
        {
            AOOplayers[i].transform.position=spawnPostion;
            AOOplayerapples[i].transform.position=spawnPostion+appleOffset;
            AOOplayeroranges[i].transform.position=spawnPostion+orangeOffset;
        
            spawnPostion.x+=1.5f;
        }
    GameObject shyguyApple=Instantiate(apple,shyGuy.transform.position+appleOffset,Quaternion.identity);
    AOOplayerapples.Add(shyguyApple);
    GameObject shyguyOrange=Instantiate(orange,shyGuy.transform.position+orangeOffset,Quaternion.identity);
    AOOplayeroranges.Add(shyguyOrange);
    }
}
