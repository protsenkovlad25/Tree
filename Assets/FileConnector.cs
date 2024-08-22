using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileConnector : MonoBehaviour
{
    [SerializeField] int from;
    [SerializeField] int to;

    void Start()
    {
        StreamWriter sw = new StreamWriter("Assets/Resources/Crosses/cross.txt");
        for (int i = from; i<= to; i++)
        {
            string baseStr = "Assets/Resources/Crosses/cross";

            baseStr += i.ToString();

            baseStr += ".txt";

            if(File.Exists(baseStr))
            {
                StreamReader sr = new StreamReader(baseStr);
                string s = sr.ReadToEnd();
                s += "\n";
                sw.Write(s);
                sr.Close();
            }
        }
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
