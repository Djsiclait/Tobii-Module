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
    private static int maxRowlength = 20;
    private static int maxLines = 5;
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

            numPages = (int)Math.Ceiling((decimal)words.Count / (decimal)(maxRowlength * maxLines));
            print(words.Count);
            print(numPages);

            int j = 0;
            int counter = 1;
            ArrayList buffer = new ArrayList();

            foreach(String s in words)
            {
                if (counter < numPages) // first and next pages
                {
                    if (j < (maxRowlength * maxLines) - 1)
                    {
                        buffer.Add(s);
                        j++;
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
                else // last page
                {
                    buffer.Add(s);
                }
            }
            
            print("buffer = " + buffer.Count);
            pages.Add(buffer);

            print("Pages =" + pages.Count);
            currentpage = 0;

        }
        catch (Exception e)
        {

            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }

        init();
        AddItemToGrid();
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

    public void clearpage(int direction)
    {
        print("direction" + direction);
        GameObject parentObj = GameObject.Find("Page");
        int childs = parentObj.transform.childCount;
        print("page has " + childs + "childs");
        foreach (Transform childTransform in parentObj.transform)
        {
            print(""+childTransform.name);
            if (childTransform.name == "row # 0")
            {
                print("first row");
                foreach (Transform childTransform2 in childTransform.transform)
                {
                    print("" + childTransform2.name);
                    childTransform2.parent = null;
                    Destroy(childTransform2.gameObject);
                }
            }
            else
            {
                Destroy(childTransform.gameObject);
            }
            //}
        }
        /*
        int childs = page.transform.childCount;
        print("page has " + childs + "childs");
        for (int i = childs - 1; i > 0; i--)
        {
            print("erasing page "+page.transform.GetChild(i).name);
            page.transform.GetChild(i).SetParent(null);

        }*/
        if (direction > 0)
        {
            AddItemToGrid();
        }
        else { GoBack(); }
    }

    public void GoBack()
    {
        AddItemToGrid();
    }

    public void AddItemToGrid()
    {


        GameObject currentRow = firstrow;

        ArrayList data = new ArrayList();

        data = pages[currentpage];

        print(data.Count);
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
                if (data[xcounter].ToString().Length < 5)
                    temp.text = "__" + data[xcounter].ToString() + "_";
                else
                {
                    temp.text = data[xcounter].ToString();
                }
                    temp.characterSize = 25;
                    //print(temp.text);
                    newText.AddComponent<BoxCollider>();
                    newText.name = temp.text;
                    //newText.text = string.Format("Item {0}", counter.ToString());
                    newText.transform.parent = currentRow.transform;
                currentRow.name = "row # " + ycounter;
            }
                else
                {
                    currentRowLength = 0;
                    var newrow = Instantiate(row) as GameObject;
                    ycounter++;
                    //newText.text = string.Format("Item {0}", counter.ToString());
                    currentRow = newrow;
                    currentRow.transform.parent = page.transform;
                currentRow.name = "row # "+ycounter;
                }
                xcounter++;
            }

        if (currentpage + 1 < numPages)
            currentpage++;
        else
            currentpage = 0;
    }

    private void init()
    {
        firstrow = Instantiate(row) as GameObject;
        //newText.text = string.Format("Item {0}", counter.ToString());
        firstrow.transform.parent = page.transform;
        //AddItemToGrid(firstrow);
        nextPage.onClick.AddListener(AddItemToGrid);
        //AddItemToGrid();
    }
}
