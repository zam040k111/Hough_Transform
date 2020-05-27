using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Accord.Imaging;
using Accord.Imaging.Filters;

namespace FigureDetection
{
    public class Processing
    {
        public Pen drawingPen { get; set; }
        public Bitmap LinesBackground { get; set; }
        public double Accuracy { get; set; }
        public Processing(Bitmap sourceImg)
        {
            drawingPen = new Pen(new SolidBrush(Color.FromArgb(200, Color.Red)), 6);
            LinesBackground = sourceImg.Clone() as Bitmap;
            Accuracy = 0.6;
        }
        public Bitmap SobelFilter(Bitmap img)
        {
            int[,] filter = { { -1, 0, 1 },
                              { -2, 0, 2 },
                              { -1, 0, 1 } };
            int[,] filter2 = { { -1, -2, -1 },
                               { 0, 0, 0 },
                               { 1, 2, 1 } };
            Bitmap bmp = CheckQuality(new Bitmap(img));
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int width = bmpData.Stride;
            int height = bmp.Size.Height;
            int bytes = bmp.Height * Math.Abs(bmpData.Stride);
            byte[] rgbValue = new byte[bytes];
            Marshal.Copy(ptr, rgbValue, 0, bytes);
            byte[] rgbNew = new byte[bytes];
            Marshal.Copy(ptr, rgbNew, 0, bytes);

            double r = 0, g = 0, b = 0;
            double r1 = 0, g1 = 0, b1 = 0;

            for (int i = 1; i < height - 1; i++)
                for (int j = 4; j < width - 8; j += 4)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        b += filter[k + 1, 0] * rgbValue[(i + k) * width + j - 4];
                        b += filter[k + 1, 1] * rgbValue[(i + k) * width + j];
                        b += filter[k + 1, 2] * rgbValue[(i + k) * width + j + 4];

                        g += filter[k + 1, 0] * rgbValue[(i + k) * width + j - 3];
                        g += filter[k + 1, 1] * rgbValue[(i + k) * width + j + 1];
                        g += filter[k + 1, 2] * rgbValue[(i + k) * width + j + 5];

                        r += filter[k + 1, 0] * rgbValue[(i + k) * width + j - 2];
                        r += filter[k + 1, 1] * rgbValue[(i + k) * width + j + 2];
                        r += filter[k + 1, 2] * rgbValue[(i + k) * width + j + 6];

                        b1 += filter2[k + 1, 0] * rgbValue[(i + k) * width + j - 4];
                        b1 += filter2[k + 1, 1] * rgbValue[(i + k) * width + j];
                        b1 += filter2[k + 1, 2] * rgbValue[(i + k) * width + j + 4];

                        g1 += filter2[k + 1, 0] * rgbValue[(i + k) * width + j - 3];
                        g1 += filter2[k + 1, 1] * rgbValue[(i + k) * width + j + 1];
                        g1 += filter2[k + 1, 2] * rgbValue[(i + k) * width + j + 5];

                        r1 += filter2[k + 1, 0] * rgbValue[(i + k) * width + j - 2];
                        r1 += filter2[k + 1, 1] * rgbValue[(i + k) * width + j + 2];
                        r1 += filter2[k + 1, 2] * rgbValue[(i + k) * width + j + 6];
                    }
                    r = Math.Sqrt((r * r) + (r1 * r1));
                    g = Math.Sqrt((g * g) + (g1 * g1));
                    b = Math.Sqrt((b * b) + (b1 * b1));

