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
	public static string drawMethod = "";
	public static string surface = "";
	public static int block;
	public static Tuple<int, Complexity> shape = new Tuple<int, Complexity>(0, Complexity.DEFAULT);
    public static List<(Vector3, long)> path = new List<(Vector3, long)>();
    public static long timeTrialStart=0;
    public static long timeDrawStart=0;
    public static long timeDrawEnd=0;

    public static DateTime BEGIN = new DateTime(2020, 1, 1);

  	public static void SetFilePath(int pid) {
  		string fp = Application.dataPath + "/Logs/participant_data_" + pid;
  		
  		while (File.Exists(fp+".csv")) {
  			fp = fp + "_n";
  		}

  		filepathData = fp + ".csv";

  		string fpRaw = Application.dataPath + "/Logs/participant_raw_" + pid;
  		
  		while (File.Exists(fpRaw+".csv")) {
  			fpRaw = fpRaw + "_n";
  		}

  		filepathRaw = fpRaw + ".csv";
  		SetHeaders();
   	}

   	public static void SetHeaders() {
   		try {
    		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathData, true)) {

    			file.WriteLine("pid;drawMethod;surface;block;shape;timeTrialStart;timeDrawStart;timeDrawEnd;stroke;");

    		}
    	}
    	catch (Exception ex) {
    		throw new ApplicationException("Could not write header: ", ex);
    	}


    	try {
    		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathRaw, true)) {

    			file.WriteLine("pid;drawMethod;surface;block;shape;timeTrialStart;timeDrawStart;timeDrawEnd;stroke;");

    		} 
    	}
    	catch (Exception ex) {
    		throw new ApplicationException("Could not write header: ", ex);
    	}
   	}

    public static void SetPID(int ID) {
        pid = ID;
    }

	public static void SetDrawMethod(string method) {
		drawMethod = method;
	}

	public static void SetSurface(string surf) {
		surface = surf;
	}

	public static void SetBlock(int b) {
		block = b;
	}

	public static void SetShape(Tuple<int, Complexity> s) {
		shape = s;
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

    public static void AddStroke(string strokeData) {
    	try {
    		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepathData, true)) {

                file.WriteLine(pid + ";" + drawMethod + ";" + surface + ";" + block + ";" + shape + ";" + timeTrialStart + ";" + timeDrawStart + ";" + timeDrawEnd + ";" + strokeData + ";");
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