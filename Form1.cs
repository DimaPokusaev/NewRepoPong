/*
 * Description:     A basic PONG simulator
 * Author:          Dmitry Pokusaev 
 * Date:            2019-02-04
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //random values
        Random randGen = new Random();

        //graphics objects for drawing
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font drawFont = new Font("Courier New", 40);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions, speed, and rectangle
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = false;
        const int BALL_SPEED = 4;
        const int PADDLE_EDGE = 20;
        Rectangle ball;

        //paddle speeds and rectangles
        const int PADDLE_SPEED = 4;
        Rectangle p1, p2, pc;

        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// check to see if a key is pressed and set is KeyDown value to true if it has
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }

        /// <summary>
        /// check to see if a key has been released and set its KeyDown value to false if it has
        /// </summary>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            //rest the game
            if (newGameOk)
            {
                player1Score = player2Score = 0;
                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            #region starting position
            p1.Width = p2.Width = pc.Width = 10;    
            p1.Height = p2.Height = pc.Height = 40;


            p1.X = PADDLE_EDGE;
            p1.Y = this.Height / 2 - p1.Height / 2;

            p2.X = this.Width - PADDLE_EDGE - p2.Width;
            p2.Y = this.Height / 2 - p2.Height / 2;

            pc.X = this.Width/2;
            pc.Y = this.Height/2;

            ball.Width = 10;
            ball.Height = 10;
            ball.X = (this.Width) / 2 - ball.Width;
            ball.Y = (this.Height) / 2 - ball.Height;

            #endregion
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            #region update ball position

            //code to move ball either left or right based on ballMoveRight and using BALL_SPEED
            if (ballMoveRight)
            {
                ball.X = ball.X + BALL_SPEED;
            }
            else
            {
                ball.X = ball.X - BALL_SPEED;
            }
            //code to move ball either down or up based on ballMoveDown and using BALL_SPEED
            if (ballMoveDown)
            {
                ball.Y = ball.Y - BALL_SPEED;
            }
            else
            {
                ball.Y = ball.Y + BALL_SPEED;
            }
            #endregion

            #region update paddle positions
            // move player 1 paddle up using p1.Y and PADDLE_SPEED

            if (aKeyDown == true && p1.Y > 4) { p1.Y = p1.Y - PADDLE_SPEED; }

            if (zKeyDown == true && p1.Y < (this.Height - p1.Height - 4)) { p1.Y = p1.Y + PADDLE_SPEED; }

            if (jKeyDown == true && p2.Y > 4) { p2.Y = p2.Y - PADDLE_SPEED; }

            if (mKeyDown == true && p2.Y < (this.Height - p2.Height - 4)) { p2.Y = p2.Y + PADDLE_SPEED; }

            if (ball.Y >= pc.Y)
            {
                if(!(pc.Y > this.Width - 200)) { pc.Y = pc.Y + PADDLE_SPEED/2; }
            }
            else
            {
                if (!(pc.Y < 4)) { pc.Y = pc.Y - PADDLE_SPEED/2; }
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0)
            {
                ballMoveDown = false;
                collisionSound.Play();
                this.BackColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
                this.Refresh();
            }

            if (ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = true;
                collisionSound.Play();
                this.BackColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
                this.Refresh();
            }

            #endregion

            #region ball colision with paddles

            if (pc.IntersectsWith(ball))
            {
                ballMoveRight = !ballMoveRight;
                collisionSound.Play();
                this.BackColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
                this.Refresh();
            }
            if (p2.IntersectsWith(ball))
            {
                ballMoveRight = false;
                collisionSound.Play();
                this.BackColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
                this.Refresh();
            }
            if (p1.IntersectsWith(ball))
            {
                ballMoveRight = true;
                collisionSound.Play();
                this.BackColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
                this.Refresh();
            }
            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                player2Score = player2Score + 1;
                scoreSound.Play();
                ballMoveRight = true;
                this.ForeColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
            }

            if (ball.X > this.Width)
            {
                player1Score = player1Score + 1;
                scoreSound.Play();
                ballMoveRight = false;
                this.ForeColor = Color.FromArgb(randGen.Next(1, 250), randGen.Next(1, 250), randGen.Next(1, 250));
            }

            if (player1Score == gameWinScore) { GameOver("p1"); }
            if (player2Score == gameWinScore) { GameOver("p2"); }
            this.Refresh();

            #endregion   
        }

        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        private void GameOver(string winner)
        {
            newGameOk = false;
            gameUpdateLoop.Stop();

            startLabel.Visible = true;
            
            if (winner is "p1") { startLabel.Text = "Player 1 won"; }
            else { startLabel.Text = "Player 2 won"; }

            this.Refresh();
            Thread.Sleep(5000);
            newGameOk = true;
            startLabel.Text = "Press the space bar to play again";
        }

        /// <summary>
        /// paint method
        /// </summary>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            #region draw objects
            e.Graphics.FillRectangle(drawBrush, p1);
            e.Graphics.FillRectangle(drawBrush, pc);
            e.Graphics.FillRectangle(drawBrush, p2);
            e.Graphics.FillEllipse(drawBrush, ball);
            #endregion

            #region draw score
            Point point1 = new Point(50, 50);
            Point point2 = new Point(500, 50);
            
            e.Graphics.DrawString(player1Score.ToString(), drawFont, drawBrush, point1);
            e.Graphics.DrawString(player2Score.ToString(), drawFont, drawBrush, point2);
            #endregion
        }
    }
}
