using System.Collections;
using System.Collections.Generic;
using System;

public class CrosswordInfo
{
    public int levelNum;
    public string baseWord;
    public char[,] crossword;
}
struct Vector2Int
{
    public int x;
    public int y;

    public static Vector2Int zero
    {
        get
        {
            return new Vector2Int(0, 0);
        }
    }

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y= y;
    }
    public static Vector2Int operator +(Vector2Int vect1, Vector2Int vect2)
    {
        return new Vector2Int(vect1.x + vect2.x, vect1.y + vect2.y);
    }
    public static Vector2Int operator -(Vector2Int vect1, Vector2Int vect2)
    {
        return new Vector2Int(vect1.x - vect2.x, vect1.y - vect2.y);
    }
}

class Letter
{
    public char sign;
    public int signWeight;

    public Letter(char sign)
    {
        this.sign = sign;
        signWeight = 1;
    }
}
class LetterState
{
    public Letter letter;
    public bool isActive = true;

    public LetterState(Letter letter)
    {
        this.letter = letter;
    }

    public void DisactiveLetter()
    {
        if (isActive)
        {
            isActive = false;
            letter.signWeight--;
        }
    }
}
class CrossWord
{
    public List<LetterState> letters;

    public Vector2Int position;
    public bool isHorizontal;

    public List<LetterState> ConnectableLetters
    {
        get
        {
            List<LetterState> letters = new List<LetterState>();
            foreach(LetterState let in this.letters)
            {
                if(let.isActive)
                    letters.Add(let);
            }
            return letters;
        }
    }

    public CrossWord()
    {
        letters = new List<LetterState>();
    }
    public void AddLetter(Letter letter)
    {
        letters.Add(new LetterState(letter));
    }
    public int GetWeight()
    {
        int wordWeight = 0;
        foreach (LetterState let in letters)
        {
            if(let.isActive)
                wordWeight += let.letter.signWeight;
        }
        return wordWeight;
    }
    public Vector2Int GetLetterPosition(LetterState let)
    {
        for(int i = 0; i< letters.Count; i++)
        {
            if(let == letters[i])
            {
                if (isHorizontal) return position + new Vector2Int(i, 0);
                else return position + new Vector2Int(0,i);
            }
        }
        return Vector2Int.zero;
    }
    public void DisactiveLetter(int letterNum)
    {
        letters[letterNum].DisactiveLetter();
        if(letterNum>0) letters[letterNum-1].DisactiveLetter();
        if (letterNum < letters.Count-1) letters[letterNum + 1].DisactiveLetter();

        
    }
}

public class CrosswordGenerator
{
     Random random;

    List<char[,]> crosswordVariants;
    char[,] cross;
    int size;

    int wordsCount;
    int maxWordsLength;

    List<CrossWord> usedWords;
    List<CrossWord> words;

    
    

    void SetDifficult(int levelNum)
    {
        /*if(levelNum<=2)
        {
            wordsCount = 2;
            maxWordsLength = 3;
        }
        else if(levelNum<=4)
        {
            wordsCount = 3;
            maxWordsLength = 4;
        }
        else if(levelNum<=7)
        {
            wordsCount = 4;
            maxWordsLength = 4;
        }
        else
        {
            int controlParam = levelNum - 8;

            maxWordsLength = 4 + controlParam % 4;
            wordsCount = 4 + controlParam / 4 + (maxWordsLength)/5;
        }

        if (wordsCount > 8) wordsCount = 8;
        if (maxWordsLength > 8) maxWordsLength = 8;*/


        //wordsCount = 5;
        //maxWordsLength = 8;

    }

