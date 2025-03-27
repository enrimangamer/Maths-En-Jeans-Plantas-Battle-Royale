using NUnit.Framework;
using UnityEngine;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class leaderBoardController : MonoBehaviour
{
    public GameObject leaderBoardPrefab;
    public GameObject[] leaderBoards= new GameObject[0];
    public int leaderBoardCount;
    public Transform leaderBoardParent;
    public void resetLeaderBoard()
    {
        leaderBoards = new GameObject[leaderBoardCount];
        for (int i = 0; i < leaderBoardCount; i++)
        {
            leaderBoards[i] = Instantiate(leaderBoardPrefab);
            leaderBoards[i].transform.SetParent(leaderBoardParent);
        }
    }
    public void setLeaderBoardValeus()
    {
        List<Plant> orderedPlants = new List<Plant>();
        foreach (var plant in GameManager.instance.plants)
        {
            int i = 0;
            bool inserted = false
            foreach(var p2 in orderedPlants)
            {
                if(p2.pointsAvailable <  plant.pointsAvailable)
                {
                    orderedPlants.Insert(i, plant);
                    inserted = true;
                }
                i++;
            }
            if(!inserted)
                orderedPlants.Add(plant);
        }
        for (int i = 0; i < leaderBoardCount; i++)
        {
            leaderBoards[i].GetComponent<TMP_Text>().text = i.ToString()+") "+ leaderBoards[i].name + "P:" + leaderBoards[i];
        }
    }

  /*  public GameObject connectedPlant;
    public void onClickedZoom()
    {
        CameraManager.instance.startLerpToPlant(connectedPlant);
    }*/

}
