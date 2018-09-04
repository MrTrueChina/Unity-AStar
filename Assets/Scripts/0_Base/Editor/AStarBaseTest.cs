using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class AStarBaseTest
{
    [Test]
    public void Creat_Normal()
    {
        int width = 5;
        int height = 10;

        AStarBase aStar = new AStarBase(width, height);

        Assert.AreEqual(aStar.matrix.GetLength(0), width);
        Assert.AreEqual(aStar.matrix.GetLength(1), height);
    }
    [Test]
    public void Creat_Zero()
    {
        int width = 0;
        int height = 0;

        AStarBase aStar = new AStarBase(width, height);

        Assert.AreEqual(aStar.matrix.GetLength(0), width);
        Assert.AreEqual(aStar.matrix.GetLength(1), height);
    }
    [Test]
    public void Creat_Negative()
    {
        int width = -10;
        int height = -5;

        AStarBase aStar = new AStarBase(width, height);

        Assert.AreEqual(aStar.matrix.GetLength(0), 0);
        Assert.AreEqual(aStar.matrix.GetLength(1), 0);
    }

    [Test]
    public void SetObstacle_Normal()
    {
        Vector2 obstaclePosition = new Vector2(5, 3);

        AStarBase aStar = new AStarBase(10, 10);

        aStar.SetObstacle(obstaclePosition);

        for(int x = 0;x < aStar.matrix.GetLength(0);x++)
            for(int y = 0;y < aStar.matrix.GetLength(1);y++)
                if(x != (int)obstaclePosition.x || y != (int)obstaclePosition.y)
                    Assert.IsTrue(aStar.matrix[x, y].canThrough);
                else
                    Assert.IsFalse(aStar.matrix[x, y].canThrough);
    }
    [Test]
    public void SetObstacle_OutOfMaterix()
    {
        AStarBase aStar = new AStarBase(10, 10);

        aStar.SetObstacle(new Vector2(-1, 3));
        aStar.SetObstacle(new Vector2(1, -3));
        aStar.SetObstacle(new Vector2(5, 10));
        aStar.SetObstacle(new Vector2(25, 3));

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.IsTrue(node.canThrough);
    }

    [Test]
    public void RemoveObstacle_Normal()
    {
        Vector2 obstaclePosition = new Vector2(5, 3);

        AStarBase aStar = new AStarBase(10, 10);

        aStar.SetObstacle(obstaclePosition);
        aStar.RemoveObstacle(obstaclePosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.IsTrue(node.canThrough);
    }
    [Test]
    public void RemoveObstacle_OutOfMaterix()
    {
        Vector2 obstaclePosition = new Vector2(5, 3);

        AStarBase aStar = new AStarBase(10, 10);

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
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);
        
        /*
         *  ↓   ←   ←   ←   ←   ←   E
         *  
         *  ↓   ←   ↑   ↑   ↑   ↑   0
         *  
         *  ↓   ←   0   0   0   0   0
         *  
         *  ↓   ←   0   0   0   0   0
         *  
         *  ↓   ←   0   0   0   0   0
         *  
         *  ↓   ←   0   0   0   0   0
         *  
         *  S   ←   0   0   0   0   0
         */
        AstarBaseNode[,] matrix = aStar.matrix;
        AstarBaseNode[,] nextNodes = new AstarBaseNode[7, 7]
           {
               { null,matrix[0,0],null,null,null,null,null},
               { matrix[0,0],matrix[0,1],null,null,null,null,null},
               { matrix[0,1],matrix[0,2],null,null,null,null,null},
               { matrix[0,2],matrix[0,3],null,null,null,null,null},
               { matrix[0,3],matrix[0,4],null,null,null,null,null},
               { matrix[0,4],matrix[0,5],matrix[2,6],matrix[3,6],matrix[4,6],matrix[5,6],null},
               { matrix[0,5],matrix[0,6],matrix[1,6],matrix[2,6],matrix[3,6],matrix[4,6],matrix[5,6]}
           };
        for (int x = 0; x < 7; x++)
            for (int y = 0; y < 7; y++)
                Assert.AreEqual(nextNodes[y, x], aStar.matrix[x, y].nextNode);      //二维数组赋值是按照横竖x的方式进行的，也就是说上面的矩阵写错了，要转置，这就是一个用 [y, x] 一个用 [x, y] 的原因
    }
    [Test]
    public void Pathfind_CanFind_Obstacle()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.SetObstacle(new Vector2(0, 1));
        aStar.SetObstacle(new Vector2(1, 1));
        aStar.SetObstacle(new Vector2(2, 1));
        aStar.SetObstacle(new Vector2(4, 1));
        aStar.SetObstacle(new Vector2(4, 2));
        aStar.SetObstacle(new Vector2(4, 3));
        aStar.SetObstacle(new Vector2(5, 3));
        aStar.SetObstacle(new Vector2(6, 3));
        aStar.SetObstacle(new Vector2(1, 4));
        aStar.SetObstacle(new Vector2(2, 4));
        aStar.SetObstacle(new Vector2(3, 4));
        aStar.SetObstacle(new Vector2(0, 6));
        aStar.SetObstacle(new Vector2(1, 6));

        aStar.FindPath(startPosition, endPosition);

        /*
         *  N   N   ↓   ←   ←   ←   E
         *  
         *  ↓   ←   ←   ←   ↑   ↑   0
         *  
         *  ↓   N   N   N   0   0   0
         *  
         *  →   →   →   ↓   N   N   N
         *  
         *  →   →   →   ↓   N   ↓   ←
         *  
         *  N   N   N   ↓   N   ↓   ←
         *  
         *  S   ←   ←   ←   ←   ←   ←
         */
        AstarBaseNode[,] matrix = aStar.matrix;
        AstarBaseNode[,] nextNodes = new AstarBaseNode[7, 7]
           {
               { null, matrix[0,0], matrix[1,0], matrix[2,0], matrix[3,0], matrix[4,0],matrix[5,0]},
               { null, null, null, matrix[3,0], null, matrix[5,0],matrix[5,1]},
               { matrix[1,2], matrix[2,2], matrix[3,2], matrix[3,1], null, matrix[5,1],matrix[5,2]},
               { matrix[1,3], matrix[2,3], matrix[3,3], matrix[3,2], null, null,null},
               { matrix[0,3], null, null, null, null, null,null},
               { matrix[0,4], matrix[0,5], matrix[1,5], matrix[2,5], matrix[4,6], matrix[5,6],null},
               { null, null, matrix[2,5], matrix[2,6], matrix[3,6], matrix[4,6],matrix[5,6]}
           };
        for (int x = 0; x < 7; x++)
            for (int y = 0; y < 7; y++)
                Assert.AreEqual(nextNodes[y, x], aStar.matrix[x, y].nextNode);      //横y竖x，何等反人类，而且竟然严格符合了数组下标顺序：前面是第一层，后面是第二层
    }
    [Test]
    public void Pathfind_CantFind()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.SetObstacle(new Vector2(0, 1));
        aStar.SetObstacle(new Vector2(1, 1));
        aStar.SetObstacle(new Vector2(2, 1));
        aStar.SetObstacle(new Vector2(4, 1));
        aStar.SetObstacle(new Vector2(4, 2));
        aStar.SetObstacle(new Vector2(4, 3));
        aStar.SetObstacle(new Vector2(5, 3));
        aStar.SetObstacle(new Vector2(6, 3));
        aStar.SetObstacle(new Vector2(1, 4));
        aStar.SetObstacle(new Vector2(2, 4));
        aStar.SetObstacle(new Vector2(3, 4));
        aStar.SetObstacle(new Vector2(2, 5));
        aStar.SetObstacle(new Vector2(0, 6));
        aStar.SetObstacle(new Vector2(1, 6));

        aStar.FindPath(startPosition, endPosition);

        /*
         *  N   N   0   0   0   0   E
         *  
         *  ↓   ←   N   0   0   0   0
         *  
         *  ↓   N   N   N   0   0   0
         *  
         *  →   →   →   ↓   N   N   N
         *  
         *  →   →   →   ↓   N   ↓   ←
         *  
         *  N   N   N   ↓   N   ↓   ←
         *  
         *  S   ←   ←   ←   ←   ←   ←
         */
        AstarBaseNode[,] matrix = aStar.matrix;
        AstarBaseNode[,] nextNodes = new AstarBaseNode[7, 7]
           {
               { null, matrix[0,0], matrix[1,0], matrix[2,0], matrix[3,0], matrix[4,0],matrix[5,0]},
               { null, null, null, matrix[3,0], null, matrix[5,0],matrix[5,1]},
               { matrix[1,2], matrix[2,2], matrix[3,2], matrix[3,1], null, matrix[5,1],matrix[5,2]},
               { matrix[1,3], matrix[2,3], matrix[3,3], matrix[3,2], null, null,null},
               { matrix[0,3], null, null, null, null, null,null},
               { matrix[0,4], matrix[0,5], null, null, null, null,null},
               { null, null, null, null, null, null,null}
           };
        for (int x = 0; x < 7; x++)
            for (int y = 0; y < 7; y++)
                Assert.AreEqual(nextNodes[y, x], aStar.matrix[x, y].nextNode);      //横y竖x，何等反人类，而且竟然严格符合了数组下标顺序：前面是第一层，后面是第二层
    }
    [Test]
    public void Pathfind_StartOutOfMatrix()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(-1, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_0()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(-1, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_1()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(7, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_2()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, -1);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartOutOfMatrix_3()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 7);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_0()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(-1, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_1()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(7, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_2()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, -1);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndOutOfMatrix_3()
    {
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 7);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartAndEndOutOfMatrix()
    {
        
        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 7);
        Vector2 endPosition = new Vector2(-1, 6);

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_StartIsObstacle()
    {

        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.SetObstacle(new Vector2(0, 0));

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
    [Test]
    public void Pathfind_EndIsObstacle()
    {

        AStarBase aStar = new AStarBase(7, 7);

        Vector2 startPosition = new Vector2(0, 0);
        Vector2 endPosition = new Vector2(6, 6);

        aStar.SetObstacle(new Vector2(6, 6));

        aStar.FindPath(startPosition, endPosition);

        foreach (AstarBaseNode node in aStar.matrix)
            Assert.AreEqual(null, node.nextNode);
    }
}
