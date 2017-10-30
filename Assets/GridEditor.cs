using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;
using Tobii.Gaming;
using System.Collections.Generic;

public class GridEditor : MonoBehaviour
{

    private int xcounter = 0;
    private int ycounter = 0;
    private int pagestart = 0;
    private int pageend = 8;
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
    private static int maxRowlength;
    private static int maxLines = 5;
    private int numPages;
    private GazePoint pos;
    private GameObject focusedObject;

    // Use this for initialization
    void Start()
    {
        words = new ArrayList();
        pages = new List<ArrayList>();
        maxRowlength = 120;
        pos = TobiiAPI.GetGazePoint();
        try
        {
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr = new StreamReader("C:\\Users\\mc\\IdeaProjects\\ReadingButler\\src\\main\\resources\\ReadingButlerExerciseTemplates\\data.txt"))
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

            foreach (String s in words)
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
        AddItemToGrid(0);
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
            new System.IO.StreamWriter(@"C:\Users\Djidjelly Siclait\Desktop\HelloWorld.txt", true))
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

    public void clearpage(int direction)
    {
        print("direction: " + direction);
        GameObject parentObj = GameObject.Find("Page");
        int childs = parentObj.transform.childCount;
        print("page has " + childs + "childs");
        foreach (Transform child in page.transform)
        {

            print(child.name);
            Destroy(child.gameObject);

        }

        if (direction > 0)
        {
            print("foward");
            AddItemToGrid(direction);
        }
        else
        {
            print("back");
            AddItemToGrid(direction);
            //GoBack(); 
        }

    }

    public void AddItemToGrid(int direction)
    {

        GameObject currentRow = firstrow;

        ArrayList data = new ArrayList();
        currentpage += direction;

        if (currentpage  >= numPages)
            currentpage = 0;
        else if(currentpage < 0)
            currentpage = numPages - 1;

        data = pages[currentpage];

        print(data.Count);
        xcounter = 0;
        ycounter = 0;

        while (xcounter < data.Count)
        {
            //if (currentRowLength < maxRowlength)
            //{
                //se suma a la fila la longitud de la palabra que se agregara
                //currentRowLength += data[xcounter].ToString().Length;
                
            //se crea un objeto palabra
            GameObject newText = Instantiate(word);
            //se obtiene el text mesh de la palabra
            TextMesh temp = newText.GetComponent(typeof(TextMesh)) as TextMesh;
            //BoxCollider boxCollider = newText.GetComponent(typeof(BoxCollider)) as BoxCollider;

            //se le asina el texto al objeto que sera la palabra con color negro y tamaño de caracter 25
            temp.text = data[xcounter].ToString();
            temp.color = Color.black;
            temp.characterSize = 20;

            //se le añade un componente de colision para el funcionamiento con la herramienta tobii
            newText.AddComponent<BoxCollider>();

            //se pone la palabra como nombre del objeto
            newText.name = temp.text;

            //se añade la palabra a la cuadricula
            newText.transform.parent = currentRow.transform;

                //se intenta recalcular la distribucion de las palabras en la fila (horizontal layout)
                //RectTransform T = currentRow.GetComponent(typeof(RectTransform)) as RectTransform;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(T);

                //se le da nombre a la cuadricula 
                currentRow.name = "page # " + currentpage;

                //se crea un objeto word para el espacio en blanco se extrae el componente para el texto y se le asigna un componente para colision
                GameObject white = Instantiate(word);
                TextMesh temp2 = white.GetComponent(typeof(TextMesh)) as TextMesh;
                white.AddComponent<BoxCollider>();

                //se pone guion como texto para visulizar los efectos del espacio en blanco
                temp2.text = " ";
                temp2.color = Color.black;
                temp2.characterSize = 25;

                //se nombra y asigna a la fila                
                white.name = "espacio en blanco";
                white.transform.parent = currentRow.transform;

                //se intenta recalcular la distribucion de las palabras en la fila (horizontal layout)
                //T = currentRow.GetComponent(typeof(RectTransform)) as RectTransform;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(T);
                //T = page.GetComponent(typeof(RectTransform)) as RectTransform;
                //LayoutRebuilder.ForceRebuildLayoutImmediate(T);
                //Canvas.ForceUpdateCanvases();

            //}
            //else
            //{
                //currentRowLength = 0;
                //var newrow = Instantiate(row) as GameObject;
                //ycounter++;
                //newText.text = string.Format("Item {0}", counter.ToString());
                //currentRow = newrow;
                //currentRow.transform.parent = page.transform;
                //currentRow.name = "row # " + ycounter;
            //}
            xcounter++;
        }
        
    }

    public void addword()
    {
        GameObject currentRow = firstrow;
        if (currentRowLength < maxRowlength)
        {
            //se suma a la fila la longitud de la palabra que se agregara
            currentRowLength += "coño".Length;
            //se crea un objeto palabra
            GameObject newText = Instantiate(word);
            //se obtiene el text mesh de la palabra
            TextMesh temp = newText.GetComponent(typeof(TextMesh)) as TextMesh;
            //BoxCollider boxCollider = newText.GetComponent(typeof(BoxCollider)) as BoxCollider;

            //se le asina el texto al objeto que sera la palabra con color negro y tamaño de caracter 25
            temp.text = "word";
            temp.color = Color.black;
            temp.characterSize = 25;

            //se le añade un componente de colision para el funcionamiento con la herramienta tobii
            newText.AddComponent<BoxCollider>();

            //se pone la palabra como nombre del objeto
            newText.name = temp.text;

            //se añade la palabra a la fila
            newText.transform.parent = currentRow.transform;

            //se intenta recalcular la distribucion de las palabras en la fila (horizontal layout)
            RectTransform T = currentRow.GetComponent(typeof(RectTransform)) as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(T);

            //se le da nombre a la fila 
            currentRow.name = "row # " + ycounter;

            //se crea un objeto word para el espacio en blanco se extrae el componente para el texto y se le asigna un componente para colision
            GameObject white = Instantiate(word);
            TextMesh temp2 = white.GetComponent(typeof(TextMesh)) as TextMesh;
            white.AddComponent<BoxCollider>();

            //se pone guion como texto para visulizar los efectos del espacio en blanco
            temp2.text = "-";
            temp2.color = Color.black;
            temp2.characterSize = 25;

            //se nombra y asigna a la fila                
            white.name = "espacio en blanco";
            white.transform.parent = currentRow.transform;

            //se intenta recalcular la distribucion de las palabras en la fila (horizontal layout)
            T = currentRow.GetComponent(typeof(RectTransform)) as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(T);
            T = page.GetComponent(typeof(RectTransform)) as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(T);
            Canvas.ForceUpdateCanvases();

        }
        else
        {
            currentRowLength = 0;
            var newrow = Instantiate(row) as GameObject;
            ycounter++;
            //newText.text = string.Format("Item {0}", counter.ToString());
            currentRow = newrow;
            currentRow.transform.parent = page.transform;
            currentRow.name = "row # " + ycounter;
        }
    }

    private void init()
    {
        firstrow = Instantiate(grid) as GameObject;
        firstrow.transform.parent = page.transform;
        
    }
}
