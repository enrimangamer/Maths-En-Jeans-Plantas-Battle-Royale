using NUnit.Framework;
using UnityEngine;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class leaderBoardController : MonoBehaviour
{
    public GameObject leaderBoardPrefab;
    public TMP_Text[] leaderBoards= new TMP_Text[0];
    public int leaderBoardCount;
    public Transform leaderBoardParent;
    private void Start()
    {
        resetLeaderBoard();
    }
    private void FixedUpdate()
    {
        setLeaderBoardValeus();
    }
    public void resetLeaderBoard()
    {
        leaderBoards = new TMP_Text[leaderBoardCount];
        for (int i = 0; i < leaderBoardCount; i++)
        {
            leaderBoards[i] = Instantiate(leaderBoardPrefab).GetComponentInChildren<TMP_Text>();
            leaderBoards[i].transform.parent.transform.SetParent(leaderBoardParent);
        }
    }
    float getValue(Plant plant)
    {
        return Mathf.Round((-GameManager.instance.GetProfPlant(plant)))+ 2 + (float)(plant.pointsAvailable * 0.2f);
    }

    public void setLeaderBoardValeus()
    {
        List<Plant> orderedPlants = new List<Plant>();
        foreach (var plant in GameManager.instance.plants)
        {
            int i = 0;
            bool inserted = false;
            int insertPos = 0;
            foreach(var p2 in orderedPlants)
            {
                if(getValue(p2) < getValue(plant))
                {
                    inserted = true;
                    insertPos = i;
                    break;
                }
                i++;
            }
            if(!inserted)
                orderedPlants.Add(plant);
            else
            {
                orderedPlants.Insert(insertPos, plant);
            }
        }
        for (int i = 0; i < leaderBoardCount; i++)
        {
            if (i > orderedPlants.Count - 1)
                leaderBoards[i].text = "";
            else
            leaderBoards[i].text = i.ToString()+") "+ orderedPlants[i].id + " P: " + getValue(orderedPlants[i]);
        }
    }

  /*  public GameObject connectedPlant;
    public void onClickedZoom()
    {
        CameraManager.instance.startLerpToPlant(connectedPlant);
    }*/

}
