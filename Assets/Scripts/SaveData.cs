using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SaveData 
{

  	public static string filepathData;
  	public static string filepathRaw;


  	public static int pid;

  	//public static List<HandPoint> hps = new List<HandPoint>();
    public static List<(Vector3, long)> path = new List<(Vector3, long)>();
    public static long timeTrialStart=0;
    public static long timeDrawStart=0;
    public static long timeDrawEnd=0;
    public static string gestureName = "G?";
    public static string gestureSize = "";
    public static string gestureComplexity = "";
    public static string condition = "";
    public static string lineArr;

    public static DateTime BEGIN = new DateTime(2020, 1, 1);

  	public static void SetFilePath(int pid) {
  		string fp = Application.dataPath + "/LoggedData/participant_data_" + pid;
  		
  		while (File.Exists(fp+".csv")) {
  			fp = fp + "_n";
  		}

  		filepathData = fp + ".csv";

  		string fpRaw = Application.dataPath + "/LoggedData/participant_raw_" + pid;
  		
  		while (File.Exists(fpRaw+".csv")) {
  			fpRaw = fpRaw + "_n";
  		}

  		filepathRaw = fpRaw + ".csv";
  		SetHeaders();
   	}

   	public static void SetHeaders() {
   		try {
    		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathData, true)) {

    			//file.WriteLine("pid;block;condition;gestureName;gestureSize;gestureComplexity;timeTrialStart;timeDrawStart;timeDrawEnd;pathData;");
    			file.WriteLine("pid;block;condition;gestureName;timeTrialStart;timeDrawStart;timeDrawEnd;pathData;");

    		}
    	}
    	catch (Exception ex) {
    		throw new ApplicationException("Could not write header: ", ex);
    	}


    	try {
    		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathRaw, true)) {

    			file.WriteLine("pid;block;condition;gestureName;gestureSize;gestureComplexity;handPoint");

    		} 
    	}
    	catch (Exception ex) {
    		throw new ApplicationException("Could not write header: ", ex);
    	}
   	}

   	public static void SetTimeTrialStart() {
   		timeTrialStart = DateTime.Now.Ticks-BEGIN.Ticks;
   	}

   	public static void SetTimeDrawStart() {
   		timeDrawStart = DateTime.Now.Ticks-BEGIN.Ticks;
   	}

   	public static void SetTimeDrawEnd() {
   		timeDrawEnd = DateTime.Now.Ticks-BEGIN.Ticks;
   	}

   	public static void SetGestureName(string name) {
   		gestureName = name;
   	}

    public static void SetPID(int ID)
    {
        pid = ID;
    }

    public static void SetGestureSize(string size) {
   		gestureSize = size;
   	}

   	public static void SetGestureComplexity(string complexity) {
   		gestureComplexity = complexity;

   	}

   	public static void SetCondition(string cond) {
   		condition = cond;
   	}

    public static void SetLine(string line)
    {
        lineArr = line;
    }

    public static void AddCurrentRecord() {
        string pathData = lineArr;
        //Debug.Log("Line Vector List : " + pathData);
        int block = PlayerPrefs.GetInt("overallState");
    	// hps.Clear();
    	path.Clear();
    	try {
    		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathData, true)) {

    			//file.WriteLine(pid+";"+block+";"+condition+";"+gestureName+";"+gestureSize+";"+gestureComplexity+";"+timeTrialStart+";"+timeDrawStart+";"+timeDrawEnd+";"+pathData);
                file.WriteLine(pid + ";" + block + ";" + condition + ";" + gestureName + ";" + timeTrialStart + ";" + timeDrawStart + ";" + timeDrawEnd + ";" + pathData);

            } 
    	}
    	catch (Exception ex) {
    		throw new ApplicationException("Could not save to file: ", ex);
    	}
    
    }

    public static string CreateJSONFromList<T>(string name, List<T> x) {
    	StringBuilder xStr = new StringBuilder("\""+name+"\":{");
    	for (int i = 0; i < x.Count; i++) {
    		xStr = xStr.Append(JsonUtility.ToJson(x[i]) + ",");
    	}
    	xStr[xStr.Length-1] = '}';
    	return xStr.ToString();
    }

    /*
    public static string CreateJSONstring(Vector3[] line)
    {
        string json = JsonUtility.ToJson(line[0]);
        Debug.Log("Line Vector List : " + json);
        //string json = JsonUtility.ToJson(line);

        return json;
    }
    */

    // public static void writeHandPoint(HandPoint handPoint)
    // {
    //     string hp = JsonUtility.ToJson(handPoint);
    //     int block = PlayerPrefs.GetInt("overallState");

    //     try
    //     {
    //         using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathRaw, true))
    //         {

    //             //file.WriteLine(pid + ";" + block + ";" + condition + ";" + gestureName + ";" + gestureSize + ";" + gestureComplexity + ";" + hp);
    //             file.WriteLine(pid + ";" + block + ";" + condition + ";" + gestureName + ";" + hp);

    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new ApplicationException("Could not save to file: ", ex);
    //     }

    // }
}