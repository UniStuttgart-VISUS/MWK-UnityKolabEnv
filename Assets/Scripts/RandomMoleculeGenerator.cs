using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class RandomMoleculeGenerator : MonoBehaviour
{
    public TextAsset pdbFile;
    public Dictionary<String, Color> elementToColor;
    private List<GameObject> spheres = new List<GameObject>();
    private float largestCoord = 0.0f;
    public float sphereRadius = 0.5f;
    public bool autoVanDerWaals = true;
    public Material atomMat;
    public float initialScale = 0.1f;
    private AtomInfoCollection ai;
    public bool useAtomSizes = true;
    
    // Start is called before the first frame update
    void Start()
    {
        //Load atomic data
        TextAsset jsonObj = Resources.Load("atomic_data") as TextAsset;
        ai = JsonUtility.FromJson<AtomInfoCollection>(jsonObj.text);
        
        //Define Colors
        elementToColor = new Dictionary<string, Color>();
        elementToColor.Add("N", new Color(0.0f,0.0f,1.0f,0.9f));
        elementToColor.Add("C", new Color(0.0f,0.0f,0.0f,0.9f));
        elementToColor.Add("H", new Color(1.0f,1.0f,1.0f,0.9f));
        elementToColor.Add("O", new Color(1.0f,0.0f,0.0f,0.9f));
        
        //Load  PDB
        string[] lines = pdbFile.text.Split(new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.RemoveEmptyEntries
        );
        
        //Process line by line
        for (int i = 0; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(new[] { " " },
                StringSplitOptions.RemoveEmptyEntries
            );
            if (fields[0] == "HETATM" || fields[0] == "ATOM")
            {
                //Get atom info for current atom
                Color col;
                var info = ai.atomInfos.FirstOrDefault(q => q.symbol == fields[2][0].ToString());
                if (info == null || !ColorUtility.TryParseHtmlString("#" + info.cpkHexColor, out col))
                {
                    continue;
                    col = Color.magenta;
                }
                col.a = 0.6f;
                
                //Create atom sphere
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.parent = this.transform;
                sphere.transform.position = new Vector3(float.Parse(fields[5], CultureInfo.InvariantCulture),
                                                        float.Parse(fields[6], CultureInfo.InvariantCulture),
                                                        float.Parse(fields[7], CultureInfo.InvariantCulture));
                if(!useAtomSizes) sphere.transform.localScale = new Vector3(sphereRadius,sphereRadius,sphereRadius);
                else sphere.transform.localScale = new Vector3(sphereRadius*info.atomicRadius/100,sphereRadius*info.atomicRadius/100,sphereRadius*info.atomicRadius/100);
                sphere.GetComponent<Renderer>().material = atomMat;
                //Get color from atom info and set
                sphere.GetComponent<Renderer>().material.color = col;
                sphere.name = info.name + i;
                //sphere.GetComponent<Renderer>().material.SetColor("_EmissionColor", col);
                spheres.Add(sphere);
                if (sphere.transform.position.z > largestCoord) largestCoord = sphere.transform.position.z;
            } else if (fields[0] == "CONECT")
            {
                //get source atom coords
                Vector3 srcCrd = new Vector3(spheres[int.Parse(fields[1])-1].transform.position.x,
                                             spheres[int.Parse(fields[1])-1].transform.position.y,
                                             spheres[int.Parse(fields[1])-1].transform.position.z);
                for (int j = 2; j < fields.Length-2; j++)
                {
                    //get target atom coords
                    Vector3 trgtcCrd = new Vector3(spheres[int.Parse(fields[j])-1].transform.position.x,
                        spheres[int.Parse(fields[j])-1].transform.position.y,
                        spheres[int.Parse(fields[j])-1].transform.position.z);
                    
                    //create connector
                    var cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    cylinder.transform.parent = this.transform;
                    cylinder.transform.position = (trgtcCrd-srcCrd)/2.0f + srcCrd; 
                    var v3T = cylinder.transform.localScale;      // Scale it
                    v3T.y = (trgtcCrd-srcCrd).magnitude - sphereRadius*2;
                    v3T.x = v3T.z = 0.1f;
                    cylinder.transform.localScale = v3T;
                    cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, trgtcCrd-srcCrd);
                    cylinder.GetComponent<Renderer>().material = atomMat;
                }
            }
        }
        
        //auto resize for pseudo van der waals style
        if (autoVanDerWaals)
        {
            //calculate scale
            float newScale = largestCoord / 2;
            
            //scale spheres
            foreach (GameObject sphere in spheres)
            {
                Vector3 ls = sphere.transform.localScale;
                ls.x *= newScale;
                ls.y *= newScale;
                ls.z *= newScale;
                sphere.transform.localScale = ls;
            }
        }
        
        //Repositioning / rescaling
        Bounds bc = transform.GetChild(0).GetComponent<Renderer>().bounds;
        foreach (Transform child in transform)
        {
            bc.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        
        //Scale down parent
        float targetScale = Mathf.Min(new float[3]{3.0f / bc.size.x, 3.0f / bc.size.y, 3.0f / bc.size.z});
        transform.localScale = new Vector3(targetScale, targetScale, targetScale);
        
        //Get new bounds
        Bounds bcNew = transform.GetChild(0).GetComponent<Renderer>().bounds;
        foreach (Transform child in transform)
        {
            bcNew.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
            child.transform.position += new Vector3(-bcNew.center.x, -bcNew.center.y+1.5f, -bcNew.center.z);
        }
        
        //Recenter all children
        /*Vector3 recenter = new Vector3(-bcNew.center.x, -bcNew.center.y+1.5f, -bcNew.center.z);
        transform.position = recenter;*/
        
    }
    
    public string RemoveIntegers(string input)
    {
        return Regex.Replace(input, @"[\d-]", string.Empty);
    }
    
    // Update is called once per frame
    void Update()
    {

    }
    
    void OnDrawGizmosSelected()
    {
        Bounds bc = transform.GetChild(0).GetComponent<Renderer>().bounds;
        foreach (Transform child in transform)
        {
            bc.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }
        
        Vector3 center = bc.center;
        Vector3 extents = bc.extents;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center,extents*2);
    }
}

[Serializable]
public class AtomInfo
{
    public int atomicNumber;
    public string symbol;
    public string name;
    public int atomicMass;
    public string cpkHexColor;
    public string electronicConfiguration;
    public float electronegativity;
    public int atomicRadius;
    public float ionRadius;
    public int vanDelWaalsRadius;
    public int ionizationEnergy;
    public int electronAffinity;
    public string oxidationStates;
    public string standardState;
    public string bondingType;
    public float meltingPoint;
    public float boilingPoint;
    public float density;
    public string groupBlock;
    public int yearDiscovered;
}

[Serializable]
public class AtomInfoCollection
{
    public AtomInfo[] atomInfos;
}