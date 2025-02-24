using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGeneration : MonoBehaviour
{
    [Range(1, 100)]
    public int percentageFilled;

    public void GenerateObstacle(List<Land> landPlots)
    {
        int plotsToFill = Mathf.RoundToInt((float)percentageFilled / 100 * landPlots.Count);

        List<int> shuffledList = ShuffleLandIndexs(landPlots.Count);

        for (int i = 0; i < plotsToFill; i++)
        {
            int index = shuffledList[i];
            Land.FarmObstacleStatus status = (Land.FarmObstacleStatus)Random.Range(1, 4);

            landPlots[index].SetObstacleStatus(status);
        }
    }

    List<int> ShuffleLandIndexs(int count)
    {
        List<int> listToReturn = new List<int>();

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, i + 1);
            listToReturn.Insert(index, i);
        }
        return listToReturn;
    }
}
