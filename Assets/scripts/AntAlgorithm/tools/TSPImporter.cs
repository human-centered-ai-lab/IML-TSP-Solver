/****************************************************
 * IML ACO implementation for TSP 
 * More information: http://hci-kdd.org/project/iml/
 * Author: Andrej Mueller
 * Year: 2017
 *****************************************************/

/* TSPImporter is a simple importer for .tsp files (http://www.iwr.uni-heidelberg.de/groups/comopt/software/TSPLIB95/) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace AntAlgorithm.tools
{
    // ReSharper disable once InconsistentNaming
    public class TSPImporter
    {
        private const string TspLibFolderName = "tspLib";
        private const string PointSection = "NODE_COORD_SECTION";

        public string TspWebDirectory;

        public List<City> Cities { get; private set; }
        public bool loadingComplete { get; private set; }

        public TSPImporter(string tspWebDirectory)
        {
            Cities = new List<City>();
            this.TspWebDirectory = tspWebDirectory;
        }

        public TSPImporter()
        {
            Cities = new List<City>();
            this.TspWebDirectory = "http://www.andrejmueller.com/TSPLIB";
        }

        public static List<City> ImportTsp(string fileName)
        {
            string line;
            int count = 0;
            List<City> cities = new List<City>();
            System.IO.StreamReader file =
               new System.IO.StreamReader(Application.dataPath + "/" + TspLibFolderName + "/" + fileName);
            while ((line = file.ReadLine()) != null)
            {
                if (line == PointSection)
                    break;
            }
            while ((line = file.ReadLine()) != null)
            {
                char[] delimiterChars = { ' ', ':', '\t' };
                string[] cityParameter = line.Split(delimiterChars);
                if (cityParameter.Length == 3)
                {
                    cities.Add(new City(Int32.Parse(cityParameter[1]), Int32.Parse(cityParameter[2]), count));
                }
                count++;
            }
            file.Close();
            return cities;
        }

        public IEnumerator ImportTspFromWeb(string tspFileToUse)
        {
            loadingComplete = false;
            string filePath = TspWebDirectory + tspFileToUse;

            UnityWebRequest www = UnityWebRequest.Get(filePath);
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.Send();

            while (!www.downloadHandler.isDone)
                yield return new WaitForEndOfFrame();

            if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string result = www.downloadHandler.text;
                LoadCities(result);
            }
            yield break;
        }

        private void LoadCities(string result)
        {
            bool pointArea = false;
            int count = 0;

            foreach (string line in result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line == PointSection)
                {
                    pointArea = true;
                }
                else if (pointArea)
                {
                    char[] delimiterChars = { ' ', ':', '\t' };
                    string[] cityParameter = line.Split(delimiterChars);
                    if (cityParameter.Length == 3)
                    {
                        // Debug.Log("City" + count + " " + cityParameter[1] + " / " + cityParameter[2]);
                        Cities.Add(new City(Int32.Parse(cityParameter[1]), Int32.Parse(cityParameter[2]), count));
                    }
                    count++;
                }
            }

            loadingComplete = true;
        }
    }
}
