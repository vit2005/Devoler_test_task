using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Devoler_test_task
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int robotsCount = 6;
        List<byte> aviableTypes = new List<byte>() { 1, 2, 3 };

        List<Robot> robots;
        List<Item> items;
        List<byte> types;

        string log;
        bool dontstop;

        List<ProgressBar> ProgressBars;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Initional code

        int ParseString()
        {
            types = new List<byte>();

            string[] types_str = types_textBox.Text.Split(new char[] { ',' });
            byte b = 0;
            foreach (string s in types_str)
            {
                try
                {
                    b = Convert.ToByte(s);
                }
                catch (Exception)
                {
                    return 1;
                }
                if (aviableTypes.IndexOf(b) == -1)
                    return 3;
                types.Add(b);
            }

            if (types.Count != 6)
                return 2;

            return 0;
        }

        void InitVariables()
        {
            dontstop = true;
            Stop_button.IsEnabled = true;
            Model_button.IsEnabled = false;
            Log_textBlock.Text = "";
            log = "";

            if (ProgressBars != null && ProgressBars.Count > 0)
                foreach (ProgressBar pb in ProgressBars)
                    MainGrid.Children.Remove(pb);


            InitItems();
            InitRobots();
        }

        void InitItems()
        {
            items = new List<Item>();
            byte nextItemType = 0;
            for (int i = 0; i < types.Count; i++)
            {
                string type = (nextItemType == 0) ? "iPlug-0" : "iCable-0";
                Item it = new Item(nextItemType, string.Format("{1}{0}", i, type));
                items.Add(it);
                nextItemType = nextItemType == 0 ? (byte)1 : (byte)0;
            }
        }

        void InitRobots()
        {
            robots = new List<Robot>();
            for (int i = 0; i < types.Count; i++)
            {
                Robot r = Robot.RobotFactory(types[i], title: string.Format("0{0}", i), items: new List<Item>() { items[i] });
                robots.Add(r);
            }

            for (int i = 0; i < types.Count-1; i++)
            {
                robots[i].AddItem(items[i + 1]);
            }
            robots[robots.Count - 1].AddItem(items[0]);

            for (int i = 0; i < types.Count; i++)
            {
                if (robots[i] is GentlemanRobot)
                    (robots[i] as GentlemanRobot).SetNeighbors(i, robots);
            }
        }

        void InitProgressbars()
        {                
            ProgressBars = new List<ProgressBar>();
            for (int i = 0; i < types.Count; i++)
            {
                ProgressBar pb = new ProgressBar();
                pb.Orientation = Orientation.Horizontal;
                pb.Width = 350;
                pb.Height = 20;
                pb.Minimum = 0;
                pb.Maximum = 100;
                pb.BorderThickness = new Thickness(3);
                pb.BorderBrush = GetBrush(types[i]);
                pb.Margin = new Thickness(375, 50 * i - MainGrid.ActualHeight + 100, 0, 0);

                ProgressBars.Add(pb);
                MainGrid.Children.Add(pb);
            }
        }

        Brush GetBrush(int id)
        {
            switch (id)
            {
                case 1:
                    return Brushes.Green;
                case 2:
                    return Brushes.Red;
                case 3:
                    return Brushes.Blue;
                default:
                    return Brushes.DimGray;
            }
        }

        #endregion

        #region Buttons events

        private void Model_button_Click(object sender, RoutedEventArgs e)
        {
            switch (ParseString())
            {
                case 0:
                    break;
                case 1:
                    MessageBox.Show("Convert error, use recognizeble types");
                    return;
                case 2:
                    MessageBox.Show("Count of robots should be 6");
                    return;
                case 3:
                    MessageBox.Show("Unaviable type");
                    return;
            }
            InitVariables();
            InitProgressbars();
            new Thread(DoWork).Start();
        }

        private void Stop_button_Click(object sender, RoutedEventArgs e)
        {
            dontstop = false;
        }

        #endregion

        #region Modelling

        [STAThread]
        void DoWork()
        {
            long time = 0;
            while (!CheckFinal())
            {
                Dispatcher.Invoke(() => Log_textBlock.Text = string.Format("\n\n{0}00) ", time));
                log += string.Format("\n\n{0}00) ", time);
                time++;
                DoModel();
                WriteLog();
            }
            MessageBox.Show("Done");
            Dispatcher.Invoke(() => {
                Log_textBlock.Text = log;
                Stop_button.IsEnabled = false;
                Model_button.IsEnabled = true;
            });
        }

        void DoModel()
        {
            foreach (Robot r in robots)
            {
                r.DoStrategy();
            }
            foreach (Robot r in robots)
            {
                r.DoStrategy();
            }
            foreach (Robot r in robots)
            {
                r.SpendTime();
            }
            bool realtime = false;
            Dispatcher.Invoke(() => realtime = UseRealTime_checkBox.IsChecked.Value);
            if (realtime)
                Thread.Sleep(100);
        }

        void WriteLog()
        {
            string robotlogstring = "";
            foreach (Robot r in robots)
            {
                robotlogstring += string.Format("\n{0} [{1}]", r.Title, r.HP);
            }

            string itemlogstring = "";
            foreach (Item i in items)
            {
                string ownerTitle = (i.Owner != null) ? i.Owner.Title : "";
                itemlogstring += string.Format("\n{0} [{1}]", i.Title, ownerTitle);
            }
            log += robotlogstring;
            log += itemlogstring;

            bool use = false;
            Dispatcher.Invoke(() => use = UseGraphic_checkBox.IsChecked.Value);
            if (use)
            {
                for (int i = 0; i < robots.Count; i++)
                {
                    Dispatcher.Invoke(() => ProgressBars[i].Value = robots[i].HP);
                }
                Dispatcher.Invoke(() => Log_textBlock.Text += robotlogstring);
                Dispatcher.Invoke(() => Log_textBlock.Text += itemlogstring);
            }
        }

        bool CheckFinal()
        {
            if (!dontstop)
                return true;

            int deadRobots = 0;
            int fullRobots = 0;

            foreach (Robot r in robots)
            {
                if (r.HP == 100)
                    fullRobots++;
                else if (r.HP == 0)
                    deadRobots++;
            }

            return deadRobots + fullRobots == robots.Count;
        }

        #endregion

    }
}
