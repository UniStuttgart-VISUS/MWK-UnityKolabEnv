using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.JSON;

public class JsonIO : MonoBehaviour
{
    public const string filePath = "/JSON/config.json";
    // Start is called before the first frame update
    void Start()
    {
        WriteFile();
        ReadFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadFile()
    {
        // read file into a string and deserialize JSON to a type
        Configuration config = Configuration.CreateFromJSON(File.ReadAllText(Application.dataPath + filePath));

        EnumParameter speed = ParameterType.CreateFromJSON<EnumParameter>(File.ReadAllText(Application.dataPath + "/JSON/speed.json"));
        Debug.Log(speed.currentValue + " - " + speed.enumList + " - " + speed.name);
    }

    public void WriteFile()
    {
        Configuration newConfig = new Configuration();
        EnumParameter speedTempos = new EnumParameter();
        var strArray = new[] { "slow", "fast", "very fast" };
        var  strList = new List<string>(strArray);
        speedTempos.currentValue = "slow";
        speedTempos.enumList = strList;
        speedTempos.name = "speeds";

        FloatParameter brightness = new FloatParameter();
        brightness.name = "brightness";
        brightness.currentValue = 0.4f;

        newConfig.Add(speedTempos);
        newConfig.Add(brightness);

        BooleanParameter boolConf = new BooleanParameter();
        boolConf.name = "visibility";
        boolConf.currentValue = true;

        newConfig.Add(boolConf);

        IntegerParameter intConf = new IntegerParameter();
        intConf.name = "amount of points";
        intConf.currentValue = 25;

        newConfig.Add(intConf);

        Debug.Log(newConfig.SaveToString());
        
        //File.WriteAllText(Application.dataPath + filePath, JsonConvert.SerializeObject(newConfig, Formatting.Indented));
        File.WriteAllText(Application.dataPath + filePath, newConfig.SaveToString());
        File.WriteAllText(Application.dataPath + "/JSON/speed.json", speedTempos.SaveToString());


    }
    
}
