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
        random = Api.random;//задаем сид уровня

        crosswordVariants = new List<char[,]>();//инициализируем лист кроссвордов


        SetDifficult(levelNum);//задаем сложность - количество слов и максимальный рамер слова

        List<string> rawWords = ST;// tree.GetWords(maxWordsLength, wordsCount); //получаем необходимые слова для кроссворда в простейшем виде

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

        while(crosswordVariants.Count<5)//цикл работает до тех пор, пока мы не получем 5 рабочих вариантов кроссворда
        {
            contrVar++;
            if (contrVar > 150)
                break;
            //блок трансформации string в CrossWord-----------------------------------------

            List<Letter> letters = new List<Letter>();//инициализируем лист букв.
            words = new List<CrossWord>();//инициализируем лист незадействованных слов
            usedWords = new List<CrossWord>();//инициализируем лист задействованных слов

            foreach (string word in rawWords)//проходим по всем словам в листе
            {
                CrossWord currentWord = new CrossWord();//создаем и добавляем новое слово
                words.Add(currentWord);

                for (int i = 0; i < word.Length; i++)//проходим по каждому символу
                {
                    if (!letters.Exists(x => x.sign == word[i]))//если такой буквы еще нет в листе  букв
                    {
                        letters.Add(new Letter(word[i]));//создаем эту букву
                    }
                    else
                    {
                        letters.Find(x => x.sign == word[i]).signWeight++;//если буква есть - увеличиваем вес этой буквы
                    }
                    currentWord.AddLetter(letters.Find(x => x.sign == word[i]));//добавляем новую букву в слово
                }
            }

            //конец блока----------------------------------------------------------------------

            //блок генерации кроссворда--------------------------------------------------------------


            size =  maxWordsLength * (int)(wordsCount / 2);//задаем намеренно увеличенный размер массива

            cross = new char[size, size];//создаем массив для текущего кроссворда
            ClearCrossword();//заполняем его символом # (очищаем)

            bool isHorizontal;//переменная которая определяет горизонтальное ли слово, и переключает после каждого слова

            if (random.Next(0, 2) == 0) isHorizontal = true;
            else isHorizontal = false;                          //задаем случайное направление для первого слова

            CrossWord firstWord = GetMaxWeightWord(); //получаем слово с наибольшим весом
            FindWordPlace(isHorizontal, firstWord);//размещаем первое слово
            usedWords.Add(firstWord);//добавляем его в лист использованных слов
            words.Remove(firstWord);//убираем из листа неиспользованных слов

            bool isActuallyGenerated = true;//контрольная переменная. если по окончанию слов она равна true - генерация прошла успешно.
                                            //иначе - генерация провалилась
            do
            {
                isHorizontal = !isHorizontal;//переключаем ориентацию

                CrossWord current = GetMaxWeightWord();//получаем следующее слово с максимальным весом

                if (!FindWordPlace(isHorizontal, current))
                {
                    isActuallyGenerated = false;//пытаемся его разместить. если не вышло - ставим контрольную переменную на false
                    break; //прерываеем цикл в случае неудачи
                }

                words.Remove(current);//перемещаем слово из неиспользованных
                usedWords.Add(current);//в использованные

            } while (words.Count > 0);//цикл завершается когда не останется неиспользованныз слов


            if (isActuallyGenerated)//если генерация удалась
            {
                CutCrossword();//обрезаем кроссворд

                crosswordVariants.Add(cross);//добавляем кроссворд в список вариантов кроссвордов
            }
            //конец блока-----------------------------------------------------------------------------------
        }
        if (contrVar > 100)
            UnityEngine.Debug.Log("cantGenerate");

        UnityEngine.Debug.Log(crosswordVariants.Count);
        char[,] finalCrossword = GetBestCorssword(); // выбор наилучшего кроссворда. Именно он становится кроссвордом для уровня levelNum

        UnityEngine.Debug.Log(finalCrossword);

        info.crossword = finalCrossword;


        return info;

    }

    private bool FindWordPlace(bool isHorizontal, CrossWord word)
    {
        int countOfAttempts = 0;//считаем количество попыток разместить слова
        if (usedWords.Count == 0)//если мы вставляем первое слово в сетку
        {
            if (isHorizontal)
            {
                TryPlaceWord(isHorizontal, new Vector2Int(random.Next(0, size - word.letters.Count), random.Next(0, size)), word);//размещаем первое горизонтальное слово по центру
            }
            else
            {
                TryPlaceWord(isHorizontal, new Vector2Int(random.Next(0, size), random.Next(0, size - word.letters.Count)), word);//размещаем первое вертикальное слово по центру
            }
        }
        else//если это не первое слово в в сетке
        {
            if (random.Next(0, 2) == 0) isHorizontal = !isHorizontal;
            bool isWordPlaced = false;//контрольная переменная. если слово было размещено она будет равна true, иначе - false
            do
            {
                countOfAttempts++;//считаем количество попыток

                CrossWord connectedWord = null;//слово с которым мы будем коннектить наше слово
                List<LetterState> connectableLetters = new List<LetterState>();//буквы, доступные для коннекта в том слове, которое мы коннектим

                int countOfSecondAttempts = 0;
                do
                {
                    countOfSecondAttempts++;
                    connectedWord = usedWords[random.Next(0, usedWords.Count)];//берем случайное слово из тех что уже есть в сетке


                    if (connectedWord.isHorizontal != isHorizontal)//если у соединяемого слова и нашего разные ориентации
                    {
                        connectableLetters = connectedWord.ConnectableLetters;//получаем все доступные буквы из соединяемого слова

                        for (int i = connectableLetters.Count - 1; i >= 0; i--)
                        {
                            if (!(word.letters.Exists(x => x.letter == connectableLetters[i].letter)))
                                connectableLetters.RemoveAt(i);//чистим все буквы которых нет в нашем слове
                        }
                    }
                    else connectedWord = null;//если ориентации одинаковые обнуляем соединяемое слово

                    if (connectableLetters.Count == 0) connectedWord = null;//если после чистки букв которых нет в нашем слове у нас осталось ноль букв - обнуляем слово
                    if (countOfSecondAttempts == 50) break;

                } while (connectedWord == null);//цикл продолжается пока в итоге мы не определим подходящее соединяемое слово



                if (countOfSecondAttempts < 50)
                {
                    countOfSecondAttempts = 0;
                    do
                    {
                        countOfSecondAttempts++;
                        LetterState connectingLetter = connectableLetters[random.Next(0, connectableLetters.Count)];//определяем букву к которой хотим присоединить наше слово

                        for (int i = 0; i < word.letters.Count; i++)//проходим все буквы в нашем слове
                        {
                            if (connectingLetter.letter == word.letters[i].letter)//если буква совпадает с той, к которой мы присоединяем
                            {
                                Vector2Int position = connectedWord.GetLetterPosition(connectingLetter);//получаем позицию буквы, к которой мы пытаемся присоединить слово
                                if (isHorizontal)
                                {
                                    position -= new Vector2Int(i, 0);//вычисляем начало нашего горизонтального слова
                                }
                                else
                                {
                                    position -= new Vector2Int(0, i);//вычисляем позицию нашего вертикального слова
                                }

                                isWordPlaced = TryPlaceWord(isHorizontal, position, word);//пробуем резместить наше слово в позицию position

                                if (isWordPlaced)//если получилось присоединить слово
                                {
                                    connectingLetter.DisactiveLetter();//делаем букву, к которой мы присоединили неактивной
                                    word.letters[i].DisactiveLetter();//делаем букву, которую мы присодинили неактивной
                                    break;//выходим из цикла
                                }
                            }
                        }
                        if (!isWordPlaced)//если присоединить наше слово к этой букве не удалось...
                        {
                            connectableLetters.Remove(connectingLetter);//мы удаляем ее из набора подходящих букв
                        }
                        if (countOfSecondAttempts == 50) break;
                    }
                    while (!isWordPlaced && connectableLetters.Count > 0);//цикл продолжается до тех пор, пока слово не будет размещено или не будет определено
                                                                          //что разместить слово невозможно. т.е. количество подходящих букв станет равно нулю
                }
                if (countOfAttempts == 50)//если количество попыток равно 50
                    isHorizontal = !isHorizontal;//переворачиваем ориентацию

            } while (!isWordPlaced && countOfAttempts <= 100);//цикл продолжается до тех пор, пока слово не будет размещено, или количство попыток не достигнет 100
        }
        if (countOfAttempts >= 100) return false;//если мы не смогли разместить слово за 100 попыток возвращаем false. что значит что этот кроссворд собрать не получилось
        else return true;//если разместить слово получилось возвращаем true
    }

    bool TryPlaceWord(bool isHorizontal, Vector2Int position, CrossWord word)
    {
        bool isPlaced = true;//контрольная перменная которая следит за тем, получилось ли у нас расположить слово
                             //в заданное место

        Vector2Int pos = position;//делаем копию позиции для проверки возможности расположить все буквы в кроссворд


        if (isHorizontal)//ПРОВЕРКИ ДЛЯ ГОРИЗОНТАЛЬНОГО СЛОВА
        {
            if (pos.x > 0)
            {
                if (cross[pos.x-1, pos.y] != '#')//если слева от горизонтального слова есть буква
                {
                    isPlaced = false;//переключаем контрольную переменную
                    return isPlaced;
                }
            }
            if(pos.x+word.letters.Count<size-1)
            {
                if (cross[pos.x + word.letters.Count, pos.y] != '#')//если справа от горизонтального слова есть буква
                {
                    isPlaced = false;
                    return isPlaced;
                }
            }


            for (int i = 0; i < word.letters.Count; i++)//поочередно примеряем, подходят ли буквы горизонтального слова к существующему кроссворду
            {
                if (pos.x == size || pos.x<0)//ПРОВЕРКА буква не должна выходить за границы кроссворда
                {
                    isPlaced = false;
                    break;
                }
                if (!(cross[pos.x, pos.y] == '#' || cross[pos.x, pos.y] == word.letters[i].letter.sign))//ПРОВЕРКА место в массиве должно быть пустым, или должно быть занято той же буквой
                {
                    isPlaced = false;
                    break;
                }
                if(cross[pos.x, pos.y]=='#')//ПРОВЕРКА если символ все-таки является пустым...
                {
                    if(pos.y>0)
                    {
                        if(cross[pos.x, pos.y - 1] !='#')//...сверху от него не должно быть никаких символов
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                    if (pos.y < size-1)
                    {
                        if (cross[pos.x, pos.y + 1] != '#')//...снизу от него не должно быть никаких символов
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                }

                pos.x++;//перемещаем позицию к следующему символу горизонтального слова
            }
        }
        else//ПРОВЕРКИ ДЛЯ ВЕРТИКАЛЬНОГО СЛОВА
        {
            if (pos.y > 0)
            {
                if (cross[pos.x,pos.y - 1] != '#')//если сверху от вертикального слова есть буква
                {
                    isPlaced = false;
                    return isPlaced;
                }
            }
            if (pos.y + word.letters.Count < size - 1)
            {
                if (cross[pos.x,pos.y + word.letters.Count] != '#')//если снизу от вертикального слова есть буква
                {
                    isPlaced = false;
                    return isPlaced;
                }
            }


            for (int i = 0; i < word.letters.Count; i++)//поочередно примеряем, подходят ли буквы вертикального слова к существующему кроссворду
            {
                if (pos.y == size || pos.y<0)//ПРОВЕРКА буква не должна выходить за границы кроссворда
                {
                    isPlaced = false;
                    break;
                }
                if (!(cross[pos.x, pos.y] == '#' || cross[pos.x, pos.y] == word.letters[i].letter.sign))//ПРОВЕРКА место в массиве должно быть пустым, или должно быть занято той же буквой
                {
                    isPlaced = false;
                    break;
                }
                if (cross[pos.x, pos.y] == '#')//ПРОВЕРКА если символ все-таки является пустым...
                {
                    if (pos.x > 0)
                    {
                        if (cross[pos.x - 1, pos.y] != '#')//...слева от него не должно быть букв
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                    if (pos.x< size - 1)
                    {
                        if (cross[pos.x + 1, pos.y] != '#')//...справа от него не должно быть букв
                        {
                            isPlaced = false;
                            break;
                        }
                    }
                }

                pos.y++;//перемещаем позицию к следующему символу вертикального слова
            }
        }

        if (isPlaced)//если ничто не мешает нам разместить слово в эту позицию
        {
            word.isHorizontal = isHorizontal;//запоминаем его направление
            word.position = position;//запоминаем позицию первой буквы слова
            if (isHorizontal)
            {
                for (int i = 0; i < word.letters.Count; i++)//вставляем в кроссворд горизонтальное слово
                {
                    cross[word.position.x + i, word.position.y] = word.letters[i].letter.sign;
                }
            }
            else
            {
                for (int i = 0; i < word.letters.Count; i++)//вставляем в кроссворд вертикальное слово
                {
                    cross[word.position.x, word.position.y + i] = word.letters[i].letter.sign;
                }
            }
        }

        return isPlaced;//возвращаем результат попытки размещения
    }


    private CrossWord GetMaxWeightWord()//получаем слово с наибольшим весом.
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

    void ClearCrossword()//очищаем кроссворд
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
        //коэфициент соотношения размеров
        //суммарный размер

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
