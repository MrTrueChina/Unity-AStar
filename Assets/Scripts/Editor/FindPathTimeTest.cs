using NUnit.Framework;
using UnityEngine;


/// <summary>
/// 测试有开闭表、移除闭表、移除闭表并用最小堆做开表，三种A*的寻路时间
/// </summary>
[TestFixture]
public class FindPathTimeTest
{
    struct MapData {
        public int width;
        public int height;
        public Vector2 startPosition;
        public Vector2 endPosition;
    };
    
    MapData _map = new MapData
    {
        width = 500,
        height = 500,
        startPosition = new Vector2(0, 0),
        endPosition = new Vector2(499, 499),
    };


    const int cycleTimes = 10;


    [Test]
    public void AllAStarFindPathTime()
    {
        Debug.Log("AStarBase：" + GetAStarBaseFindpathTime());
        Debug.Log("AStarWithOutClosedList:" + GetAStarWithOutClosedListFindpathTime());
        Debug.Log("AStarWithMinBinary：" + GetAStarWithMinBinaryHeapFindpathTime());
    }


    [Test]
    public void AStarDemoFindpathTime()
    {
        float averageTime = GetAStarBaseFindpathTime();

        Debug.Log("AStar Base：" + averageTime);
    }
    float GetAStarBaseFindpathTime()
    {
        AStarBase aStar = new AStarBase(_map.width, _map.height);

        float averageTime = GetFunctionMillisecondTime(aStar.FindPath, _map.startPosition, _map.endPosition);

        return averageTime;
    }


    [Test]
    public void AStarWithOutClosedListFindpathTime()
    {
        float averageTime = GetAStarWithOutClosedListFindpathTime();

        Debug.Log("AStar With Out Closed List：" + averageTime);
    }
    float GetAStarWithOutClosedListFindpathTime()
    {
        AStarWithOutClosedList aStar = new AStarWithOutClosedList(_map.width, _map.height);

        float averageTime = GetFunctionMillisecondTime(aStar.FindPath, _map.startPosition, _map.endPosition);

        return averageTime;
    }

    [Test]
    public void AStarWithMinBinaryHeapFindpathTime()
    {
        float averageTime = GetAStarWithMinBinaryHeapFindpathTime();

        Debug.Log("AStar With Min Binary Heap：" + averageTime);
    }
    float GetAStarWithMinBinaryHeapFindpathTime()
    {
        AStarWithMinBinary aStar = new AStarWithMinBinary(_map.width, _map.height);

        float averageTime = GetFunctionMillisecondTime(aStar.FindPath, _map.startPosition, _map.endPosition);

        return averageTime;
    }



    delegate void FindPathFunction(Vector2 startPosition, Vector2 endPosition);
    float GetFunctionMillisecondTime(FindPathFunction function, Vector2 startPosition, Vector2 endPosition)
    {
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        long averageTime = 0;

        for (int i = 0; i < cycleTimes; i++)
        {
            timer.Reset();
            timer.Start();

            function(startPosition, endPosition);

            timer.Stop();
            averageTime += timer.ElapsedMilliseconds;
        }

        return (float)averageTime / cycleTimes;
    }
}
