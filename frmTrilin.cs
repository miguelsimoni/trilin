using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Trilin
{
    public partial class frmTrilin : Form
    {
        #region Language

        private string ST_GO = "Go!";
        private string ST_GAME_OVER = "Game Over";
        private string ST_PLAYER_WIN = "Wins!";
        private string ST_PLAY_AGAIN = "Play again!";
        private string ST_LAST_PLAY = "Last play";
        private string ST_IS_PLAYING = "is playing";

        private enum GameLanguage
        {
            Spanish,
            English
        }

        private GameLanguage language;

        private GameLanguage Language
        {
            get { return language; }
            set { language = value; }
        }

        private void LoadLanguage()
        {
            if(File.Exists(Application.ExecutablePath + ".config"))
            {
                if(ConfigurationManager.AppSettings["Language"] == GameLanguage.Spanish.ToString())
                    Language = GameLanguage.Spanish;
                else if(ConfigurationManager.AppSettings["Language"] == GameLanguage.English.ToString())
                    Language = GameLanguage.English;
                else
                    Language = GameLanguage.Spanish;
            }
            else
            {
                Language = GameLanguage.Spanish;
            }
            mitLangSP.Checked = Language == GameLanguage.Spanish;
            mitLangEN.Checked = Language == GameLanguage.English;
            SetLanguage();
        }

        private void SetLanguage()
        {
            switch(Language)
            {
                case GameLanguage.Spanish:
                    tsbStart.Text = "Inicio";
                    tsbStart.ToolTipText = "Inicia una partida nueva";
                    ddbMode.Text = "Modo";
                    ddbMode.ToolTipText = "Selecciona el modo de juego";
                    mitPlayer1.Text = "Jugador 1";
                    mitPlayer2.Text = "Jugador 2";
                    mitVersus.Text = "Versus";
                    ddbLevel.Text = "Nivel";
                    ddbLevel.ToolTipText = "Selecciona el nivel de dificultad";
                    mitEasy.Text = "Tonto";
                    mitMedium.Text = "Lógico";
                    mitHard.Text = "I.A.";
                    ddbOptions.Text = "Opciones";
                    ddbOptions.ToolTipText = "Opciones";
                    mitPlayerImage1.Text = "Imagen Jugador 1";
                    mitPlayerImage2.Text = "Imagen Jugador 2";
                    mitLanguage.Text = "Idioma";
                    ddbAbout.Text = "Acerca de";
                    ddbAbout.ToolTipText = "Acerca de " + Application.ProductName;
                    mitWebsite.Text = "Visita xmsim website";
                    mitWebsite.ToolTipText = "Ir a http://xmsim.net.googlepages.com/";
                    mitAbout.Text = "Acerca de " + Application.ProductName;
                    mitAbout.ToolTipText = "Acerca de " + Application.ProductName;
                    ST_GO = "A jugar!";
                    ST_GAME_OVER = "Fin del Juego";
                    ST_PLAYER_WIN = "Ganó!";
                    ST_PLAY_AGAIN = "Juega de nuevo!";
                    ST_LAST_PLAY = "Última jugada";
                    ST_IS_PLAYING = "está jugando";
                    break;
                case GameLanguage.English:
                    tsbStart.Text = "Start";
                    tsbStart.ToolTipText = "Start new game";
                    ddbMode.Text = "Mode";
                    ddbMode.ToolTipText = "Select game mode";
                    mitPlayer1.Text = "Player 1";
                    mitPlayer2.Text = "Player 2";
                    mitVersus.Text = "Versus";
                    ddbLevel.Text = "Level";
                    ddbLevel.ToolTipText = "Select difficulty level";
                    mitEasy.Text = "Dummy";
                    mitMedium.Text = "Logic";
                    mitHard.Text = "A.I.";
                    ddbOptions.Text = "Options";
                    ddbOptions.ToolTipText = "Options";
                    mitPlayerImage1.Text = "Player Image 1";
                    mitPlayerImage2.Text = "Player Image 2";
                    mitLanguage.Text = "Language";
                    ddbAbout.Text = "About";
                    ddbAbout.ToolTipText = "About...";
                    mitWebsite.Text = "Visit xmsim website";
                    mitWebsite.ToolTipText = "Go to http://xmsim.net.googlepages.com/";
                    mitAbout.Text = "About " + Application.ProductName;
                    mitAbout.ToolTipText = "About " + Application.ProductName;
                    ST_GO = "Go!";
                    ST_GAME_OVER = "Game Over";
                    ST_PLAYER_WIN = "Wins!";
                    ST_PLAY_AGAIN = "Play again!";
                    ST_LAST_PLAY = "Last play";
                    ST_IS_PLAYING = "is playing";
                    break;
            }
        }

        private void mitLanguage_Click(object sender, EventArgs e)
        {
            bool changeLanguage = true;
            if(Turn > 0)
            {
                string msg;
                if(Language == GameLanguage.Spanish)
                    msg = "Cambiar el idioma reiniciará el juego." + Environment.NewLine + "¿Está seguro de que desea cambiar el idioma?";
                else
                    msg = "Change language cause restart the game." + Environment.NewLine + "Are you sure you want to change the language?";
                if(MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    changeLanguage = true;
                else
                    changeLanguage = false;
            }
            if(changeLanguage)
            {
                mitLangSP.Checked = false;
                mitLangEN.Checked = false;
                ToolStripMenuItem mit = (ToolStripMenuItem)sender;
                mit.Checked = true;
                if(mitLangSP.Checked)
                {
                    Language = GameLanguage.Spanish;
                    UpdateConfig("Language", GameLanguage.Spanish.ToString());
                }
                if(mitLangEN.Checked)
                {
                    Language = GameLanguage.English;
                    UpdateConfig("Language", GameLanguage.English.ToString());
                }
                SetLanguage();
                StartGame();
            }
        }

        #endregion //Language

        #region Game Mode
        private enum GameModes
        {
            FirstPlayer = 0,
            SecondPlayer = 1,
            Versus = 2,
        }

        private GameModes mode;

        private GameModes Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private void mitMode_Click(object sender, EventArgs e)
        {
            mitPlayer1.Checked = false;
            mitPlayer2.Checked = false;
            mitVersus.Checked = false;
            ToolStripMenuItem mit = (ToolStripMenuItem)sender;
            mit.Checked = true;
            if(mitPlayer1.Checked)
                Mode = GameModes.FirstPlayer;
            if(mitPlayer2.Checked)
                Mode = GameModes.SecondPlayer;
            if(mitVersus.Checked)
                Mode = GameModes.Versus;
            ddbLevel.Enabled = !mitVersus.Checked;
            StartGame();
        }
        #endregion //Game Mode

        #region Game Level 
        private enum GameLevels
        {
            Easy = 0,
            Medium = 1,
            Hard = 2
        }

        private GameLevels level;

        private GameLevels Level
        {
            get { return level; }
            set { level = value; }
        }

        private void mitLevel_Click(object sender, EventArgs e)
        {
            mitEasy.Checked = false;
            mitMedium.Checked = false;
            mitHard.Checked = false;
            ToolStripMenuItem mit = (ToolStripMenuItem)sender;
            mit.Checked = true;
            if(mitEasy.Checked)
                Level = GameLevels.Easy;
            if(mitMedium.Checked)
                Level = GameLevels.Medium;
            if(mitHard.Checked)
                Level = GameLevels.Hard;
            StartGame();
        }
        #endregion //Game Level

        #region Board 
        public class CBoard
        {
            private int[,] cells;
            public int[,] Cells
            {
                get { return cells; }
            }

            public CBoard(int rows, int columns)
            {
                cells = new int[rows, columns];
            }

            public int this[int x, int y]
            {
                get { return cells[x, y]; }
                set { cells[x, y] = value; }
            }
        }

        private void ClearCells()
        {
            PictureBox picCell;
            Control[] ctrl;
            for(int x = 0; x < 3; x++)
                for(int y = 0; y < 3; y++)
                {
                    Board[x, y] = NoPlayer;
                    ctrl = Controls.Find("picCell" + x.ToString() + y.ToString(), true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.Image = null;
                    picCell.Cursor = Cursors.Hand;
                    picCell.BackColor = Color.White;
                }
        }

        private void BlockCells()
        {
            PictureBox picCell;
            Control[] ctrl;
            for(int x = 0; x < 3; x++)
                for(int y = 0; y < 3; y++)
                {
                    ctrl = Controls.Find("picCell" + x.ToString() + y.ToString(), true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.Cursor = Cursors.No;
                }
        }

        private void UnblockCells()
        {
            PictureBox picCell;
            Control[] ctrl;
            for(int x = 0 ; x < 3 ; x++)
                for(int y = 0 ; y < 3 ; y++)
                {
                    ctrl = Controls.Find("picCell" + x.ToString() + y.ToString(), true);
                    picCell = (PictureBox)ctrl[0];
                    if(Board[x, y] == NoPlayer)
                        picCell.Cursor = Cursors.Hand;
                    else
                        picCell.Cursor = Cursors.No;
                }
        }

        private CBoard Board;

        #endregion //Board

        #region Players 
        public const int NoPlayer = -1;

        private const int PlayerA = 0;
        private Image PlayerImageA = null;

        private const int PlayerB = 1;
        private Image PlayerImageB = null;
        
        private void LoadPlayerImages()
        {
            //menu player image 1
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Star", imageListIcon.Images["star"], mitPlayerImage1_Click, "star"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Heart", imageListIcon.Images["heart"], mitPlayerImage1_Click, "heart"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Cross", imageListIcon.Images["cross"], mitPlayerImage1_Click, "cross"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("X", imageListIcon.Images["x"], mitPlayerImage1_Click, "x"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Flash", imageListIcon.Images["flash"], mitPlayerImage1_Click, "flash"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Lifebelt", imageListIcon.Images["lifebelt"], mitPlayerImage1_Click, "lifebelt"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Lightbulb", imageListIcon.Images["lightbulb"], mitPlayerImage1_Click, "lightbulb"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Lightbulb On", imageListIcon.Images["lightbulb_on"], mitPlayerImage1_Click, "lightbulb_on"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Music", imageListIcon.Images["music"], mitPlayerImage1_Click, "music"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Star Grey", imageListIcon.Images["star_grey"], mitPlayerImage1_Click, "star_grey"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Square Blue", imageListIcon.Images["square_blue"], mitPlayerImage1_Click, "square_blue"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Square Red", imageListIcon.Images["square_red"], mitPlayerImage1_Click, "square_red"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Windows", imageListIcon.Images["win"], mitPlayerImage1_Click, "win"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Linux", imageListIcon.Images["linux"], mitPlayerImage1_Click, "linux"));
            mitPlayerImage1.DropDownItems.Add(new ToolStripMenuItem("Mac", imageListIcon.Images["mac"], mitPlayerImage1_Click, "mac"));
            //menu player image 2
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Star", imageListIcon.Images["star"], mitPlayerImage2_Click, "star"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Heart", imageListIcon.Images["heart"], mitPlayerImage2_Click, "heart"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Cross", imageListIcon.Images["cross"], mitPlayerImage2_Click, "cross"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("X", imageListIcon.Images["x"], mitPlayerImage2_Click, "x"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Flash", imageListIcon.Images["flash"], mitPlayerImage2_Click, "flash"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Lifebelt", imageListIcon.Images["lifebelt"], mitPlayerImage2_Click, "lifebelt"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Lightbulb", imageListIcon.Images["lightbulb"], mitPlayerImage2_Click, "lightbulb"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Lightbulb On", imageListIcon.Images["lightbulb_on"], mitPlayerImage2_Click, "lightbulb_on"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Music", imageListIcon.Images["music"], mitPlayerImage2_Click, "music"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Star Grey", imageListIcon.Images["star_grey"], mitPlayerImage2_Click, "star_grey"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Square Blue", imageListIcon.Images["square_blue"], mitPlayerImage2_Click, "square_blue"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Square Red", imageListIcon.Images["square_red"], mitPlayerImage2_Click, "square_red"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Windows", imageListIcon.Images["win"], mitPlayerImage2_Click, "win"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Linux", imageListIcon.Images["linux"], mitPlayerImage2_Click, "linux"));
            mitPlayerImage2.DropDownItems.Add(new ToolStripMenuItem("Mac", imageListIcon.Images["mac"], mitPlayerImage2_Click, "mac"));
            //set player images
            if(File.Exists(Application.ExecutablePath + ".config"))
            {
                SetPlayerImage(PlayerA, ConfigurationManager.AppSettings["PlayerImage1"]);
                SetPlayerImage(PlayerB, ConfigurationManager.AppSettings["PlayerImage2"]);
            }
            else
            {
                SetPlayerImage(PlayerA, "star");
                SetPlayerImage(PlayerB, "heart");
            }
        }

        private void SetPlayerImage(int player, string imageName)
        {
            if(player == PlayerA)
            {
                ((ToolStripMenuItem)mitPlayerImage1.DropDownItems[imageName]).Checked = true;
                PlayerImageA = imageList.Images[imageName];
                mitPlayer1.Image = imageListIcon.Images[imageName];
                //disable the same image in the player 2 list
                mitPlayerImage2.DropDownItems[imageName].Enabled = false;
                for(int i = 0 ; i < mitPlayerImage2.DropDownItems.Count - 1 ; i++)
                    if(mitPlayerImage2.DropDownItems[i].Name != imageName)
                        mitPlayerImage2.DropDownItems[i].Enabled = true;
            }
            else
            {
                ((ToolStripMenuItem)mitPlayerImage2.DropDownItems[imageName]).Checked = true;
                PlayerImageB = imageList.Images[imageName];
                mitPlayer2.Image = imageListIcon.Images[imageName];
                mitPlayerImage1.DropDownItems[imageName].Enabled = false;
                //disable the same image in the player 1 list
                for(int i = 0 ; i < mitPlayerImage1.DropDownItems.Count - 1 ; i++)
                    if(mitPlayerImage1.DropDownItems[i].Name != imageName)
                        ((ToolStripMenuItem)mitPlayerImage1.DropDownItems[i]).Enabled = true;
            }
        }

        private void mitPlayerImage1_Click(object sender, EventArgs e)
        {
            for(int i = 0 ; i < mitPlayerImage1.DropDownItems.Count - 1 ; i++)
                ((ToolStripMenuItem)mitPlayerImage1.DropDownItems[i]).Checked = false;
            ToolStripMenuItem mit = (ToolStripMenuItem)sender;
            SetPlayerImage(PlayerA, mit.Name);
            if(File.Exists(Application.ExecutablePath + ".config"))
                UpdateConfig("PlayerImage1", mit.Name);
            StartGame();
        }

        private void mitPlayerImage2_Click(object sender, EventArgs e)
        {
            for(int i = 0 ; i < mitPlayerImage2.DropDownItems.Count - 1 ; i++)
                ((ToolStripMenuItem)mitPlayerImage2.DropDownItems[i]).Checked = false;
            ToolStripMenuItem mit = (ToolStripMenuItem)sender;
            SetPlayerImage(PlayerB, mit.Name);
            if(File.Exists(Application.ExecutablePath + ".config"))
                UpdateConfig("PlayerImage2", mit.Name);
            StartGame();
        }

        private bool PlayerWin(int player)
        {
            if(Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
            {
                picCell00.BackColor = Color.LightYellow;
                picCell11.BackColor = Color.LightYellow;
                picCell22.BackColor = Color.LightYellow;
                return true;
            }
            if(Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
            {
                picCell02.BackColor = Color.LightYellow;
                picCell11.BackColor = Color.LightYellow;
                picCell20.BackColor = Color.LightYellow;
                return true;
            }
            for(int i = 0; i < 3; i++)
            {
                if(Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                {
                    PictureBox picCell;
                    Control[] ctrl;
                    ctrl = Controls.Find("picCell" + i.ToString() + "0", true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.BackColor = Color.LightYellow;
                    ctrl = Controls.Find("picCell" + i.ToString() + "1", true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.BackColor = Color.LightYellow;
                    ctrl = Controls.Find("picCell" + i.ToString() + "2", true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.BackColor = Color.LightYellow;
                    return true;
                }
                if(Board[0, i] == player && Board[1, i] == player && Board[2, i] == player)
                {
                    PictureBox picCell;
                    Control[] ctrl;
                    ctrl = Controls.Find("picCell0" + i.ToString(), true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.BackColor = Color.LightYellow;
                    ctrl = Controls.Find("picCell1" + i.ToString(), true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.BackColor = Color.LightYellow;
                    ctrl = Controls.Find("picCell2" + i.ToString(), true);
                    picCell = (PictureBox)ctrl[0];
                    picCell.BackColor = Color.LightYellow;
                    return true;
                }
            }
            return false;
        }
        #endregion //Players

        public frmTrilin()
        {
            InitializeComponent();
            LoadLanguage();
            LoadPlayerImages();
            try
            {
                webBrowser.Navigate(@"http://usuarios.lycos.es/rigelsaint/adsense_trilin.htm#adsensetrilin");
            }
            catch(Exception)
            {
            }
        }

        private void frmTrilin_Load(object sender, EventArgs e)
        {
            Board = new CBoard(3, 3);
            if(mitEasy.Checked)
                Level = GameLevels.Easy;
            if(mitMedium.Checked)
                Level = GameLevels.Medium;
            if(mitHard.Checked)
                Level = GameLevels.Hard;
            StartGame();
        }


        #region Game 

        private void tsbStart_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            ClearCells();
            Turn = 0;
            stlStatus.Image = imageListIcon.Images["about"];
            stlStatus.Text = ST_GO;
            stlTurn.Image = mitPlayer1.Image;
            stlTurn.Text = ST_IS_PLAYING;
            progressBar.Style = ProgressBarStyle.Marquee;
            time = new TimeSpan(0, 0, 0);
            timer.Enabled = true;
            if(Mode == GameModes.SecondPlayer)
                ComputerPlay();
        }

        private void picCell_Click(object sender, EventArgs e)
        {
            PictureBox picCell = (PictureBox)sender;
            if(picCell.Cursor == Cursors.Hand)
            {
                int x = (int)picCell.Name[7] - 48;
                int y = (int)picCell.Name[8] - 48;
                Board[x, y] = CurrentPlayer;
                if(Board[x, y] == PlayerA)
                    picCell.Image = PlayerImageA;
                else
                    picCell.Image = PlayerImageB;
                picCell.Cursor = Cursors.No;
                stlStatus.Text = ST_LAST_PLAY + " [" + x.ToString() + "," + y.ToString() + "]";
                if(PlayerWin(CurrentPlayer))
                    FinishGame(CurrentPlayer);
                else
                    NextTurn();
                if((Mode == GameModes.FirstPlayer && CurrentPlayer == PlayerB) || (Mode == GameModes.SecondPlayer && CurrentPlayer == PlayerA))
                    ComputerPlay();
            }
        }

        private const int LastTurn = 8;
        private int Turn;

        public int CurrentPlayer
        {
            get { return Turn % 2; }
        }

        public int CurrentOpponent
        {
            get { return (Turn + 1) % 2; }
        }

        private void NextTurn()
        {
            if(Turn < LastTurn)
            {
                Turn++;
                if(CurrentPlayer == PlayerA)
                    stlTurn.Image = mitPlayer1.Image;
                else
                    stlTurn.Image = mitPlayer2.Image;
            }
            else
            {
                FinishGame(NoPlayer);
            }
        }

        private void FinishGame(int winner)
        {
            switch(winner)
            {
                case NoPlayer:
                    stlStatus.Image = imageListIcon.Images["gameover"];
                    stlStatus.Text = ST_GAME_OVER;
                    break;
                case PlayerA:
                    stlStatus.Image = mitPlayer1.Image;
                    stlStatus.Text = ST_PLAYER_WIN;
                    break;
                case PlayerB:
                    stlStatus.Image = mitPlayer2.Image;
                    stlStatus.Text = ST_PLAYER_WIN;
                    break;
            }
            stlTurn.Image = null;
            stlTurn.Text = ST_PLAY_AGAIN;
            progressBar.Style = ProgressBarStyle.Blocks;
            timer.Enabled = false;
            BlockCells();
        }

        private bool Play(int x, int y)
        {
            Board[x, y] = CurrentPlayer;
            Control[] ctrl = Controls.Find("picCell" + x.ToString() + y.ToString(), true);
            PictureBox picCell = (PictureBox)ctrl[0];
            if(Board[x, y] == PlayerA)
                picCell.Image = PlayerImageA;
            else
                picCell.Image = PlayerImageB;
            picCell.Cursor = Cursors.No;
            return true;
        }

        #endregion //Game

        #region Computer 

        private void ComputerPlay()
        {
            switch(Level)
            {
                case GameLevels.Easy:
                    RandomPlay();
                    break;
                case GameLevels.Medium:
                    if(!WinnerPlay(CurrentPlayer))
                        if(!WinnerPlay(CurrentOpponent))
                            RandomPlay();
                    break;
                case GameLevels.Hard:
                    if(!WinnerPlay(CurrentPlayer))
                        if(!WinnerPlay(CurrentOpponent))
                            SmartPlay();
                    break;
            }
            if(PlayerWin(CurrentPlayer))
                FinishGame(CurrentPlayer);
            else
                NextTurn();
        }

        private bool WinnerPlay(int player)
        {
            if(Board[0, 0] == NoPlayer && Board[1, 1] == player && Board[2, 2] == player)
                return Play(0, 0);
            if(Board[0, 2] == NoPlayer && Board[1, 1] == player && Board[2, 0] == player)
                return Play(0, 2);
            if((Board[0, 0] == player && Board[1, 1] == NoPlayer && Board[2, 2] == player) || (Board[0, 2] == player && Board[1, 1] == NoPlayer && Board[2, 0] == player))
                return Play(1, 1);
            if(Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == NoPlayer)
                return Play(2, 0);
            if(Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == NoPlayer)
                return Play(2, 2);
            for(int i = 0; i < 3; i++)
            {
                if(Board[i, 0] == NoPlayer && Board[i, 1] == player && Board[i, 2] == player)
                    return Play(i, 0);
                if(Board[i, 0] == player && Board[i, 1] == NoPlayer && Board[i, 2] == player)
                    return Play(i, 1);
                if(Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == NoPlayer)
                    return Play(i, 2);
                if(Board[0, i] == NoPlayer && Board[1, i] == player && Board[2, i] == player)
                    return Play(0, i);
                if(Board[0, i] == player && Board[1, i] == NoPlayer && Board[2, i] == player)
                    return Play(1, i);
                if(Board[0, i] == player && Board[1, i] == player && Board[2, i] == NoPlayer)
                    return Play(2, i);
            }
            return false;
        }

        private bool RandomPlay()
        {
            Random random = new Random();
            int x, y;
            bool ready = false;
            do
            {
                x = random.Next(3);
                y = random.Next(3);
                if(Board[x, y] == NoPlayer)
                    ready = Play(x, y);
            } while(!ready);
            return ready;
        }

        private bool SmartPlay()
        {
            if(CurrentPlayer == PlayerA)
            {
                //first play
                if(Turn == 0)
                    return CornerPlay();
                //force with diagonal
                if(Board[1, 1] == CurrentOpponent)
                {
                    if(Board[0, 0] == CurrentPlayer && Board[2, 2] == NoPlayer)
                        return Play(2, 2);
                    if(Board[0, 2] == CurrentPlayer && Board[2, 0] == NoPlayer)
                        return Play(2, 0);
                    if(Board[2, 0] == CurrentPlayer && Board[0, 2] == NoPlayer)
                        return Play(0, 2);
                    if(Board[2, 2] == CurrentPlayer && Board[0, 0] == NoPlayer)
                        return Play(0, 0);
                }
                //playing adjacent corners
                if(Board[0, 0] == CurrentPlayer && (Board[0, 2] == CurrentOpponent || Board[2, 0] == CurrentOpponent) && Board[2, 2] == NoPlayer)
                    return Play(2, 2);
                if(Board[0, 2] == CurrentPlayer && (Board[0, 0] == CurrentOpponent || Board[2, 2] == CurrentOpponent) && Board[2, 0] == NoPlayer)
                    return Play(2, 0);
                if(Board[2, 0] == CurrentPlayer && (Board[0, 0] == CurrentOpponent || Board[2, 2] == CurrentOpponent) && Board[0, 2] == NoPlayer)
                    return Play(0, 2);
                if(Board[2, 2] == CurrentPlayer && (Board[0, 2] == CurrentOpponent || Board[2, 0] == CurrentOpponent) && Board[0, 0] == NoPlayer)
                    return Play(0, 0);
                //cardinals
                if(Board[1, 1] == CurrentPlayer)
                {
                    //finish cardinal with [0,0]
                    if(Board[0, 0] == CurrentPlayer)
                    {
                        if(Board[0, 1] == CurrentOpponent && Board[2, 0] == NoPlayer)
                            return Play(2, 0);
                        if(Board[1, 0] == CurrentOpponent && Board[0, 2] == NoPlayer)
                            return Play(0, 2);
                        //cases [1,2] and [2,1] are resolved by WinnerPlay()
                    }
                    //finish cardinal with [0,2]
                    if(Board[0, 2] == CurrentPlayer)
                    {
                        if(Board[0, 1] == CurrentOpponent && Board[2, 2] == NoPlayer)
                            return Play(2, 2);
                        if(Board[1, 2] == CurrentOpponent && Board[0, 0] == NoPlayer)
                            return Play(0, 0);
                        //cases [1,0] and [2,1] are resolved by WinnerPlay()
                    }
                    //finish cardinal with [2,0]
                    if(Board[2, 0] == CurrentPlayer)
                    {
                        if(Board[1, 0] == CurrentOpponent && Board[2, 2] == NoPlayer)
                            return Play(2, 2);
                        if(Board[2, 1] == CurrentOpponent && Board[0, 0] == NoPlayer)
                            return Play(0, 0);
                        //cases [0,1] and [1,2] are resolved by WinnerPlay()
                    }
                    //finish cardinal with [2,2]
                    if(Board[2, 2] == CurrentPlayer)
                    {
                        if(Board[1, 2] == CurrentOpponent && Board[2, 0] == NoPlayer)
                            return Play(2, 0);
                        if(Board[2, 1] == CurrentOpponent && Board[0, 2] == NoPlayer)
                            return Play(0, 2);
                        //cases [0,1] and [1,0] are resolved by WinnerPlay()
                    }
                }
                //playing cardinals
                if(Board[0, 0] == CurrentPlayer || Board[0, 2] == CurrentPlayer || Board[2, 0] == CurrentPlayer || Board[2, 2] == CurrentPlayer)
                    if(Board[0, 1] == CurrentOpponent || Board[1, 0] == CurrentOpponent || Board[1, 2] == CurrentOpponent || Board[2, 1] == CurrentOpponent)
                        if(Board[1, 1] == NoPlayer)
                            return Play(1, 1);
                //NON ADJACENT CORNERS
                //finish non adjacent with [0,0]
                if(Board[0, 0] == CurrentPlayer && Board[2, 2] == CurrentOpponent)
                {
                    if(Board[0, 2] == CurrentPlayer && Board[2, 0] == NoPlayer)
                        return Play(2, 0);
                    if(Board[2, 0] == CurrentPlayer && Board[0, 2] == NoPlayer)
                        return Play(0, 2);
                }
                //finish non adjacent with [0,2]
                if(Board[0, 2] == CurrentPlayer && Board[2, 0] == CurrentOpponent)
                {
                    if(Board[0, 0] == CurrentPlayer && Board[2, 2] == NoPlayer)
                        return Play(2, 2);
                    if(Board[2, 2] == CurrentPlayer && Board[0, 0] == NoPlayer)
                        return Play(0, 0);
                }
                //finish non adjacent with [2,0]
                if(Board[2, 0] == CurrentPlayer && Board[0, 2] == CurrentOpponent)
                {
                    if(Board[0, 0] == CurrentPlayer && Board[2, 2] == NoPlayer)
                        return Play(2, 2);
                    if(Board[2, 2] == CurrentPlayer && Board[0, 0] == NoPlayer)
                        return Play(0, 0);
                }
                //finish non adjacent with [2,2]
                if(Board[2, 2] == CurrentPlayer && Board[0, 0] == CurrentOpponent)
                {
                    if(Board[0, 2] == CurrentPlayer && Board[2, 0] == NoPlayer)
                        return Play(2, 0);
                    if(Board[2, 0] == CurrentPlayer && Board[0, 2] == NoPlayer)
                        return Play(0, 2);
                }
                //playing non adjacent corners
                if((Board[0, 0] == CurrentPlayer && Board[2, 2] == CurrentOpponent) || (Board[2, 2] == CurrentPlayer && Board[0, 0] == CurrentOpponent))
                {
                    int sw = new Random().Next(2);
                    if(sw == 0 && Board[0, 2] == NoPlayer)
                        return Play(0, 2);
                    if(sw == 1 && Board[2, 0] == NoPlayer)
                        return Play(2, 0);
                }
                if((Board[0, 2] == CurrentPlayer && Board[2, 0] == CurrentOpponent) || (Board[2, 0] == CurrentPlayer && Board[0, 2] == CurrentOpponent))
                {
                    int sw = new Random().Next(2);
                    if(sw == 0 && Board[0, 0] == NoPlayer)
                        return Play(0, 0);
                    if(sw == 1 && Board[2, 2] == NoPlayer)
                        return Play(2, 2);
                }
            }
            else //CurrentPlayer==PlayerB
            {
                //play free center
                if(Board[1, 1] == NoPlayer)
                    return Play(1, 1);
                //force diagonal
                if(Board[1, 1] == CurrentPlayer)
                {
                    if(Board[0, 1] == CurrentOpponent && Board[1, 0] == CurrentOpponent)
                    {
                        int sw = new Random().Next(3);
                        if(sw == 0 && Board[0, 0] == NoPlayer)
                            return Play(0, 0);
                        if(sw == 1 && Board[0, 2] == NoPlayer)
                            return Play(0, 2);
                        if(sw == 2 && Board[2, 0] == NoPlayer)
                            return Play(2, 0);
                    }
                    if(Board[0, 1] == CurrentOpponent && Board[1, 2] == CurrentOpponent)
                    {
                        int sw = new Random().Next(3);
                        if(sw == 0 && Board[0, 2] == NoPlayer)
                            return Play(0, 2);
                        if(sw == 1 && Board[0, 0] == NoPlayer)
                            return Play(0, 0);
                        if(sw == 2 && Board[2, 2] == NoPlayer)
                            return Play(2, 2);
                    }
                    if(Board[1, 0] == CurrentOpponent && Board[2, 1] == CurrentOpponent)
                    {
                        int sw = new Random().Next(3);
                        if(sw == 0 && Board[2, 0] == NoPlayer)
                            return Play(2, 0);
                        if(sw == 1 && Board[0, 0] == NoPlayer)
                            return Play(0, 0);
                        if(sw == 2 && Board[2, 2] == NoPlayer)
                            return Play(2, 2);
                    }
                    if(Board[1, 2] == CurrentOpponent && Board[2, 1] == CurrentOpponent)
                    {
                        int sw = new Random().Next(3);
                        if(sw == 0 && Board[2, 2] == NoPlayer)
                            return Play(2, 2);
                        if(sw == 1 && Board[0, 2] == NoPlayer)
                            return Play(0, 2);
                        if(sw == 2 && Board[2, 0] == NoPlayer)
                            return Play(2, 0);
                    }
                    if((Board[0, 0] == CurrentOpponent && Board[2, 2] == CurrentOpponent) || (Board[0, 2] == CurrentOpponent && Board[2, 0] == CurrentOpponent))
                        if(!CardinalPlay())
                            return RandomPlay();
                }
                if(Board[1, 1] == CurrentPlayer)
                {
                    if(Board[0, 1] == CurrentOpponent)
                    {
                        if(Board[2, 0] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[0, 0] == NoPlayer)
                                return Play(0, 0);
                            if(sw == 1 && Board[1, 2] == NoPlayer)
                                return Play(1, 2);
                        }
                        if(Board[2, 2] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[0, 2] == NoPlayer)
                                return Play(0, 2);
                            if(sw == 1 && Board[1, 0] == NoPlayer)
                                return Play(1, 0);
                        }
                    }
                    if(Board[2, 1] == CurrentOpponent)
                    {
                        if(Board[0, 0] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[2, 0] == NoPlayer)
                                return Play(2, 0);
                            if(sw == 1 && Board[1, 2] == NoPlayer)
                                return Play(1, 2);
                        }
                        if(Board[0, 2] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[2, 2] == NoPlayer)
                                return Play(2, 2);
                            if(sw == 1 && Board[1, 0] == NoPlayer)
                                return Play(1, 0);
                        }
                    }
                    if(Board[1, 0] == CurrentOpponent)
                    {
                        if(Board[0, 2] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[0, 0] == NoPlayer)
                                return Play(0, 0);
                            if(sw == 1 && Board[2, 1] == NoPlayer)
                                return Play(2, 1);
                        }
                        if(Board[2, 2] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[2, 0] == NoPlayer)
                                return Play(2, 0);
                            if(sw == 1 && Board[0, 1] == NoPlayer)
                                return Play(0, 1);
                        }
                    }
                    if(Board[1, 2] == CurrentOpponent)
                    {
                        if(Board[0, 0] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[0, 2] == NoPlayer)
                                return Play(0, 2);
                            if(sw == 1 && Board[2, 1] == NoPlayer)
                                return Play(2, 1);
                        }
                        if(Board[2, 0] == CurrentOpponent)
                        {
                            int sw = new Random().Next(2);
                            if(sw == 0 && Board[2, 2] == NoPlayer)
                                return Play(2, 2);
                            if(sw == 1 && Board[0, 1] == NoPlayer)
                                return Play(0, 1);
                        }
                    }
                }
                if(Board[1, 1] == CurrentOpponent)
                    if(!CornerPlay())
                        RandomPlay();
            }
            return false;
        }

        private bool CardinalPlay()
        {
            List<int> sort = new List<int>(4);
            sort.Add(new Random().Next(4));
            for(int i = 0 ; i < 3 ; i++)
                sort.Add((sort[i] + 1) % 4);
            foreach(int item in sort)
            {
                if(item == 0 && Board[0, 1] == NoPlayer)
                    return Play(0, 1);
                if(item == 1 && Board[1, 2] == NoPlayer)
                    return Play(1, 2);
                if(item == 2 && Board[2, 1] == NoPlayer)
                    return Play(2, 1);
                if(item == 3 && Board[1, 0] == NoPlayer)
                    return Play(1, 0);
            }
            return false;
        }

        private bool CornerPlay()
        {
            List<int> sort = new List<int>(4);
            sort.Add(new Random().Next(4));
            for(int i = 0 ; i < 3 ; i++)
                sort.Add((sort[i] + 1) % 4);
            foreach(int item in sort)
            {
                if(item == 0 && Board[0, 0] == NoPlayer)
                    return Play(0, 0);
                if(item == 1 && Board[0, 2] == NoPlayer)
                    return Play(0, 2);
                if(item == 2 && Board[2, 0] == NoPlayer)
                    return Play(2, 0);
                if(item == 3 && Board[2, 2] == NoPlayer)
                    return Play(2, 2);
            }
            return false;
        }

        #endregion //Computer

        #region Time 

        private TimeSpan time;

        private void timer_Tick(object sender, EventArgs e)
        {
            time = time.Add(new TimeSpan(0, 0, 1));
            stlTime.Text = time.ToString();
        }

        #endregion

        private void tsbAbout_Click(object sender, EventArgs e)
        {
            string msg = Application.ProductName + "  v" + Application.ProductVersion;
            msg += Environment.NewLine + Environment.NewLine;
            if(Language == GameLanguage.Spanish)
                msg += "Desarrollado por: " + Application.CompanyName;
            else
                msg += "Developed by: " + Application.CompanyName;
            msg += Environment.NewLine + Environment.NewLine;
            msg += "E-mail: miguel.simoni@gmail.com   ";
            MessageBox.Show(msg, "About " + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static void UpdateConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save();
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if(!e.Url.ToString().Contains(@"http://usuarios.lycos.es/rigelsaint/adsense_trilin.htm"))
            {
                System.Diagnostics.Process.Start("iexplore", e.Url.ToString());
                e.Cancel = true;
            }
        }

        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void mitWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("iexplore", "http://xmsim.net.googlepages.com/");
        }

        private void mitAbout_Click(object sender, EventArgs e)
        {
            string msg = Application.ProductName + "  v" + Application.ProductVersion;
            msg += Environment.NewLine + Environment.NewLine;
            if(Language == GameLanguage.Spanish)
                msg += "Desarrollado por: " + Application.CompanyName;
            else
                msg += "Developed by: " + Application.CompanyName;
            msg += Environment.NewLine + Environment.NewLine;
            msg += "E-mail: miguel.simoni@gmail.com   ";
            MessageBox.Show(msg, "About " + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

    }
}