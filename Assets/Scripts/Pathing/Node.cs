using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Node 
{
    public int FieldType {get;set;}
    public Transform Transform;
    public List<Node> neighbors;
    public bool visited;


    public Node(Transform pos)
    {
        this.Transform = pos;
        this.neighbors = new List<Node>();
        this.visited = false;
    }

    public void addNeighbor(Node n)
    {
        this.neighbors.Add(n);
    }

    public override bool Equals(object obj)
    {
        Node n = (Node) obj;
        return this.Transform.position == n.Transform.position;
    }

    public override int GetHashCode()
    {
        return this.Transform.position.GetHashCode();
    }
}