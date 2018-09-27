using UnityEngine;
using UnityEngine.UI;

public class AStarWithMinBinaryHeapDisplayObject : MonoBehaviour
{
    [SerializeField]
    GameObject _nodeDisplayerPrefab;

    [SerializeField]
    int _width;
    [SerializeField]
    int _height;


    GameObject[,] _displayMatrix;

    Vector2 _startPosition;
    Vector2 _endPosition;

    AStarWithMinBinary _aStar;

    private void Start()
    {
        CreatAStar();
        CreatDisplayMatrix();
        CalculateCamera();
    }
    void CreatAStar()
    {
        _aStar = new AStarWithMinBinary(_width, _height);
    }
    void CreatDisplayMatrix()
    {
        _displayMatrix = new GameObject[_width, _height];

        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
            {
                GameObject displayer = Instantiate(_nodeDisplayerPrefab);
                displayer.transform.position = new Vector3(x, y, 0);

                _displayMatrix[x, y] = displayer;
            }
    }
    void CalculateCamera()
    {
        Camera mainCamera = Camera.main;

        mainCamera.transform.position = new Vector3(_width / 2.0f, _height / 2.0f, -10);


        float requiedHeightWithHeight = _height / 2;
        float requiedHeightWithWidth = _width / 2 / mainCamera.aspect;

        mainCamera.orthographicSize = Mathf.Max(requiedHeightWithHeight, requiedHeightWithWidth) * 1.1f;  //添加10%的边框
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            MouseLeftDown();
        if (Input.GetMouseButtonDown(1))
            MoustRightDown();
        if (Input.GetMouseButtonDown(2))
            MouseMiddleDown();
    }

    void MouseLeftDown()
    {
        Vector2 position = GetMousePosition();

        _startPosition = position;

        DoFindPath();
    }

    void MoustRightDown()
    {
        Vector2 position = GetMousePosition();

        _endPosition = position;

        DoFindPath();
    }

    void MouseMiddleDown()
    {
        Vector2 position = GetMousePosition();

        AStarWithMinBinaryHeapNode node = _aStar.GetNode(position);

        if (node == null) return;

        if (node.canThrough)
            _aStar.SetObstacle(position);
        else
            _aStar.RemoveObstacle(position);

        DoFindPath();
    }

    Vector2 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void DoFindPath()
    {
        _aStar.FindPath(_startPosition, _endPosition);

        DisplayPathfinding();
    }

    void DisplayPathfinding()
    {
        DisplayCost();
        DisplayPath();
    }
    void DisplayCost()
    {
        float maxStartToCost = GetMaxStartToCost();
        float maxToEndCost = GetMaxToEndCost();

        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                DisplayANode(_aStar.matrix[x, y], _displayMatrix[x, y], maxStartToCost, maxToEndCost);
    }
    float GetMaxStartToCost()
    {
        float maxStartToCost = _aStar.GetNode(_startPosition).toEndCost;

        foreach (AStarWithMinBinaryHeapNode node in _aStar.matrix)
            if (node.startToCost > maxStartToCost)
                maxStartToCost = node.startToCost;

        return maxStartToCost;
    }
    float GetMaxToEndCost()
    {
        float maxToEndCost = _aStar.GetNode(_startPosition).toEndCost;

        foreach (AStarWithMinBinaryHeapNode node in _aStar.matrix)
            if (node.toEndCost > maxToEndCost)
                maxToEndCost = node.toEndCost;

        return maxToEndCost;
    }
    void DisplayANode(AStarWithMinBinaryHeapNode node, GameObject displayer, float maxStartToCost, float maxToEndCost)
    {
        displayer.GetComponentInChildren<Image>().color = GetNodeColor(node, maxStartToCost, maxToEndCost);
        displayer.GetComponentInChildren<Text>().text = GetDirectionText(node);
    }
    Color GetNodeColor(AStarWithMinBinaryHeapNode node, float maxStartToCost, float maxToEndCost)
    {
        if (!node.canThrough) return Color.black;

        Color nodeRed = Color.red * node.startToCost / maxStartToCost;
        Color nodeBlue = Color.blue * node.toEndCost / maxToEndCost;
        
        return nodeRed + nodeBlue + Color.gray * 0.5f;
    }
    string GetDirectionText(AStarWithMinBinaryHeapNode node)
    {
        if (node == null || node.nextNode == null) return "";

        Vector2 direction = node.nextNode.position - node.position;

        if (direction == Vector2.up) return "↑";
        if (direction == Vector2.right) return "→";
        if (direction == Vector2.down) return "↓";
        if (direction == Vector2.left) return "←";

        return "";
    }

    void DisplayPath()
    {
        AStarWithMinBinaryHeapNode currentNode = _aStar.GetNode(_endPosition);

        if (currentNode == null || currentNode.nextNode == null) return;

        while (currentNode.nextNode != null)
        {
            AddGreen(currentNode);

            currentNode = currentNode.nextNode;
        }

        AddGreen(currentNode);
    }

    void AddGreen(AStarWithMinBinaryHeapNode currentNode)
    {
        Vector2 position = currentNode.position;
        _displayMatrix[(int)position.x, (int)position.y].GetComponentInChildren<Image>().color += Color.green;
    }
}
