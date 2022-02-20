using ESGameListGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ESGameListGenerator
{
    public partial class frmGameListGen : Form
    {
        List<string> extensions = new List<string>();

        public frmGameListGen()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtRomFolder.Text = folderBrowserDialog1.SelectedPath;
                UpdateExtensionList();
            }
        }

        private void UpdateExtensionList()
        {
            extensions.Clear();
            extensions = DirectoryScanner.GetFileExtensionsInDirectory(txtRomFolder.Text);
            cboRomExt.DataSource = new BindingSource(extensions, null);
        }

        private void btnGenPreview_Click(object sender, EventArgs e)
        {
            GeneratePreviewXML();
        }

        private void GeneratePreviewXML()
        {
            var files = DirectoryScanner.FindFilesMatching(txtRomFolder.Text, cboRomExt.SelectedText);

            var games = files.Select(f => new Game
            {
                Name = Path.GetFileNameWithoutExtension(f.Name),
                Path = string.Format("./{0}", f.Name)
            }).ToList();

            var shortList = games.Take(1).ToList();
            
            SetGameImages(shortList);
            SetGameVideo (shortList);
            SetGameMarquee (shortList); 

            var gameList = new GameList { Game = shortList };
            var videoList = new VideoList { videos = shortList };
            //var marqueeList = new marqueeList { marquee = shortList };
            var xml = ProcessXML.SaveModelToXML(gameList);
            txtXmlPreview.Text = xml;
        }

        private void SetGameImages(List<Game> gamelist)
        {
            if (chkImagefield.Checked)
            {
                var imgDir = txtRomFolder.Text;
                imgDir = chkImgUseSubfolder.Checked ? Path.Combine(imgDir, txtImgSubfolder.Text) : imgDir;
                var piSubDir = chkImgUseSubfolder.Checked ? txtImgSubfolder.Text : "";

                var imageFiles = DirectoryScanner.FindImageFiles(imgDir);

                foreach (var imgFile in imageFiles)
                {
                    var game = gamelist.FirstOrDefault(g => g.Name == Path.GetFileNameWithoutExtension(imgFile.Name));

                    if (game != null)
                    {
                        game.Image = string.Format("./{0}/{1}", piSubDir, imgFile.Name);
                    }
                }
            }
        }
        private void SetGameVideo(List<Game> gamelist)
        {
            if (chkVideoField.Checked)
            {
                var vidDir = txtRomFolder.Text;
                vidDir = chkVidUseSubfolder.Checked ? Path.Combine(vidDir, txtVidSubfolder.Text) : vidDir;
                var piSubDir = chkVidUseSubfolder.Checked ? txtVidSubfolder.Text : "";

                var videoFiles = DirectoryScanner.FindVideoFiles(vidDir);

                foreach (var vidFile in videoFiles)
                {
                    var game = gamelist.FirstOrDefault(g => g.Name == Path.GetFileNameWithoutExtension(vidFile.Name));

                    if (game != null)
                    {
                        game.Image = string.Format("./{0}/{1}", piSubDir, vidFile.Name);
                    }
                }
            }
        }
        private void SetGameMarquee(List<Game> gamelist)
        {
            if (chkMarField.Checked)
            {
                var marDir = txtRomFolder.Text;
                marDir = chkImgUseSubfolder.Checked ? Path.Combine(marDir, txtMarSubfolder.Text) : marDir;
                var piSubDir = chkMarUseSubfolder.Checked ? txtMarSubfolder.Text : "";

                var marqueeFiles = DirectoryScanner.FindMarqueeFiles(marDir);

                foreach (var marFile in marqueeFiles)
                {
                    var game = gamelist.FirstOrDefault(g => g.Name == Path.GetFileNameWithoutExtension(marFile.Name));

                    if (game != null)
                    {
                        game.Marquee = string.Format("./{0}/{1}", piSubDir, marFile.Name);
                    }
                }
            }
        }


        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                var files = DirectoryScanner.FindFilesMatching(txtRomFolder.Text, cboRomExt.SelectedText);

                var games = files.Select(f => new Game
                {
                    Name = Path.GetFileNameWithoutExtension(f.Name),
                    Path = string.Format("./{0}", f.Name)
                }).ToList();

                SetGameImages(games);
                SetGameVideo(games);
                SetGameMarquee(games);
                var gameList = new GameList { Game = games };

                var xml = ProcessXML.SaveModelToXML(gameList);

                System.IO.File.WriteAllText(Path.Combine(txtRomFolder.Text, "gamelist.xml"), xml);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void chkImgUseSubfolder_CheckedChanged(object sender, EventArgs e)
        {
            lblImgSubfolder.Enabled = txtImgSubfolder.Enabled = chkImgUseSubfolder.Checked;
        }
        private void chkImagefield_CheckedChanged(object sender, EventArgs e)
        {
            chkImgUseSubfolder.Enabled = chkImagefield.Checked;

            if (!chkImagefield.Checked)
            {
                lblImgSubfolder.Enabled = txtImgSubfolder.Enabled = chkImagefield.Checked;
            }
            else
            {
                lblImgSubfolder.Enabled = txtImgSubfolder.Enabled = chkImgUseSubfolder.Checked;
            }
        }
        private void chkVidUseSubfolder_CheckedChanged(object sender, EventArgs e)
        {
            lblVidSubfolder.Enabled = txtVidSubfolder.Enabled = chkVidUseSubfolder.Checked;
        }

        private void chkVideoField_CheckedChanged(object sender, EventArgs e)
        {
            chkVidUseSubfolder.Enabled = chkVideoField.Checked;

            if (!chkVideoField.Checked)
            {
                lblVidSubfolder.Enabled = txtVidSubfolder.Enabled = chkVideoField.Checked;
            }
            else
            {
                lblVidSubfolder.Enabled = txtVidSubfolder.Enabled = chkVidUseSubfolder.Checked;
            }

        }

        private void chkMarUseSubfolder_CheckedChanged(object sender, EventArgs e)
        {
            lblMarSubfolder.Enabled = txtMarSubfolder.Enabled = chkMarUseSubfolder.Checked;
        }
        private void chkMarField_CheckedChanged(object sender, EventArgs e)
        {
            chkMarUseSubfolder.Enabled = chkMarField.Checked;

            if (!chkMarField.Checked)
            {
                lblMarSubfolder.Enabled = txtMarSubfolder.Enabled = chkMarField.Checked;
            }
            else
            {
                lblMarSubfolder.Enabled = txtMarSubfolder.Enabled = chkMarUseSubfolder.Checked;
            }
        }
        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void frmGameListGen_Load(object sender, EventArgs e)
        {

        }
    }
}
