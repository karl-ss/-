using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarDemo
{
    public partial class Form1 : Form
    {
        private String name;
        private Bitmap m_Bitmap;//存放最先打开的图片
        private Bitmap always_Bitmap;
        int[] gray = new int[256];
        int flag = 0, flag1 = 0;
        float[,] m = new float[5000, 5000];
        private float count;
        private float[] gl = new float[256];
        private Bitmap c_Bitmap;//车牌图像
        Pen pen1 = new Pen(Color.Black);
        private Bitmap extract_Bitmap_one;
        private Bitmap extract_Bitmap_two;
        private Bitmap z_Bitmap0;
        private Bitmap z_Bitmap1;
        private Bitmap z_Bitmap2;
        private Bitmap z_Bitmap3;
        private Bitmap z_Bitmap4;
        private Bitmap z_Bitmap5;
        private Bitmap z_Bitmap6;
        private Bitmap[] z_Bitmaptwo = new Bitmap[7];//用于储存最终的黑白字体
        private Bitmap objNewPic;
        private int max = 0;
        private Bitmap srcBitmap = null;
        private byte[] Mcoarse = null;
        private byte[] Mfine = null;
        private double[] tx = null;
        private byte[] Ix = null;
        string[] charString;//存储的路径
        string[] provinceString;//省份字体
        string[] charDigitalString;
        string[] provinceDigitalString;
        private Bitmap[] charFont;
        private Bitmap[] provinceFont;
        public static string charSourceBath = "..\\..\\MYsource\\char\\";
        public static string provinceSourceBath = "..\\..\\MYsource\\font\\";

      

        public Form1()
        {
            InitializeComponent();
            panelMain3.Visible = false;
            panelMain4.Visible = false;
        }


        /// <summary>
        /// 文件操作按键
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            panelMain3.Visible = !panelMain3.Visible;
        }

        /// <summary>
        /// 图片操作按键
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            panelMain4.Visible = !panelMain4.Visible;
        }

        /// <summary>
        /// 打开图片
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            this.clearAllPanel();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp| 所有合适文件(*.bmp/*.jpg)|*.bmp/*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                name = openFileDialog.FileName;
                m_Bitmap = (Bitmap)Bitmap.FromFile(name, false);
                this.always_Bitmap = m_Bitmap.Clone(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), PixelFormat.DontCare);
                this.panel1.AutoScroll = true;
                this.panel1.AutoScrollMinSize = new Size((int)(m_Bitmap.Width), (int)m_Bitmap.Height);
                panel1.Invalidate();//使panel1内的图像重新绘制

            }
        }
        private void clearAllPanel()
        {
            Graphics g = this.LocatedPanel.CreateGraphics();
            g.Clear(this.LocatedPanel.BackColor);
            g = this.ExtractPanel.CreateGraphics();
            g.Clear(this.ExtractPanel.BackColor);
            g = this.FontPanel1.CreateGraphics();
            g.Clear(this.FontPanel1.BackColor);
            g = this.FontPanel2.CreateGraphics();
            g.Clear(this.FontPanel2.BackColor);
            g = this.FontPanel3.CreateGraphics();
            g.Clear(this.FontPanel3.BackColor);
            g = this.FontPanel4.CreateGraphics();
            g.Clear(this.FontPanel4.BackColor);
            g = this.FontPanel5.CreateGraphics();
            g.Clear(this.FontPanel5.BackColor);
            g = this.FontPanel6.CreateGraphics();
            g.Clear(this.FontPanel6.BackColor);
            g = this.FontPanel7.CreateGraphics();
            g.Clear(this.FontPanel7.BackColor);
            this.textBox1.Text = "";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (m_Bitmap != null)
            {
                Graphics g = e.Graphics;
                if (m_Bitmap.Width < panel1.Width && m_Bitmap.Height < panel1.Height)
                {
                    g.DrawImage(m_Bitmap, new Rectangle((this.panel1.Width - m_Bitmap.Width) / 2, (this.panel1.Height - m_Bitmap.Height) / 2, (int)(m_Bitmap.Width), (int)(m_Bitmap.Height)));
                }
                else
                {
                    g.DrawImage(m_Bitmap, new Rectangle(this.panel1.AutoScrollPosition.X, this.panel1.AutoScrollPosition.Y, (int)(m_Bitmap.Width), (int)(m_Bitmap.Height)));
                }
            }
        }

        /// <summary>
        /// 灰度化
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            if (m_Bitmap != null)
            {
                int tt = 0;
                for (int i = 0; i < 256; i++)
                {
                    gray[i] = 0;
                }
                BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - m_Bitmap.Width * 3;
                    byte red, green, blue;
                    int nWidth = m_Bitmap.Width;
                    int nHeight = m_Bitmap.Height;
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];
                            tt = p[0] = p[1] = p[2] = (byte)(0.299 * red + 0.587 * green + 0.114 * blue);
                            gray[tt]++;     
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                m_Bitmap.UnlockBits(bmData);
                flag = 1;
                graydo();
            }
        }

        /// <summary>
        /// 灰度均衡化
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (m_Bitmap != null)
            {
                BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                int tt = 0;
                int[] SumGray = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    SumGray[i] = 0;
                }
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - m_Bitmap.Width * 3;
                    int nHeight = m_Bitmap.Height;
                    int nWidth = m_Bitmap.Width;
                    SumGray[0] = gray[0]; 
                    for (int i = 1; i < 256; ++i)                        
                        SumGray[i] = SumGray[i - 1] + gray[i];
                    for (int i = 0; i < 256; ++i)                    
                        SumGray[i] = (int)(SumGray[i] * 255 / count);
                    for (int i = 0; i < 256; i++)
                    {
                        gray[i] = 0;
                    }
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            tt = p[0] = p[1] = p[2] = (byte)(SumGray[p[0]]);
                            gray[tt]++;
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                m_Bitmap.UnlockBits(bmData);
                flag = 1;
                graydo();
            }
        }

        /// <summary>
        /// 中值滤波
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            if (m_Bitmap != null)
            {
                BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                for (int i = 0; i < 256; i++)
                {
                    gray[i] = 0;
                }
                unsafe
                {
                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;
                    byte* p = (byte*)(void*)Scan0;
                    byte* pp;
                    int tt;
                    int nOffset = stride - m_Bitmap.Width * 3;
                    int nWidth = m_Bitmap.Width;
                    int nHeight = m_Bitmap.Height;
                    long sum = 0;
                    int[,] gaussianMatrix = { { 1, 2, 3, 2, 1 }, { 2, 4, 6, 4, 2 }, { 3, 6, 7, 6, 3 }, { 2, 4, 6, 4, 2 }, { 1, 2, 3, 2, 1 } };
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if (!(x <= 1 || x >= nWidth - 2 || y <= 1 || y >= nHeight - 2))
                            {
                                pp = p;
                                sum = 0;
                                int dividend = 79;
                                for (int i = -2; i <= 2; i++)
                                    for (int j = -2; j <= 2; j++)
                                    {
                                        pp += (j * 3 + stride * i);
                                        sum += pp[0] * gaussianMatrix[i + 2, j + 2];
                                        if (i == 0 && j == 0)
                                        {
                                            if (pp[0] > 240)
                                            {
                                                sum += p[0] * 30;
                                                dividend += 30;
                                            }
                                            else if (pp[0] > 230)
                                            {
                                                sum += pp[0] * 20;
                                                dividend += 20;
                                            }
                                            else if (pp[0] > 220)
                                            {
                                                sum += p[0] * 15;
                                                dividend += 15;
                                            }
                                            else if (pp[0] > 210)
                                            {
                                                sum += pp[0] * 10;
                                                dividend += 10;
                                            }
                                            else if (p[0] > 200)
                                            {
                                                sum += pp[0] * 5;
                                                dividend += 5;
                                            }
                                        }
                                        pp = p;
                                    }
                                sum = sum / dividend;
                                if (sum > 255)
                                {
                                    sum = 255;
                                }
                                p[0] = p[1] = p[2] = (byte)(sum);
                            }
                            tt = p[0];
                            gray[tt]++;
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                flag = 1;
                m_Bitmap.UnlockBits(bmData);
                graydo();
            }
        }

        /// <summary>
        /// 边缘检测
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            if (m_Bitmap != null)
            {
                BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                float valve = 67;
                for (int i = 0; i < 256; i++)
                {
                    gray[i] = 0;
                }
                unsafe
                {
                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;
                    byte* p = (byte*)(void*)Scan0;
                    byte* pp;
                    int tt;
                    int nOffset = stride - m_Bitmap.Width * 3;
                    int nWidth = m_Bitmap.Width;
                    int nHeight = m_Bitmap.Height;
                    int Sx = 0;
                    int Sy = 0;
                    double sumM = 0;
                    double sumCount = 0;
                    int[] marginalMx = { -1, 0, 1, -2, 0, 2, -1, 0, 1 };   
                    int[] marginalMy = { 1, 2, 1, 0, 0, 0, -1, -2, -1 };
                    int[,] dlta = new int[nHeight, nWidth];
                    for (int y = 0; y < nHeight; ++y)      
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if (!(x <= 0 || x >= nWidth - 1 || y <= 0 || y >= nHeight - 1))
                            {
                                pp = p;
                                Sx = 0;
                                Sy = 0;
                                for (int i = -1; i <= 1; i++)
                                    for (int j = -1; j <= 1; j++)
                                    {
                                        pp += (j * 3 + stride * i);
                                        Sx += pp[0] * marginalMx[(i + 1) * 3 + j + 1];
                                        Sy += pp[0] * marginalMy[(i + 1) * 3 + j + 1];
                                        pp = p;
                                    }
                                m[y, x] = (int)(Math.Sqrt(Sx * Sx + Sy * Sy));
                                if (m[y, x] > valve / 2) 
                                {
                                    if (p[0] > 240)
                                    {
                                        m[y, x] += valve;
                                    }
                                    else if (p[0] > 220)
                                    {
                                        m[y, x] += (float)(valve * 0.8);
                                    }
                                    else if (p[0] > 200)
                                    {
                                        m[y, x] += (float)(valve * 0.6);
                                    }
                                    else if (p[0] > 180)
                                    {
                                        m[y, x] += (float)(valve * 0.4);
                                    }
                                    else if (p[0] > 160)
                                    {
                                        m[y, x] += (float)(valve * 0.2);
                                    }
                                }
                                float tan;
                                if (Sx != 0)
                                {
                                    tan = Sy / Sx;
                                }
                                else tan = 10000;
                                if (-0.41421356 <= tan && tan < 0.41421356)//角度为-22.5度到22.5度之间
                                {
                                    dlta[y, x] = 0;                                 
                                }
                                else if (0.41421356 <= tan && tan < 2.41421356)//角度为22.5度到67.5度之间
                                {
                                    dlta[y, x] = 1; 
                                }
                                else if (tan >= 2.41421356 || tan < -2.41421356)//角度为67.5度到90度之间或-90度到-67.5度
                                {
                                    dlta[y, x] = 2;    
                                }
                                else
                                {
                                    dlta[y, x] = 3; 
                                }
                            }
                            else
                                m[y, x] = 0;
                            p += 3;
                            if (m[y, x] > 0)
                            {
                                sumCount++;
                                sumM += m[y, x];
                            }
                        }
                        p += nOffset;

                    }
                    p = (byte*)(void*)Scan0; 
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if (m[y, x] > sumM / sumCount * 1.2)
                            {
                                p[0] = p[1] = p[2] = (byte)(m[y, x]);                           
                            }
                            else
                            {
                                m[y, x] = 0;
                                p[0] = p[1] = p[2] = 0;
                            }
                            if (x >= 1 && x <= nWidth - 1 && y >= 1 && y <= nHeight - 1 && m[y, x] > valve)
                            {
                                switch (dlta[y, x])
                                {
                                    case 0:
                                        if (m[y, x] >= m[y, x - 1] && m[y, x] >= m[y, x + 1])//水平边缘
                                        {
                                            p[0] = p[1] = p[2] = 255;
                                        }
                                        break;

                                    case 1:
                                        if (m[y, x] >= m[y + 1, x - 1] && m[y, x] >= m[y - 1, x + 1])//正斜45度边缘
                                        {
                                            p[0] = p[1] = p[2] = 255;
                                        }
                                        break;

                                    case 2:
                                        if (m[y, x] >= m[y - 1, x] && m[y, x] >= m[y + 1, x])//垂直边缘
                                        {
                                            p[0] = p[1] = p[2] = 255;
                                        }
                                        break;

                                    case 3:
                                        if (m[y, x] >= m[y + 1, x + 1] && m[y, x] >= m[y - 1, x - 1])//反斜45度边缘
                                        {
                                            p[0] = p[1] = p[2] = 255;
                                        }
                                        break;
                                }
                            }
                            if (p[0] == 255)
                            {
                                m[y, x] = 1;
                            }
                            else
                            {
                                m[y, x] = 0;
                                p[0] = p[1] = p[2] = 0;
                            }

                            tt = p[0];
                            gray[tt]++;
                            p += 3;
                        }
                        //  p += nOffset;
                    }
                    m_Bitmap.UnlockBits(bmData);
                    flag = 1;
                    graydo();
                }
            }
        }

        /// <summary>
        /// 车牌定位
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            this.c_Bitmap = Recoginzation.licensePlateLocation(m_Bitmap, always_Bitmap, m);
            extract_Bitmap_one = c_Bitmap.Clone(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height), PixelFormat.DontCare);
            this.panel1.Invalidate();
            this.LocatedPanel.Invalidate();
            this.ExtractPanel.Invalidate();
        }

        private void LocatedPanel_Paint(object sender, PaintEventArgs e)
        {
            if (c_Bitmap != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(c_Bitmap, 0, 0, this.LocatedPanel.Width, LocatedPanel.Height);
            }
            else
            {
                Graphics g = this.LocatedPanel.CreateGraphics();
                g.Clear(this.LocatedPanel.BackColor);
            }
        }

        private void ExtractPanel_Paint(object sender, PaintEventArgs e)
        {
            if (extract_Bitmap_two != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(extract_Bitmap_two, 0, 0, this.ExtractPanel.Width, ExtractPanel.Height);
            }
            else
            {
                Graphics g = this.ExtractPanel.CreateGraphics();
                g.Clear(this.ExtractPanel.BackColor);
            }
        }



        /// <summary>
        /// 车牌灰度化
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            if (c_Bitmap != null)
            {
                int tt = 0;
                for (int i = 0; i < 256; i++)
                {
                    gray[i] = 0;
                }
                BitmapData bmData = c_Bitmap.LockBits(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - c_Bitmap.Width * 3;
                    byte red, green, blue;
                    int nWidth = c_Bitmap.Width;
                    int nHeight = c_Bitmap.Height;
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];
                            tt = p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);
                            gray[tt]++;
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                c_Bitmap.UnlockBits(bmData);
                flag = 2;
                this.LocatedPanel.Invalidate();
                panel1.Invalidate();
                graydo();
            }
        }

        /// <summary>
        /// 车牌二值化
        /// </summary>
        private void button10_Click(object sender, EventArgs e)
        {
            if (c_Bitmap != null)
            {
                int Mr = 0;
                long sum = 0;
                int count = 0;
                for (int i = 0; i < 256; i++)
                {
                    sum += gray[i] * i;
                    count += gray[i];
                }
                Mr = (int)(sum / count);
                int sum1 = 0;
                int count1 = 0;
                for (int i = 0; i <= Mr; i++)
                {
                    sum1 += gray[i] * i;
                    count1 += gray[i];
                }
                int g1 = sum1 / count1;

                int sum2 = 0;
                int count2 = 0;
                for (int i = Mr; i <= 255; i++)
                {
                    sum2 += gray[i] * i;
                    count2 += gray[i];
                }
                int g2 = sum2 / count2;
                int va;
                if (count1 < count2)
                {
                    va = Mr - count1 / count2 * Math.Abs(g1 - Mr);
                }
                else
                {
                    va = Mr + count2 / count1 * Math.Abs(g2 - Mr);
                }
                BitmapData bmData = c_Bitmap.LockBits(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - c_Bitmap.Width * 3;

                    int nWidth = c_Bitmap.Width;
                    int nHeight = c_Bitmap.Height;


                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if (p[0] > va)
                            {
                                p[0] = p[1] = p[2] = 255;
                            }
                            else
                                p[0] = p[1] = p[2] = 0;

                            p += 3;
                        }
                        p += nOffset;

                    }

                }
                c_Bitmap.UnlockBits(bmData);
                LocatedPanel.Invalidate();
            }
        }

        /// <summary>
        /// 车牌区域化
        /// </summary>
        private void button11_Click(object sender, EventArgs e)
        {
            if (c_Bitmap != null)
            {
                flag1 = 1;
                BitmapData bmData = c_Bitmap.LockBits(new Rectangle(0, 0, c_Bitmap.Width, c_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    int stride = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - c_Bitmap.Width * 3;

                    int nWidth = c_Bitmap.Width;
                    int nHeight = c_Bitmap.Height;
                    int[] countHeight = new int[nHeight];
                    int[] countWidth = new int[nWidth];
                    int Yheight = nHeight, YBottom = 0;
                    for (int i = 0; i < nHeight; i++)
                    {
                        countHeight[i] = 0;
                    }
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if ((p[0] == 0 && p[3] == 255) || (p[0] == 255 && p[3] == 0))
                            {
                                countHeight[y]++;
                            }


                            p += 3;
                        }
                        Console.WriteLine(y + "*******************跳变数     " + countHeight[y]);
                        p += nOffset;

                    }
                    for (int y = nHeight / 2; y > 0; y--)
                    {
                        if (countHeight[y] >= 16 && countHeight[(y + 1) % nHeight] >= 12)//12,6,11
                        {
                            if (Yheight > y)
                            {
                                Yheight = y;
                            }
                            if ((Yheight - y) == 1)
                            { Yheight = y - 3; Console.WriteLine("------------" + Yheight); }
                        }

                    }
                    for (int y = nHeight / 2; y < nHeight; y++)
                    {
                        if (countHeight[y] >= 12 && countHeight[(y + 1) % nHeight] >= 12)
                        {
                            if (YBottom < y)
                            {
                                YBottom = y;
                            }
                            if ((y - YBottom) == 1)
                            { YBottom = y + 3; Console.WriteLine("------------" + YBottom); }
                        }
                    }
                    YBottom += 1;
                    byte* p1 = (byte*)(void*)Scan0;
                    p1 += stride * (Yheight - 1);
                    for (int y = Yheight; y < YBottom; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {

                            if (p1[0] == 255)
                                countWidth[x]++;

                            p1 += 3;
                        }
                        p1 += nOffset;
                    }
                    int contg = 0, contd = 0, countRightEdge = 0, countLeftEdge = 0, Yl = nWidth, Yr = 0;
                    int[] XLeft = new int[20];
                    int[] XRight = new int[20];
                    foreach (int i in XRight)
                    {
                        XRight[i] = 0;
                    }
                    foreach (int i in XLeft)
                    {
                        XLeft[i] = 0;
                    }
                    for (int y = 1; y < YBottom - Yheight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            if (countWidth[(x + 1) % nWidth] < y && countWidth[x] >= y && countWidth[Math.Abs((x - 1) % nWidth)] >= y && contg >= 2)
                            {
                                if (countRightEdge == 6)
                                { Yr = x; }
                                if ((countRightEdge == 2 && (x >= XLeft[2] && XLeft[2] > 0)))
                                {
                                    XRight[countRightEdge] = x;
                                    countRightEdge++;
                                    contd = 0;
                                }
                                else
                                {
                                    if ((countRightEdge != 2))
                                    {
                                        if (countRightEdge == 0 && contg < 4)
                                        {
                                            XLeft[0] = 0;
                                            countLeftEdge = 0;
                                        }
                                        if ((x >= XLeft[0] && XLeft[0] > 0))
                                        {
                                            XRight[countRightEdge] = x;
                                            countRightEdge++;
                                            contd = 0;
                                        }
                                    }
                                }
                            }
                            if (countWidth[Math.Abs((x - 1) % nWidth)] < y && countWidth[x] >= y && countWidth[(x + 1) % nWidth] >= y && contd >= 2)
                            {
                                if (countLeftEdge == 0 && countWidth[(x + 2) % nWidth] >= y)
                                { Yl = x; }
                                if ((countLeftEdge == 2 && contd > 5))
                                {
                                    XLeft[countLeftEdge] = x;
                                    countLeftEdge++;

                                }
                                else
                                {
                                    if ((countLeftEdge != 2))
                                    {
                                        XLeft[countLeftEdge] = x;
                                        countLeftEdge++;
                                        contg = 0;
                                        if (countLeftEdge == 0 && countWidth[(x + 2) % nWidth] < y)
                                        {
                                            XLeft[0] = 0;
                                            countLeftEdge = 0;
                                        }

                                    }
                                }
                            }
                            contg++;
                            contd++;

                        }
                        if (countRightEdge + countLeftEdge >= 14)
                        {
                            break;
                        }
                        countRightEdge = 0;
                        countLeftEdge = 0;
                        for (int i = 0; i < XRight.Length; i++)
                        {
                            XRight[i] = 0;
                        }
                        for (int i = 0; i < XLeft.Length; i++)
                        {
                            XLeft[i] = 0;
                        }
                    }
                    c_Bitmap.UnlockBits(bmData);
                    if ((YBottom - Yheight) > 1 && (Yr - Yl) > 1)
                    {
                        Rectangle sourceRectangle = new Rectangle(Yl, Yheight, Yr - Yl, YBottom - Yheight);
                        extract_Bitmap_two = extract_Bitmap_one.Clone(sourceRectangle, PixelFormat.DontCare);
                        BitmapData bmData2 = extract_Bitmap_two.LockBits(new Rectangle(0, 0, extract_Bitmap_two.Width, extract_Bitmap_two.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        int stride2 = bmData2.Stride;
                        System.IntPtr Scan02 = bmData2.Scan0;
                        byte* p2 = (byte*)(void*)Scan02;
                        int nOffset2 = stride2 - extract_Bitmap_two.Width * 3;

                        int nWidth2 = extract_Bitmap_two.Width;
                        int nHeight2 = extract_Bitmap_two.Height;
                        for (int y = 0; y < nHeight2; ++y)
                        {
                            for (int x = 0; x < nWidth2; ++x)
                            {

                                if (x == (XRight[0] - Yl) || x == (XLeft[0] - Yl) || x == (XRight[1] - Yl) || x == (XLeft[1] - Yl) || x == (XRight[2] - Yl) || x == (XLeft[2] - Yl) || x == (XRight[3] - Yl) || x == (XLeft[3] - Yl) || x == (XRight[4] - Yl) || x == (XLeft[4] - Yl) || x == (XRight[5] - Yl) || x == (XLeft[5] - Yl) || x == (XRight[6] - Yl) || x == (XLeft[6] - Yl) || x == (XRight[7] - Yl) || x == (XLeft[7] - Yl))
                                {
                                    if (x != 0)
                                    {
                                        p2[2] = 255; p2[0] = p2[1] = 0;
                                    }
                                }

                                p2 += 3;
                            }
                            p2 += nOffset2;

                        }
                        extract_Bitmap_two.UnlockBits(bmData2);
                        this.ExtractPanel.Invalidate();
                        if ((YBottom - Yheight) > 1 && (XRight[1] - XLeft[1]) > 1)
                        {

                            Rectangle sourceRectangle2 = new Rectangle(XLeft[1], Yheight, XRight[1] - XLeft[1], YBottom - Yheight);
                            z_Bitmap1 = extract_Bitmap_one.Clone(sourceRectangle2, PixelFormat.DontCare);
                            z_Bitmaptwo[1] = c_Bitmap.Clone(sourceRectangle2, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[1], 9, 16);
                            z_Bitmaptwo[1] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/1.bmp");
                            objNewPic = null;
                            FontPanel2.Invalidate();
                        }
                        if ((YBottom - Yheight) > 1 && (XRight[2] - XLeft[2]) > 1)
                        {

                            Rectangle sourceRectangle3 = new Rectangle(XLeft[2], Yheight, XRight[2] - XLeft[2], YBottom - Yheight);
                            z_Bitmap2 = extract_Bitmap_one.Clone(sourceRectangle3, PixelFormat.DontCare);
                            z_Bitmaptwo[2] = c_Bitmap.Clone(sourceRectangle3, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[2], 9, 16);
                            z_Bitmaptwo[2] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/2.bmp");
                            objNewPic = null;
                            FontPanel3.Invalidate();
                        }
                        if ((YBottom - Yheight) > 1 && (XRight[3] - XLeft[3]) > 1)
                        {
                            Rectangle sourceRectangle4 = new Rectangle(XLeft[3], Yheight, XRight[3] - XLeft[3], YBottom - Yheight);
                            z_Bitmap3 = extract_Bitmap_one.Clone(sourceRectangle4, PixelFormat.DontCare);
                            z_Bitmaptwo[3] = c_Bitmap.Clone(sourceRectangle4, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[3], 9, 16);
                            z_Bitmaptwo[3] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/3.bmp");
                            objNewPic = null;
                            FontPanel4.Invalidate();
                        }
                        if ((YBottom - Yheight) > 1 && (XRight[4] - XLeft[4]) > 1)
                        {
                            Rectangle sourceRectangle5 = new Rectangle(XLeft[4], Yheight, XRight[4] - XLeft[4], YBottom - Yheight);
                            z_Bitmap4 = extract_Bitmap_one.Clone(sourceRectangle5, PixelFormat.DontCare);
                            z_Bitmaptwo[4] = c_Bitmap.Clone(sourceRectangle5, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[4], 9, 16);
                            z_Bitmaptwo[4] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/4.bmp");
                            objNewPic = null;
                            FontPanel5.Invalidate();
                        }
                        if ((YBottom - Yheight) > 1 && (XRight[5] - XLeft[5]) > 1)
                        {
                            Rectangle sourceRectangle6 = new Rectangle(XLeft[5], Yheight, XRight[5] - XLeft[5], YBottom - Yheight);
                            z_Bitmap5 = extract_Bitmap_one.Clone(sourceRectangle6, PixelFormat.DontCare);
                            z_Bitmaptwo[5] = c_Bitmap.Clone(sourceRectangle6, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[5], 9, 16);
                            z_Bitmaptwo[5] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/5.bmp");
                            objNewPic = null;
                            FontPanel6.Invalidate();
                        }
                        if ((YBottom - Yheight) > 1 && (XRight[6] - XLeft[6]) > 1)
                        {

                            Rectangle sourceRectangle7 = new Rectangle(XLeft[6], Yheight, XRight[6] - XLeft[6], YBottom - Yheight);
                            z_Bitmap6 = extract_Bitmap_one.Clone(sourceRectangle7, PixelFormat.DontCare);
                            z_Bitmaptwo[6] = c_Bitmap.Clone(sourceRectangle7, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[6], 9, 16);
                            z_Bitmaptwo[6] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/6.bmp");
                            objNewPic = null;
                            FontPanel7.Invalidate();
                        }
                        if ((YBottom - Yheight) > 1 && (XRight[0] - XLeft[0]) > 1)
                        {

                            Rectangle sourceRectangle0 = new Rectangle(XLeft[0], Yheight, XRight[0] - XLeft[0], YBottom - Yheight);
                            z_Bitmap0 = extract_Bitmap_one.Clone(sourceRectangle0, PixelFormat.DontCare);
                            z_Bitmaptwo[0] = c_Bitmap.Clone(sourceRectangle0, PixelFormat.DontCare);
                            objNewPic = new System.Drawing.Bitmap(z_Bitmaptwo[0], 9, 16);
                            z_Bitmaptwo[0] = objNewPic;
                            objNewPic.Save("../../MYsource/Resource/0.bmp");
                            objNewPic = null;
                            FontPanel1.Invalidate();
                        }

                    }

                }
            }
        }

        private void FontPanel1_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap0 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap0, 0, 0, FontPanel1.Width, FontPanel1.Height);
            }
            else
            {
                Graphics g = this.FontPanel1.CreateGraphics();
                g.Clear(this.FontPanel1.BackColor);
            }
        }

        private void FontPanel2_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap1 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap1, 0, 0, FontPanel2.Width, FontPanel2.Height);
            }
            else
            {
                Graphics g = this.FontPanel2.CreateGraphics();
                g.Clear(this.FontPanel2.BackColor);
            }
        }

        private void FontPanel3_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap2 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap2, 0, 0, FontPanel3.Width, FontPanel3.Height);
            }
            else
            {
                Graphics g = this.FontPanel3.CreateGraphics();
                g.Clear(this.FontPanel3.BackColor);
            }
        }

        private void FontPanel4_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap3 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap3, 0, 0, FontPanel4.Width, FontPanel4.Height);
            }
            else
            {
                Graphics g = this.FontPanel4.CreateGraphics();
                g.Clear(this.FontPanel4.BackColor);
            }
        }

        private void FontPanel5_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap4 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap4, 0, 0, FontPanel5.Width, FontPanel5.Height);
            }
            else
            {
                Graphics g = this.FontPanel5.CreateGraphics();
                g.Clear(this.FontPanel5.BackColor);
            }
        }

        private void FontPanel6_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap5 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap5, 0, 0, FontPanel6.Width, FontPanel6.Height);
            }
            else
            {
                Graphics g = this.FontPanel6.CreateGraphics();
                g.Clear(this.FontPanel6.BackColor);
            }
        }

        private void FontPanel7_Paint(object sender, PaintEventArgs e)
        {
            if (z_Bitmap6 != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(z_Bitmap6, 0, 0, FontPanel7.Width, FontPanel7.Height);

            }
            else
            {
                Graphics g = this.FontPanel7.CreateGraphics();
                g.Clear(this.FontPanel7.BackColor);
            }
        }


        /// <summary>
        /// 车牌识别
        /// </summary>
        private void button12_Click(object sender, EventArgs e)
        {
            int charBmpCount = this.TransformFiles(charSourceBath);
            int provinceBmpCount = this.TransformFiles(provinceSourceBath);
            int[] charMatch = new int[charBmpCount];
            int[] provinceMatch = new int[provinceBmpCount];

            charFont = new Bitmap[charBmpCount];
            provinceFont = new Bitmap[provinceBmpCount];
            for (int i = 0; i < charBmpCount; i++)
            {
                charMatch[i] = 0;
            }
            for (int i = 0; i < provinceBmpCount; i++)
            {
                provinceMatch[i] = 0;
            }
            for (int i = 0; i < charBmpCount; i++)
            {
                charFont[i] = (Bitmap)Bitmap.FromFile(charString[i], false);
            }
            for (int i = 0; i < provinceBmpCount; i++)
            {
                provinceFont[i] = (Bitmap)Bitmap.FromFile(provinceString[i], false);
            }

            int matchIndex = 0;
            string[] digitalFont = new string[7];
            unsafe
            {
                if (z_Bitmaptwo[0] != null)
                {
                    BitmapData bmData = z_Bitmaptwo[0].LockBits(new Rectangle(0, 0, z_Bitmaptwo[0].Width, z_Bitmaptwo[0].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    int stride = bmData.Stride;
                    System.IntPtr Scan = bmData.Scan0;
                    int nOffset = stride - z_Bitmaptwo[0].Width * 3;
                    int nWidth = z_Bitmaptwo[0].Width;
                    int nHeight = z_Bitmaptwo[0].Height;

                    for (int i = 0; i < provinceBmpCount; i++)
                    {
                        byte* p = (byte*)(void*)Scan;
                        BitmapData bmData1 = provinceFont[i].LockBits(new Rectangle(0, 0, provinceFont[i].Width, provinceFont[i].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        int stride1 = bmData1.Stride;
                        System.IntPtr Scan1 = bmData1.Scan0;
                        byte* p1 = (byte*)(void*)Scan1;
                        int nOffset1 = stride1 - provinceFont[i].Width * 3;
                        int nWidth1 = provinceFont[i].Width;
                        int nHeight1 = provinceFont[i].Height;
                        int ccc0 = 0, ccc1 = 0;
                        for (int y = 0; y < nHeight; ++y)
                        {
                            for (int x = 0; x < nWidth; ++x)
                            {

                                if ((p[0] - p1[0]) != 0)
                                {
                                    provinceMatch[i]++;
                                }
                                p1 += 3;
                                p += 3;
                            }
                            p1 += nOffset;
                            p += nOffset;
                        }
                        matchIndex = this.minNumber(provinceMatch);
                        digitalFont[0] = provinceDigitalString[matchIndex].Substring(0, 1);
                        provinceFont[i].UnlockBits(bmData1);

                    }
                    z_Bitmaptwo[0].UnlockBits(bmData);
                }



                if (z_Bitmaptwo[1] != null && z_Bitmaptwo[2] != null && z_Bitmaptwo[3] != null && z_Bitmaptwo[4] != null && z_Bitmaptwo[5] != null && z_Bitmaptwo[6] != null)
                {
                    for (int j = 1; j < 7; j++)
                    {
                        BitmapData bmData = z_Bitmaptwo[j].LockBits(new Rectangle(0, 0, z_Bitmaptwo[j].Width, z_Bitmaptwo[j].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        int stride = bmData.Stride;
                        System.IntPtr Scan = bmData.Scan0;
                        int nOffset = stride - z_Bitmaptwo[j].Width * 3;
                        int nWidth = z_Bitmaptwo[j].Width;
                        int nHeight = z_Bitmaptwo[j].Height;
                        int lv, lc = 0;
                        for (int i = 0; i < charBmpCount; i++)
                        {
                            charMatch[i] = 0;
                        }
                        for (int i = 0; i < charBmpCount; i++)
                        {
                            byte* p = (byte*)(void*)Scan;
                            BitmapData bmData1 = charFont[i].LockBits(new Rectangle(0, 0, charFont[i].Width, charFont[i].Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                            int stride1 = bmData1.Stride;
                            System.IntPtr Scan1 = bmData1.Scan0;
                            byte* p1 = (byte*)(void*)Scan1;
                            int nOffset1 = stride1 - charFont[i].Width * 3;
                            int nWidth1 = charFont[i].Width;
                            int nHeight1 = charFont[i].Height;
                            int ccc0 = 0, ccc1 = 0;
                            lv = 0;
                            for (int y = 0; y < nHeight; ++y)
                            {
                                for (int x = 0; x < nWidth; ++x)
                                {

                                    if ((p[0] - p1[0]) != 0)
                                    {
                                        charMatch[i]++;
                                    }
                                    lv++;
                                    p1 += 3;
                                    p += 3;
                                }
                                p1 += nOffset;
                                p += nOffset;
                            }
                            matchIndex = this.minNumber(charMatch);
                            digitalFont[j] = charDigitalString[matchIndex].Substring(0, 1);
                            charFont[i].UnlockBits(bmData1);
                        }
                        z_Bitmaptwo[j].UnlockBits(bmData);
                    }
                }
            }
            this.textBox1.Text = "" + digitalFont[0] + digitalFont[1] + digitalFont[2] + digitalFont[3] + digitalFont[4] + digitalFont[5] + digitalFont[6];
        }

        public int TransformFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            //DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles("*.bmp");

            int i = 0, j = 0;
            try
            {
                foreach (FileInfo f in files)
                {
                    i++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            if (path.Equals(charSourceBath))
            {
                this.charString = new string[i];
                this.charDigitalString = new string[i];
                try
                {

                    foreach (FileInfo f in files)
                    {


                        charString[j] = (dir + f.ToString());
                        charDigitalString[j] = Path.GetFileNameWithoutExtension(charString[j]);
                        //Console.WriteLine(charDigitalString[j]);
                        j++;

                    }
                    //Console.WriteLine(j);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                provinceString = new string[i];
                provinceDigitalString = new string[i];

                try
                {

                    foreach (FileInfo f in files)
                    {


                        provinceString[j] = (dir + f.ToString());
                        provinceDigitalString[j] = Path.GetFileNameWithoutExtension(provinceString[j]);
                        //Console.WriteLine(provinceString[j]);

                        j++;

                    }
                    // Console.WriteLine(j);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }

            return i;
        }
        private int minNumber(int[] num)
        {
            int minIndex = 0;
            int minNum = 1000;
            for (int i = 0; i < num.Length; i++)
            {
                if (minNum > num[i])
                {
                    minNum = num[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }




        /// <summary>
        /// 灰度直方图
        /// </summary>
        private void graydo()
        {
            // this.flag = 1;
            switch (flag)
            {
                case 1:
                    {
                        count = m_Bitmap.Width * m_Bitmap.Height;
                        gl = new float[256];
                        for (int i = 0; i < 256; i++)
                            gl[i] = gray[i] / count * 8000;
                        pen1 = Pens.Red;
                        panel1.Invalidate();

                        break;
                    }

                case 2:
                    {
                        count = c_Bitmap.Width * c_Bitmap.Height;
                        gl = new float[256];
                        for (int i = 0; i < 256; i++)
                            gl[i] = gray[i] / count * 5000;
                        pen1 = Pens.Gray;
                        panel1.Invalidate();

                        break;
                    }
                default:
                    break;

            }
        }

    }
}
