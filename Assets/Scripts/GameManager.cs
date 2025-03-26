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
    public string id;
    public string password;
    public int pointsAvailable = 5;
    public float[] oddsleft = { 1, 1, 1, 1, 1 };
    public float[] oddsright = { 1, 1, 1, 1, 1 };
    public float[] oddsattack = { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };

    public Color color;

    public List<GameObject> allTiges = new List<GameObject>();
    public GameObject gameObject;
    public bool isAlive = true;
    public void destroyPlant()
    {
        isAlive = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GameManager.instance.deadPlant;
        gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(GameManager.instance.deadPlantColor, 0.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 1.0f) }
        );
        foreach (var t in allTiges)
        {
            t.GetComponent<LineRenderer>().colorGradient = gradient;
            t.GetComponent<Collider2D>().enabled = false;
        }
    }
}

public class GameManager : MonoBehaviour
{
    // public List<GameObject> plantstiges = new List<GameObject>();
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
    public static GameManager instance;
    public Sprite deadPlant;
    public Color deadPlantColor;
    private int name = 1;
    Camera cam;
    public float timebetweenmoves = 300;
    private Plant plantmenuuser = new Plant();
    public GameObject plantMenu;
    public GameObject pointsvailableobject;
    public float oddschangedirection = 0.3f;
    public float oddschangeattack = 0.15f;
    public TMP_Text[] oddslefttext;
    public TMP_Text[] oddsrighttext;
    public TMP_Text[] oddsattacktext;
    public GameObject passwordinput;
    public GameObject giftusername;
    public GameObject giftquantity;
    public float oddsofwinningifbiggerprof = 0.04f;

    Plant getPlantById(string id)
    {
        foreach (Plant plant in plants)
        {
            if(plant.id == id) return plant;
        }
        return null;
    }

    public void plantsCollision(string id1, string id2)
    {
        Plant plant1 = getPlantById(id1);
        Plant plant2 = getPlantById(id2);
        if (!plant1.isAlive || !plant2.isAlive) return;
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
        
        Plant toDestroy = null;
        float totalodds = oddsplant1 + oddsplant2;
        float random = Random.Range(0, totalodds);
        if (random < oddsplant1)
        {
            toDestroy = plant2;
        }
        else
        {
            toDestroy = plant1;
        }
        toDestroy.destroyPlant();
        plants.Remove(toDestroy);
    }

    void CheckDoubleTige()
    {
        GameObject tige1;
        GameObject tige2;
        LineRenderer tige1line;
        LineRenderer tige2line;
        foreach (Plant plant in plants)
        {
            int count = plant.allTiges.Count;
            for (int i = 0; i < count; i++)
            {
                tige1 = plant.allTiges[i];
                tige1line = tige1.GetComponent<LineRenderer>();
                for (int y = 0; y < count; y++)
                {
                    if (i != y)
                    {
                        tige2 = plant.allTiges[y];
                        tige2line = tige2.GetComponent<LineRenderer>();
                        float difference = (tige1.transform.position.x + tige1line.GetPosition(tige1line.positionCount - 1).x) - (tige2.transform.position.x + tige2line.GetPosition(tige2line.positionCount - 1).x);
                        difference = Mathf.Round(difference * 10) / 10;
                        if (difference == 0f)
                        {
                            plant.allTiges.RemoveAt(y);
                            y -= 1;
                            count -= 1;
                        }
                    }
                }
            }
        }
    }

