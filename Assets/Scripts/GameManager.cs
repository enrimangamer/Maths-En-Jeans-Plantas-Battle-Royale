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

public class Plant
{
    public string id { get; set; }
    public string password { get; set; }
    public int pointsAvailable { get; set; } = 5;
    public float[] oddsleft { get; set; } = { 1, 1, 1, 1, 1};
    public float[] oddsright { get; set; } = { 1, 1, 1, 1, 1};
    public float[] oddsattack { get; set; } = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f};
    
}

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
    public List<Plant> plants = new List<Plant>();
    public float timebetweenmoves = 300;
    private Plant plantmenuuser = new Plant();
    public GameObject plantMenu;
    public GameObject pointsvailableobject;
    public float oddschangedirection = 0.3f;
    public float oddschangeattack = 0.15f;
    public GameObject[] oddslefttext;
    public GameObject[] oddsrighttext;
    public GameObject[] oddsattacktext;
    public GameObject passwordinput;
    public GameObject giftusername;
    public GameObject giftquantity;
    public float oddsofwinningifbiggerprof = 0.04f;


    private int name = 1;
    Camera cam;


    List<GameObject> Evolve()
    {
        List<GameObject> newtiges = new List<GameObject>();
        for (int i = 0; i < plantstiges.Count; i++)
        {
            LineRenderer linerenderer = plantstiges[i].GetComponent<LineRenderer>();
            
            //Check userplant to see probs
            Plant userplant = new Plant();
            foreach (Plant plant in plants)
            {
                if (plant.id == plantstiges[i].transform.parent.name)
                {
                    userplant = plant;
                    break;
                }
            }
            
            float rand = Random.Range(0f, 3f);
            int finalrand;
            if (rand < userplant.oddsleft[0])
            {
                finalrand = 0;
            } else if (rand < userplant.oddsleft[0] + userplant.oddsright[0])
            {
                finalrand = 1;
            }
            else
            {
                finalrand = 2;
            }
            
            if (finalrand == 2)
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
            if (finalrand == 0 || finalrand == 2)
            {
                newposition += new Vector3(-1, -1, 0);
            }
            if (finalrand == 1)
            {
                newposition += new Vector3(1, -1, 0);
            }
            linerenderer.SetPosition(linerenderer.positionCount - 1, newposition);
        }

        return newtiges;
    }

    void UpdateOddsText()
    {
        for (int i = 0; i < 5; i++)
        {
            oddslefttext[i].GetComponent<TMP_Text>().text = Math.Round(plantmenuuser.oddsleft[i] * 33).ToString() + "%";
            oddsrighttext[i].GetComponent<TMP_Text>().text = Math.Round(plantmenuuser.oddsright[i] * 33).ToString() + "%";
            oddsattacktext[i].GetComponent<TMP_Text>().text = Math.Round(plantmenuuser.oddsattack[i] * 100).ToString() + "%";
            pointsvailableobject.GetComponent<TMP_Text>().text = plantmenuuser.pointsAvailable.ToString();
        }
    }

    public void ChangeOddsLeft(int movenumber)
    {
        if (plantmenuuser.pointsAvailable > 0 && movenumber <= 4 && plantmenuuser.oddsleft[movenumber] < 3)
        {
            plantmenuuser.pointsAvailable -= 1;
            plantmenuuser.oddsleft[movenumber] += oddschangedirection;
            if (plantmenuuser.oddsleft[movenumber] > 3)
            {
                plantmenuuser.oddsleft[movenumber] = 3;
            }
            plantmenuuser.oddsright[movenumber] -= oddschangedirection / 2;
            if (plantmenuuser.oddsright[movenumber] < 0)
            {
                plantmenuuser.oddsright[movenumber] = 0;
            }
            UpdateOddsText();
        }
    }
    public void ChangeOddsRight(int movenumber)
    {
        if (plantmenuuser.pointsAvailable > 0 && movenumber <= 4 && plantmenuuser.oddsright[movenumber] < 3)
        {
            plantmenuuser.pointsAvailable -= 1;
            plantmenuuser.oddsright[movenumber] += oddschangedirection;
            if (plantmenuuser.oddsright[movenumber] > 3)
            {
                plantmenuuser.oddsright[movenumber] = 3;
            }
            plantmenuuser.oddsleft[movenumber] -= oddschangedirection / 2;
            if (plantmenuuser.oddsleft[movenumber] < 0)
            {
                plantmenuuser.oddsleft[movenumber] = 0;
            }

            UpdateOddsText();
        }
    }
    public void ChangeOddsAttack(int movenumber)
    {
        if (plantmenuuser.pointsAvailable > 0 && movenumber <= 4 && plantmenuuser.oddsattack[movenumber] < 1)
        {
            plantmenuuser.pointsAvailable -= 1;
            plantmenuuser.oddsattack[movenumber] += oddschangeattack;
            if (plantmenuuser.oddsattack[movenumber] > 1)
            {
                plantmenuuser.oddsattack[movenumber] = 1;
            }

            UpdateOddsText();
        }
    }

    float GetProfPlant(Plant plant)
    {
        float minprof = 100;
        float tigeprof;
        foreach (GameObject tige in plantstiges)
        {
            if (tige.transform.parent.name == plant.id)
            {
                tigeprof = tige.transform.position.y + tige.GetComponent<LineRenderer>()
                    .GetPosition(tige.GetComponent<LineRenderer>().positionCount - 1).y;
                if (tigeprof < minprof ){
                    minprof = tigeprof;
                }
            }
        }

        return minprof;
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
                            //Check userplant to see probs
                            Plant plant1 = new Plant();
                            Plant plant2 = new Plant();
                            foreach (Plant plant in plants)
                            {
                                if (plant.id == p1.transform.parent.name)
                                {
                                    plant1 = plant;
                                }
                                if (plant.id == p2.transform.parent.name)
                                {
                                    plant2 = plant;
                                }
                            }
                            
                            float oddsplant1 = plant1.oddsattack[0];
                            float oddsplant2 = plant2.oddsattack[0];
                            float prof1 = GetProfPlant(plant1);
                            float prof2 = GetProfPlant(plant2);
                            if (prof1 < prof2)
                            {
                                oddsplant1 += (prof2 - prof1) * oddsofwinningifbiggerprof;
                            } else if (prof2 < prof1)
                            {
                                oddsplant2 += (prof1 - prof2) * oddsofwinningifbiggerprof;
                            }
                            
                            int finalrand = 0;
                            int maxwhile = 0;
                            while (finalrand == 0 && maxwhile < 10)
                            {
                                maxwhile += 1;
                                float rand1 = Random.Range(0f, 1f);
                                float rand2 = Random.Range(0f, 1f);
                                finalrand = 0;
                                if (rand1 > oddsplant1)
                                {
                                    finalrand -= 1;
                                } 
                                if (rand2 > oddsplant2)
                                {
                                    finalrand += 1;
                                }
                            }

                            if (finalrand == 0)
                            {
                                finalrand = (Random.Range(0, 2) * 2) - 1;
                            }
                            //if -1, plant1 won, if 1, plant 2 won, if 0, none won or both won
                            if (finalrand == 1)
                            {
                                p1.transform.parent.gameObject.SetActive(false);
                                Destroy(p1.transform.parent.gameObject);
                            }
                            else if (finalrand == -1)
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

    public void SendGift()
    {
        if (giftusername.GetComponent<TMP_InputField>().text.Length > 0 &&
            giftquantity.GetComponent<TMP_InputField>().text.Length > 0)
        {
            try
            {
                int test = int.Parse(giftquantity.GetComponent<TMP_InputField>().text);
            }
            catch (Exception e)
            {
                return;
            }

            String username = giftusername.GetComponent<TMP_InputField>().text;
            int quantity = int.Parse(giftquantity.GetComponent<TMP_InputField>().text);

            if (quantity > 0 && plantmenuuser.pointsAvailable >= quantity)
            {
                Plant planttogift = new Plant();
                bool userfound = false;
                foreach (Plant plant in plants)
                {
                    if (plant.id == username)
                    {
                        userfound = true;
                        planttogift = plant;
                        break;
                    }
                }
                if (!userfound)
                {
                    return;
                }

                planttogift.pointsAvailable += quantity;
                plantmenuuser.pointsAvailable -= quantity;
                UpdateOddsText();
            }
            
        }
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
            String input = newplantinput.GetComponent<TMP_InputField>().text;
            String password = passwordinput.GetComponent<TMP_InputField>().text;
            if (input.Length == 0 || password.Length == 0)
            {
                return;
            }
            bool idexists = false;
            Plant userplant = new Plant();
            foreach (Plant plant in plants)
            {
                if (plant.id == input)
                {
                    idexists = true;
                    userplant = plant;
                    break;
                }
            }
            if (idexists)
            {
                if (userplant.password != password)
                {
                    newplantinput.GetComponent<TMP_InputField>().text = "";
                    passwordinput.GetComponent<TMP_InputField>().text = "";
                    return;
                }
                //User can control what his plant does
                plantmenuuser = userplant;
                plantMenu.SetActive(true);
                pointsvailableobject.GetComponent<TMP_Text>().text = userplant.pointsAvailable.ToString();
                UpdateOddsText();
                
            } else {
                newplantinput.GetComponent<TMP_InputField>().text = "";
                passwordinput.GetComponent<TMP_InputField>().text = "";
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
                
                Plant newplant = new Plant();
                newplant.id = input;
                newplant.password = password;
                plants.Add(newplant);
            }
        }
        selectionCircle.transform.position =
            new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, ground.transform.position.y, 0);
        if (move > 0)
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

            foreach (Plant plant in plants)
            {
                plant.pointsAvailable += 1;
                //shift 1 left
                for (int i = 1; i < 5; i++)
                {
                    plant.oddsleft[i - 1] = plant.oddsleft[i];
                    plant.oddsright[i - 1] = plant.oddsright[i];
                    plant.oddsattack[i - 1] = plant.oddsattack[i];
                }

                plant.oddsleft[4] = 1;
                plant.oddsright[4] = 1;
                plant.oddsattack[4] = 0.5f;
                UpdateOddsText();
            }
        }

        if (timer + timebetweenmoves < Time.time)
        {
            move = 1;
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            plantMenu.SetActive(false);
        }
        
        //jump 1 night thing
        if(Input.anyKeyDown) {
            if(Input.GetKeyDown(KeyCode.L)) {
                if(lastkey == 0) {
                    lastkey = 1;
                } else {
                    lastkey = 1;
                }
            } else if(Input.GetKeyDown(KeyCode.F)) {
                if(lastkey == 1) {
                    lastkey = 2;
                } else {
                    lastkey = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.M)) {
                if(lastkey == 2) {
                    lastkey = 3;
                } else {
                    lastkey = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.E)) {
                if(lastkey == 3) {
                    lastkey = 4;
                } else {
                    lastkey = 0;
                }
            } else if(Input.GetKeyDown(KeyCode.S)) {
                if(lastkey == 4)
                {
                    timebetweenmoves -= 30;
                } else {
                    lastkey = 0;
                }
            } else {
                lastkey = 0;
            }
        }
        
    }
}