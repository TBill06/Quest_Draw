using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

// This script logs the data of the tubes drawn in the virtual environment.
public class LogData : MonoBehaviour
{
    private GameObject[] tubes = new GameObject[0];

    public void ContinueButtonClicked()
    {
        string timestampForFile = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        string fileName = $"Assets/Logs/VSurface_{timestampForFile}.csv";
        
        tubes = GameObject.FindGameObjectsWithTag("Tube");
        if (tubes.Length == 0)
        {
            Debug.Log("No tubes to log.");
            return;
        }

        VSurfacePoint vSurfacePoint = FindObjectOfType<VSurfacePoint>();
        if (vSurfacePoint != null)
        {
            bool drawnByPoint = vSurfacePoint.drawnByPoint;
        }

        using (StreamWriter sw = new StreamWriter(fileName))
        {
            StringBuilder sb = new StringBuilder();
            foreach (GameObject tube in tubes)
            {
                ProceduralTube pt = tube.GetComponent<ProceduralTube>();
                foreach (Vector3 point in pt.Points)
                {
                    string timestamp = System.DateTime.Now.ToString("o");
                    sb.AppendLine($"{point.x},{point.y},{point.z},{timestamp}");
                }
            }
            sw.Write(sb.ToString());

        }

        foreach (GameObject tube in tubes)
        {
            Destroy(tube);
        }
        tubes.Clear();
    }
}
