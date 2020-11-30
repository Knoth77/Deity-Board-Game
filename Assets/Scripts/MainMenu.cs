using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button BtnPlay;
    public Dropdown DdNumOfPlayers;

    public static int NumOfPlayers;

    // Start is called before the first frame update
    void Start()
    {
        BtnPlay.onClick.AddListener(Play);
        DdNumOfPlayers.onValueChanged.AddListener(delegate {
            PlayerNumberUpdate();
            });
    }

    // Update is called once per frame
    void Update()
    {
        //NumOfPlayers = int.Parse(DdNumOfPlayers.value.ToString());
    }

    void Play()
    {
        if (DdNumOfPlayers.value != 0)
        {
            Debug.Log("Play");
            SceneManager.LoadScene("WorldMapScene", LoadSceneMode.Single);
        }
    }

    void PlayerNumberUpdate()
    {
        Debug.Log("DD Change: " + DdNumOfPlayers.value.ToString());
        NumOfPlayers = int.Parse(DdNumOfPlayers.value.ToString());
        Debug.Log("Main Menu - Num Players: " + NumOfPlayers);
    }

}