    public CrosswordInfo GenerateCrossword(int levelNum, TreeHolder tree, List<string> ST = null)
    {
        random = Api.random;//������ ��� ������

        crosswordVariants = new List<char[,]>();//�������������� ���� �����������


        SetDifficult(levelNum);//������ ��������� - ���������� ���� � ������������ ����� �����

        List<string> rawWords = ST;// tree.GetWords(maxWordsLength, wordsCount); //�������� ����������� ����� ��� ���������� � ���������� ����

        maxWordsLength = rawWords[0].Length;
        wordsCount = rawWords.Count;

        CrosswordInfo info = new CrosswordInfo();
        info.levelNum = levelNum;
        info.baseWord = "";

        List<char> baseWord = new List<char>();

        for(int i = 0; i< rawWords.Count; i++)
        {
            List<char> bufer = new List<char>();
            for (int j = 0; j< rawWords[i].Length; j++)
            {
                if(baseWord.Exists(x => x == rawWords[i][j]))
                {
                    bufer.Add(rawWords[i][j]);
                    baseWord.Remove(rawWords[i][j]);
                }
                else
                {
                    bufer.Add(rawWords[i][j]);
                }
            }

            baseWord.AddRange(bufer);
        }

        for (int i = 0; i < baseWord.Count; i++)
            info.baseWord += baseWord[i];

        UnityEngine.Debug.Log(info.baseWord);

        int contrVar = 0;

        while(crosswordVariants.Count<5)//���� �������� �� ��� ���, ���� �� �� ������� 5 ������� ��������� ����������
        {
            contrVar++;
            if (contrVar > 150)
                break;
            //���� ������������� string � CrossWord-----------------------------------------

            List<Letter> letters = new List<Letter>();//�������������� ���� ����.
            words = new List<CrossWord>();//�������������� ���� ����������������� ����
            usedWords = new List<CrossWord>();//�������������� ���� ��������������� ����

            foreach (string word in rawWords)//�������� �� ���� ������ � �����
            {
                CrossWord currentWord = new CrossWord();//������� � ��������� ����� �����
                words.Add(currentWord);

                for (int i = 0; i < word.Length; i++)//�������� �� ������� �������
                {
                    if (!letters.Exists(x => x.sign == word[i]))//���� ����� ����� ��� ��� � �����  ����
                    {
                        letters.Add(new Letter(word[i]));//������� ��� �����
                    }
                    else
                    {
                        letters.Find(x => x.sign == word[i]).signWeight++;//���� ����� ���� - ����������� ��� ���� �����
                    }
                    currentWord.AddLetter(letters.Find(x => x.sign == word[i]));//��������� ����� ����� � �����
                }
            }

            //����� �����----------------------------------------------------------------------

            //���� ��������� ����������--------------------------------------------------------------


            size =  maxWordsLength * (int)(wordsCount / 2);//������ ��������� ����������� ������ �������

            cross = new char[size, size];//������� ������ ��� �������� ����������
            ClearCrossword();//��������� ��� �������� # (�������)

            bool isHorizontal;//���������� ������� ���������� �������������� �� �����, � ����������� ����� ������� �����

            if (random.Next(0, 2) == 0) isHorizontal = true;
            else isHorizontal = false;                          //������ ��������� ����������� ��� ������� �����

            CrossWord firstWord = GetMaxWeightWord(); //�������� ����� � ���������� �����
            FindWordPlace(isHorizontal, firstWord);//��������� ������ �����
            usedWords.Add(firstWord);//��������� ��� � ���� �������������� ����
            words.Remove(firstWord);//������� �� ����� ���������������� ����

            bool isActuallyGenerated = true;//����������� ����������. ���� �� ��������� ���� ��� ����� true - ��������� ������ �������.
                                            //����� - ��������� �����������
            do
            {
                isHorizontal = !isHorizontal;//����������� ����������

                CrossWord current = GetMaxWeightWord();//�������� ��������� ����� � ������������ �����

                if (!FindWordPlace(isHorizontal, current))
                {
                    isActuallyGenerated = false;//�������� ��� ����������. ���� �� ����� - ������ ����������� ���������� �� false
                    break; //���������� ���� � ������ �������
                }

                words.Remove(current);//���������� ����� �� ����������������
                usedWords.Add(current);//� ��������������

            } while (words.Count > 0);//���� ����������� ����� �� ��������� ���������������� ����


            if (isActuallyGenerated)//���� ��������� �������
            {
                CutCrossword();//�������� ���������

                crosswordVariants.Add(cross);//��������� ��������� � ������ ��������� �����������
            }
            //����� �����-----------------------------------------------------------------------------------
        }
        if (contrVar > 100)
            UnityEngine.Debug.Log("cantGenerate");

        UnityEngine.Debug.Log(crosswordVariants.Count);
        char[,] finalCrossword = GetBestCorssword(); // ����� ���������� ����������. ������ �� ���������� ����������� ��� ������ levelNum

        UnityEngine.Debug.Log(finalCrossword);

        info.crossword = finalCrossword;


        return info;

    }

