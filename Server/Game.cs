using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public delegate void WriteMessage(string message);
    public delegate string GetDataFromPlayer();
    class Game
    {
        public WriteMessage writemessagePlayer1;
        public WriteMessage writemessagePlayer2;
        public GetDataFromPlayer getdatafromplayer1;
        public GetDataFromPlayer getdatafromplayer2;
        byte[,] board; // игровая доска
        const byte maxorunds = 3;
        byte currentround;
        string player1 { get; set; }
        string player2 { get; set; }
        int player1score { get; set; }
        int player2score { get; set; }
        bool roundIsOver;
        byte winner; // 0 - никто, 1 - первый, 2 - второй.
        byte nowPlaying; // чей ход? 0 - никто, 1 - первый, 2 - второй.

        public Game()
        {
            board = new byte[3, 3];
            
        }

        public void Go() // основной цикл игры
        {
            currentround = 0;

            writemessagePlayer1("Введите имя 1 игрока");
            writemessagePlayer2("1 игрок вводит имя, подождите...");
            player1 = getdatafromplayer1();
            writemessagePlayer2("Введите имя 2 игрока");
            writemessagePlayer1("2 игрок вводит имя, подождите...");
            player2 = getdatafromplayer2();

            while (currentround<=maxorunds)
            {
                newRound();
                while (!roundIsOver)
                {
                    DrawBoard();
                    nextStep();
                    AnalyzeBoard();
                    if (winner != 0)
                    {
                        if (winner == 1)
                        {
                            writemessageboth(string.Format("Победил {0}!", player1));
                            player1score++;
                        }
                        if (winner == 2)
                        {
                            writemessageboth(string.Format("Победил {0}!", player2));
                            player2score++;
                        }
                        if (winner == 3) { writemessageboth(string.Format("Ничья!")); }
                        roundIsOver = true;
                    }
                }
            }
            if (player1score > player2score)
                writemessageboth(string.Format("По результатам {0} матчей победил {1}!", maxorunds, player1));
            if (player1score < player2score)
                writemessageboth(string.Format("По результатам {0} матчей победил {1}!", maxorunds, player2));
            if (player1score < player2score)
                writemessageboth(string.Format("По результатам {0} матчей получилась ничья ^_^!", maxorunds));
            writemessageboth(string.Format("Спасибо за игру"));
        }

        private void nextStep()
        {
            
            if (nowPlaying == 1) writemessageboth(string.Format("Ход игрока {0}", player1));
            if (nowPlaying == 2) writemessageboth(string.Format("Ход игрока {0}", player2));
            string response = "";
            if (nowPlaying == 1) response = getdatafromplayer1();
            if (nowPlaying == 2) response = getdatafromplayer2();
            while (!AnalyzeAnswer(response))
            {
                if (nowPlaying == 1)
                {
                    writemessagePlayer1(
                        "Ответ пользователя неправильного формата или ходить в данное поле нельзя. Введите ответ заново");
                    response = getdatafromplayer1();
                }
                if (nowPlaying == 2)
                {
                    writemessagePlayer2(
                        "Ответ пользователя неправильного формата или ходить в данное поле нельзя. Введите ответ заново");
                    response = getdatafromplayer2();
                }
                
            }
            byte nextPlaying = 0;
            if (nowPlaying == 1) nextPlaying = 2;
            if (nowPlaying == 2) nextPlaying = 1;
            nowPlaying = nextPlaying;
        }

        private void writemessageboth(string message)
        {
            writemessagePlayer1(message);
            writemessagePlayer2(message);
        }

        public void newRound()
        {
            currentround++;
            winner = 0;
            roundIsOver = false;
            writemessageboth(string.Format("Начало {0} раунда!", currentround));
            writemessageboth(string.Format("{0} - {1}, {2}- {3}", player1, player1score, player2, player2score));

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++) board[i, j] = 0;
            }// обнуляем доску в начале раунда
            roundIsOver = false;
            nowPlaying = 1;
        }

        private bool AnalyzeAnswer(string response) // анализ ответа true - всё нормально, false - ответ непонятен
        {
            // формат ответа : pxy
            // p - номер игрока (1,2)
            // x - номер по горизонтали, куда ставится элемент (0-2)
            // x - номер по вертикали, куда ставится элемент (0-2)
            if (response.Length != 3) return false; // если в ответе не 3 элемента, то явно ответ неверен
            char p = response.ToCharArray()[0];
            char x = response.ToCharArray()[1];
            char y = response.ToCharArray()[2];
            if (p != '1' && p != '2') return false; // проверка номера игрока
            if (p == '1' && nowPlaying == 2 || p == '2' && nowPlaying == 1) return false; // проверка, играет ли игрок в свой ход 
            if (x != '0' && x != '1' && x != '2') return false;
            if (y != '0' && y != '1' && y != '2') return false;
            int x_ = byte.Parse(x.ToString());
            int y_ = byte.Parse(y.ToString());
            if (board[x_, y_] != 0) return false;
            board[x_, y_] = byte.Parse(p.ToString()); // если всё верно, помещаем элемент на доску

            return true;
        }

        private void AnalyzeBoard() // анализ доски на предмет выигрыша одной из сторон
        {
            //проверка по строкам
            for (int j = 0; j <= 2; j++)
            {
                if (board[0,j]==board[1,j]&&board[0,j]==board[2,j] && board[0,j]!=0)
                {
                    winner = board[0, j]; 
                    return;
                }
            }

            //проверка по столбцам
            for (int i = 0; i <= 2; i++)
            {
                if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i,0]!=0)
                {
                    winner = board[i, 0];
                    return;
                }
            }
            
            //проверка по главной диагонали
            if (board[0,0] == board[1,1] && board[1,1] == board[2,2] && board[0,0]!=0)
            {
                winner = board[0, 0];
                return;
            }

            //проверка по побочной диагонали
            if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[0,2]!=0)
            {
                winner = board[0, 2];
                return;
            }

           
            // смотрим, нет ли ничьей
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (board[i, j] == 0)
                    {
                        winner = 0;
                        return ;
                    }
                }
            }
            winner = 3; return; 
        }

        

        private void DrawBoard()
        {
            string boardImage="";
            // |x| |x|
            // |x|o|o|
            // |o|x| |
            //  
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++) 
                {
                    boardImage += "|";
                    if (board[i, j] == 0) boardImage += " ";
                    if (board[i, j] == 1) boardImage += "X";
                    if (board[i, j] == 2) boardImage += "O";
                    if (j == 2) boardImage += "|\r\n";
                }
            }
            writemessageboth(boardImage);
        }


    }
}
