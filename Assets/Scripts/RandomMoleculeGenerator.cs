using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    
    // Start is called before the first frame update
    void Start()
    {
        //Define Colors
        elementToColor = new Dictionary<string, Color>();
        elementToColor.Add("N", new Color(0.0f,0.0f,0.8f,0.8f));
        elementToColor.Add("C", new Color(0.0f,0.0f,0.0f,0.8f));
        elementToColor.Add("H", new Color(0.8f,0.8f,0.8f,0.8f));
        elementToColor.Add("O", new Color(0.8f,0.0f,0.0f,0.8f));
        
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
                //Create atom sphere
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.parent = this.transform;
                sphere.transform.position = new Vector3(float.Parse(fields[5], CultureInfo.InvariantCulture),
                                                        float.Parse(fields[6], CultureInfo.InvariantCulture),
                                                        float.Parse(fields[7], CultureInfo.InvariantCulture));
                sphere.transform.localScale = new Vector3(sphereRadius,sphereRadius,sphereRadius);
                sphere.GetComponent<Renderer>().material = atomMat;
                sphere.GetComponent<Renderer>().material.color = elementToColor[RemoveIntegers(fields[2])];
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
            
            //scale down parent accordingly
            transform.position.Scale(new Vector3(1/newScale,1/newScale,1/newScale));
        }
    }
    
    public string RemoveIntegers(string input)
    {
        return Regex.Replace(input, @"[\d-]", string.Empty);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
