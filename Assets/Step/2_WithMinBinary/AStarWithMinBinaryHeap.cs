using UnityEngine;

/*
 *  将开表改为最小堆
 *  
 *  节省了在开表里寻找最低成本节点的时间
 */

public enum AStarWithMinBinaryNodeState
{
    unopened = 0,
    opened = 1,
    closed = 2
}

public class AStarWithMinBinaryHeapNode
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

    public AStarWithMinBinaryNodeState state
    {
        get { return _state; }
    }
    AStarWithMinBinaryNodeState _state = AStarWithMinBinaryNodeState.unopened;

    public float startToCost
    {
        get { return _startToCost; }
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

    public AStarWithMinBinaryHeapNode nextNode
    {
        get { return _nextNode; }
    }
    AStarWithMinBinaryHeapNode _nextNode;


    public AStarWithMinBinaryHeapNode(Vector2 position)
    {
        Doconstructed(position);
    }
    public AStarWithMinBinaryHeapNode(int x, int y)
    {
        Doconstructed(new Vector2(x, y));
    }
    void Doconstructed(Vector2 position)
    {
        _position = new Vector2Int((int)position.x, (int)position.y);
    }


    public void Open(Vector2 destination)
    {
        _state = AStarWithMinBinaryNodeState.opened;
        ComputeCost(destination);
    }
    public void Open(AStarWithMinBinaryHeapNode nextNode, Vector2 destination)
    {
        _state = AStarWithMinBinaryNodeState.opened;
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
        _state = AStarWithMinBinaryNodeState.closed;
    }


    public void Reset()     //寻路开始时清空成本和指向
    {
        _state = AStarWithMinBinaryNodeState.unopened;
        _nextNode = null;
        _startToCost = 0;
        _toEndCost = 0;
        _totalCost = 0;
    }
}

public class AStarWithMinBinaryMinBinaryHeap
{
    MinBinaryHeap<MinBinaryHeap<AStarWithMinBinaryHeapNode>> _heap = new MinBinaryHeap<MinBinaryHeap<AStarWithMinBinaryHeapNode>>();


    public bool isEmpty
    {
        get { return _heap == null || _heap.isEmpty || _heap.GetTopNodeObject().isEmpty; }
    }

    public void Set(AStarWithMinBinaryHeapNode node)
    {
        MinBinaryHeap<AStarWithMinBinaryHeapNode> subHeap = _heap.FindFirstThroughValue(node.totalCost);
        
        if (subHeap != null)
        {
            subHeap.SetNode(node, node.toEndCost);
        }
        else
        {
            subHeap = new MinBinaryHeap<AStarWithMinBinaryHeapNode>();
            subHeap.SetNode(node, node.toEndCost);

            _heap.SetNode(subHeap, node.totalCost);
        }
    }

    public AStarWithMinBinaryHeapNode GetMinimumCostNode()
    {
        if (isEmpty) return null;
        
        return _heap.GetTopNodeObject().GetTopNodeObject();
    }
    
    public void Remove(AStarWithMinBinaryHeapNode node)
    {
        var targetSubHeap = _heap.FindFirstThroughValue(node.totalCost);

        targetSubHeap.RemoveFirstThroughObj(node);

        if (targetSubHeap.Count == 0)
            _heap.RemoveFirstThroughObj(targetSubHeap);
    }
}

public class AStarWithMinBinary
{
    public AStarWithMinBinaryHeapNode[,] matrix
    {
        get { return _matrix; }
    }
    AStarWithMinBinaryHeapNode[,] _matrix;

    AStarWithMinBinaryMinBinaryHeap _openedList;

    Vector2 _startPosition;
    Vector2 _endPosition;


    public AStarWithMinBinary(int width, int height)
    {
        if (width < 0) width = 0;                   //防止传入负数
        if (height < 0) height = 0;                 //防止传入负数

        _matrix = new AStarWithMinBinaryHeapNode[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _matrix[x, y] = new AStarWithMinBinaryHeapNode(x, y);
    }


    public void FindPath(Vector2 startPosition, Vector2 endPosition)
    {
        if (!InMatrix(startPosition) || !InMatrix(endPosition) || !GetNode(startPosition).canThrough || !GetNode(endPosition).canThrough) return;

        InitializingPathfinding(startPosition, endPosition);

        DoFindPath();
    }

    private void InitializingPathfinding(Vector2 startPosition, Vector2 endPosition)
    {
        _startPosition = new Vector2((int)startPosition.x, (int)startPosition.y);       //这步极其重要，一定要把起点终点的坐标进行精确，否则算成本的时候会出现不可预料的小数进而造成误差
        _endPosition = new Vector2((int)endPosition.x, (int)endPosition.y);

        ResetMatrix();
        ResetOpenList();
    }
    void ResetMatrix()
    {
        foreach (AStarWithMinBinaryHeapNode node in _matrix)
            node.Reset();
    }
    void ResetOpenList()
    {
        _openedList = new AStarWithMinBinaryMinBinaryHeap();
    }

    private void DoFindPath()
    {
        OpenStartNode();

        while (!_openedList.isEmpty)
        {
            AStarWithMinBinaryHeapNode currentNode = _openedList.GetMinimumCostNode();

            if (IsDestination(currentNode)) break;

            OpenNode(currentNode, Vector2.up);
            OpenNode(currentNode, Vector2.right);
            OpenNode(currentNode, Vector2.down);
            OpenNode(currentNode, Vector2.left);

            CloseNode(currentNode);
        }
    }
    void OpenStartNode()
    {
        AStarWithMinBinaryHeapNode startNode = GetNode(_startPosition);

        startNode.Open(_endPosition);

        _openedList.Set(startNode);
    }
    bool IsDestination(AStarWithMinBinaryHeapNode currentNode)
    {
        return currentNode == GetNode(_endPosition);
    }
    void OpenNode(AStarWithMinBinaryHeapNode currentNode, Vector2 direction)
    {
        AStarWithMinBinaryHeapNode newNode = GetNode(currentNode, direction);

        if (newNode == null || !newNode.canThrough) return;                     //如果没获取到新节点或新节点不能通行则直接return
        switch (newNode.state)
        {
            case AStarWithMinBinaryNodeState.unopened:
                DoOpenNode(currentNode, newNode);
                break;

            case AStarWithMinBinaryNodeState.opened:
                if (currentNode.startToCost < newNode.nextNode.startToCost)     //如果新节点已经开了但从起点到指向的节点的成本比当前节点高
                    ReopenNode(currentNode, newNode);
                break;

            case AStarWithMinBinaryNodeState.closed:
                break;
        }
    }
    void DoOpenNode(AStarWithMinBinaryHeapNode currentNode, AStarWithMinBinaryHeapNode openNode)
    {
        openNode.Open(currentNode, _endPosition);

        _openedList.Set(openNode);
    }
    void ReopenNode(AStarWithMinBinaryHeapNode currentNode, AStarWithMinBinaryHeapNode openNode)
    {
        _openedList.Remove(openNode);

        DoOpenNode(currentNode, openNode);
    }
    void CloseNode(AStarWithMinBinaryHeapNode node)
    {
        _openedList.Remove(node);
        node.Close();
    }




    public AStarWithMinBinaryHeapNode GetNode(Vector2 position)
    {
        if (!InMatrix(position)) return null;

        return _matrix[(int)position.x, (int)position.y];
    }
    public AStarWithMinBinaryHeapNode GetNode(AStarWithMinBinaryHeapNode node, Vector2 direction)
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
