using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class AStarEightDirectionsTest

{
    [Test]
    public void Creat_Normal()
    {
        int width = 5;
        int height = 10;

        AStarEightDirections aStar = new AStarEightDirections(width, height);

        Assert.AreEqual(aStar.matrix.GetLength(0), width);
        Assert.AreEqual(aStar.matrix.GetLength(1), height);
    }
    [Test]
    public void Creat_Zero()
    {
        int width = 0;
        int height = 0;

        AStarEightDirections aStar = new AStarEightDirections(width, height);

        Assert.AreEqual(aStar.matrix.GetLength(0), width);
        Assert.AreEqual(aStar.matrix.GetLength(1), height);
    }
    [Test]
    public void Creat_Negative()
    {
        int width = -10;
        int height = -5;

        AStarEightDirections aStar = new AStarEightDirections(width, height);

        Assert.AreEqual(aStar.matrix.GetLength(0), 0);
        Assert.AreEqual(aStar.matrix.GetLength(1), 0);
    }

    [Test]
    public void SetObstacle_Normal()
    {
        Vector2 obstaclePosition = new Vector2(5, 3);

        AStarEightDirections aStar = new AStarEightDirections(10, 10);

        aStar.SetObstacle(obstaclePosition);

        for (int x = 0; x < aStar.matrix.GetLength(0); x++)
            for (int y = 0; y < aStar.matrix.GetLength(1); y++)
                if (x != (int)obstaclePosition.x || y != (int)obstaclePosition.y)
                    Assert.IsTrue(aStar.matrix[x, y].canThrough);
                else
                    Assert.IsFalse(aStar.matrix[x, y].canThrough);
    }
    [Test]
    public void SetObstacle_OutOfMaterix()
    {
        AStarEightDirections aStar = new AStarEightDirections(10, 10);

        aStar.SetObstacle(new Vector2(-1, 3));
        aStar.SetObstacle(new Vector2(1, -3));
        aStar.SetObstacle(new Vector2(5, 10));
        aStar.SetObstacle(new Vector2(25, 3));

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.IsTrue(node.canThrough);
    }

    [Test]
    public void RemoveObstacle_Normal()
    {
        Vector2 obstaclePosition = new Vector2(5, 3);

        AStarEightDirections aStar = new AStarEightDirections(10, 10);

        aStar.SetObstacle(obstaclePosition);
        aStar.RemoveObstacle(obstaclePosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.IsTrue(node.canThrough);
    }
    [Test]
    public void RemoveObstacle_OutOfMaterix()
    {
        Vector2 obstaclePosition = new Vector2(5, 3);

        AStarEightDirections aStar = new AStarEightDirections(10, 10);

        aStar.SetObstacle(obstaclePosition);

        aStar.RemoveObstacle(new Vector2(100, 5));
        aStar.RemoveObstacle(new Vector2(5, 100));
        aStar.RemoveObstacle(new Vector2(-10, 5));
        aStar.RemoveObstacle(new Vector2(5, -10));

        for (int x = 0; x < aStar.matrix.GetLength(0); x++)
            for (int y = 0; y < aStar.matrix.GetLength(1); y++)
                if (x != (int)obstaclePosition.x || y != (int)obstaclePosition.y)
                    Assert.IsTrue(aStar.matrix[x, y].canThrough);
                else
                    Assert.IsFalse(aStar.matrix[x, y].canThrough);
    }

    [Test]
    public void Pathfind_CanFind_NoObstacle()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        /*
         *  0   0   0   0   ↘   ↓   E
         *  
         *  0   0   0   ↘   ↓   ↙   ←
         *  
         *  0   0   ↘   ↓   ↙   ←   ↖
         *  
         *  0   ↘   ↓   ↙   ←   ↖   0
         *  
         *  ↘   ↓   ↙   ←   ↖   0   0
         *  
         *  ↓   ↙   ←   ↖   0   0   0
         *  
         *  S   ←   ↖   0   0   0   0
         */
        AStarEightDirectionsHeapNode[,] matrix = aStar.matrix;
        AStarEightDirectionsHeapNode[,] nextNodes = new AStarEightDirectionsHeapNode[7, 7]
           {
               { null,matrix[0,0],matrix[1,1],null,null,null,null},
               { matrix[0,0],matrix[0,0],matrix[1,1],matrix[2,2],null,null,null},
               { matrix[1,1],matrix[1,1],matrix[1,1],matrix[2,2],matrix[3,3],null,null},
               { null,matrix[2,2],matrix[2,2],matrix[2,2],matrix[3,3],matrix[4,4],null},
               { null,null,matrix[3,3],matrix[3,3],matrix[3,3],matrix[4,4],matrix[5,5]},
               { null,null,null,matrix[4,4],matrix[4,4],matrix[4,4],matrix[5,5]},
               { null,null,null,null,matrix[5,5],matrix[5,5],matrix[5,5]}
           };
        for (int x = 0; x < 7; x++)
            for (int y = 0; y < 7; y++)
                Assert.AreEqual(nextNodes[y, x], aStar.matrix[x, y].nextNode);      //二维数组赋值是按照横竖x的方式进行的，也就是说上面的矩阵写错了，要转置，这就是一个用 [y, x] 一个用 [x, y] 的原因
    }

    [Test]
    public void Pathfind_StartOutOfMatrix()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(-1, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_0()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(-1, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_1()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(7, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_2()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, -1);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_3()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 7);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_0()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(-1, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_1()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(7, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_2()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, -1);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_3()
    {
        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 7);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartAndEndOutOfMatrix()
    {

        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 7);
        Vector2 endPosition = new Vector2(-1, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartIsObstacle()
    {

        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.SetObstacle(new Vector2(0, 0));

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndIsObstacle()
    {

        AStarEightDirections aStar = new AStarEightDirections(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.SetObstacle(new Vector2(6, 6));

        aStar.FindPath(startPosition, endPosition);

        foreach (AStarEightDirectionsHeapNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
}

