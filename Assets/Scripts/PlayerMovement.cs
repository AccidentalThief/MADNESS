using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
   public GameObject target;
   public float speed =5f;
   private bool pushLeft=false;
   private bool firstUpdate=true;
   private Vector3 spacing=new Vector3(0f,0f,0f);
   private float heightFixer=1f; // trust me we need this
    // Update is called once per frame
    int playerNumber;
     void Start()
    {
        playerNumber = Settings.Instance.playerNumber;
    }
  void Update()
    {
    
        if (firstUpdate)
        {
            firstUpdate=false;
            if (transform.position.x<0){
            pushLeft=true;
            spacing.x-=transform.position.x;
            spacing.y+=heightFixer;

        }
        else{
            spacing.x+=transform.position.x;
            spacing.y-=heightFixer;

        }
            
        }
        if (pushLeft){
            transform.position=Vector3.MoveTowards(transform.position, target.transform.position-spacing, speed*Time.deltaTime);
        }
        else{
            transform.position=Vector3.MoveTowards(transform.position, target.transform.position+spacing, speed*Time.deltaTime);
        }
        
    }
}
