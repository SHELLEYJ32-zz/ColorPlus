using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public int myRow, myCol;
    public bool activated = false;
    GameController myGameController;

    // Start is called before the first frame update
    void Start()
    {
        myGameController = GameObject.Find("GameControllerObject").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseDown()
    {
            myGameController.ProcessMouseClick(gameObject, myRow, myCol, gameObject.GetComponent<Renderer>().material.color);
    }
}