    private void Evolve()
    {
        foreach (Plant plant in plants)
        {
            List<GameObject> newtiges = new List<GameObject>();
            for (int i = 0; i < plant.allTiges.Count; i++)
            {
                LineRenderer lineRenderer = plant.allTiges[i].GetComponent<LineRenderer>();

                float rand = Random.Range(0f, 3f);
                int finalrand;
                if (rand < plant.oddsleft[0])
                {
                    finalrand = 0;
                }
                else if (rand < plant.oddsleft[0] + plant.oddsright[0])
                {
                    finalrand = 1;
                }
                else
                {
                    finalrand = 2;
                }

                if (finalrand == 2)
                {
                    Vector2 newPos = plant.allTiges[i].transform.position + lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                    GameObject newtige = CreateNewTige(plant.allTiges[i].transform.parent, plant, newPos);

                    newtige.GetComponent<LineRenderer>().positionCount += 1;
                    newtige.GetComponent<LineRenderer>().SetPosition(1, new Vector3(1, -1, 0));
                    tigerSetColider(newtige, Vector3.zero, new Vector2(1, -1));

                    //tigerSetColider(newtige, Vector2.zero, Vector2.one);
                    newtiges.Add(newtige);
                }
                lineRenderer.positionCount += 1;
                Vector3 newPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 2);

                if (finalrand == 0 || finalrand == 2)
                {
                    newPosition += new Vector3(-1, -1, 0);
                }
                if (finalrand == 1)
                {
                    newPosition += new Vector3(1, -1, 0);
                }
                tigerSetColider(plant.allTiges[i], lineRenderer.GetPosition(lineRenderer.positionCount - 2), newPosition);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPosition);
            }
            foreach (var nTige in newtiges)
            {
                plant.allTiges.Add(nTige);

            }
        }
    }
    
    void UpdateOddsText()
    {
        for (int i = 0; i < 5; i++)
        {
            oddslefttext[i].text = Math.Round(plantmenuuser.oddsleft[i] * 33).ToString() + "%";
            oddsrighttext[i].text = Math.Round(plantmenuuser.oddsright[i] * 33).ToString() + "%";
            oddsattacktext[i].text = Math.Round(plantmenuuser.oddsattack[i] * 100).ToString() + "%";
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
        foreach (GameObject tige in plant.allTiges)
        {
            tigeprof = tige.transform.position.y + tige.GetComponent<LineRenderer>()
                .GetPosition(tige.GetComponent<LineRenderer>().positionCount - 1).y;
            if (tigeprof < minprof ){
                minprof = tigeprof;
            }
        }

        return minprof;
    }


    /* int CheckForCollisions(int iterationnum)
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
                                 if (plant.id == int.Parse(p1.transform.parent.name))
                                 {
                                     plant1 = plant;
                                 }
                                 if (plant.id == int.Parse(p2.transform.parent.name))
                                 {
                                     plant2 = plant;
                                 }
                             }

                             int finalrand = 0;
                             int maxwhile = 0;
                             while (finalrand == 0 && maxwhile < 10)
                             {
                                 maxwhile += 1;
                                 float rand1 = Random.Range(0f, 1f);
                                 float rand2 = Random.Range(0f, 1f);
                                 finalrand = 0;
                                 if (rand1 > plant1.oddsleft[iterationnum])
                                 {
                                     finalrand -= 1;
                                 }
                                 if (rand2 > plant2.oddsleft[iterationnum])
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
     }*/
    
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
        //  plantstiges = (GameObject.FindGameObjectsWithTag("PlantTige")).ToList();
        timer = Time.time;
        cam = Camera.main;
        instance = this;
    }

    void tigerSetColider(GameObject tige, Vector2 lastPoint, Vector2 newPoint)
    {
        PolygonCollider2D col = tige.GetComponent<PolygonCollider2D>();
        col.pathCount++;
        Vector2[] pathPoints = new Vector2[4];
        pathPoints[0] = lastPoint - new Vector2(0.1f, 0);
        pathPoints[1] = lastPoint + new Vector2(0.1f, 0);
        pathPoints[2] = newPoint + new Vector2(0.1f, 0);
        pathPoints[3] = newPoint - new Vector2(0.1f, 0);
        col.SetPath(col.pathCount - 1, pathPoints);
    }

    GameObject CreateNewTige(Transform parent, Plant plant, Vector3 position)
    {
        GameObject newtige = Instantiate(tigePrefab, position, Quaternion.identity);
        newtige.transform.parent = parent;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(plant.color, 0.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1.0f) }
        );
        newtige.GetComponent<LineRenderer>().colorGradient = gradient;
        return newtige;
    }

    void CreateOrEditPlant()
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
            CameraManager.instance.startLerpToPlant(userplant.gameObject);
            pointsvailableobject.GetComponent<TMP_Text>().text = userplant.pointsAvailable.ToString();
            UpdateOddsText();
        }
        else
        {
            plantMenu.SetActive(false);
            newplantinput.GetComponent<TMP_InputField>().text = "";
            passwordinput.GetComponent<TMP_InputField>().text = "";
            Plant newplant = new Plant();
            newplant.id = input;
            newplant.password = password;
            plants.Add(newplant);
            newplant.color = sliderhandle.GetComponent<Image>().color;

            newplantinput.GetComponent<TMP_InputField>().text = "";
            GameObject newparent = Instantiate(parentPrefab, selectionCircle.transform.position, Quaternion.identity);
            newparent.name = input;
            newparent.transform.GetChild(0).GetChild(0).transform.GetComponent<SpriteRenderer>().color = newplant.color;
            newplant.gameObject = newparent;
            newplant.allTiges.Add(CreateNewTige(newparent.transform, newplant, selectionCircle.transform.position));
            newparent.GetComponent<PlantCollision>().plantId = newplant.id;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CreateOrEditPlant();
        }
        selectionCircle.transform.position =
            new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, ground.transform.position.y, 0);
        if (move > 0)
        {
            move -= 1;

            Evolve();
            CheckDoubleTige();

            /*  for (int tige = 0; tige < newtiges.Count; tige++)
              {
                  plantstiges.Add(newtiges[tige]);
              }*/

            //   int collisionsverified = CheckForCollisions(4 - move);
            /* int limit = 0;
             while (collisionsverified == 0 && limit < 50)
             {
                 limit += 1;
                 plantstiges = (GameObject.FindGameObjectsWithTag("PlantTige")).ToList();
                 collisionsverified = CheckForCollisions(4 - move);
             }*/
            timer = Time.time;
            
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
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (lastkey == 0)
                {
                    lastkey = 1;
                }
                else
                {
                    lastkey = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if (lastkey == 1)
                {
                    lastkey = 2;
                }
                else
                {
                    lastkey = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                if (lastkey == 2)
                {
                    lastkey = 3;
                }
                else
                {
                    lastkey = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (lastkey == 3)
                {
                    lastkey = 4;
                }
                else
                {
                    lastkey = 0;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (lastkey == 4)
                {
                    move = 1;
                }
                else
                {
                    lastkey = 0;
                }
            }
            else
            {
                lastkey = 0;
            }
        }

    }
}
