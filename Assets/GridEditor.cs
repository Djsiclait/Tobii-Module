using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using Tobii.Gaming;
using System.Collections.Generic;

public class GridEditor : MonoBehaviour {

    private int xcounter = 0;
    private int ycounter = 0;
    private int pagestart = 0;
    private int pageend= 8;
    private int currentRowLength = 0;
    private GameObject firstrow;
    public GameObject page;
    public GameObject row;
    public GameObject word;
    public Button nextPage;
    private ArrayList words;
    private List<ArrayList> pages;
    private int currentpage;
    private static int maxRowlength = 80;
    private static int maxLineLength = 5;
    private int numPages;
    // Use this for initialization
    void Start () {
        words = new ArrayList();
        pages = new List<ArrayList>();
        maxRowlength = 80;
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

            numPages = words.Count / (maxRowlength * maxLineLength);
            print(words.Count);
            print(numPages);

            for (int i = 0; i < numPages; i++)
            {
                ArrayList buffer = new ArrayList();

                for (int j = (maxRowlength * maxLineLength) * i; j < (maxRowlength * maxLineLength) * (i + 1); j++)
                {
                    while (j < words.Count)
                        buffer.Add(words[j]);
                }

                pages.Add(buffer);
            }
            currentpage = 0;

        }
        catch (Exception e)
        {

            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        firstrow = Instantiate(row) as GameObject;
        //newText.text = string.Format("Item {0}", counter.ToString());
        firstrow.transform.parent = page.transform;
        //AddItemToGrid(firstrow);
        nextPage.onClick.AddListener(AddItemToGrid);
        //AddItemToGrid();
    }

    // Update is called once per frame
    void Update () {
        //if (TobiiAPI.GetFocusedObject() != null) {
        //  print(TobiiAPI.GetFocusedObject().name);
        //}
        GameObject focusedObject = TobiiAPI.GetFocusedObject();
        if (null != focusedObject)
        {
            print("The focused game object is: " + focusedObject.name + " (ID: " + focusedObject.GetInstanceID() + ")");
        }
    }

    public void GoBack()
    {
        AddItemToGrid();
    }

    public void AddItemToGrid()
    {
        foreach (Transform child in page.transform)
            child.parent = null;

        GameObject currentRow = firstrow;

        ArrayList data = new ArrayList();
        int counter = 0;

        foreach (ArrayList a in pages)
            if (counter == currentpage)
                data = a;
            else
                counter++;

        xcounter = 0;
        ycounter = 0;

            while (xcounter < data.Count && ycounter < pageend)
            {
                if (currentRowLength < maxRowlength)
                {
                    currentRowLength += data[xcounter].ToString().Length;
                    GameObject newText = Instantiate(word);
                    TextMesh temp = newText.GetComponent(typeof(TextMesh)) as TextMesh;
                    BoxCollider boxCollider = newText.GetComponent(typeof(BoxCollider)) as BoxCollider;
                    temp.text = data[xcounter].ToString();
                    temp.characterSize = 25;
                    print(temp.text);
                    newText.AddComponent<BoxCollider>();
                    newText.name = temp.text;
                    //newText.text = string.Format("Item {0}", counter.ToString());
                    newText.transform.parent = currentRow.transform;
                }
                else
                {
                    currentRowLength = 0;
                    var newrow = Instantiate(row) as GameObject;
                    ycounter++;
                    //newText.text = string.Format("Item {0}", counter.ToString());
                    currentRow = newrow;
                    currentRow.transform.parent = page.transform;
                }
                xcounter++;
            }

        if (currentpage + 1 < numPages)
            currentpage++;
        else
            currentpage = 0;
    }
}
