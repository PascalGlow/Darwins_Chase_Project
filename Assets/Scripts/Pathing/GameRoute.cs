using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
public class GameRoute : MonoBehaviour
{
    Transform[] fieldPositions;
    public List<Node> childNodeList = new List<Node>();
    string fieldCategories;

    public Node GetNodeFromPosition(Vector3 position)
    {
        foreach (Node n in childNodeList)
        {
            if (n.Transform.position == position) return n;
        }
        return null;
    }

    private void Start() {
        fieldCategories = Resources.Load<TextAsset>("GameBoard/GameBoard").text;
        FillNodes();
        AssignCategories();
        ConnectNodes();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        FillNodes();
        ConnectNodes();

        // Draw Lines between connected nodes
        foreach(Node n in childNodeList)
        {
            foreach(Node neighbor in n.neighbors)
            {
                Gizmos.DrawLine(n.Transform.position, neighbor.Transform.position);
            }
        }
    }

    void FillNodes()
    {
        childNodeList.Clear();
        fieldPositions = GetComponentsInChildren<Transform>();   
        for (int i = 0; i < fieldPositions.Length; i++)
        {
            if (fieldPositions[i] != this.transform)
            {
                childNodeList.Add(new Node(fieldPositions[i]));
            }
        }
    }

    void AssignCategories()
    {
        //Feld:Typ (0 = Start/Ziel, 1 = BLAU, 2 = ORANGE, 3 = ROT, 4 = RAKETE, 5 = GRAU)
        string[] fieldCategoriesLines = Regex.Split(fieldCategories, "\n|\r|\r\n");
        foreach (string line in fieldCategoriesLines)
        {
            if (line.Length > 0)
            {
                string[] values = line.Split(':');
                int index =  int.Parse(values[0]);
                int type = int.Parse(values[1]);
                childNodeList[index].FieldType = type;
            }
        }
    }

    void ConnectNodes()
    {
        for (int i = 0; i <= 19; i++)
        {
            Vector2 currentNode = childNodeList[i].Transform.position;
            if(i > 0)
            {
                Vector2 prevNode = childNodeList[i - 1].Transform.position;
                childNodeList[i - 1].addNeighbor(childNodeList[i]);
            }
        }

        childNodeList[19].addNeighbor(childNodeList[20]);
        childNodeList[19].addNeighbor(childNodeList[23]);
        childNodeList[20].addNeighbor(childNodeList[21]);
        childNodeList[21].addNeighbor(childNodeList[22]);
        childNodeList[23].addNeighbor(childNodeList[24]);
        childNodeList[24].addNeighbor(childNodeList[25]);
        childNodeList[25].addNeighbor(childNodeList[26]);
        childNodeList[26].addNeighbor(childNodeList[22]);
        childNodeList[22].addNeighbor(childNodeList[27]);
        childNodeList[27].addNeighbor(childNodeList[28]);
        childNodeList[28].addNeighbor(childNodeList[29]);
        childNodeList[29].addNeighbor(childNodeList[30]);
        childNodeList[30].addNeighbor(childNodeList[31]);
        childNodeList[31].addNeighbor(childNodeList[32]);
        childNodeList[32].addNeighbor(childNodeList[33]);
        childNodeList[30].addNeighbor(childNodeList[34]);
        childNodeList[34].addNeighbor(childNodeList[35]);
        childNodeList[35].addNeighbor(childNodeList[36]);
        childNodeList[36].addNeighbor(childNodeList[37]);
        childNodeList[37].addNeighbor(childNodeList[33]);
        childNodeList[33].addNeighbor(childNodeList[38]);

        for (int i = 39; i < childNodeList.Count; i++)
        {
            Vector2 currentNode = childNodeList[i].Transform.position;
            if(i > 0)
            {
                Vector2 prevNode = childNodeList[i - 1].Transform.position;
                childNodeList[i-1].addNeighbor(childNodeList[i]);
            }
        }
    }
}
