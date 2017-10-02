using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using Tobii.Gaming;
public class GridEditor : MonoBehaviour {

    private int xcounter = 0;
    private int ycounter = 0;
    private int pagestart = 0;
    private int pageend= 10;
    private int currentRowLength = 0;
    public GameObject page;
    public GameObject row;
    public GameObject word;
    private ArrayList words;
    private static int maxRowlength = 80;
    // Use this for initialization
    void Start () {
        words = new ArrayList();
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
        }
        catch (Exception e)
        {

            // Let the user know what went wrong.
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
        var firstrow = Instantiate(row) as GameObject;
        //newText.text = string.Format("Item {0}", counter.ToString());
        firstrow.transform.parent = page.transform;
        AddItemToGrid(firstrow);
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

    public void AddItemToGrid(GameObject firstRow)
    {
        GameObject currentRow = firstRow;
        while (xcounter < words.Count && ycounter < pageend) {
            if (currentRowLength < maxRowlength)
            {
                currentRowLength += words[xcounter].ToString().Length;
                GameObject newText = Instantiate(word);
                TextMesh temp = newText.GetComponent(typeof(TextMesh)) as TextMesh;
                BoxCollider boxCollider = newText.GetComponent(typeof(BoxCollider)) as BoxCollider;
                temp.text = words[xcounter].ToString();
                temp.characterSize = 25;
                print(temp.text);
                newText.AddComponent<BoxCollider>();
                newText.name = temp.text;
                //newText.text = string.Format("Item {0}", counter.ToString());
                newText.transform.parent = currentRow.transform;
            }
            else {
                currentRowLength = 0;
                var newrow = Instantiate(row) as GameObject;
                ycounter++;
                //newText.text = string.Format("Item {0}", counter.ToString());
                currentRow = newrow;
                currentRow.transform.parent = page.transform;
            }
            xcounter++;
        }
       
    }
}