    private bool FindWordPlace(bool isHorizontal, CrossWord word)
    {
        int countOfAttempts = 0;//������� ���������� ������� ���������� �����
        if (usedWords.Count == 0)//���� �� ��������� ������ ����� � �����
        {
            if (isHorizontal)
            {
                TryPlaceWord(isHorizontal, new Vector2Int(random.Next(0, size - word.letters.Count), random.Next(0, size)), word);//��������� ������ �������������� ����� �� ������
            }
            else
            {
                TryPlaceWord(isHorizontal, new Vector2Int(random.Next(0, size), random.Next(0, size - word.letters.Count)), word);//��������� ������ ������������ ����� �� ������
            }
        }
        else//���� ��� �� ������ ����� � � �����
        {
            if (random.Next(0, 2) == 0) isHorizontal = !isHorizontal;
            bool isWordPlaced = false;//����������� ����������. ���� ����� ���� ��������� ��� ����� ����� true, ����� - false
            do
            {
                countOfAttempts++;//������� ���������� �������

                CrossWord connectedWord = null;//����� � ������� �� ����� ���������� ���� �����
                List<LetterState> connectableLetters = new List<LetterState>();//�����, ��������� ��� �������� � ��� �����, ������� �� ���������

                int countOfSecondAttempts = 0;
                do
                {
                    countOfSecondAttempts++;
                    connectedWord = usedWords[random.Next(0, usedWords.Count)];//����� ��������� ����� �� ��� ��� ��� ���� � �����


                    if (connectedWord.isHorizontal != isHorizontal)//���� � ������������ ����� � ������ ������ ����������
                    {
                        connectableLetters = connectedWord.ConnectableLetters;//�������� ��� ��������� ����� �� ������������ �����

                        for (int i = connectableLetters.Count - 1; i >= 0; i--)
                        {
                            if (!(word.letters.Exists(x => x.letter == connectableLetters[i].letter)))
                                connectableLetters.RemoveAt(i);//������ ��� ����� ������� ��� � ����� �����
                        }
                    }
                    else connectedWord = null;//���� ���������� ���������� �������� ����������� �����

                    if (connectableLetters.Count == 0) connectedWord = null;//���� ����� ������ ���� ������� ��� � ����� ����� � ��� �������� ���� ���� - �������� �����
                    if (countOfSecondAttempts == 50) break;

                } while (connectedWord == null);//���� ������������ ���� � ����� �� �� ��������� ���������� ����������� �����



                if (countOfSecondAttempts < 50)
                {
                    countOfSecondAttempts = 0;
                    do
                    {
                        countOfSecondAttempts++;
                        LetterState connectingLetter = connectableLetters[random.Next(0, connectableLetters.Count)];//���������� ����� � ������� ����� ������������ ���� �����

                        for (int i = 0; i < word.letters.Count; i++)//�������� ��� ����� � ����� �����
                        {
                            if (connectingLetter.letter == word.letters[i].letter)//���� ����� ��������� � ���, � ������� �� ������������
                            {
                                Vector2Int position = connectedWord.GetLetterPosition(connectingLetter);//�������� ������� �����, � ������� �� �������� ������������ �����
                                if (isHorizontal)
                                {
                                    position -= new Vector2Int(i, 0);//��������� ������ ������ ��������������� �����
                                }
                                else
                                {
                                    position -= new Vector2Int(0, i);//��������� ������� ������ ������������� �����
                                }

                                isWordPlaced = TryPlaceWord(isHorizontal, position, word);//������� ���������� ���� ����� � ������� position

                                if (isWordPlaced)//���� ���������� ������������ �����
                                {
                                    connectingLetter.DisactiveLetter();//������ �����, � ������� �� ������������ ����������
                                    word.letters[i].DisactiveLetter();//������ �����, ������� �� ����������� ����������
                                    break;//������� �� �����
                                }
                            }
                        }
                        if (!isWordPlaced)//���� ������������ ���� ����� � ���� ����� �� �������...
                        {
                            connectableLetters.Remove(connectingLetter);//�� ������� �� �� ������ ���������� ����
                        }
                        if (countOfSecondAttempts == 50) break;
                    }
                    while (!isWordPlaced && connectableLetters.Count > 0);//���� ������������ �� ��� ���, ���� ����� �� ����� ��������� ��� �� ����� ����������
                                                                          //��� ���������� ����� ����������. �.�. ���������� ���������� ���� ������ ����� ����
                }
                if (countOfAttempts == 50)//���� ���������� ������� ����� 50
                    isHorizontal = !isHorizontal;//�������������� ����������

            } while (!isWordPlaced && countOfAttempts <= 100);//���� ������������ �� ��� ���, ���� ����� �� ����� ���������, ��� ��������� ������� �� ��������� 100
        }
        if (countOfAttempts >= 100) return false;//���� �� �� ������ ���������� ����� �� 100 ������� ���������� false. ��� ������ ��� ���� ��������� ������� �� ����������
        else return true;//���� ���������� ����� ���������� ���������� true
    }

