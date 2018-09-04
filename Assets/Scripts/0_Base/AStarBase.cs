using System.Collections.Generic;
using UnityEngine;

/*
 *  只能上下左右进行寻路
 */
public class AstarBaseNode
{
    public Vector2 position
    {
        get { return _position; }
    }
    Vector2Int _position;

    public bool canThrough
    {
        get { return _canThrough; }
        set { _canThrough = value; }
    }
    bool _canThrough = true;

    public float startToCost
    {
        get { return _startToCost; }
        set { _startToCost = value; }
    }
    float _startToCost;

    public float toEndCost
    {
        get{ return _toEndCost; }
    }
    float _toEndCost;

    public float totalCost
    {
        get { return _totalCost; }
    }
    float _totalCost;

    public AstarBaseNode nextNode
    {
        get { return _nextNode; }
        set { _nextNode = value; }
    }
    AstarBaseNode _nextNode;


    public AstarBaseNode(Vector2 position)
    {
        Doconstructed(position);
    }
    public AstarBaseNode(int x, int y)
    {
        Doconstructed(new Vector2(x, y));
    }
    void Doconstructed(Vector2 position)
    {
        _position = new Vector2Int((int)position.x, (int)position.y);
    }


    public void Open(AstarBaseNode nextNode, Vector2 destination)
    {
        _nextNode = nextNode;
        ComputeCost(destination);
    }
    void ComputeCost(Vector2 destination)
    {
        ComputeToEndCost(destination);
        ComputestartToCost();
        ComputeTotalCost();
    }
    void ComputeToEndCost(Vector2 destination)
    {
        _toEndCost = Mathf.Abs(destination.x - _position.x) + Mathf.Abs(destination.y - _position.y);   //假设xy轴的一单位距离cost是1
    }
    void ComputestartToCost()
    {
        if (nextNode == null)
            _startToCost = 0;
        else
            _startToCost = _nextNode.startToCost + Mathf.Abs(_position.x - _nextNode.position.x) + Mathf.Abs(_position.y - _nextNode.position.y);
    }
    void ComputeTotalCost()
    {
        _totalCost = _startToCost + _toEndCost;
    }


    public void Reset()     //寻路开始时清空成本和指向
    {
        _nextNode = null;
        _startToCost = 0;
        _toEndCost = 0;
        _totalCost = 0;
    }
}

public class AStarBase
{
    public AstarBaseNode[,] matrix
    {
        get { return _matrix; }
    }
    AstarBaseNode[,] _matrix;
    List<AstarBaseNode> _openedList;
    List<AstarBaseNode> _closedList;

    Vector2 _startPosition;
    Vector2 _endPosition;


    public AStarBase(int width, int height)
    {
        if (width < 0) width = 0;                   //防止传入负数
        if (height < 0) height = 0;                 //防止传入负数

        _matrix = new AstarBaseNode[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _matrix[x, y] = new AstarBaseNode(x, y);
    }


    public void FindPath(Vector2 startPosition, Vector2 endPosition)
    {
        if (!InMatrix(startPosition) || !InMatrix(endPosition) || !GetNode(startPosition).canThrough || !GetNode(endPosition).canThrough) return;

        ResetMatrix(_matrix.GetLength(0), _matrix.GetLength(1));

        _openedList = new List<AstarBaseNode>();
        _closedList = new List<AstarBaseNode>();

        _startPosition = new Vector2((int)startPosition.x, (int)startPosition.y);       //这步极其重要，一定要把起点终点的坐标进行精确，否则算成本的时候会出现不可预料的小数进而造成误差
        _endPosition = new Vector2((int)endPosition.x,(int)endPosition.y);

        _openedList.Add(GetNode(_startPosition));               //将起点加入开表

        while (_openedList.Count > 0)
        {
            AstarBaseNode currentNode = GetMinimumCostNodeInOpenedList();

            if (IsDestination(currentNode)) break;

            OpenNode(currentNode, Vector2.up);
            OpenNode(currentNode, Vector2.right);
            OpenNode(currentNode, Vector2.down);
            OpenNode(currentNode, Vector2.left);
            
            CloseNode(currentNode);
        }
    }
    void ResetMatrix(int width, int height)
    {
        foreach (AstarBaseNode node in _matrix)
            node.Reset();
    }

    AstarBaseNode GetMinimumCostNodeInOpenedList()
    {
        AstarBaseNode minimumCostNode = _openedList[0];

        foreach (AstarBaseNode currentNode in _openedList)
            if (currentNode.totalCost < minimumCostNode.totalCost)
                minimumCostNode = currentNode;
            else if (currentNode.totalCost == minimumCostNode.totalCost && currentNode.toEndCost < minimumCostNode.toEndCost)   //当前节点总成本和最小成本节点总成本相同，但距离终点更近的话
                minimumCostNode = currentNode;


        return minimumCostNode;
    }
    bool IsDestination(AstarBaseNode currentNode)
    {
        return currentNode == GetNode(_endPosition);
    }

    void OpenNode(AstarBaseNode currentNode, Vector2 direction)                 //有缺陷，假设一个格子已经开了但有更低价的格子可以指向，缺少对这时候的处理，这个处理在后面几个里有
    {
        AstarBaseNode newNode = GetNode(currentNode, direction);

        if (newNode == null || !newNode.canThrough) return;                     //如果没获取到新节点或新节点不能通行则直接return

        if (!_openedList.Contains(newNode) && !_closedList.Contains(newNode))   //不在开表里也不在闭表里，说明是未开启状态
            DoOpenNode(currentNode, newNode);
    }
    void DoOpenNode(AstarBaseNode currentNode, AstarBaseNode openNode)
    {
        openNode.Open(currentNode, _endPosition);

        _openedList.Add(openNode);
    }

    void CloseNode(AstarBaseNode node)
    {
        _openedList.Remove(node);
        _closedList.Add(node);
    }




    public AstarBaseNode GetNode(Vector2 position)
    {
        if (!InMatrix(position)) return null;

        return _matrix[(int)position.x, (int)position.y];
    }
    public AstarBaseNode GetNode(AstarBaseNode node, Vector2 direction)
    {
        return GetNode(node.position + direction);
    }
    
    bool InMatrix(Vector2 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;

        return (x >= 0 && x < _matrix.GetLength(0) && y >= 0 && y < _matrix.GetLength(1));
    }



    public void SetObstacle(Vector2 position)
    {
        if(InMatrix(position))
            GetNode(position).canThrough = false;
    }
    public void RemoveObstacle(Vector2 posiiton)
    {
        if (InMatrix(posiiton))
            GetNode(posiiton).canThrough = true;
    }
}
