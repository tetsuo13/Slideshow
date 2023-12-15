using System;
using System.IO;
using System.Windows.Forms;
using ExifLib;

namespace ImageViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.jpg;*.jpeg)|*.jpg;*.jpeg";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Text = Path.GetFileName(openFileDialog1.FileName);
                listBox1.Items.Clear();
                ShowImageInfo(openFileDialog1.FileName);
            }
        }

        private void ShowImageInfo(string fileName)
        {
            pictureBox1.ImageLocation = fileName;
            ExifReader reader;

            try
            {
                reader = new ExifReader(fileName);
            }
            catch (Exception e)
            {
                // Probably no tags to display.
                listBox1.Items.Add(String.Format("Error reading file: {0}", e.Message));
                return;
            }

            foreach (ExifTags tag in Enum.GetValues(typeof(ExifTags)))
            {
                try
                {
                    string exifValue;
                    reader.GetTagValue<string>(tag, out exifValue);

                    if (!String.IsNullOrEmpty(exifValue))
                    {
                        string item = String.Format("{0}: {1}",
                            Enum.GetName(typeof(ExifTags), tag), exifValue);
                        listBox1.Items.Add(item);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
