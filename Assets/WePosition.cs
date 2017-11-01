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
    public Button nextPage;
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
        //AddItemToGrid(0);
        AddWordsToScreen(0);
        //ClearPage(0);
        //AddWordsToScreen(1);
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
        //if (TobiiAPI.GetFocusedObject() != null) {
        //  print(TobiiAPI.GetFocusedObject().name);
        //}
        focusedObject = TobiiAPI.GetFocusedObject();
        pos = TobiiAPI.GetGazePoint();

        if (null != focusedObject)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Djidjelly Siclait\Desktop\data.txt", true))
            {
                file.WriteLine("" + focusedObject.GetInstanceID() + "," + Time.time);
            }

            /*using (System.IO.StreamWriter file = 
				new System.IO.StreamWriter(@"C:\Users\mc\Desktop\WriteLines2.txt", true))
			{
				file.WriteLine("The focused game object is: " + focusedObject.name + " (ID: " + focusedObject.GetInstanceID() + ")");
				file.WriteLine(""+focusedObject.GetInstanceID());
			}*/
            print("The focused game object is: " + focusedObject.name + " (ID: " + focusedObject.GetInstanceID() + ")");
        }
        else
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\mc\Desktop\IdTime.txt", true))
            {
                file.WriteLine("N/A , " + Time.time);
            }

        }
        using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\Users\mc\Desktop\posTime.txt", true))
        {
            file.WriteLine(pos.Screen.x + "," + pos.Screen.x + "," + Time.time);
            //file.WriteLine("" + focusedObject.GetInstanceID());
        }
    }

    public void ClearPage(int direction)
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
        int pointer = 1;

        const int limit = 2127;
        int startPointX = 37;
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
                startPointX = 37;
                startPointY -= 30;
            }

        }

        print("There are " + blacklist.Count + " candidates to eliminate.");
    }

    private void init()
    {
        firstrow = Instantiate(grid) as GameObject;
        firstrow.transform.parent = page.transform;

    }
}