                    if (r < 0)
                        r = 0;
                    if (r > 255)
                        r = 255;
                    if (g < 0)
                        g = 0;
                    if (g > 255)
                        g = 255;
                    if (b < 0)
                        b = 0;
                    if (b > 255)
                        b = 255;
                    if (r1 < 0)
                        r1 = 0;
                    if (r1 > 255)
                        r1 = 255;
                    if (g1 < 0)
                        g1 = 0;
                    if (g1 > 255)
                        g1 = 255;
                    if (b1 < 0)
                        b1 = 0;
                    if (b1 > 255)
                        b1 = 255;
                    double r2 = r * 0.299 + g * 0.587 + b * 0.114;
                    double g2 = r * 0.299 + g * 0.587 + b * 0.114;
                    double b2 = r * 0.299 + g * 0.587 + b * 0.114;
                    rgbNew[i * width + j + 2] = (byte)r2;
                    rgbNew[i * width + j + 1] = (byte)g2;
                    rgbNew[i * width + j] = (byte)b2;
                    r = 0;
                    b = 0;
                    g = 0;
                    r1 = 0;
                    g1 = 0;
                    b1 = 0;
                }
            Marshal.Copy(rgbNew, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap CheckQuality(Bitmap img)
        {
            double coefficient;
            int newHeight, newWidth;
            if (img.Height > 600 || img.Width > 600)
            {
                if (img.Height > img.Width)
                    coefficient = img.Height / 600;
                else coefficient = img.Width / 600;
                newHeight = Convert.ToInt32(img.Height / coefficient);
                newWidth = Convert.ToInt32(img.Width / coefficient);
                return new Bitmap(img, newWidth, newHeight);
            }
            else return img;
        }

        public Bitmap InterferenceRemoval(Bitmap img) 
        {
            Bitmap bmp = new Bitmap(img);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmp.Height * Math.Abs(bmpData.Stride);
            byte[] rgbValue = new byte[bytes];
            Marshal.Copy(ptr, rgbValue, 0, bytes);
            byte[] rgbNew = new byte[bytes];
            Marshal.Copy(ptr, rgbNew, 0, bytes);

            for (int j = 4; j < bytes; j += 4)
            {
                if (rgbValue[j - 4] > 200 && rgbValue[j - 3] > 200 && rgbValue[j - 2] > 200)
                {
                    rgbNew[j - 2] = rgbValue[j - 2];
                    rgbNew[j - 3] = rgbValue[j - 3];
                    rgbNew[j - 4] = rgbValue[j - 4];
                }
                else
                {
                    rgbNew[j - 2] = 0;
                    rgbNew[j - 3] = 0;
                    rgbNew[j - 4] = 0;
                }
            }

            Marshal.Copy(rgbNew, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
        public Bitmap ColorInverse(Bitmap img)
        {
            Bitmap bmp = new Bitmap(img);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmp.Height * Math.Abs(bmpData.Stride);
            byte[] rgbValue = new byte[bytes];
            Marshal.Copy(ptr, rgbValue, 0, bytes);
            byte[] rgbNew = new byte[bytes];
            Marshal.Copy(ptr, rgbNew, 0, bytes);

            for (int j = 4; j < bytes; j += 4)
            {
                if (rgbValue[j - 4] > 200 && rgbValue[j - 3] > 200 && rgbValue[j - 2] > 200)
                {
                    rgbNew[j - 2] = 0;
                    rgbNew[j - 3] = 0;
                    rgbNew[j - 4] = 0;
                }
                else
                {
                    rgbNew[j - 2] = 255;
                    rgbNew[j - 3] = 255;
                    rgbNew[j - 4] = 255;
                }
            }

            Marshal.Copy(rgbNew, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
        public Bitmap HoughTransform(Bitmap img)
        {
            Bitmap bmp = new Bitmap(img);
            Graphics g = Graphics.FromImage(bmp);

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int width = bmpData.Stride;
            int widthImg = bmp.Width;
            int height = bmp.Height;
            int bytes = bmp.Height * Math.Abs(bmpData.Stride);
            byte[] rgbValue = new byte[bytes];
            Marshal.Copy(ptr, rgbValue, 0, bytes);

            bmp.UnlockBits(bmpData);
            //g.Clear(Color.Transparent);

            Pen p = new Pen(Color.Black, 1.0F);

            var numAngleCells = 360;
            var rhoMax = Math.Sqrt(widthImg * widthImg + height * height);

            int[,] accum = new int[numAngleCells, 2000];

            var cosTable = new double[numAngleCells];
            var sinTable = new double[numAngleCells];
            double theta = 0;
            for (int thetaIndex = 0; thetaIndex < numAngleCells; theta += Math.PI / numAngleCells, thetaIndex++)
            {
                cosTable[thetaIndex] = Math.Cos(theta);
                sinTable[thetaIndex] = Math.Sin(theta);
            }

            int x = 0;
            int y = 0;
            Brush sb = new SolidBrush(Color.FromArgb(25, Color.Black));

            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 4; j < width - 8; j += 4)
                {
                    if (rgbValue[i * width + j + 2] == 0 && rgbValue[i * width + j + 1] == 0 && rgbValue[i * width + j] == 0)
                    {
                        x = i;
                        y = j / 4;
                        int rho;
                        int thetaIndex = 0;


                        x -= widthImg / 2;
                        y -= height / 2;
                        for (; thetaIndex < numAngleCells; thetaIndex++)
                        {
                            rho = Convert.ToInt32(rhoMax + x * cosTable[thetaIndex] + y * sinTable[thetaIndex]);
                            rho >>= 1;

                            accum[thetaIndex, rho]++;
                        }
                    }
                }
            }
            return bmp;
        }
        public Bitmap HoughTransformViaAccord(Bitmap img)
        {
            var sequence = new FiltersSequence(Grayscale.CommonAlgorithms.BT709);

            Bitmap binaryImage = sequence.Apply(img);
                        
            //binaryImage.Save("binaryImage.png");

            var lineTransform = new HoughLineTransformation();
            lineTransform.ProcessImage(binaryImage);

            Bitmap houghLineImage = lineTransform.ToBitmap();

            
            //houghLineImage.Save("hough-output.png");
            

            HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity(Accuracy);
            //HoughLine[] lines = lineTransform.GetMostIntensiveLines(5);


            foreach (HoughLine line in lines)
            {
                // get line's radius and theta values
                int r = line.Radius;
                double t = line.Theta;

                // check if line is in lower part of the image
                if (r < 0)
                {
                    t += 180;
                    r = -r;
                }

                // convert degrees to radians
                t = (t / 180) * Math.PI;

                // get image centers (all coordinate are measured relative to center)
                int w2 = img.Width / 2;
                int h2 = img.Height / 2;

                double x0 = 0, x1 = 0, y0 = 0, y1 = 0;

                if (line.Theta != 0)
                {
                    // non-vertical line
                    x0 = -w2; // most left point
                    x1 = w2;  // most right point

                    // calculate corresponding y values
                    y0 = (-Math.Cos(t) * x0 + r) / Math.Sin(t);
                    y1 = (-Math.Cos(t) * x1 + r) / Math.Sin(t);
                }
                else
                {
                    // vertical line
                    x0 = line.Radius;
                    x1 = line.Radius;

                    y0 = h2;
                    y1 = -h2;
                }
                if (LinesBackground == null) LinesBackground = new Bitmap(binaryImage);
                Graphics g = Graphics.FromImage(LinesBackground);
                g.DrawLine(drawingPen, (int)x0 + w2, h2 - (int)y0, (int)x1 + w2, h2 - (int)y1);
            }
            return LinesBackground;
        }
    }
}
