using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.UI;

public class WePosition : MonoBehaviour {

    private int xcounter = 0;
    private int ycounter = 0;
    private int currentRowLength = 0;
    private GameObject firstrow;
    public GameObject page;
    public GameObject row;
    public GameObject grid;
    public GameObject word;
    private ArrayList words;
    private List<ArrayList> pages;
    private int currentpage;
    private int numPages;

    // TOBII VARIABLES
    private GazePoint pos;
    private GameObject focusedObject;

    private const int wordCountLimit = 700;

    // Registry to know which objects to destroy before changing pages
    private ArrayList blacklist;

    // Use this for initialization
    void Start()
    {
        words = new ArrayList();
        pages = new List<ArrayList>();
        pos = TobiiAPI.GetGazePoint();
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("C:/Users/Djidjelly Siclait/Desktop/Hello.txt"))
            {
                string line;

                // Read and display lines from the file until 
                // the end of the file is reached. 
                while ((line = sr.ReadLine()) != null)
                {
                    words.AddRange(line.Split(' '));
                }
            }

            numPages = (int)Math.Ceiling((decimal)words.Count / wordCountLimit);
            print("Total words: " + words.Count);
            print("Posible #pages: " + numPages);

            int j = 0;
            int counter = 1;
            ArrayList buffer = new ArrayList();

            foreach (String s in words)
            {
                if (j < wordCountLimit - 1 )
                {
                    if (s[s.Length - 1] == '.' && (wordCountLimit - j <= 50 || wordCountLimit - j <= 100))
                    {
                        buffer.Add(s);
                        print("buffer = " + buffer.Count);
                        pages.Add(buffer);
                        j = 0; counter++;
                        buffer = new ArrayList();
                    }
                    else
                    {
                        buffer.Add(s);
                        j++;
                    }
                }
                else
                {
                    buffer.Add(s);
                    print("buffer = " + buffer.Count);
                    pages.Add(buffer);
                    j = 0; counter++;
                    buffer = new ArrayList();
                }
            }

            print("buffer = " + buffer.Count);
            pages.Add(buffer);

            print("Real Pages =" + pages.Count);
            currentpage = 0;

        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        init();

        AddWordsToScreen(currentpage);

    }

    public void PreviousPage()
    {
        if (currentpage != 0)
        {
            ClearPage();
            AddWordsToScreen(--currentpage);
        }
    }

    public void NextPage()
    {
        if(currentpage == pages.Count - 1)
        {
            String URL = @"C:\Users\Djidjelly Siclait\Desktop\meta.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(URL, true))
            {
                file.WriteLine("finish_time:" + DateTime.Now.ToString("h:mm:ss"));
            }

            Application.Quit();
        }

        if (currentpage != pages.Count - 1)
        {
            ClearPage();
            AddWordsToScreen(++currentpage);
        }
    }

    public void fixit()
    {
        //Canvas.ForceUpdateCanvases();
        RectTransform T = page.GetComponent(typeof(RectTransform)) as RectTransform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(T);
    }

    // Update is called once per frame
    void Update()
    {
        focusedObject = TobiiAPI.GetFocusedObject();
        pos = TobiiAPI.GetGazePoint();

        if (null != focusedObject)
        {
            String URL1 = @"C:\Users\Djidjelly Siclait\Desktop\reading" + (currentpage + 1).ToString() + ".txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(URL1, true))
            {
                if(10 < pos.Screen.x && pos.Screen.x < 2300 && 10 < (597 - pos.Screen.y))
                    file.WriteLine(pos.Screen.x + "," + (597 - pos.Screen.y));
            }

            String URL2 = @"C:\Users\Djidjelly Siclait\Desktop\keywords.txt";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(URL2, true))
            {
                file.WriteLine(focusedObject.name + "," + (currentpage + 1).ToString() + "," + Time.time);
            }

            print("The focused game object is: " + focusedObject.name + " (ID: " + focusedObject.GetInstanceID() + ")");
        }
        
    }

    public void ClearPage()
    {
        foreach(GameObject word in blacklist)
        {
            Destroy(word);
        }

        print("Eliminated candidates");

    }

    public void AddWordsToScreen(int pageIndex)
    {
        blacklist = new ArrayList();

        const int limit = 2127;
        int startPointX = 47;
        int startPointY = -12;
        int lastWidth = 0;

        foreach(String w in pages[pageIndex])
        {
            GameObject newWord = Instantiate(word);
            TextMesh temp = newWord.GetComponent(typeof(TextMesh)) as TextMesh;
            
            temp.text = w + "  ";
            temp.color = Color.black;
            temp.characterSize = 20;

            newWord.AddComponent<BoxCollider>();
            newWord.name = temp.text;
            newWord.transform.SetPositionAndRotation(new Vector3(startPointX, startPointY), Quaternion.identity);

            BoxCollider box = newWord.GetComponent<BoxCollider>();
            lastWidth = (int)box.size.x;
            
            blacklist.Add(newWord);

            if (startPointX + lastWidth < limit)
                startPointX += lastWidth;
            else
            {
                startPointX = 47;
                startPointY -= 30;
            }

        }

        GameObject parentObj = GameObject.Find("Index");
        Text position = parentObj.GetComponentInChildren<Text>();
        position.text = (currentpage + 1).ToString() + " de " + pages.Count.ToString();

        print("There are " + blacklist.Count + " candidates to eliminate.");

        SavePageScreenshot();
    }

    private void init()
    {
        firstrow = Instantiate(grid) as GameObject;
        firstrow.transform.parent = page.transform;

        String URL = @"C:\Users\Djidjelly Siclait\Desktop\meta.txt";
        using (System.IO.StreamWriter file =
        new System.IO.StreamWriter(URL, false))
        {
            file.WriteLine("pages:" + pages.Count);

            int count = 1;
            int sum = 0;
            foreach (ArrayList p in pages)
            {
                file.WriteLine(count++ + ":" + p.Count);
                sum += p.Count;
            }

            file.WriteLine("words:" + sum);
            file.WriteLine("start_time:" + DateTime.Now.ToString("h:mm:ss"));
        }
    }

    private void SavePageScreenshot()
    {
        string file = "page" + (currentpage + 1).ToString() + ".png";

        if (!File.Exists(file))
        {
            Application.CaptureScreenshot(file);
            print("Saving page screenshot");
        }
        else
            print("This page already exist");
    }
}
