using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;

namespace Quiz
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region Local variables
        BuzzIOWrite.BuzzIOWrite buzz = null;
        bool c1 = false;
        bool c2 = false;
        bool c3 = false;
        bool c4 = false;
        bool c1used = false;
        bool c2used = false;
        bool c3used = false;
        bool c4used = false;
        bool c1pressed = false;
        bool c2pressed = false;
        bool c3pressed = false;
        bool c4pressed = false;
        int activeController = 0;
        int timeAvailable = 15;
        bool controllersActive = false;
        bool timerActive = false;
        Timer timer;
        System.Windows.Shapes.Shape currentQuestion = null;
        int c1points = 0;
        int c2points = 0;
        int c3points = 0;
        int c4points = 0;
        int controllerToChoose = 0;
        int timeleft = 0;
        #endregion

        private void buttonDownEventHandler(object sender, KeyEventArgs e)
        {
            if(controllersActive && !timerActive)
            {
                if (e.Key == Key.Q && !c1pressed && !c1used && !timerActive)
                {
                    c1 = true;
                    c2 = c3 = c4 = false;
                    c1used = true;
                    c1pressed = true;
                    activeController = 1;
                    timerActive = true;
                    timer.Enabled = true;
                    turnOnSingleLight(1);
                }
                if (e.Key == Key.W && !c2pressed && !c2used && !timerActive)
                {
                    c2 = true;
                    c1 = c3 = c4 = false;
                    c2used = true;
                    c2pressed = true;
                    activeController = 2;
                    timerActive = true;
                    timer.Enabled = true;
                    turnOnSingleLight(2);
                }
                if (e.Key == Key.E && !c3pressed && !c3used && !timerActive)
                {
                    c3 = true;
                    c1 = c2 = c4 = false;
                    c3used = true;
                    c3pressed = true;
                    activeController = 3;
                    timerActive = true;
                    timer.Enabled = true;
                    turnOnSingleLight(3);
                }
                if (e.Key == Key.R && !c4pressed && !c4used && !timerActive)
                {
                    c4 = true;
                    c1 = c2 = c3 = false;
                    c4used = true;
                    c4pressed = true;
                    activeController = 4;
                    timerActive = true;
                    timer.Enabled = true;
                    turnOnSingleLight(4);
                }
            }
        }

        private void buttonUpEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
                c1pressed = false;
            if (e.Key == Key.W)
                c2pressed = false;
            if (e.Key == Key.E)
                c3pressed = false;
            if (e.Key == Key.R)
                c4pressed = false;
        }

        private void activate_btn_Click(object sender, RoutedEventArgs e)
        {
            controllersActive = !controllersActive;
            if (controllersActive)
            {                
                timerActive = false;
                turnOnRemainingLight();
            }
            else
            {
                activeController = 0;
                timer.Enabled = false;             
                turnOffAllLight();
            }
        }

        private void OnTimerEnd(object sender, ElapsedEventArgs e)
        {
            MyDelegateType tOSL = delegate
            {
                timeleft -= 1;
                if (timeleft > 0)
                {                    
                    timeLeft.Content = "Timeleft: " + timeleft.ToString();
                    timer.Enabled = false;
                    timer.Enabled = true;
                }
                else
                {
                    timeLeft.Content = "Timeleft: " + timeleft.ToString();
                    timeleft = timeAvailable;
                    activeController = 0;
                    activeController = 0;
                    timer.Enabled = false;
                    timerActive = false;
                    controllersActive = false;

                    label1.Content = string.Format(@"Active controller: {0}", (activeController != 0 ?
                        activeController.ToString() : "None"));
                    currentQuestion.Fill = Brushes.PaleGreen;
                    turnOffAllLight();
                }
            };
            this.Dispatcher.Invoke(DispatcherPriority.Normal, tOSL);            
        }

        public delegate void MyDelegateType();

        private void turnOnRemainingLight()
        {
            MyDelegateType tOSL = delegate
            {
                listBox1.Items.Add((c1used ? "x" : "-") + " " + (c2used ? "x" : "-") + " " + (c3used ? "x" : "-") + " " +
                (c4used ? "x" : "-"));
                label1.Content = string.Format(@"Active controller: {0}", (activeController!=0 ? 
                    activeController.ToString() : "None"));
                buzz.Write(!c1used, !c2used, !c3used, !c4used);
                currentQuestion.Fill = Brushes.PaleGreen;
            };
            this.Dispatcher.Invoke(DispatcherPriority.Normal, tOSL);
        }

        private void turnOnSingleLight(int cnr)
        {
            listBox1.Items.Add((cnr==1 ? "1":"x") + " " + (cnr==2 ? "2":"x") + " " +(cnr==3 ? "3":"x") + " " +
                (cnr==4 ? "4":"x"));
            label1.Content = string.Format(@"Active controller: {0}", (activeController != 0 ?
                    activeController.ToString() : "None"));
            buzz.Write((cnr==1 ? true: false),(cnr==2 ? true: false), (cnr==3 ? true: false), (cnr==4 ? true: false));
            currentQuestion.Fill = Brushes.PowderBlue;
            timeLeft.Content = "Timeleft: " + timeleft.ToString();
        }
        
        private void turnOffAllLight()
        {
            label1.Content = string.Format(@"Active controller: {0}", (activeController != 0 ?
                    activeController.ToString() : "None"));
            buzz.Write(false, false, false, false);
        }

        private void turnOnAllLight()
        {
            label1.Content = string.Format(@"Active controller: {0}", (activeController != 0 ?
                    activeController.ToString() : "None"));
            buzz.Write(true, true, true, true);
        }
                
        public Window1()
        {
            InitializeComponent();
            initialize();
            this.PreviewKeyDown += new KeyEventHandler(buttonDownEventHandler);
            this.PreviewKeyUp += new KeyEventHandler(buttonUpEventHandler);
            timeleft = timeAvailable;
            timer = new Timer(1000);
            timer.AutoReset = false;            
            timer.Elapsed += new ElapsedEventHandler(OnTimerEnd);

            #region Categories
            kat1.Content = "Category 1";
            kat2.Content = "Category 2";
            kat3.Content = "Category 3";
            kat4.Content = "Category 4";
            kat5.Content = "Category 5";
            kat6.Content = "Category 6";
            katYear.Content = "Year";
            #endregion
        }

        private void initialize()
        {
            point_11.Text = "100";
            buzz = new BuzzIOWrite.BuzzIOWrite();
            buzz.FindTheHid();
            turnOffAllLight();
        }

        //#region quizboard
        private void question_11_Click(object sender, RoutedEventArgs e)
        {
            questionTextBox.Text = "Dette kan være et spørgsmål som deltagerne skal besvare.\n\nSvar: Hvad er et svar?";
            currentQuestion = back11;
        }

        private void activate_11_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion = back11;
            currentQuestion.Fill = Brushes.PaleGreen;
            activate_btn_Click(this, null);
            timeleft = timeAvailable;
        }

        private void bonus_11_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion = back11;
            currentQuestion.Fill = Brushes.PaleGreen;
        }

        private void stop_11_Click(object sender, RoutedEventArgs e)
        {
            currentQuestion.Fill = Brushes.Lavender;
            question_11.IsEnabled = false;
            activate_11.IsEnabled = false;
            bonus_11.IsEnabled = false;
            stop_11.IsEnabled = false;
            point_11.IsEnabled = false;
            c1_p_11.IsEnabled = false;
            c1_m_11.IsEnabled = false;
            c2_p_11.IsEnabled = false;
            c2_m_11.IsEnabled = false;
            c3_p_11.IsEnabled = false;
            c3_m_11.IsEnabled = false;
            c4_p_11.IsEnabled = false;
            c4_m_11.IsEnabled = false;
            timer.Enabled = false;
            timeLeft.Content = "Timeleft: ";
            controllersActive = false;
            c1used = c2used = c3used = c4used = false;
            questionTextBox.Text = "";
            listBox1.Items.Clear();
        }

        private void c1_p_11_Click(object sender, RoutedEventArgs e)
        {
            c1points += Int32.Parse(point_11.Text.ToString());
            controllerToChoose = 1;
            cToChoose.Content = "Selecting: " + controllerToChoose.ToString();
            c1pointslabel.Content = 1.ToString() + ": " + c1points.ToString();
        }

        private void c1_m_11_Click(object sender, RoutedEventArgs e)
        {
            c1points -= Int32.Parse(point_11.Text.ToString());
            c1pointslabel.Content = 1.ToString() + ": " + c1points.ToString();
        }

        private void c2_p_11_Click(object sender, RoutedEventArgs e)
        {
            c2points += Int32.Parse(point_11.Text.ToString());
            controllerToChoose = 2;
            cToChoose.Content = "Selecting: " + controllerToChoose.ToString();
            c2pointslabel.Content = 2.ToString() + ": " + c2points.ToString();
        }

        private void c2_m_11_Click(object sender, RoutedEventArgs e)
        {
            c2points -= Int32.Parse(point_11.Text.ToString());
            c2pointslabel.Content = 2.ToString() + ": " + c2points.ToString();
        }

        private void c3_p_11_Click(object sender, RoutedEventArgs e)
        {
            c3points += Int32.Parse(point_11.Text.ToString());
            controllerToChoose = 3;
            cToChoose.Content = "Selecting: " + controllerToChoose.ToString();
            c3pointslabel.Content = 3.ToString() + ": " + c3points.ToString();
        }

        private void c3_m_11_Click(object sender, RoutedEventArgs e)
        {
            c3points -= Int32.Parse(point_11.Text.ToString());
            c3pointslabel.Content = 3.ToString() + ": " + c3points.ToString();
        }

        private void c4_p_11_Click(object sender, RoutedEventArgs e)
        {
            c4points += Int32.Parse(point_11.Text.ToString());
            controllerToChoose = 4;
            cToChoose.Content = "Selecting: " + controllerToChoose.ToString();
            c4pointslabel.Content = 4.ToString() + ": " + c4points.ToString();
        }

        private void c4_m_11_Click(object sender, RoutedEventArgs e)
        {
            c4points -= Int32.Parse(point_11.Text.ToString());
            c4pointslabel.Content = 4.ToString() + ": " + c4points.ToString();
        }
    }
}
