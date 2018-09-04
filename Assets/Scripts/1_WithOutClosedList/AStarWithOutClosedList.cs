using System.Collections.Generic;
using UnityEngine;

/*
 *  移除闭表，用enum表示状态
 *  
 *  节省了在闭表里查找节点的时间（查找一个元素在不在List里耗时接近于遍历）
 */

public enum AStarWithOutClosedListNodeState
{
    unopened = 0,
    opened = 1,
    closed = 2
}

public class AStarWithOutClosedListNode
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

    public AStarWithOutClosedListNodeState state
    {
        get { return _state; }
    }
    AStarWithOutClosedListNodeState _state = AStarWithOutClosedListNodeState.unopened;

    public float startToCost
    {
        get { return _startToCost; }
        set { _startToCost = value; }
    }
    float _startToCost;

    public float toEndCost
    {
        get { return _toEndCost; }
    }
    float _toEndCost;

    public float totalCost
    {
        get { return _totalCost; }
    }
    float _totalCost;

    public AStarWithOutClosedListNode nextNode
    {
        get { return _nextNode; }
        set { _nextNode = value; }
    }
    AStarWithOutClosedListNode _nextNode;


    public AStarWithOutClosedListNode(Vector2 position)
    {
        Doconstructed(position);
    }
    public AStarWithOutClosedListNode(int x, int y)
    {
        Doconstructed(new Vector2(x, y));
    }
    void Doconstructed(Vector2 position)
    {
        _position = new Vector2Int((int)position.x, (int)position.y);
    }


    public void Open(AStarWithOutClosedListNode nextNode, Vector2 destination)
    {
        _state = AStarWithOutClosedListNodeState.opened;
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


    public void Close()
    {
        _state = AStarWithOutClosedListNodeState.closed;
    }


    public void Reset()     //寻路开始时清空成本和指向
    {
        _state = AStarWithOutClosedListNodeState.unopened;
        _nextNode = null;
        _startToCost = 0;
        _toEndCost = 0;
        _totalCost = 0;
    }
}

public class AStarWithOutClosedList
{
    public AStarWithOutClosedListNode[,] matrix
    {
        get { return _matrix; }
    }
    AStarWithOutClosedListNode[,] _matrix;
    List<AStarWithOutClosedListNode> _openedList;

    Vector2 _startPosition;
    Vector2 _endPosition;


    public AStarWithOutClosedList(int width, int height)
    {
        if (width < 0) width = 0;                   //防止传入负数
        if (height < 0) height = 0;                 //防止传入负数

        _matrix = new AStarWithOutClosedListNode[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _matrix[x, y] = new AStarWithOutClosedListNode(x, y);
    }


    public void FindPath(Vector2 startPosition, Vector2 endPosition)
    {
        if (!InMatrix(startPosition) || !InMatrix(endPosition) || !GetNode(startPosition).canThrough || !GetNode(endPosition).canThrough) return;

        ResetMatrix(_matrix.GetLength(0), _matrix.GetLength(1));

        _openedList = new List<AStarWithOutClosedListNode>();

        _startPosition = new Vector2((int)startPosition.x, (int)startPosition.y);       //这步极其重要，一定要把起点终点的坐标进行精确，否则算成本的时候会出现不可预料的小数进而造成误差
        _endPosition = new Vector2((int)endPosition.x, (int)endPosition.y);

        _openedList.Add(GetNode(_startPosition));               //将起点加入开表

        while (_openedList.Count > 0)
        {
            AStarWithOutClosedListNode currentNode = GetMinimumCostNodeInOpenedList();

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
        foreach (AStarWithOutClosedListNode node in _matrix)
            node.Reset();
    }

    AStarWithOutClosedListNode GetMinimumCostNodeInOpenedList()
    {
        AStarWithOutClosedListNode minimumCostNode = _openedList[0];

        foreach (AStarWithOutClosedListNode currentNode in _openedList)
            if (currentNode.totalCost < minimumCostNode.totalCost)
                minimumCostNode = currentNode;
            else if (currentNode.totalCost == minimumCostNode.totalCost && currentNode.toEndCost < minimumCostNode.toEndCost)   //当前节点总成本和最小成本节点总成本相同，但距离终点更近的话
                minimumCostNode = currentNode;


        return minimumCostNode;
    }
    bool IsDestination(AStarWithOutClosedListNode currentNode)
    {
        return currentNode == GetNode(_endPosition);
    }

    void OpenNode(AStarWithOutClosedListNode currentNode, Vector2 direction)                 //有缺陷，假设一个格子已经开了但有更低价的格子可以指向，缺少对这时候的处理
    {
        AStarWithOutClosedListNode newNode = GetNode(currentNode, direction);

        if (newNode == null || !newNode.canThrough) return;                     //如果没获取到新节点或新节点不能通行则直接return

        switch (newNode.state)
        {
            case AStarWithOutClosedListNodeState.unopened:
                DoOpenNode(currentNode, newNode);
                break;

            case AStarWithOutClosedListNodeState.opened:
                if (currentNode.startToCost < newNode.nextNode.startToCost)     //如果新节点已经开了但从起点到指向的节点的成本比当前节点高
                    ReopenNode(currentNode, newNode);
                break;

            case AStarWithOutClosedListNodeState.closed:
                break;
        }
    }
    void DoOpenNode(AStarWithOutClosedListNode currentNode, AStarWithOutClosedListNode openNode)
    {
        openNode.Open(currentNode, _endPosition);

        _openedList.Add(openNode);
    }
    void ReopenNode(AStarWithOutClosedListNode currentNode, AStarWithOutClosedListNode openNode)
    {
        openNode.Open(currentNode, _endPosition);
    }
    void CloseNode(AStarWithOutClosedListNode node)
    {
        _openedList.Remove(node);
        node.Close();
    }




    public AStarWithOutClosedListNode GetNode(Vector2 position)
    {
        if (!InMatrix(position)) return null;

        return _matrix[(int)position.x, (int)position.y];
    }
    public AStarWithOutClosedListNode GetNode(AStarWithOutClosedListNode node, Vector2 direction)
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
        if (InMatrix(position))
            GetNode(position).canThrough = false;
    }
    public void RemoveObstacle(Vector2 posiiton)
    {
        if (InMatrix(posiiton))
            GetNode(posiiton).canThrough = true;
    }
}
