using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    #region Properties
    public Button BtnUp;
    public Button BtnRight;
    public Button BtnDown;
    public Button BtnLeft;

    public Button BtnDice;
    public Button BtnItem;
    public Button BtnCamera;
    public Button BtnStat;
    public Button BtnCollapse;

    public Button BtnStatClose;
    public Image StatImage;

    public List<GameObject> ListMarker;
    public GameObject Player;
    public List<GameObject> ListPlayer;
    public Camera Camera;
    //public List<Camera> CameraList;
    public Dictionary<string, Sprite> SpriteDict;
    public GameObject Panel; 

    public int Movement;
    public int PlayerTurn;

    public bool FreeCameraMode;

    private float _mainSpeed = 100.0f; //regular speed
    private float _maxShift = 1000.0f; //Maximum speed when holdin gshift
    private float _totalRun = 1.0f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SpriteDict = new Dictionary<string, Sprite>();
        LoadStates();
        ListMarker = GameObject.FindGameObjectsWithTag("Marker").ToList();
        //Player = GameObject.FindGameObjectWithTag("Player");

        //BtnUp.onClick.AddListener(MovingUp);
        //BtnRight.onClick.AddListener(MovingRight);
        //BtnDown.onClick.AddListener(MovingDown);
        //BtnLeft.onClick.AddListener(MovingLeft);
        BtnDice.onClick.AddListener(Roll);
        BtnItem.onClick.AddListener(Item);
        BtnCamera.onClick.AddListener(FreeCamera);
        BtnStat.onClick.AddListener(Stat);
        BtnCollapse.onClick.AddListener(CollapseStat);


        Movement = 0;

        Debug.Log("Character Move - Number of players: " + MainMenu.NumOfPlayers);

        for (int i = 1; i <= MainMenu.NumOfPlayers; i++)
        {
            //Debug.Log("Hit the loop");
            var go = new GameObject("Player " + i);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteDict["player"+i];
            go.transform.position = new Vector3(0f, .8f, -6f);
            go.transform.localScale = new Vector3(1f, 1f, 1f);
            ListPlayer.Add(go);
        }

        if (ListPlayer.Any())
        {
            Player = ListPlayer.First();
            PlayerTurn = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(FreeCameraMode)
        {
            CameraModeUpdate();
            return;
        }

        if (Movement > 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MovingUp();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MovingDown();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MovingRight();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MovingLeft();
            }
            if(Movement <= 0)
            {
                BtnDice.GetComponentInChildren<Text>().text = "Roll";
                EndTurn();
            }
        }
    }

    void CameraModeUpdate()
    {
        float f = 0.0f;
        Vector3 p = GetBaseInput();
        
        _totalRun = Mathf.Clamp(_totalRun * 0.5f, 1f, 1000f);

        p = p * Time.deltaTime * _mainSpeed;
        Vector3 newPosition = Camera.transform.position;
        //If player wants to move on X and Z axis only
        Camera.transform.Translate(p);
        newPosition.x = Camera.transform.position.x;
        newPosition.z = Camera.transform.position.z;
        Camera.transform.position = newPosition;
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, .5f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -.5f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-.5f, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(.5f, 0, 0);
        }
        return p_Velocity;
    }

    #region Methods

    private void LoadStates()
    {
        object[] loadedIcons = Resources.LoadAll("PlayerSprites", typeof(Sprite));
        for (int x = 0; x < loadedIcons.Length; x++)
        {
            var name = ((Sprite)loadedIcons[x]).name;
            Debug.Log(name);
            SpriteDict.Add(name, (Sprite)loadedIcons[x]);
        }
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn");

        if (MainMenu.NumOfPlayers == PlayerTurn)
        {
            PlayerTurn = 1;
        }
        else
        {
            PlayerTurn++;
        }
        Player = ListPlayer[PlayerTurn - 1];
        Camera.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 2.7f, Player.transform.position.z - 4);
    }

    #endregion

    #region Buttons
    public void MovingUp()
    {
        GameObject closestMarker = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = Player.transform.position;
        foreach (var potentialTarget in ListMarker)
        {
            if(potentialTarget.transform.position.z <= currentPosition.z)
            {
                continue;
            }

            if(potentialTarget.transform.position.x != currentPosition.x)
            {
                continue;
            }

            var directionToTarget = potentialTarget.transform.position.z - currentPosition.z;
            if (directionToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = directionToTarget;
                closestMarker = potentialTarget;
            }
        }
        //Debug.Log(closestDistanceSqr);
        Debug.Log(Movement);

        if (closestMarker != null && !closestMarker.name.Contains("NoMarker"))
        {
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, closestMarker.transform.position.z);
            Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z + closestDistanceSqr);
            Movement--;
            BtnDice.GetComponentInChildren<Text>().text = Movement.ToString();
        }
    }

    public void MovingRight()
    {
        GameObject closestMarker = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = Player.transform.position;
        foreach (var potentialTarget in ListMarker)
        {
            if (potentialTarget.transform.position.z != currentPosition.z)
            {
                continue;
            }

            if (potentialTarget.transform.position.x <= currentPosition.x)
            {
                continue;
            }

            var directionToTarget = potentialTarget.transform.position.x - currentPosition.x;
            if (directionToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = directionToTarget;
                closestMarker = potentialTarget;
            }
        }
        //Debug.Log(closestDistanceSqr);
        Debug.Log(Movement);

        if (closestMarker != null && !closestMarker.name.Contains("NoMarker"))
        {
            Player.transform.position = new Vector3(closestMarker.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            Camera.transform.position = new Vector3(Camera.transform.position.x + closestDistanceSqr, Camera.transform.position.y, Camera.transform.position.z);
            Movement--; 
            BtnDice.GetComponentInChildren<Text>().text = Movement.ToString();
        }
    }

    public void MovingDown()
    {
        GameObject closestMarker = null;
        float closestDistanceSqr = Mathf.NegativeInfinity;
        Vector3 currentPosition = Player.transform.position;
        foreach (var potentialTarget in ListMarker)
        {
            if (potentialTarget.transform.position.z >= currentPosition.z)
            {
                continue;
            }

            if (potentialTarget.transform.position.x != currentPosition.x)
            {
                continue;
            }

            var directionToTarget = potentialTarget.transform.position.z - currentPosition.z;
            if (directionToTarget > closestDistanceSqr)
            {
                closestDistanceSqr = directionToTarget;
                closestMarker = potentialTarget;
            }
        }
        //Debug.Log(closestDistanceSqr);
        Debug.Log(Movement);

        if (closestMarker != null && !closestMarker.name.Contains("NoMarker"))
        {
            Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, closestMarker.transform.position.z);
            Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, Camera.transform.position.z + closestDistanceSqr);
            Movement--;
            BtnDice.GetComponentInChildren<Text>().text = Movement.ToString();
        }
    }

    public void MovingLeft()
    {
        GameObject closestMarker = null;
        float closestDistanceSqr = Mathf.NegativeInfinity;
        Vector3 currentPosition = Player.transform.position;
        foreach (var potentialTarget in ListMarker)
        {
            if (potentialTarget.transform.position.z != currentPosition.z)
            {
                continue;
            }

            if (potentialTarget.transform.position.x >= currentPosition.x)
            {
                continue;
            }

            var directionToTarget = potentialTarget.transform.position.x - currentPosition.x;
            if (directionToTarget > closestDistanceSqr)
            {
                closestDistanceSqr = directionToTarget;
                closestMarker = potentialTarget;
            }
        }
        //Debug.Log(closestDistanceSqr);
        Debug.Log(Movement);

        if (closestMarker != null && !closestMarker.name.Contains("NoMarker"))
        {
            Player.transform.position = new Vector3(closestMarker.transform.position.x, Player.transform.position.y, Player.transform.position.z);
            Camera.transform.position = new Vector3(Camera.transform.position.x + closestDistanceSqr, Camera.transform.position.y, Camera.transform.position.z);
            Movement--;
            BtnDice.GetComponentInChildren<Text>().text = Movement.ToString();
        }
    }

    public void Roll()
    {
        if (Movement <= 0)
        {
            var rando = new System.Random();
            Movement = rando.Next(1, 6);
            BtnDice.GetComponentInChildren<Text>().text = Movement.ToString();
            Debug.Log(Movement);
        }
    }

    public void Item()
    {

    }

    public void FreeCamera()
    {
        if(FreeCameraMode)
        {
            Camera.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 2.7f, Player.transform.position.z - 4);
        }

        FreeCameraMode = !FreeCameraMode;
    }

    public void Stat()
    {
        StatImage.enabled = true;
    }

    public void CollapseStat()
    {
        var rectTrans = Panel.GetComponent<RectTransform>();
        if (BtnCollapse.GetComponentInChildren<Text>().text == ">")
        {
            Utility.SetLeft(rectTrans, 1650);
            Utility.SetRight(rectTrans, 0);
            Utility.SetTop(rectTrans, 2);
            Utility.SetBottom(rectTrans, 100);
            BtnCollapse.GetComponentInChildren<Text>().text = "<";
        }
        else
        {
            Utility.SetLeft(rectTrans, 1075);
            Utility.SetRight(rectTrans, 0);
            Utility.SetTop(rectTrans, 2);
            Utility.SetBottom(rectTrans, 100);
            BtnCollapse.GetComponentInChildren<Text>().text = ">";
        }    
    }

    public void CloseStats()
    {
        StatImage.enabled = false;
    }

    #endregion

}
