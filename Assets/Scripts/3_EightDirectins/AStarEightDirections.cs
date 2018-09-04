using UnityEngine;


/*
 *  增加斜向寻路功能
 */


public enum AStarEightDirectionsNodeState
{
    unopened = 0,
    opened = 1,
    closed = 2
}

public class AStarEightDirectionsHeapNode
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

    public AStarEightDirectionsNodeState state
    {
        get { return _state; }
    }
    AStarEightDirectionsNodeState _state = AStarEightDirectionsNodeState.unopened;

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

    public AStarEightDirectionsHeapNode nextNode
    {
        get { return _nextNode; }
    }
    AStarEightDirectionsHeapNode _nextNode;


    public AStarEightDirectionsHeapNode(Vector2 position)
    {
        Doconstructed(position);
    }
    public AStarEightDirectionsHeapNode(int x, int y)
    {
        Doconstructed(new Vector2(x, y));
    }
    void Doconstructed(Vector2 position)
    {
        _position = new Vector2Int((int)position.x, (int)position.y);
    }


    public void Open(Vector2 destination)
    {
        _state = AStarEightDirectionsNodeState.opened;
        ComputeCost(destination);
    }
    public void Open(AStarEightDirectionsHeapNode nextNode, Vector2 destination)
    {
        _state = AStarEightDirectionsNodeState.opened;
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
        if (_nextNode == null)
            _startToCost = 0;
        else
        {
            if (position.x != _nextNode.position.x && position.y != _nextNode.position.y)
                _startToCost = _nextNode.startToCost + 1.414f;
            else
                _startToCost = _nextNode.startToCost + 1;
        }
    }
    void ComputeTotalCost()
    {
        _totalCost = _startToCost + _toEndCost;
    }


    public void Close()
    {
        _state = AStarEightDirectionsNodeState.closed;
    }


    public void Reset()     //寻路开始时清空成本和指向
    {
        _state = AStarEightDirectionsNodeState.unopened;
        _nextNode = null;
        _startToCost = 0;
        _toEndCost = 0;
        _totalCost = 0;
    }
}

public class AStarEightDirectionsMinBinaryHeap
{
    MinBinaryHeap<MinBinaryHeap<AStarEightDirectionsHeapNode>> _heap = new MinBinaryHeap<MinBinaryHeap<AStarEightDirectionsHeapNode>>();


    public bool isEmpty
    {
        get { return _heap == null || _heap.isEmpty || _heap.GetTopNodeObject().isEmpty; }
    }

    public void Set(AStarEightDirectionsHeapNode node)
    {
        MinBinaryHeap<AStarEightDirectionsHeapNode> subHeap = _heap.FindFirstThroughValue(node.totalCost);
        
        if (subHeap != null)
        {
            subHeap.SetNode(node, node.toEndCost);
        }
        else
        {
            subHeap = new MinBinaryHeap<AStarEightDirectionsHeapNode>();
            subHeap.SetNode(node, node.toEndCost);

            _heap.SetNode(subHeap, node.totalCost);
        }
    }

    public AStarEightDirectionsHeapNode GetMinimumCostNode()
    {
        if (isEmpty) return null;
        
        return _heap.GetTopNodeObject().GetTopNodeObject();
    }
    
    public void Remove(AStarEightDirectionsHeapNode node)
    {
        var targetSubHeap = _heap.FindFirstThroughValue(node.totalCost);

        targetSubHeap.RemoveFirstThroughObj(node);

        if (targetSubHeap.Count == 0)
            _heap.RemoveFirstThroughObj(targetSubHeap);
    }
}

public class AStarEightDirections
{
    public AStarEightDirectionsHeapNode[,] matrix
    {
        get { return _matrix; }
    }
    AStarEightDirectionsHeapNode[,] _matrix;

    AStarEightDirectionsMinBinaryHeap _openedList;

    Vector2 _startPosition;
    Vector2 _endPosition;


    public AStarEightDirections(int width, int height)
    {
        if (width < 0) width = 0;                   //防止传入负数
        if (height < 0) height = 0;                 //防止传入负数

        _matrix = new AStarEightDirectionsHeapNode[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _matrix[x, y] = new AStarEightDirectionsHeapNode(x, y);
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
        foreach (AStarEightDirectionsHeapNode node in _matrix)
            node.Reset();
    }
    void ResetOpenList()
    {
        _openedList = new AStarEightDirectionsMinBinaryHeap();
    }

    private void DoFindPath()
    {
        OpenStartNode();

        while (!_openedList.isEmpty)
        {
            AStarEightDirectionsHeapNode currentNode = _openedList.GetMinimumCostNode();

            if (IsDestination(currentNode)) break;

            OpenNode(currentNode, Vector2.up);
            OpenNode(currentNode, new Vector2(1, 1));
            OpenNode(currentNode, Vector2.right);
            OpenNode(currentNode, new Vector2(1, -1));
            OpenNode(currentNode, Vector2.down);
            OpenNode(currentNode, new Vector2(-1, -1));
            OpenNode(currentNode, Vector2.left);
            OpenNode(currentNode, new Vector2(-1, 1));

            CloseNode(currentNode);
        }
    }
    void OpenStartNode()
    {
        AStarEightDirectionsHeapNode startNode = GetNode(_startPosition);

        startNode.Open(_endPosition);

        _openedList.Set(startNode);
    }
    bool IsDestination(AStarEightDirectionsHeapNode currentNode)
    {
        return currentNode == GetNode(_endPosition);
    }
    void OpenNode(AStarEightDirectionsHeapNode currentNode, Vector2 direction)
    {
        AStarEightDirectionsHeapNode newNode = GetNode(currentNode, direction);

        if (newNode == null || !newNode.canThrough) return;                     //如果没获取到新节点或新节点不能通行则直接return
        switch (newNode.state)
        {
            case AStarEightDirectionsNodeState.unopened:
                DoOpenNode(currentNode, newNode);
                break;

            case AStarEightDirectionsNodeState.opened:
                if (currentNode.startToCost < newNode.nextNode.startToCost)     //如果新节点已经开了但从起点到指向的节点的成本比当前节点高
                    ReopenNode(currentNode, newNode);
                break;

            case AStarEightDirectionsNodeState.closed:
                break;
        }
    }
    void DoOpenNode(AStarEightDirectionsHeapNode currentNode, AStarEightDirectionsHeapNode openNode)
    {
        openNode.Open(currentNode, _endPosition);

        _openedList.Set(openNode);
    }
    void ReopenNode(AStarEightDirectionsHeapNode currentNode, AStarEightDirectionsHeapNode openNode)
    {
        _openedList.Remove(openNode);

        DoOpenNode(currentNode, openNode);
    }
    void CloseNode(AStarEightDirectionsHeapNode node)
    {
        _openedList.Remove(node);
        node.Close();
    }




    public AStarEightDirectionsHeapNode GetNode(Vector2 position)
    {
        if (!InMatrix(position)) return null;

        return _matrix[(int)position.x, (int)position.y];
    }
    public AStarEightDirectionsHeapNode GetNode(AStarEightDirectionsHeapNode node, Vector2 direction)
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
