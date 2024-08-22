using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeUnit
{
    public int wordSize;
    public int childsCount;
    public string word;
    public List<TreeUnit> childs;

    public TreeUnit(string word)
    {
        wordSize = word.Length;

        this.word = word;
        childs = new List<TreeUnit>();
    }

    public List<TreeUnit> GetAllChilds()
    {
        List<TreeUnit> result = new List<TreeUnit>();

        for (int i = 0; i < childs.Count; i++)
        {
            var newChilds = childs[i].GetAllChilds();
            result.AddRange(newChilds);

        }
        result.Add(this);

        return result;
    }

    protected static bool CompareWords(string baseWord, string comparedWord)
    {

        for (int i = 0; i < comparedWord.Length; i++)
        {
            int isFind = -1;
            for (int j = 0; j < baseWord.Length; j++)
            {
                if (baseWord[j] == comparedWord[i])
                {
                    isFind = j;
                    break;
                }
            }
            if (isFind >= 0)
            {
                baseWord = baseWord.Remove(isFind, 1);
            }
            else
                return false;
        }

        return true;
    }

    public bool TryPlaceWord(TreeUnit word)
    {
        bool result = false;


        if(word == this)
        {
            return true;
        }
        if (CompareWords(this.word, word.word))
        {
            result = true;
            bool isPlaced = false;

            int childsCount = childs.Count;

            for (int i = 0; i < childsCount; i++)
            {
                if (childs[i].TryPlaceWord(word))
                {
                    isPlaced = true;
                }
            }
            if (!isPlaced)
            {

                for (int i = childs.Count - 1; i >= 0; i--)
                {
                    if (CompareWords(word.word, childs[i].word))
                    {
                        word.childs.Add(childs[i]);
                        childs.RemoveAt(i);
                    }
                }

                childs.Add(word);

            }
        }

        return result;
    }
}

public class TreeHolder 
{
    List<string> words = new List<string>() { "муха", "хам", "ум", "уха" };

    List<TreeUnit> treeRoot = new List<TreeUnit>();

    System.Random random = new System.Random();

    protected void PlaceWord(TreeUnit word)
    {
        bool isPlaced = false;
        for(int i = 0; i< treeRoot.Count; i++)
        {
            if(treeRoot[i].TryPlaceWord(word))
            {
                isPlaced = true;
            }
        }

        if(!isPlaced)
        {
            for (int i = treeRoot.Count-1; i >= 0; i--)
            {
                if (word.TryPlaceWord(treeRoot[i]))
                    treeRoot.RemoveAt(i);
            }
            treeRoot.Add(word);
        }

    }

    public void LoadTree(List<string> words)
    {

        for (int i = 0; i < words.Count; i++)
        {
            PlaceWord(new TreeUnit(words[i]));
        }
        Debug.Log(words.Count + "  " + treeRoot.Count);
        
    }
    public List<string> GetWords(int maxLength, int wordsCount)
    {
        List<string> result = new List<string>();

        random = Api.random;
        
        /*TreeUnit enabledRoot = null;
        do
        {
            result = new List<string>();
            int id = random.Next(0, treeRoot.Count);

            List<TreeUnit> enabledWords = null;

            if (treeRoot[id].wordSize >= maxLength)
            {
                enabledRoot = treeRoot[id];

                while (enabledRoot.wordSize > maxLength && enabledRoot != null) 
                {
                    int nextId = random.Next(0, enabledRoot.childs.Count);
                    if (enabledRoot.childs.Exists(x => x.wordSize >= maxLength))
                    {
                        if (enabledRoot.childs[nextId].wordSize >= maxLength)
                        {
                            enabledRoot = enabledRoot.childs[nextId];
                        }
                    }
                    else
                    {
                        enabledRoot = null;
                        break;
                    }
                } 

                if(enabledRoot!=null)
                    enabledWords = enabledRoot.GetAllChilds();
            }
            else
                enabledRoot = null;


            if (enabledRoot != null)
            {
                for (int i = 0; i < enabledWords.Count; i++)
                {
                    if(!result.Exists(x=> x == enabledWords[i].word))
                    {
                        if(enabledWords[i].word.Length<=maxLength)
                        {
                            result.Add(enabledWords[i].word);
                        }
                    }
                }
            }
        } while (result.Count<wordsCount );

        for (int i = 0; i < result.Count; i++) Debug.Log(result[i]);

        while(result.Count>wordsCount)
        {
            int id = random.Next(0, result.Count);

            if (result[id].Length < maxLength)
                result.RemoveAt(id);
        }

        */


        return words;
    }
}
