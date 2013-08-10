using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing.Imaging;

// http://www.sno.phy.queensu.ca/~phil/exiftool/TagNames/index.html

namespace Slideshow
{
    public partial class Form1 : Form
    {
        private int ImageDuration = 1000;
        private List<Media> images = new List<Media>();
        private int currentImageIndex = 0;
        private Timer imageTimer = new Timer();
        private Rectangle screenSize;

        // http://msdn.microsoft.com/en-us/library/ms534418(v=vs.85).aspx
        private enum PropertyTag : int
        {
            ImageTitle = 0x10E,
            SubjectLocation = 0xA214,
            Artist = 0x013B,
            Description = 0x010E
        }

        public Form1()
        {
            InitializeComponent();

            this.KeyPress += new KeyPressEventHandler(Form1_KeyPress);

            screenSize = Screen.FromControl(this).Bounds;

            int fontHeight = 48;
            //this.label1.Width = 777;
            this.label1.Width = screenSize.Width;
            this.label1.Height = (int)(fontHeight * 3.25);
            //this.label1.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            //this.label1.BorderStyle = BorderStyle.FixedSingle;
            this.label1.Font = new Font("Verdana", fontHeight, FontStyle.Bold);
            this.label1.AutoSize = false;
            //this.label1.TextAlign = ContentAlignment.MiddleRight;
            this.label1.Location = new Point(0, screenSize.Height - 200);
            //this.label1.BackColor = Color.Transparent;

            //this.label1.Parent = this.pictureBox1;

            LoadImages();

            imageTimer.Interval = ImageDuration;
            imageTimer.Start();
            imageTimer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            using (FileStream stream = new FileStream(images[currentImageIndex].FilePath, FileMode.Open,
                FileAccess.Read))
            {
                using (Image image = Image.FromStream(stream))
                {

                    int newHeight = image.Height * screenSize.Width / image.Width;
                    int newWidth;

                    if (newHeight <= screenSize.Height)
                    {
                        newWidth = screenSize.Width;
                        this.pictureBox1.Size = new Size(screenSize.Height, screenSize.Width);
                    }
                    else
                    {
                        newWidth = image.Width * screenSize.Height / image.Height;
                        newHeight = screenSize.Height;
                        this.pictureBox1.Size = new Size(screenSize.Width, screenSize.Height);
                    }

                    Bitmap newImage = new Bitmap(newWidth, newHeight);
                    Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

                    this.pictureBox1.Image = newImage;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                    if (images[currentImageIndex].Title == null)
                    {
                        images[currentImageIndex].Title = GetExifData(image, (int)PropertyTag.Artist);
                    }

                    if (images[currentImageIndex].Subject == null)
                    {
                        images[currentImageIndex].Subject = GetExifData(image, (int)PropertyTag.Description);
                    }

                    List<string> info = new List<string>
                    {
                        images[currentImageIndex].Title,
                        images[currentImageIndex].Subject
                    };

                    this.label1.Text = String.Join("\n", info);
                }
            }

            currentImageIndex++;

            if (currentImageIndex >= images.Count)
            {
                currentImageIndex = 0;
            }

            //imageTimer.Interval = 100000;
        }

        private string GetExifData(Image image, int propertyTag)
        {
            string data = String.Empty;

            try
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                PropertyItem item = image.GetPropertyItem(propertyTag);
                data = encoding.GetString(item.Value, 0, item.Len - 1);
            }
            catch (Exception)
            {
            }

            return data;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Escape:
                    this.Close();
                    break;

                case (char)Keys.Add:
                    break;

                case (char)Keys.Subtract:
                    break;

                case (char)Keys.Left:
                    break;

                case (char)Keys.Right:
                    break;

                default:
                    break;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            Cursor.Hide();

            // Fade application in? Use timer.
            //this.Opacity = 0.75;
        }

        private void LoadImages()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string assemblyPath = Uri.UnescapeDataString(uri.Path);
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);

            const string searchPattern = "*.jpg";
            string[] paths = Directory.GetFiles(assemblyDirectory, searchPattern,
                SearchOption.TopDirectoryOnly);

            foreach (string path in paths)
            {
                images.Add(new Media
                {
                    FilePath = path
                });
            }
        }
    }
}
