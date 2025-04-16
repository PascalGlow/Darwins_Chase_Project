using UnityEngine;

public class PathSelect : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public GameRoute gameRoute;
    private void Start() {
        gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
        gameRoute = (GameRoute) FindObjectOfType(typeof(GameRoute));
    }
    private void OnMouseDown()
    {
        gameManager.currentPlayer.nextNode = gameRoute.GetNodeFromPosition(transform.parent.position);
        gameManager.currentPlayer.paused = false;
    }
    private void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
        this.transform.localScale *= 1.1f;
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.5f);
        this.transform.localScale /= 1.1f;
    }
}
