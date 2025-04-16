using UnityEngine;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;

public class HandleVoteClick : MonoBehaviourPun
{
    UIManager UIManager;
    PointManager PointManager;
    GameManager GameManager;
    Canvas voteCanvas;
    public int votesAccept = 0;
    public int votesReject = 0;
    
    void Start()
    {
        UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        PointManager = GameObject.FindGameObjectWithTag("PointManager").GetComponent<PointManager>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void ClickedAccept()
    {
        if (voteCanvas != null) Destroy(voteCanvas.gameObject);
        if (GameObject.FindGameObjectWithTag("Timer") != null) Destroy(GameObject.FindGameObjectWithTag("Timer"));
        photonView.RPC("AddVoteAccept", RpcTarget.AllViaServer);
    }

    public void ClickedReject()
    {
        if (voteCanvas != null) Destroy(voteCanvas.gameObject);
        if (GameObject.FindGameObjectWithTag("Timer") != null) Destroy(GameObject.FindGameObjectWithTag("Timer"));
        photonView.RPC("AddVoteReject", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void AddVoteAccept()
    {
        votesAccept++;
    }

    [PunRPC]
    public void AddVoteReject()
    {
        votesReject++;
    }

    [PunRPC]
    public void StartVoting(int points, Photon.Realtime.Player playerTarget, bool nextTurnAfterVote)
    {
        voteCanvas = UIManager.GenerateVotePopup("black");
        voteCanvas.GetComponentsInChildren<Button>()[0].onClick.AddListener(ClickedAccept);
        voteCanvas.GetComponentsInChildren<Button>()[1].onClick.AddListener(ClickedReject);
        StartCoroutine(WaitForVotes(points, playerTarget, nextTurnAfterVote));
    }

    [PunRPC]
    public IEnumerator WaitForVotesDelayed(int points, Photon.Realtime.Player playerTarget, bool nextTurnAfterVote)
    {
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(40 + UIManager.waitingTime);
        yield return new WaitForSeconds(40 + UIManager.waitingTime);
        StartCoroutine(WaitForVotes(points, playerTarget, nextTurnAfterVote));
    }
    public IEnumerator WaitForVotes(int points, Photon.Realtime.Player playerTarget, bool nextTurnAfterVote)
    {
        votesAccept = 0; votesReject = 0; // Reset values
        Destroy(GameObject.FindGameObjectWithTag("Timer"));
        UIManager.GenerateTimer(UIManager.waitingTime);
        yield return new WaitForSeconds(UIManager.waitingTime); // Wait for votes
        if (votesAccept >= votesReject)
        {
            int currentPoints = PointManager.GetPointsCount(playerTarget);
            PointManager.SetPoints(playerTarget, currentPoints + points);
        }

        UIManager.GenerateDissolvingTextPopup("Richtig: " + votesAccept + "\n Falsch: " + votesReject, 2f, "black");
        if (nextTurnAfterVote) GameManager.nextTurn();
    }
}
