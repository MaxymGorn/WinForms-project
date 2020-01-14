using ClassLibrary1;
using Solar_system_WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maxs_Gorn_Solar_system
{
    public partial class Form1 : Form
    {
        Image RotatingBlocks;
        Rectangle InvalidRect;
        List<Planet> planets = new List<Planet>();
        List<Point> pointFs = new List<Point>();
        List<string> listinfo = new List<string>();
        List<Point> pointsp=new List<Point>() { new Point(0,0), new Point(0, 0), new Point(0, 0) };
        Satelite moon;
        Satelite phobos;
        Satelite deimos;
        Random random = new Random();
        ListQueAnsw listQueAnsw = new ListQueAnsw();
        List<int> vs = new List<int>();
        string file_name = "test.txt";
        int ind = 0,task=-1;
        int trueQuetion;


        public Form1()
        {
           
            InitializeComponent();
            Init_Test();
    
            RotatingBlocks = new Bitmap("JYxV.gif"); Point DrawHere;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = Image.FromFile(@"InformationPlanet\Sun.png");
            DrawHere = new Point(0, 0);
            InvalidRect = new Rectangle(DrawHere, RotatingBlocks.Size);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            pointFs.Add(new Point(800, 450));
            pointFs.Add(new Point(800, 480));
            pointFs.Add(new Point(800, 510));
            pointFs.Add(new Point(800, 550));
            pointFs.Add(new Point(800, 620));
            pointFs.Add(new Point(800, 670));
            pointFs.Add(new Point(800, 730));
            pointFs.Add(new Point(800, 750));
            pointFs.Add(new Point(800, 760));
            pointFs.Add(new Point(800, 780));
            Planet mercury;
            Planet venus;
            Planet earth;
            Planet mars;
            Planet jupiter;
            Planet saturn;
            Planet uranus;
            Planet neptun;
            Planet pluton;
            mercury = new Planet(Convert.ToInt32(Height / 2 - pointFs[0].Y), 10.75, pointFs[0]);
            venus = new Planet(Convert.ToInt32(Height / 2 - pointFs[1].Y), 40.48, pointFs[1]);
            earth = new Planet(Convert.ToInt32(Height / 2 - pointFs[2].Y), 90.84, pointFs[2]);
            mars = new Planet(Convert.ToInt32(Height / 2 - pointFs[3].Y), 100.67, pointFs[3]);
            jupiter = new Planet(Convert.ToInt32(Height / 2 - pointFs[4].Y), 130.07, pointFs[4]);
            saturn = new Planet(Convert.ToInt32(Height / 2 - pointFs[5].Y), 440.13, pointFs[5]);
            uranus = new Planet(Convert.ToInt32(Height / 2 - pointFs[6].Y), 290.76, pointFs[6]);
            neptun = new Planet(Convert.ToInt32(Height / 2 - pointFs[7].Y), 350.02, pointFs[7]);
            pluton = new Planet(Convert.ToInt32(Height / 2 - (pointFs[8].Y)), 470.87, pointFs[8]);
            moon = new Satelite(15, 80.84, pointFs[3]);
            phobos = new Satelite(20, 50.84, pointFs[4]);
            deimos = new Satelite(31, 90.84, pointFs[4]);
            planets.Add(mercury);
            planets.Add(venus);
            planets.Add(earth);
            planets.Add(mars);
            planets.Add(jupiter);
            planets.Add(saturn);
            planets.Add(uranus);
            planets.Add(neptun);
            planets.Add(pluton);
            planets.Add(new Planet(Convert.ToInt32(Height / 2 - pointFs[9].Y), 60.48, pointFs[9]));
            timer1.Start();
            Height = 1080;
            listinfo.Add("Mercury.png");
            listinfo.Add("Venus.png");
            listinfo.Add("Earth.png");
            listinfo.Add("Mars.png");
            listinfo.Add("Jupiter.png");
            listinfo.Add("Saturn.png");
            listinfo.Add("Uranus.png");
            listinfo.Add("Neptune.png");
            listinfo.Add("Pluto.png");
           
        }


      


        protected override void OnPaint(PaintEventArgs e)
        {
            ImageAnimator.UpdateFrames(RotatingBlocks);
            e.Graphics.DrawImage(RotatingBlocks, new Point(0,0));
        }

        private void OnFrameChanged(object o, EventArgs e)
        {
            this.Invalidate(InvalidRect);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (ImageAnimator.CanAnimate(RotatingBlocks))
            {
                ImageAnimator.Animate(RotatingBlocks, new EventHandler(this.OnFrameChanged));
            }
            
        }

        private void DrawPlanet(int i, ref Graphics g, ref Image image)
        {
            g.DrawImage(image, pointFs[i]);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox2.Invalidate();

        }



        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < planets.Count - 1; i++)
            {
                new Thread(delegate () { MovePlanet(i); }).Start();
            }
         
            pointsp[0]= moon.Move(pointFs[3]);
            pointsp[1] = phobos.Move(pointFs[4]);
            pointsp[2] = deimos.Move(pointFs[4]);
            Graphics g = e.Graphics;
            Bitmap bmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g = Graphics.FromImage(bmp);
            Matrix m1 = new Matrix();
        

            g.Transform = m1;
     
            Image img1 = Image.FromFile("Sun.png");
            g.DrawImage(img1, new Point(450, 300));

            for (int i = 0; i < planets.Count - 1; i++)
            {
                img1 = Image.FromFile(listinfo[i]);
                new Thread(delegate () {DrawPlanet(i, ref g,ref img1); }).Start();
            }

            img1 = Image.FromFile("moon.png");
            g.DrawImage(img1, pointsp[0]);
            img1 = Image.FromFile("moon.png");
            g.DrawImage(img1, pointsp[1]);
            img1 = Image.FromFile("moon.png");
            g.DrawImage(img1, pointsp[2]);
            m1.Reset();
            pictureBox2.BackgroundImage = bmp;

        }

        private void MovePlanet(int i)
        {
            pointFs[i] = planets[i].Move();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(@"InformationPlanet\" + listinfo[comboBox1.SelectedIndex]);
        }

  

        private void button1_Click(object sender, EventArgs e)
        {
            trueQuetion = 0;
            textBox1.Visible = true;
            panel1.Visible = true;
            button1.Visible = false;
            GenerateQution();
        }
        void GenerateQution()
        {
           
            while (true) { 
                ind = random.Next(0, listQueAnsw.quetionAnswers.Count);
                if(!vs.Contains(ind))
                {
                    break;
                }
                if(vs.Count==listQueAnsw.quetionAnswers.Count)
                {
                    break;
                }
              
            }
            if (vs.Count != listQueAnsw.quetionAnswers.Count)
            {
                
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;
                radioButton4.Visible = false;
                radioButton5.Visible = false;
                radioButton6.Visible = false;
                textBox1.Text = listQueAnsw.quetionAnswers[ind].Quetion;
                vs.Add(ind);
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                radioButton3.Visible = true;
                radioButton4.Visible = true;
                radioButton5.Visible = true;
                radioButton6.Visible = true;
                if (listQueAnsw.quetionAnswers[ind].Answers.Count == 1)
                {
                    radioButton1.Text = listQueAnsw.quetionAnswers[ind].Answers[0];
                    radioButton2.Visible = false;
                    radioButton3.Visible = false;
                    radioButton4.Visible = false;
                    radioButton5.Visible = false;
                    radioButton6.Visible = false;
                }
                if (listQueAnsw.quetionAnswers[ind].Answers.Count == 2)
                {
                    radioButton1.Text = listQueAnsw.quetionAnswers[ind].Answers[0];
                    radioButton2.Text = listQueAnsw.quetionAnswers[ind].Answers[1];
                    radioButton3.Visible = false;
                    radioButton4.Visible = false;
                    radioButton5.Visible = false;
                    radioButton6.Visible = false;

                }
                if (listQueAnsw.quetionAnswers[ind].Answers.Count == 3)
                {
                    radioButton1.Text = listQueAnsw.quetionAnswers[ind].Answers[0];
                    radioButton2.Text = listQueAnsw.quetionAnswers[ind].Answers[1];
                    radioButton3.Text = listQueAnsw.quetionAnswers[ind].Answers[2];
                    radioButton4.Visible = false;
                    radioButton5.Visible = false;
                    radioButton6.Visible = false;
                }
                if (listQueAnsw.quetionAnswers[ind].Answers.Count == 4)
                {
                    radioButton1.Text = listQueAnsw.quetionAnswers[ind].Answers[0];
                    radioButton2.Text = listQueAnsw.quetionAnswers[ind].Answers[1];
                    radioButton3.Text = listQueAnsw.quetionAnswers[ind].Answers[2];
                    radioButton4.Text = listQueAnsw.quetionAnswers[ind].Answers[3];
                    radioButton5.Visible = false;
                    radioButton6.Visible = false;
                }
                if (listQueAnsw.quetionAnswers[ind].Answers.Count == 5)
                {
                    radioButton1.Text = listQueAnsw.quetionAnswers[ind].Answers[0];
                    radioButton2.Text = listQueAnsw.quetionAnswers[ind].Answers[1];
                    radioButton3.Text = listQueAnsw.quetionAnswers[ind].Answers[2];
                    radioButton4.Text = listQueAnsw.quetionAnswers[ind].Answers[3];
                    radioButton5.Text = listQueAnsw.quetionAnswers[ind].Answers[4];
                    radioButton6.Visible = false;
                }
                if (listQueAnsw.quetionAnswers[ind].Answers.Count == 6)
                {
                    radioButton1.Text = listQueAnsw.quetionAnswers[ind].Answers[0];
                    radioButton2.Text = listQueAnsw.quetionAnswers[ind].Answers[1];
                    radioButton3.Text = listQueAnsw.quetionAnswers[ind].Answers[2];
                    radioButton4.Text = listQueAnsw.quetionAnswers[ind].Answers[3];
                    radioButton5.Text = listQueAnsw.quetionAnswers[ind].Answers[4];
                    radioButton6.Text = listQueAnsw.quetionAnswers[ind].Answers[5];
                }
            
            }
            else if(vs.Count==listQueAnsw.quetionAnswers.Count)
            {
                ShowText($"Верно {trueQuetion}/{listQueAnsw.quetionAnswers.Count-3}");

                textBox1.Visible = false;
                panel1.Visible = false;
                button1.Visible = true;
                vs.Clear();

                trueQuetion = 0;
            }


        }
        void Init_Test()
        {
            DatManage datManage = new DatManage();
            listQueAnsw= datManage.DeserializeXML<ListQueAnsw>("AnswQuet.xml");
        }
        //public event EventHandler CheckAnswer;
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = false;

            if(listQueAnsw.quetionAnswers[ind].otvet_int==1)
            {
                trueQuetion++;
            }
            GenerateQution();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = false;
            if (listQueAnsw.quetionAnswers[ind].otvet_int == 2)
            {
                trueQuetion++;
            }
            GenerateQution();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Checked = false;
            if (listQueAnsw.quetionAnswers[ind].otvet_int == 3)
            {
                trueQuetion++;
            }
            GenerateQution();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton4.Checked = false;
            if (listQueAnsw.quetionAnswers[ind].otvet_int == 4)
            {
                trueQuetion++;
   
            }

            GenerateQution();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            radioButton5.Checked = false;
            if (listQueAnsw.quetionAnswers[ind].otvet_int == 5)
            {
                trueQuetion++;
            }
            GenerateQution();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowText($"Верно {trueQuetion}/{listQueAnsw.quetionAnswers.Count-3}");
           
            textBox1.Visible = false;
            panel1.Visible = false;
            button1.Visible = true;
            vs.Clear();
            
            trueQuetion = 0;
        }
        async void ShowText(string text)
        {
            label1.Text = text;
            label1.Visible = true;
            await  Task.Delay(2000);
            label1.Visible = false;
           
        }



        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (listQueAnsw.quetionAnswers[ind].otvet_int == 6)
            {
                trueQuetion++;
            }
            GenerateQution();
        }


    }

    class Satelite : Planet
    {
        public Satelite(int rotateRadius, double speed, PointF point): base(rotateRadius, speed, point){ }
        public Point Move(Point planetXY)
        {
            angle += Math.PI / speed;
            X = planetXY.X + (int)(distanceToSun * Math.Cos(angle));
            Y = planetXY.Y + (int)(distanceToSun * Math.Sin(angle));
            return new Point(X, Y);
        }
    }
    class Planet
    {
        protected double speed;
        protected int distanceToSun;
        protected double angle;
        PointF startLocation;
        public int X { get; set; }
        public int Y { get; set; }
        public Planet(int rotateRadius, double speed, PointF point)
        {
            this.speed = speed;
            angle = -190;
            distanceToSun = rotateRadius;
            startLocation = point;

        }
      
        public Point Move()
        {
            angle += Math.PI / speed;
            X = 470 + (int)(distanceToSun * Math.Cos(angle)) + 40;
            Y = 350 + (int)(distanceToSun * Math.Sin(angle));
            return new Point(X, Y);

        }

    }
}

