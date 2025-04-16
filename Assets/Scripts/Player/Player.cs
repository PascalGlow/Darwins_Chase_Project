using System.Collections;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPunCallbacks
{
    public Node currentNode;
    public Node nextNode;
    public SetupCards cards;
    public PlayerDCNotes Notes;
    public int steps;
    public bool moving;
    public bool paused = false;
    public bool goalReached;
    public bool isTurn;

    [SerializeField] PathSelect pathSelect;
    CardActions CardActions;
    GameRoute gameRoute;
    GameManager GameManager;
    void Start() {
        // Get Components
        transform.Find("NameDisplay").GetComponentInChildren<TMP_Text>().text = photonView.Owner.NickName;
            

        Notes = GetComponent<PlayerDCNotes>();
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        CardActions = GetComponent<CardActions>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        cards = GameManager.GetComponent<SetupCards>();
        pathSelect = pathSelect.GetComponent<PathSelect>();
        gameRoute = GameManager.GetComponentInChildren<GameRoute>();
        
        // Initialize Player
        goalReached = false;
        if (photonView.IsMine) GameManager.photonView.RPC("AddPlayer", RpcTarget.AllBufferedViaServer, photonView.ViewID);
        currentNode = gameRoute.childNodeList[0];

        Color c = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
            props.Add("color", new Vector3(c.r, c.g, c.b));
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            photonView.RPC("RPC_Set_Color", RpcTarget.All, new Vector3(c.r, c.g, c.b)); // RPC doesn't support Color Object, thus use Vector3
        }
        
    }

    [PunRPC]
    void RPC_Set_Color(Vector3 c)
    {
        GetComponent<SpriteRenderer>().color = new Color(c.x, c.y, c.z);
    }

    public IEnumerator Move()
    {
        if (moving)
        {
            yield break;
        }
        moving = true;

        while(steps > 0 && !goalReached)
        {
            // Check if goal is reached
            if (currentNode.neighbors.Count == 0)
            {
                goalReached = true;
                break;
            }


			if (currentNode.neighbors.Count > 1 )
            {
                if (!currentNode.neighbors[0].visited && !currentNode.neighbors[1].visited) // Force path to unvisited rocket
                {
                    nextNode = currentNode.neighbors[1];
                } else {
                    // Spawn path selection UI
                    PathSelect path1 = Instantiate(pathSelect, currentNode.neighbors[0].Transform.position, Quaternion.identity);
                    PathSelect path2 = Instantiate(pathSelect, currentNode.neighbors[1].Transform.position, Quaternion.identity);
                    path1.transform.parent = currentNode.neighbors[0].Transform;
                    path2.transform.parent = currentNode.neighbors[1].Transform;
                    
                    paused = true;
                    while (paused) yield return new WaitForSeconds(.2f); // Wait for a choice
                    path1.GetComponent<SpriteRenderer>().enabled = false;
                    path2.GetComponent<SpriteRenderer>().enabled = false;
                    Destroy(path1);
                    Destroy(path2);
                }
                
            } else {
                nextNode = currentNode.neighbors[0];
            }
            Vector2 nextNodePos = nextNode.Transform.position;
            while(MoveTo(nextNodePos)) {yield return null;}
            yield return new WaitForSeconds(.2f);

            if (nextNode.FieldType == 4 && !nextNode.visited) // Check if unvisited rocket field ahead
            {
                steps = 0;
                GameManager.photonView.RPC("RPC_Set_Last_Rocket", RpcTarget.All);
            } else {
                steps--;
            }

            nextNode.visited = true;
            photonView.RPC("Visit", RpcTarget.AllBufferedViaServer, nextNode.Transform.position);
            currentNode = nextNode;
        }
        ShowQuestion();
        moving = false;
    }

    [PunRPC]
    public void Visit(Vector3 v)
    {
        gameRoute.GetNodeFromPosition(v).visited = true;
    }
    
    [PunRPC]
    void SubtractPoints() {
        Node n = currentNode;
        int distanceToGoal = 0;
        while(n.neighbors.Count > 0) {
            distanceToGoal++;
            n = n.neighbors[0];
        }
        PointManager pm = GameObject.FindGameObjectWithTag("PointManager").GetComponent<PointManager>();
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        props.Add("points", pm.GetPointsCount(PhotonNetwork.LocalPlayer) - distanceToGoal);
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().photonView.RPC("GenerateEndingPopup", RpcTarget.All);
    }
    void ShowQuestion()
    {
        Card c = null;
        int favoriteCardCount = 0;
        int almostReachedCount = 0;
        switch (currentNode.FieldType)
        {
            case 0:
                CardActions.gameEndAction();
                break;
            case 1:
                c = cards.quiz_knowledge[UnityEngine.Random.Range(0, cards.quiz_knowledge.Count)];
                c.processCard = () => CardActions.quizKnowledgeAction(c);

                if (c == cards.quiz_knowledge[5] && favoriteCardCount == 0)
                {
                    FindObjectOfType<DF2Client>().FavoriteCard();
					favoriteCardCount++;
                }
				if (c == cards.quiz_knowledge[8] && almostReachedCount == 0)
				{
					FindObjectOfType<DF2Client>().almostReachedEnd();
					almostReachedCount++;
				}

				break;
            case 2:
                c = cards.pattern_recognize[UnityEngine.Random.Range(0, cards.pattern_recognize.Count)];
                c.processCard = () => CardActions.patternRecognizeAction(c.question, c.answer, c.points, c.note);
                photonView.RPC("patternRecognizeAction", RpcTarget.Others, c.question, c.answer, c.points, c.note); // Everyone plays
				if (c == cards.pattern_recognize[5] && favoriteCardCount == 0)
				{
					FindObjectOfType<DF2Client>().FavoriteCard();
					favoriteCardCount++;
				}
				if (c == cards.pattern_recognize[8] && almostReachedCount == 0)
				{
					FindObjectOfType<DF2Client>().almostReachedEnd();
					almostReachedCount++;
				}

				break;
            case 3:
                c = cards.creativity[UnityEngine.Random.Range(0, cards.creativity.Count)];
                c.processCard = () => CardActions.creativityAction(c.question, c.points, c.note);
                photonView.RPC("creativityAction", RpcTarget.Others, c.question, c.points, c.note); // Everyone plays
				if (c == cards.creativity[5] && favoriteCardCount == 0)
				{
					FindObjectOfType<DF2Client>().FavoriteCard();
					favoriteCardCount++;
				}
				if (c == cards.creativity[8] && almostReachedCount == 0)
				{
					FindObjectOfType<DF2Client>().almostReachedEnd();
					almostReachedCount++;
				}
				break;
            case 4:
                c = cards.rocket[GameManager.lastRocket - 1];
                c.processCard = () => CardActions.rocketAction(c.note, c.extra);
                photonView.RPC("rocketAction", RpcTarget.Others, c.note, c.extra);

                for (int i = 0; i <= 8; i++)
                {
                    if (c == cards.rocket[i]) {
                        FindObjectOfType<DF2Client>().RocketField(i); 
                    }
                }


				break;
            case 5:
                c = cards.environment[UnityEngine.Random.Range(0, cards.environment.Count)];
                c.processCard = () => CardActions.environmentAction(c.question, c.answer, c.points, c.note);
				if (c == cards.environment[5] && favoriteCardCount == 0)
				{
					FindObjectOfType<DF2Client>().FavoriteCard();
					favoriteCardCount++;
				}
				if (c == cards.environment[8] && almostReachedCount == 0)
				{
					FindObjectOfType<DF2Client>().almostReachedEnd();
					almostReachedCount++;
				}
				break; 
        }
        if (c != null) try { c.processCard(); } catch (System.Exception e) {GameManager.nextTurn();}
    }
    
    bool MoveTo(Vector2 dest)
    {
        Vector3 dest3D = new Vector3(dest.x, dest.y);
        return dest3D != (transform.position = Vector2.MoveTowards(transform.position, dest, 800f * Time.smoothDeltaTime));
    }
}
