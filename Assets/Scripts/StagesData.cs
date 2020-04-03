using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagesData
{
    //Really need to figure out a more gracefull idea to make stage/item databases...
    static Dictionary<uint, Stage> stageList = new Dictionary<uint, Stage>
    {
        {000,   new Stage("SampleScene",    3,  300.0f, 240.0f, 180.0f)},
        {001,   new Stage("TestScene1",    2,  200.0f, 120.0f, 60.0f)}
    };



    public static Stage GetStageParameters(uint stageID) //returns null if no stage of given stageID is found
    {
        Stage result;

        if (!stageList.TryGetValue(stageID, out result))
        {
            result = null;
        }

        return result;
    }
}

[System.Serializable]
public class Stage
{
    public string name;
    //public uint id;
    public int minTrapsTriggeredToClear;
    public float parTimeBronze;
    public float parTimeSilver;
    public float parTimeGold;

    public Stage(string _name, int _minTrapsTriggeredToClear, float _parTimeBronze, float _parTimeSilver, float _parTimeGold)
    {
        name = _name;
        minTrapsTriggeredToClear = _minTrapsTriggeredToClear;
        parTimeBronze = _parTimeBronze;
        parTimeSilver = _parTimeSilver;
        parTimeGold = _parTimeGold;
    }
}