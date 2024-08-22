using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;


public class Api : MonoBehaviour
{
    public static System.Random random;
    CrosswordGenerator generator;
    TreeHolder tree;

    [SerializeField] List<string> words;
    int level;

    [SerializeField] Text state;
    [SerializeField] Text crossword;
    [SerializeField] InputField input;

    CrosswordInfo cross;

    void Start()
    {
        //SpaceToEnter();
        //LoadWords();


        //Clear();

        //Tree();

        //Crossword(1);
    }

    public void Tree()
    {
        tree = new TreeHolder();
        tree.LoadTree(words);

        state.text = "Trees are loaded!";
    }

    public void Crossword()
    {
        level = int.Parse(input.text);

        generator = new CrosswordGenerator();

        random = new System.Random();
        cross = generator.GenerateCrossword(level, tree, words);

        DebugCrossword(cross);

        state.text = "Crossword are Generated";
    }

    public void Clear()
    {
        for (int i = words.Count - 1; i >= 0; i--)
        {
            if (words[i].Length > 8)
            {
                words.RemoveAt(i);
            }
            else if (words[i].IndexOf('-') != -1)
            {
                words.RemoveAt(i);
            }
            else if (words[i].Length < 3)
                words.RemoveAt(i);
        }

        StreamWriter sw = new StreamWriter("Assets/Resources/Words.txt");
        foreach (string word in words)
        {
            sw.WriteLine(word);
        }
        sw.Close();

        state.text = words.Count + " Words";
    }

    static string[] Quicksort(string[] s)
    {
        if (s.Length < 2) { return s; }
        int p = s[0].Length;
        return Quicksort(s.Where(x => x.Length > p).ToArray())
            .Concat(s.Where(x => x.Length == p))
            .Concat(Quicksort(s.Where(x => x.Length < p).ToArray()))
            .ToArray();
    }

    public void LoadWords()
    {
        words = new List<string>();
        string[] readedLines = new string[4] {"муха", "хам", "ум","уха" };//File.ReadAllLines("/Users/bd/Tree/Assets/Resources/Words.txt");

        

        readedLines = Quicksort(readedLines);

        foreach(string lin in readedLines)
        {
            if (!words.Exists(x => x == lin))
                words.Add(lin);
        }

        if(words.Count<readedLines.Length)
        {
            StreamWriter sw = new StreamWriter("Assets/Resources/Words.txt");

            foreach(string word in words)
            {
                sw.WriteLine(word);
            }
            Debug.Log("removed " + (readedLines.Length - words.Count) + " words");
            sw.Close();
        }


        state.text = "Loaded " + words.Count + " words";
    }

    void DebugCrossword(CrosswordInfo cross)
    {

        string s = "";
        for (int i = 0; i < cross.crossword.GetLength(1); i++)
        {
            s += '\n';
            for (int j = 0; j < cross.crossword.GetLength(0); j++)
            {
                s += cross.crossword[j, i] + " ";
            }

        }
        crossword.text = s;
        Debug.Log("Crossword=>"+s);
    }
    public void SaveCrossInFile()
    {
        string SavingCross = "INSERT INTO crosswords (main_word, letters, body, level) VALUES ('1', '[";
        for(int i = 0; i< cross.baseWord.Length; i++)
        {
            SavingCross += '\"' ;
            SavingCross += cross.baseWord[i];
            SavingCross += '\"';
            if (i < cross.baseWord.Length - 1) SavingCross += ',';
        }
        SavingCross += "]', '";


        for (int i = 0; i < cross.crossword.GetLength(1); i++)
        {
            SavingCross += '[';
            for (int j = 0; j < cross.crossword.GetLength(0); j++)
            {
                SavingCross += '\"';
                if (cross.crossword[j, i] != '#')
                    SavingCross += cross.crossword[j, i];

                SavingCross += '\"';
                if (j < cross.crossword.GetLength(0) - 1)
                    SavingCross += ",";
            }

            SavingCross += ']';
            if (i < cross.crossword.GetLength(1) - 1)
                SavingCross += ",\r\n";
            else
                SavingCross += "', ";

        }
        SavingCross += cross.levelNum.ToString();
        SavingCross += ");";

        state.text = SavingCross;

        StreamWriter sw = new StreamWriter("Assets/Resources/Crosses/cross" + cross.levelNum + ".txt");
        sw.Write(SavingCross);
        sw.Close();
    }

    void SpaceToEnter()
    {
        StreamReader sr = new StreamReader("Assets/Resources/Words.txt");
        string s = sr.ReadLine();

        while(s.IndexOf(' ')>-1)
            s = s.Replace(' ', '\n');
        sr.Close();

        StreamWriter sw = new StreamWriter("Assets/Resources/Words.txt");

        sw.Write(s);
        sw.Close();
    }
}