    bool TryPlaceWord(bool isHorizontal, Vector2Int position, CrossWord word)
    {
        bool isPlaced = true;//����������� ��������� ������� ������ �� ���, ���������� �� � ��� ����������� �����
                             //� �������� �����

        Vector2Int pos = position;//������ ����� ������� ��� �������� ����������� ����������� ��� ����� � ���������


        if (isHorizontal)//�������� ��� ��������������� �����
        {
            if (pos.x > 0)
            {
                if (cross[pos.x-1, pos.y] != '#')//���� ����� �� ��������������� ����� ���� �����
                {
                    isPlaced = false;//����������� ����������� ����������
                    return isPlaced;
                }
            }
            if(pos.x+word.letters.Count<size-1)
            {
                if (cross[pos.x + word.letters.Count, pos.y] != '#')//���� ������ �� ��������������� ����� ���� �����
                {
                    isPlaced = false;
                    return isPlaced;
                }
            }


            for (int i = 0; i < word.letters.Count; i++)//���������� ���������, �������� �� ����� ��������������� ����� � ������������� ����������
            {
                if (pos.x == size || pos.x<0)//�������� ����� �� ������ �������� �� ������� ����������
                {
                    isPlaced = false;
                    break;
                }
                if (!(cross[pos.x, pos.y] == '#' || cross[pos.x, pos.y] == word.letters[i].letter.sign))//�������� ����� � ������� ������ ���� ������, ��� ������ ���� ������ ��� �� ������
                {
                    isPlaced = false;
                    break;
                }
                if(cross[pos.x, pos.y]=='#')//�������� ���� ������ ���-���� �������� ������...
                {
                    if(pos.y>0)
                    {
                        if(cross[pos.x, pos.y - 1] !='#')//...������ �� ���� �� ������ ���� ������� ��������
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                    if (pos.y < size-1)
                    {
                        if (cross[pos.x, pos.y + 1] != '#')//...����� �� ���� �� ������ ���� ������� ��������
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                }

                pos.x++;//���������� ������� � ���������� ������� ��������������� �����
            }
        }
        else//�������� ��� ������������� �����
        {
            if (pos.y > 0)
            {
                if (cross[pos.x,pos.y - 1] != '#')//���� ������ �� ������������� ����� ���� �����
                {
                    isPlaced = false;
                    return isPlaced;
                }
            }
            if (pos.y + word.letters.Count < size - 1)
            {
                if (cross[pos.x,pos.y + word.letters.Count] != '#')//���� ����� �� ������������� ����� ���� �����
                {
                    isPlaced = false;
                    return isPlaced;
                }
            }


            for (int i = 0; i < word.letters.Count; i++)//���������� ���������, �������� �� ����� ������������� ����� � ������������� ����������
            {
                if (pos.y == size || pos.y<0)//�������� ����� �� ������ �������� �� ������� ����������
                {
                    isPlaced = false;
                    break;
                }
                if (!(cross[pos.x, pos.y] == '#' || cross[pos.x, pos.y] == word.letters[i].letter.sign))//�������� ����� � ������� ������ ���� ������, ��� ������ ���� ������ ��� �� ������
                {
                    isPlaced = false;
                    break;
                }
                if (cross[pos.x, pos.y] == '#')//�������� ���� ������ ���-���� �������� ������...
                {
                    if (pos.x > 0)
                    {
                        if (cross[pos.x - 1, pos.y] != '#')//...����� �� ���� �� ������ ���� ����
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                    if (pos.x< size - 1)
                    {
                        if (cross[pos.x + 1, pos.y] != '#')//...������ �� ���� �� ������ ���� ����
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                }

                pos.y++;//���������� ������� � ���������� ������� ������������� �����
            }
        }

        if (isPlaced)//���� ����� �� ������ ��� ���������� ����� � ��� �������
        {
            word.isHorizontal = isHorizontal;//���������� ��� �����������
            word.position = position;//���������� ������� ������ ����� �����
            if (isHorizontal)
            {
                for (int i = 0; i < word.letters.Count; i++)//��������� � ��������� �������������� �����
                {
                    cross[word.position.x + i, word.position.y] = word.letters[i].letter.sign;
                }
            }
            else
            {
                for (int i = 0; i < word.letters.Count; i++)//��������� � ��������� ������������ �����
                {
                    cross[word.position.x, word.position.y + i] = word.letters[i].letter.sign;
                }
            }
        }

        return isPlaced;//���������� ��������� ������� ����������
    }


    private CrossWord GetMaxWeightWord()//�������� ����� � ���������� �����.
    {
        CrossWord maxWord = words[0];

        foreach (CrossWord word in words)
        {
            if (word.GetWeight() > maxWord.GetWeight())
            {
                maxWord = word;
            }
        }
        return maxWord;

    }

    void CutCrossword()
    {
        char[,] cuttedCross;


        Vector2Int startPos = new Vector2Int(200,200);
        Vector2Int endPos = new Vector2Int(-200,-200);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if(cross[x,y]!='#')
                {
                    if (startPos.x > x) startPos.x = x;
                    if (startPos.y > y) startPos.y = y;

                    if (endPos.x < x) endPos.x = x;
                    if (endPos.y < y) endPos.y = y;
                }
            }

        }
        cuttedCross = new char[endPos.x-startPos.x+1, endPos.y - startPos.y+1];

        for (int y = 0; y < cuttedCross.GetLength(1); y++)
        {
            for (int x = 0; x < cuttedCross.GetLength(0); x++)
            {
                cuttedCross[x, y] = cross[startPos.x + x, startPos.y + y];
            }

        }
        cross = cuttedCross;
    }

    void ClearCrossword()//������� ���������
    {
        cross = new char[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                cross[i,j] = '#';
            }

        }
    }

    /*void DebugCrossword()
    {

        string s = "Crossword=>";
        for (int i = 0; i < cross.GetLength(1); i++)
        {
            s += '\n';
            for (int j = 0; j < cross.GetLength(0); j++)
            {
                s += cross[j,i] + " ";
            }

        }
        Debug.Log(s);
    }*/

    public char[,] GetBestCorssword()
    {
        //���������� ����������� ��������
        //��������� ������

        float[] crosswordWeights= new float[crosswordVariants.Count];

        int minWeightId = -1;

        for(int i = 0; i< crosswordVariants.Count; i++)
        {
            if (crosswordVariants[i].GetLength(1) > crosswordVariants[i].GetLength(0))
                crosswordWeights[i] = crosswordVariants[i].GetLength(0) / crosswordVariants[i].GetLength(1);
            else
                crosswordWeights[i] = crosswordVariants[i].GetLength(1) / crosswordVariants[i].GetLength(0);

             crosswordWeights[i] *= (size / crosswordVariants[i].GetLength(0)) * (size / crosswordVariants[i].GetLength(1));

            if (minWeightId < 0) minWeightId = 0;
            else if (crosswordWeights[minWeightId] < crosswordWeights[i])
                minWeightId = i;
        }
            return crosswordVariants[minWeightId];
    }

}
