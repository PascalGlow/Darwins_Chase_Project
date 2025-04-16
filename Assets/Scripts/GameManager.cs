using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> players;
    public Player currentPlayer;
    [SerializeField] GameObject dice;
    [SerializeField] Canvas Menu;
    PointManager PointManager;
    [SerializeField] PlayerDCNotes PlayerDCNotes;
    int turnIndex = 0; public int lastRocket = 0;

    void Start()
    {
        StartCoroutine(WaitForFirstPlayer());
        PlayerDCNotes = PlayerDCNotes.GetComponent<PlayerDCNotes>();
        PointManager = GameObject.FindGameObjectWithTag("PointManager").GetComponent<PointManager>();
        
        // Setup ESC Main Menu
        Menu.gameObject.GetComponentInChildren<Image>().color = new Color(0, 0, 0, 0.85f);
        Menu.gameObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<Button>().interactable = false;
    }

    IEnumerator WaitForFirstPlayer()
    {
        yield return new WaitWhile(() => players.Count < 1);
        currentPlayer = players[0].GetComponent<Player>();
        currentPlayer.isTurn = true;
        UIManager.ToggleTransparancy(dice, currentPlayer);
 
    }
  
    [PunRPC]
    public void nextTurn()
    {
		currentPlayer.isTurn = false;
        if (++turnIndex >= players.Count) turnIndex = 0;
        currentPlayer = players[turnIndex].GetComponent<Player>();
        currentPlayer.isTurn = true;
        UIManager.ToggleTransparancy(dice, currentPlayer);
        dice.GetComponent<Animator>().Play("New Animation");
		if (currentPlayer.isTurn) 
        { 
            FindObjectOfType<DF2Client>().MyTurn(); 
        }
	}

	[PunRPC]
    public void AddPlayer(int playerID)
    {
        PhotonView playerView = PhotonView.Find(playerID);
        players.Add(playerView.gameObject);
        PointManager.AddPlayer(playerView.Owner);
    } 

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // TODO Lösche Spieler aus Liste
        nextTurn();
        base.OnPlayerLeftRoom(otherPlayer);
    }

    [PunRPC]
    void RPC_Set_Last_Rocket()
    {
        lastRocket++;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.gameObject.SetActive(!Menu.gameObject.activeSelf);
        }
    }
}
