using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool _walkable;
    public Vector3 _worldPosition;
    public int GridX;
    public int GridY;
    public int MovementPanelty;

    public int Gcost;
    public int Hcost;
    public Node Parent;
    int _heapIndex;

    public int Fcost
    {
        get
        {
            return Gcost + Hcost;
        }
    }

    public  Node(bool walkable, Vector3 worldPos,int gridX, int gridY, int penalty)
    {
        _walkable = walkable;
        _worldPosition = worldPos;
        GridX = gridX;
        GridY = gridY;
        MovementPanelty = penalty;
    }

    public int HeapIndex
    {
        get { return _heapIndex;}
        set
        {
            _heapIndex = value;
        } 
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = Fcost.CompareTo(nodeToCompare.Fcost);
        if (compare == 0)
        {
            compare = Hcost.CompareTo(nodeToCompare.Hcost);
        }
        return -compare;
    }
}
