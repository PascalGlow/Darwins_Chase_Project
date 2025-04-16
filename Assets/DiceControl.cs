using UnityEngine;
using Photon.Pun;
using System.Collections;

public class DiceControl : MonoBehaviourPun
{
    Sprite[] diceSides;
    SpriteRenderer sprRend;
    UIManager UIManager;
    public GameManager gameManager;

    void Start()
    {
        gameManager = gameManager.GetComponent<GameManager>();
        UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        sprRend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("Art/Dice/");
        sprRend.sprite = diceSides[5];
    }

    void OnMouseDown()
    {
        StartCoroutine(RollDice());
    }

    IEnumerator RollDice()
    {
        Player playerScript = gameManager.currentPlayer;
        if (!playerScript.moving && playerScript.isTurn && playerScript.GetComponent<PhotonView>().IsMine)
        {
            int diceNumber = 1;

            for (int i = 0; i < 15; i++)
            {
                diceNumber = Random.Range(1, 7);
                sprRend.sprite = diceSides[diceNumber - 1];
                yield return new WaitForSeconds(.05f);
            }
            UIManager.ToggleTransparancy(gameObject, playerScript);
            playerScript.steps = diceNumber;
            StartCoroutine(playerScript.Move());
            base.photonView.RPC("ShowDiceFace", RpcTarget.All, diceNumber);
        }
    }

    [PunRPC]
    void ShowDiceFace(int face)
    {
        sprRend.sprite = diceSides[face - 1]; // Show dice
    }
}
