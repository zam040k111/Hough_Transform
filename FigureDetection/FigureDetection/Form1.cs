using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.Util;
using DirectShowLib;
using Accord.Math;

namespace FigureDetection
{
    public partial class Form1 : Form
    {
        private Processing img;
        private Bitmap baseImg;
        private VideoCapture capture;
        private DsDevice[] webCams;
        private int selectedCamId;
        public Form1()
        {
            InitializeComponent();
            img = new Processing(pictureBox1.Image.Clone() as Bitmap);
            baseImg = pictureBox1.Image.Clone() as Bitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (исходноеИзображениеToolStripMenuItem.Checked)
                Lines_Background(null, new ToolStripItemClickedEventArgs(исходноеИзображениеToolStripMenuItem));
            if (обработанноеИзображениеToolStripMenuItem.Checked)
                Lines_Background(null, new ToolStripItemClickedEventArgs(обработанноеИзображениеToolStripMenuItem));
            if (чистыйФонToolStripMenuItem.Checked)
                Lines_Background(null, new ToolStripItemClickedEventArgs(чистыйФонToolStripMenuItem));
            pictureBox2.Image = img.SobelFilter(baseImg.Clone() as Bitmap);
            pictureBox2.Image = img.InterferenceRemoval(pictureBox2.Image as Bitmap);
            //pictureBox2.Image = img.ColorInverse(pictureBox2.Image as Bitmap);
            //pictureBox2.Image = img.HoughTransform(pictureBox2.Image as Bitmap, progressBar1);
            pictureBox2.Image = img.HoughTransformViaAccord(pictureBox2.Image as Bitmap);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                baseImg = Processing.CheckQuality(new Bitmap(openFileDialog1.FileName));
                pictureBox1.Image = baseImg;
                if (исходноеИзображениеToolStripMenuItem.Checked)
                    Lines_Background(null, new ToolStripItemClickedEventArgs(исходноеИзображениеToolStripMenuItem));
                if (обработанноеИзображениеToolStripMenuItem.Checked)
                    Lines_Background(null, new ToolStripItemClickedEventArgs(обработанноеИзображениеToolStripMenuItem));
                if (чистыйФонToolStripMenuItem.Checked)
                    Lines_Background(null, new ToolStripItemClickedEventArgs(чистыйФонToolStripMenuItem));
                pictureBox1.Invalidate();
            }
        }

        private void Line_Color_Changed(object sender, ToolStripItemClickedEventArgs e)
        {
            черныйToolStripMenuItem.Checked = false;
            красныйToolStripMenuItem.Checked = false;
            желтыйToolStripMenuItem.Checked = false;
            зеленыйToolStripMenuItem.Checked = false;
            switch (e.ClickedItem.Text)
            {
                case "Черный":
                    img.drawingPen.Color = Color.Black;
                    черныйToolStripMenuItem.Checked = true;
                    break;
                case "Красный":
                    img.drawingPen.Color = Color.Red;
                    красныйToolStripMenuItem.Checked = true;
                    break;
                case "Желтый":
                    img.drawingPen.Color = Color.Yellow;
                    желтыйToolStripMenuItem.Checked = true;
                    break;
                case "Зеленый":
                    img.drawingPen.Color = Color.Green;
                    зеленыйToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }
            button2_Click(sender, e);
        }

        private void Line_Width_Changed(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripMenuItem2.Checked = false;
            toolStripMenuItem3.Checked = false;
            toolStripMenuItem4.Checked = false;
            toolStripMenuItem5.Checked = false;
            toolStripMenuItem6.Checked = false;
            switch (e.ClickedItem.Text)
            {
                case "2 px":
                    img.drawingPen.Width = 2;
                    toolStripMenuItem2.Checked = true;
                    break;
                case "4 px":
                    img.drawingPen.Width = 4;
                    toolStripMenuItem3.Checked = true;
                    break;
                case "6 px":
                    img.drawingPen.Width = 6;
                    toolStripMenuItem4.Checked = true;
                    break;
                case "8 px":
                    img.drawingPen.Width = 8;
                    toolStripMenuItem5.Checked = true;
                    break;
                case "10 px":
                    img.drawingPen.Width = 10;
                    toolStripMenuItem6.Checked = true;
                    break;
                default:
                    break;
            }
            button2_Click(sender, e);
        }

        private void Line_Transparent_Changed(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripMenuItem7.Checked = false;
            toolStripMenuItem8.Checked = false;
            toolStripMenuItem9.Checked = false;
            toolStripMenuItem10.Checked = false;
            toolStripMenuItem11.Checked = false;
            switch (e.ClickedItem.Text)
            {
                case "50":
                    img.drawingPen.Color = Color.FromArgb(50, img.drawingPen.Color);
                    toolStripMenuItem7.Checked = true;
                    break;
                case "100":
                    img.drawingPen.Color = Color.FromArgb(100, img.drawingPen.Color);
                    toolStripMenuItem8.Checked = true;
                    break;
                case "150":
                    img.drawingPen.Color = Color.FromArgb(150, img.drawingPen.Color);
                    toolStripMenuItem9.Checked = true;
                    break;
                case "200":
                    img.drawingPen.Color = Color.FromArgb(200, img.drawingPen.Color);
                    toolStripMenuItem10.Checked = true;
                    break;
                case "250":
                    img.drawingPen.Color = Color.FromArgb(250, img.drawingPen.Color);
                    toolStripMenuItem11.Checked = true;
                    break;
                default:
                    break;
            }
            button2_Click(sender, e);
        }

