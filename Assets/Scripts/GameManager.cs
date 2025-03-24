using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Object = System.Object;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> plantstiges = new List<GameObject>();
    private float timer;
    public GameObject selectionCircle;
    public GameObject ground;
    public GameObject tigePrefab;
    public GameObject parentPrefab;
    private int lastkey;
    private int move;
    Vector3 lastMousePosition;
    public GameObject newplantinput;
    public GameObject sliderhandle;


    private int name = 1;
    Camera cam;


    List<GameObject> Evolve()
    {
        List<GameObject> newtiges = new List<GameObject>();
        for (int i = 0; i < plantstiges.Count; i++)
        {
            LineRenderer linerenderer = plantstiges[i].GetComponent<LineRenderer>();
            int rand = Random.Range(0, 3);
            if (rand == 2)
            {
                GameObject newtige = Instantiate(tigePrefab, plantstiges[i].transform.position + linerenderer.GetPosition(linerenderer.positionCount - 1), Quaternion.identity);
                newtige.GetComponent<LineRenderer>().positionCount += 1;
                newtige.GetComponent<LineRenderer>().SetPosition(1,new Vector3(1,-1,0));
                newtige.transform.parent = plantstiges[i].transform.parent;
                
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(newtige.transform.parent.GetComponent<SpriteRenderer>().color, 0.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f)}
                );
                newtige.GetComponent<LineRenderer>().colorGradient = gradient;
                
                newtiges.Add(newtige);
            }
            linerenderer.positionCount += 1;
            Vector3 newposition =
                linerenderer.GetPosition(linerenderer.positionCount - 2);
            if (rand == 0 || rand == 2)
            {
                newposition += new Vector3(-1, -1, 0);
            }
            if (rand == 1)
            {
                newposition += new Vector3(1, -1, 0);
            }
            linerenderer.SetPosition(linerenderer.positionCount - 1, newposition);
        }

        return newtiges;
    }

    
    int CheckForCollisions()
    {

        foreach (GameObject p1 in plantstiges)
        {
            foreach (GameObject p2 in plantstiges)
            {
                if (p1 != p2)
                {
                    float difference2 =
                        (p1.transform.position + p1.GetComponent<LineRenderer>()
                            .GetPosition(p1.GetComponent<LineRenderer>().positionCount - 1)).x -
                        (p2.transform.position + p2.GetComponent<LineRenderer>()
                            .GetPosition(p2.GetComponent<LineRenderer>().positionCount - 1)).x;
                    if (difference2 == 0)
                    {
                        if (p1.GetComponent<LineRenderer>().positionCount >
                            p2.GetComponent<LineRenderer>().positionCount)
                        {
                            p1.tag = "PlantNoEvolveTige";
                            return 0;
                        }
                        p2.tag = "PlantNoEvolveTige";
                        return 0;
                    }
                    if (p1.transform.parent != p2.transform.parent)
                    {
                        float difference1 =
                            (p1.transform.position + p1.GetComponent<LineRenderer>()
                                .GetPosition(p1.GetComponent<LineRenderer>().positionCount - 2)).x -
                            (p2.transform.position + p2.GetComponent<LineRenderer>()
                                .GetPosition(p2.GetComponent<LineRenderer>().positionCount - 2)).x;
                        if (difference1 * difference2 < 0)
                        {
                            if (Random.Range(0, 2) == 1)
                            {
                                p1.transform.parent.gameObject.SetActive(false);
                                Destroy(p1.transform.parent.gameObject);
                            }
                            else
                            {
                                p2.transform.parent.gameObject.SetActive(false);
                                Destroy(p2.transform.parent.gameObject);
                            }
                            
                            return 0;
                        }
                    }
                }
            }
        }

        return 1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plantstiges = (GameObject.FindGameObjectsWithTag("PlantTige")).ToList();
        timer = Time.time;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (newplantinput.GetComponent<TMP_InputField>().text.Length == 4)
            {
                String input = newplantinput.GetComponent<TMP_InputField>().text;
                newplantinput.GetComponent<TMP_InputField>().text = "";
                GameObject newparent = Instantiate(parentPrefab, selectionCircle.transform.position, Quaternion.identity);
                newparent.name = input;
                newparent.GetComponent<SpriteRenderer>().color = sliderhandle.GetComponent<Image>().color;
                GameObject newtige = Instantiate(tigePrefab, selectionCircle.transform.position, Quaternion.identity);
                newtige.transform.parent = newparent.transform;
            
                float alpha = 1.0f;
                Gradient gradient = new Gradient();
                gradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(newtige.transform.parent.GetComponent<SpriteRenderer>().color, 0.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f)}
                );
                newtige.GetComponent<LineRenderer>().colorGradient = gradient;
            
                plantstiges.Add(newtige);
            }
        }
        selectionCircle.transform.position =
            new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, ground.transform.position.y, 0);
        if (timer + 2 < Time.time && move > 0)
        {
            move -= 1;
            List<GameObject> newtiges = Evolve();
            for (int tige = 0; tige < newtiges.Count; tige++)
            {
                plantstiges.Add(newtiges[tige]);
            }

            int collisionsverified = CheckForCollisions();
            int limit = 0;
            while (collisionsverified == 0 && limit < 50)
            {
                limit += 1;
                plantstiges = (GameObject.FindGameObjectsWithTag("PlantTige")).ToList();
                collisionsverified = CheckForCollisions();
            }
            timer = Time.time;
        }
        
        //to add
        if (Input.GetKeyDown(KeyCode.Z))
        {
            cam.orthographicSize += 1;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            cam.orthographicSize -= 1;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }
        
        //jump 1 night thing
        if(Input.anyKeyDown) {
            if(Input.GetKeyDown(KeyCode.E)) {
                if(lastkey == 0) {
                    lastkey = 1;
                } else {
                    lastkey = 1;
                }
            } else if(Input.GetKeyDown(KeyCode.N)) {
                if(lastkey == 1) {
                    lastkey = 2;
                } else {
                    lastkey = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.T)) {
                if(lastkey == 2) {
                    lastkey = 3;
                } else {
                    lastkey = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.I)) {
                if(lastkey == 3) {
                    lastkey = 4;
                } else {
                    lastkey = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.Q)) {
                if(lastkey == 4)
                {
                    move = 5;
                } else {
                    lastkey = 0;
                }
            } else {
                lastkey = 0;
            }
        }
        
    }
}