        private void Lines_Background(object sender, ToolStripItemClickedEventArgs e)
        {
            исходноеИзображениеToolStripMenuItem.Checked = false;
            обработанноеИзображениеToolStripMenuItem.Checked = false;
            чистыйФонToolStripMenuItem.Checked = false;
            switch (e.ClickedItem.Text)
            {
                case "Исходное изображение":
                    img.LinesBackground = pictureBox1.Image.Clone() as Bitmap;
                    исходноеИзображениеToolStripMenuItem.Checked = true;
                    break;
                case "Обработанное изображение":
                    img.LinesBackground = null;
                    обработанноеИзображениеToolStripMenuItem.Checked = true;
                    break;
                case "Чистый фон":
                    Bitmap tmp = pictureBox1.Image.Clone() as Bitmap;
                    Graphics.FromImage(tmp).Clear(Color.White);
                    img.LinesBackground = tmp;
                    чистыйФонToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }
        }

        private void accuracy_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripMenuItem12.Checked = false;
            toolStripMenuItem13.Checked = false;
            toolStripMenuItem14.Checked = false;
            toolStripMenuItem15.Checked = false;
            toolStripMenuItem16.Checked = false;
            switch (e.ClickedItem.Text)
            {
                case "Самый высокий":
                    img.Accuracy = 0.2;
                    trackBar1.Value = Convert.ToInt32(0.2 * 20);
                    toolStripMenuItem12.Checked = true;
                    break;
                case "Высокий":
                    img.Accuracy = 0.4;
                    trackBar1.Value = Convert.ToInt32(0.4 * 20);
                    toolStripMenuItem13.Checked = true;
                    break;
                case "Нормальный":
                    img.Accuracy = 0.6;
                    trackBar1.Value = Convert.ToInt32(0.6* 20);
                    toolStripMenuItem14.Checked = true;
                    break;
                case "Низкий":
                    img.Accuracy = 0.8;
                    trackBar1.Value = Convert.ToInt32(0.8 * 20);
                    toolStripMenuItem15.Checked = true;
                    break;
                case "Самый низкий":
                    img.Accuracy = 1;
                    trackBar1.Value = 20;
                    toolStripMenuItem16.Checked = true;
                    break;
                default:
                    break;
            }
            button2_Click(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webCams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            for (int i = webCams.Length - 1; i >= 0; i--)
                toolStripComboBox1.Items.Add(webCams[i].Name);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCamId = toolStripComboBox1.SelectedIndex;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (webCams.Length == 0)
                    throw new Exception("Камер не обнаружено");
                else if (toolStripComboBox1.SelectedItem == null)
                    throw new Exception("Выберите камеру из списка доступных");
                else if (capture != null)
                    capture.Start();
                else
                {
                    capture = new VideoCapture(selectedCamId);
                    capture.ImageGrabbed += Capture_ImageGrabbed;
                    capture.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat mat = new Mat();
                capture.Retrieve(mat);
                Image im = mat.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal).Bitmap;
                if (исходноеИзображениеToolStripMenuItem.Checked)
                    img.LinesBackground = im as Bitmap;
                if (обработанноеИзображениеToolStripMenuItem.Checked)
                    img.LinesBackground = null;
                if (чистыйФонToolStripMenuItem.Checked)
                {
                    Bitmap tmp = im.Clone() as Bitmap;
                    Graphics.FromImage(tmp).Clear(Color.White);
                    img.LinesBackground = tmp;
                }
                im = img.SobelFilter(im as Bitmap);
                im = img.InterferenceRemoval(im as Bitmap);
                pictureBox3.Image = img.HoughTransformViaAccord(im as Bitmap);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (capture != null)
                    capture.Pause();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            try
            {
                if (capture != null)
                {
                    capture.Pause();
                    capture.Dispose();
                    capture = null;
                    pictureBox3.Image.Dispose();
                    pictureBox3.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    Bitmap im = pictureBox2.Image as Bitmap;
                    if (im != null)
                        im.Save(saveFileDialog1.FileName);
                    else throw new Exception("Обработаной картинки не обнаружено");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            toolStripMenuItem12.Checked = false;
            toolStripMenuItem13.Checked = false;
            toolStripMenuItem14.Checked = false;
            toolStripMenuItem15.Checked = false;
            toolStripMenuItem16.Checked = false;
            img.Accuracy = Convert.ToDouble(trackBar1.Value) / 20;
            button2_Click(sender, e);
        }
    }
}
