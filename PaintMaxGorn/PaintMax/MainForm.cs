using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;
using System.Threading;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using MaxsGorn;

namespace Maxs_Gorn
{
    public enum DrawMode
    {
        Draw,
        Fill,
        Picker,
        DrawText
    }

    public partial class MainForm : Form
    {
      
        private Palette palette;

  
        private int bmWidth = 16, bmHeight = 16;
        private byte[] bmData = new byte[256];
        bool drawtext = false;
  
        private Bitmap canvas;
        private int zoom = 2;

        
        private bool mouseDraw = false;
        private DrawMode drawMode;

        // історія
        private Stack<HistoryEvent> undoStack = new Stack<HistoryEvent>();
        private Stack<HistoryEvent> redoStack = new Stack<HistoryEvent>();

        public MainForm()
        {
            palette = Palette.VGA;
            InitializeComponent();

            ColorSelector_Update();

         
            this.statusMode.Text = drawMode.ToString();
            FullRedraw();
            UpdateStackButtons();
        }

        void ColorSelector_Update()
        {
 
            while (colorSelector.Items.Count > 0)
                colorSelector.Items.RemoveAt(0);

            ImageList imgList = new ImageList();
            colorSelector.LargeImageList = imgList;
            colorSelector.SmallImageList = imgList;
            for (int i = 0; i < Palette.Length; i++)
            {
                Color color = palette[i];
                Bitmap bm = new Bitmap(16, 16);
                for (int y = 0; y < 16; y++)
                    for (int x = 0; x < 16; x++)
                        bm.SetPixel(x, y, color);

                ListViewItem listItem = new ListViewItem(i.ToString(), i);
                listItem.SubItems.Add(String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B));

                colorSelector.Items.Add(listItem);
                imgList.Images.Add(bm);
            }

            FullRedraw();
        }

        private void ShowPalette_Click(object sender, EventArgs e)
        {
            bmWidth = 16;
            bmHeight = Palette.Length / bmWidth;
            bmData = new byte[bmWidth * bmHeight];

            for (int i = 0; i <= byte.MaxValue; i++)
            {
                bmData[i] = (byte)i;
            }

            // clear the history stacks
            undoStack.Clear();
            redoStack.Clear();

            FullRedraw();
        }


        private Bitmap RenderBitmap(int scale, bool grid)
        {
            if (scale <= 0) return null;

           
            Bitmap bm = new Bitmap(bmWidth * scale, bmHeight * scale);

            
            for (int y = 0; y < bm.Height; y += scale)
                for (int x = 0; x < bm.Width; x += scale)
                {
                    Color pixel = palette[bmData[y / scale * bmWidth + (x / scale)]];
                    for (int py = 0; py < scale; py++)
                        for (int px = 0; px < scale; px++)
                            bm.SetPixel(x + px, y + py, pixel);
                }

            // малювати сітку
            if (grid && scale > 1)
            {
                Color gridColor = Color.Gray;
                for (int x = scale - 1; x < bm.Width; x += scale)
                    for (int y = 0; y < bm.Height; y++)
                        bm.SetPixel(x, y, gridColor);
                for (int y = scale - 1; y < bm.Height; y += scale)
                    for (int x = 0; x < bm.Width; x++)
                        bm.SetPixel(x, y, gridColor);
            }

            return bm;
        }

   

        private void FullRedraw()
        {
            canvas = RenderBitmap(zoom, showGrid.Checked);
            Redraw();
        }



        private void Redraw()
        {
            try
            {
                canvasBox.Image = canvas;
                canvasBox.Refresh();
                canvasBox.Update();
            }
            catch (Exception) { }
         
        }

        private void RedrawUITrigger(object sender, EventArgs e)
        {
            FullRedraw();
        }



        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoomLabel.Text = String.Format("Збільшити: {0}%", zoom * 100);
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            zoom++;
            FullRedraw();
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            if (zoom > 1) zoom--;
            FullRedraw();
        }

        private void ZoomReset_Click(object sender, EventArgs e)
        {
            zoom = Int32.Parse((sender as ToolStripItem).Tag as string);
            FullRedraw();
        }



        public const string FileHeader = "TGRV";
        private  void OpenImg(string filename)
        {
            Stream file = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(file, Encoding.ASCII);


            char[] header = br.ReadChars(FileHeader.Length);
            if (new string(header) != FileHeader) throw new Exception("Недійсний формат");


            bmWidth = br.ReadUInt16();
            bmHeight = br.ReadUInt16();


            bmData = br.ReadBytes(bmWidth * bmHeight);


            br.Close();
        }
        private void OpenImage_Click(object sender, EventArgs e)
        {
            // get the filename
            var result = openVGA.ShowDialog(this);
            if (result != DialogResult.OK) return;


            OpenImg(openVGA.FileName);

            // очищаємо історію
            undoStack.Clear();
            redoStack.Clear();

            // перемальовуємо картинку
            FullRedraw();
        }

        private void SaveImage_Click(object sender, EventArgs e)
        {
            //отримуємо імя файлу
            var result = saveVGA.ShowDialog(this);
            if (result == DialogResult.Cancel && e is FormClosingEventArgs)
            {
                (e as FormClosingEventArgs).Cancel = true;
            }
            if (result != DialogResult.OK) return;

            // зберігаємо картинку
            Stream file = new FileStream(saveVGA.FileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(file, Encoding.ASCII);
            bw.Write(FileHeader.ToCharArray()); // записуємо заголовок
            bw.Write((ushort)bmWidth);
            bw.Write((ushort)bmHeight);
            bw.Write(bmData);
            bw.Close();
        }

        private void ImportImage_Click(object sender, EventArgs e)
        {
      
            var result = openImport.ShowDialog();
            if (result != DialogResult.OK) return;

       
            newToolStripMenuItem.Enabled = false;
            openImage.Enabled = false;
            saveImage.Enabled = false;
            exportImage.Enabled = false;
            importImage.Enabled = false;
            showPalette.Enabled = false;
            zoom100.Enabled = false;
            zoom200.Enabled = false;
            zoom500.Enabled = false;
            zoomIn.Enabled = false;
            zoomOut.Enabled = false;
            showGrid.Enabled = false;

           
            importProgress.Visible = true;
            importProgress.Value = 0;

          
            undoStack.Clear();
            redoStack.Clear();
            UpdateStackButtons();

   
            importer.RunWorkerAsync();
        }

        private void ExportImage_Click(object sender, EventArgs e)
        {
            // get the filename
            var result = saveExport.ShowDialog(this);
            if (result != DialogResult.OK) return;

            Bitmap bm = RenderBitmap(1, false);
            bm.Save(saveExport.FileName);
        }



        private void DoMouse(int mouseX, int mouseY)
        {
            // 
            if (importer.IsBusy) return;

            // підберіть колір
            byte color = 0;
            if (colorSelector.SelectedItems.Count > 0)
                color = Byte.Parse(colorSelector.SelectedItems[0].Text);

            //отримати реальні координати миші
            mouseX /= zoom;
            mouseY /= zoom;

            // не робіть нічого поза рамками
            if (mouseX >= bmWidth || mouseY >= bmHeight) return;

            // перевірка меж
            int offset = mouseY * bmWidth + mouseX;
            if (offset < 0 || offset >= bmData.Length) return;

            //зміни пікселів для приєднання до історії
            var changes = new HashSet<PixelChange>();

            switch (drawMode)
            {
                case DrawMode.Draw:
                    var change = DrawPixel(mouseX, mouseY, color);
                    if (change != null) changes.Add(change);
                    redoStack.Clear();
                    break;
                case DrawMode.Fill:
                    var changeSet = FloodFill(mouseX, mouseY, color);
                    if (changeSet != null) changes.UnionWith(changeSet);
                    redoStack.Clear();
                    break;
                case DrawMode.Picker:
                    colorSelector.Items[bmData[offset]].Selected = true;
                    break;
                case DrawMode.DrawText:

                    DrawText(mouseX, mouseY, color);



                    break;
                    
                default:
                    throw new NotImplementedException("Draw mode " + drawMode.ToString() + " not implemented.");
            }

           
            if (changes.Count > 0)
                undoStack.Push(new HistoryEvent(changes));
            UpdateStackButtons();

            // тригер canvasBox перемальовується
            Redraw();
        }

        void  DrawText(int x, int y, byte color)
        {
   
            var gfx = Graphics.FromImage(canvas);

            var fontFamily = new FontFamily("Times New Roman");
            var font = new Font(fontFamily, 12, FontStyle.Regular, GraphicsUnit.Pixel);
            var solidBrush = new SolidBrush(Color.AliceBlue);

            gfx.DrawString("Your Text Here", font, solidBrush, new PointF(x * zoom, y * zoom));
           
       
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewArtwork newDialog = new NewArtwork();
            var result = newDialog.ShowDialog();
            if (result != DialogResult.OK) return;


            bmWidth = (int)newDialog.bmWidth.Value;
            bmHeight = (int)newDialog.bmHeight.Value;
            bmData = new byte[bmWidth * bmHeight];

            FullRedraw();
        }

        private void CanvasBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDraw = true;
            DoMouse(e.X, e.Y);
        }

        private void CanvasBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDraw = false;
            DoMouse(e.X, e.Y);
        }

        private void CanvasBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!importer.IsBusy)
                toolStripStatusLabel1.Text = String.Format("{0}, {1}", e.X / zoom, e.Y / zoom);

            if (!mouseDraw) return;
            DoMouse(e.X, e.Y);
        }

        private void CanvasBox_MouseLeave(object sender, EventArgs e)
        {
            mouseDraw = false;
        }

       

        private PixelChange DrawPixel(int x, int y, byte color)
        {
            //зсув precalc та перевірка межі
            int offset = y * bmWidth + x;
            if (offset < 0 || offset >= bmData.Length) return null;

            // якщо піксель однаковий
            if (bmData[offset] == color) return null;

            // змініть піксель і відзначте зміни
            var change = new PixelChange(offset, bmData[offset], color);
            bmData[offset] = color;

            // оновити картинку
            var gfx = Graphics.FromImage(canvas);
            if (drawtext == true)
            {
                //var fontFamily = new FontFamily("Times New Roman");
                //var font = new Font(fontFamily, 32, FontStyle.Regular, GraphicsUnit.Pixel);
                //var solidBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 255));

                //gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
                //gfx.DrawString("Your Text Here", font, solidBrush, new PointF(x * zoom, y * zoom));
                //drawtext = false;
            }
            gfx.FillRectangle(new SolidBrush(palette[color]),
                              x * zoom, y * zoom,
                              zoom - (showGrid.Checked ? 1 : 0),
                              zoom - (showGrid.Checked ? 1 : 0));

    


            return change;
        }

        private ISet<PixelChange> FloodFill(int x, int y, byte color)
        {
            
            int offset = y * bmWidth + x;
            if (offset < 0 || offset >= bmData.Length) return null;

            var changes = new HashSet<PixelChange>();
            var visitedPixels = new HashSet<int>();

            var toVisit = new Queue<int>();
            toVisit.Enqueue(y * bmWidth + x);

            var gfx = Graphics.FromImage(canvas);

            while (toVisit.Count > 0)
            {
                var pixelOffset = toVisit.Dequeue();
                var pixelColor = bmData[pixelOffset];
                if (visitedPixels.Contains(pixelOffset)) continue;

                changes.Add(new PixelChange(pixelOffset, pixelColor, color));
                bmData[pixelOffset] = color;
                gfx.FillRectangle(new SolidBrush(palette[color]),
                                  (pixelOffset % bmWidth) * zoom, 
                                  (pixelOffset / bmWidth) * zoom,
                                  zoom - (showGrid.Checked ? 1 : 0),
                                  zoom - (showGrid.Checked ? 1 : 0));

                visitedPixels.Add(pixelOffset);

                int[] diff = new int[] { -bmWidth, bmWidth, -1, 1 };
                foreach (int offsetDiff in diff)
                {
                    var nextOffset = pixelOffset + offsetDiff;
                    try
                    {
                        if (pixelColor != bmData[nextOffset]) continue;
                        if (visitedPixels.Contains(nextOffset)) continue;

                        toVisit.Enqueue(nextOffset);
                    }
                    catch
                    {
                        ;
                    }
                }
            }

            return changes;
        }

    
        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateStackButtons()
        {
            undoAction.Enabled = undoStack.Count > 0;
            redoAction.Enabled = redoStack.Count > 0;
        }

        private void UndoAction_Click(object sender, EventArgs e)
        {
            if (undoStack.Count <= 0) return;
            var action = undoStack.Pop();

            // скасувати
            foreach (var change in action.changes)
            {
                int x = change.Offset % bmWidth;
                int y = change.Offset / bmWidth;
                DrawPixel(x, y, change.OldColor);
            }

           
            redoStack.Push(action);

            Redraw();
            UpdateStackButtons();
        }

        private void RedoAction_Click(object sender, EventArgs e)
        {
            if (redoStack.Count <= 0) return;
            var action = redoStack.Pop();

            // скасувати
            foreach (var change in action.changes)
            {
                int x = change.Offset % bmWidth;
                int y = change.Offset / bmWidth;
                DrawPixel(x, y, change.NewColor);
            }

           
            undoStack.Push(action);

            Redraw();
            UpdateStackButtons();
        }

        private void Importer_DoWork(object sender, DoWorkEventArgs e)
        {

            var bm = new Bitmap(openImport.FileName);
            var bmWidth = bm.Width;
            var bmHeight = bm.Height;
            var bmData = new byte[bmWidth * bmHeight];
            int update = 0;
            for (int y = 0; y < bmHeight; y++)
                for (int x = 0; x < bmWidth; x++)
                {
                    Color thisColor = bm.GetPixel(x, y);
                    if (update++ == 10)
                    {
                        importer.ReportProgress(100 * (y * bmWidth + x) / bmData.Length,new ImporterStatus(y * bmWidth + x, bmData.Length));
                        update = 0;
                    }

                    if (thisColor.A == 0) {
                        bmData[y * bmWidth + x] = 0x00;
                        continue;
                    }

                    int minIndex = -1, min = int.MaxValue;
                    int[] overallDiff = new int[Palette.Length];
                    for (int i = 0; i < overallDiff.Length; i++)
                    {
                        Color vgaColor = palette[i];
                        overallDiff[i] += Math.Abs(thisColor.R - vgaColor.R);
                        overallDiff[i] += Math.Abs(thisColor.G - vgaColor.G);
                        overallDiff[i] += Math.Abs(thisColor.B - vgaColor.B);

                        if (overallDiff[i] < min)
                        {
                            min = overallDiff[i];
                            minIndex = i;
                        }
                    }

                    // встановити цей піксель як колір
                    bmData[y * bmWidth + x] = (byte)minIndex;
                }


            importer.ReportProgress(100, new ImporterStatus(true, bmData.Length));

            // зберегти картинку
            this.bmWidth = bmWidth;
            this.bmHeight = bmHeight;
            this.bmData = bmData;
            this.canvas = RenderBitmap(this.zoom, this.showGrid.Checked);

            e.Result = true;
        }

        private void Importer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // оновити інтерфейс користувача щодо прогресу імпортера
            importProgress.Value = e.ProgressPercentage;
            toolStripStatusLabel1.Text = e.UserState.ToString();
        }

        private void Importer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //приховати панель ходу імпорту
            importProgress.Visible = false;
            toolStripStatusLabel1.Text = "";

           
            newToolStripMenuItem.Enabled = true;
            openImage.Enabled = true;
            saveImage.Enabled = true;
            exportImage.Enabled = true;
            importImage.Enabled = true;
            showPalette.Enabled = true;
            zoom100.Enabled = true;
            zoom200.Enabled = true;
            zoom500.Enabled = true;
            zoomIn.Enabled = true;
            zoomOut.Enabled = true;
            showGrid.Enabled = true;

            // перемалювати інтерфейс користувача
            Redraw();
        }

  
        private void HelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.instagram.com/not_your_maxson/?hl=uk");// мій інстаграм
        }

        private void ReportAnissueToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            System.Diagnostics.Process.Start("mailto:maximus56133@gmail.com");//написати мені лист на почту Windows 10
        }

        private void DrawTool_Select(object sender, EventArgs e)
        {
            foreach (var el in menuEdit.DropDownItems)
            {
                if (!(el is ToolStripMenuItem)) continue;
                (el as ToolStripMenuItem).Checked = false;
            }

            if (!(sender is ToolStripMenuItem)) return;
            ToolStripMenuItem toolSelector = (ToolStripMenuItem)sender;
            drawMode = (DrawMode)Enum.Parse(drawMode.GetType(), toolSelector.Tag as string);
            toolSelector.Checked = true;
            statusMode.Text = drawMode.ToString();
        }

        private void ColorSelector_DoubleClick(object sender, EventArgs e)
        {
            byte color = 0;
            if (colorSelector.SelectedItems.Count > 0)
                color = Byte.Parse(colorSelector.SelectedItems[0].Text);

            colorDialog1.Color = palette[color];
            var result = colorDialog1.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            palette[color] = colorDialog1.Color;
            ColorSelector_Update();
        }

        private void GrayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            palette = Palette.Grayscale;
            ColorSelector_Update();
        }

        private void VGAMode13hToolStripMenuItem_Click(object sender, EventArgs e)
        {
            palette = Palette.VGA;
            ColorSelector_Update();
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
            var importDialog = new OpenFileDialog();
            importDialog.Filter = "PaintMax Gorn|*.gorn";
            importDialog.DefaultExt = "gorn";
            importDialog.Title = "Імпортуйте палітру ілюстрацій";
            var dialogResult = importDialog.ShowDialog(this);

          
            if (dialogResult != DialogResult.OK) return;

          
            palette = Palette.Load(importDialog.FileName);
            ColorSelector_Update();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var exportDialog = new SaveFileDialog();
            exportDialog.Filter = "PaintMax Gorn|*.gorn";
            exportDialog.DefaultExt = "gorn";
            exportDialog.Title = "Імпортуйте палітру ілюстрацій";
            var dialogResult = exportDialog.ShowDialog(this);

          
            if (dialogResult != DialogResult.OK) return;

            palette.Save(exportDialog.FileName);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;
        }

        private void importProgress_Click(object sender, EventArgs e)
        {

        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void canvasBox_Click(object sender, EventArgs e)
        {

        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) => new AboutBox().ShowDialog();

        private void збільшити1000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoom = Int32.Parse((sender as ToolStripItem).Tag as string);
            FullRedraw();
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e) => new AboutBox().ShowDialog();

        private void loadPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void добавитьТекстToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var el in menuEdit.DropDownItems)
            {
                if (!(el is ToolStripMenuItem)) continue;
                (el as ToolStripMenuItem).Checked = false;
            }

            if (!(sender is ToolStripMenuItem)) return;
            ToolStripMenuItem toolSelector = (ToolStripMenuItem)sender;
            drawMode = (DrawMode)Enum.Parse(drawMode.GetType(), toolSelector.Tag as string);
            toolSelector.Checked = true;
            statusMode.Text = drawMode.ToString();
        }

        private void canvasBox_Paint(object sender, PaintEventArgs e)
        {

        }

        private void colorSelector_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void відтінкиЧервоногоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            palette = Palette.RedScale;
            ColorSelector_Update();
        }

        private void відтінкиЗеленогоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            palette = Palette.PurpleScale;
            ColorSelector_Update();
        }

        private void відтінкиЖовтогоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            palette = Palette.BlueScale;
            ColorSelector_Update();
        }

        private void відтінкиЖовтогоToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            palette = Palette.GreenScale;
            ColorSelector_Update();
        }

        private void відтінкиЖовтогоToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            palette = Palette.LightBlueScale;
            ColorSelector_Update();
        }

        private void відтінкиЖовтогоToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            palette = Palette.YellowScale;
            ColorSelector_Update();
        }

        private void гаммаToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void гаммаToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int r=0, g=0, b=0;
            Gamma gamma = new Gamma(2, 5, "Гамма R");
            if (gamma.ShowDialog() == DialogResult.OK)
            {
                r = gamma.value;
            }
            gamma =  new Gamma(2, 5, "Гамма G");
            if (gamma.ShowDialog() == DialogResult.OK)
            {
                g = gamma.value;
            }

            gamma = new Gamma(2, 5, "Гамма B");
            if (gamma.ShowDialog() == DialogResult.OK)
            {
                b = gamma.value;
            }
            if(r!=0 && g!=0 && b!=0)
            {
                zoom = 1;
                FullRedraw();
                SetGamma(r / 10, g / 10, b / 10);

            }

            
        }

        private byte[] CreateGammaArray(double color)
        {
            byte[] gammaArray = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                gammaArray[i] = (byte)Math.Min(255,
        (int)((255.0 * Math.Pow(i / 255.0, 1.0 / color)) + 0.5));
            }
            return gammaArray;
        }

        void SetBrightness(int brightness)
        {
            Bitmap bmap = (Bitmap)canvasBox.Image;
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    bmap.SetPixel(i, j,
        Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            SaveSLoadEffect((Bitmap)bmap);
        }
        private void SaveSLoadEffect(Bitmap bmap)
        {

            try { bmap.Save("def.png", ImageFormat.Png); }
            catch (Exception er) { MessageBox.Show(er.Message, "Notifications", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            var bm = new Bitmap("def.png");
            var bmWidth = bm.Width;
            var bmHeight = bm.Height;
            var bmData = new byte[bmWidth * bmHeight];
            int update = 0;
            for (int y = 0; y < bmHeight; y++)
                for (int x = 0; x < bmWidth; x++)
                {
                    Color thisColor = bm.GetPixel(x, y);
                    if (update++ == 10)
                    {

                        update = 0;
                    }

                    if (thisColor.A == 0)
                    {
                        bmData[y * bmWidth + x] = 0x00;
                        continue;
                    }

                    int minIndex = -1, min = int.MaxValue;
                    int[] overallDiff = new int[Palette.Length];
                    for (int i = 0; i < overallDiff.Length; i++)
                    {
                        Color vgaColor = palette[i];
                        overallDiff[i] += Math.Abs(thisColor.R - vgaColor.R);
                        overallDiff[i] += Math.Abs(thisColor.G - vgaColor.G);
                        overallDiff[i] += Math.Abs(thisColor.B - vgaColor.B);

                        if (overallDiff[i] < min)
                        {
                            min = overallDiff[i];
                            minIndex = i;
                        }
                    }

                    // встановити цей піксель як колір
                    bmData[y * bmWidth + x] = (byte)minIndex;
                }




            // зберегти картинку
            this.bmWidth = bmWidth;
            this.bmHeight = bmHeight;
            this.bmData = bmData;
            this.canvas = RenderBitmap(this.zoom, this.showGrid.Checked);
            FullRedraw();
        }
        void SetGamma(double red, double green, double blue)
        {
           
            Bitmap bmap = (Bitmap)canvasBox.Image;
            Color c;
            byte[] redGamma = CreateGammaArray(red);
            byte[] greenGamma = CreateGammaArray(green);
            byte[] blueGamma = CreateGammaArray(blue);
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j, Color.FromArgb(redGamma[c.R],
                       greenGamma[c.G], blueGamma[c.B]));
                }
            }
            SaveSLoadEffect((Bitmap)bmap);
            
          

        }


        private void яркостьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Gamma gamma = new Gamma(-255,255, "Гамма");
            if(gamma.ShowDialog()==DialogResult.OK)
            {

                zoom = 1;
                FullRedraw();
                SetBrightness(gamma.value);
            }

        }
        public void DrawOutCropArea(int xPosition, int yPosition, int width, int height)
        {
            
            Bitmap bmap = (Bitmap)canvasBox.Image;
            Graphics gr = Graphics.FromImage(bmap);
            Brush cBrush = new Pen(Color.FromArgb(150, Color.White)).Brush;
            Rectangle rect1 = new Rectangle(0, 0, canvasBox.Width, yPosition);
            Rectangle rect2 = new Rectangle(0, yPosition, xPosition, height);
            Rectangle rect3 = new Rectangle
            (0, (yPosition + height), canvasBox.Width, canvasBox.Height);
            Rectangle rect4 = new Rectangle((xPosition + width),
        yPosition, (canvasBox.Width - xPosition - width), height);
            gr.FillRectangle(cBrush, rect1);
            gr.FillRectangle(cBrush, rect2);
            gr.FillRectangle(cBrush, rect3);
            gr.FillRectangle(cBrush, rect4);
            canvasBox.Image = (Bitmap)bmap.Clone();
        }
        private void контрастToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Gamma gamma = new Gamma(-100, 100,"Контраст");
            if (gamma.ShowDialog() == DialogResult.OK)
            {
                zoom = 1;
                FullRedraw();
                SetContrast(gamma.value);
            }
        }
        void SetContrast(double contrast)
        {
            Bitmap bmap = (Bitmap)canvasBox.Image;
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    bmap.SetPixel(i, j,
                    Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            SaveSLoadEffect((Bitmap)bmap);
        }


        void RotateFlip(RotateFlipType rotateFlipType)
        {
           
            Bitmap bmap = (Bitmap)canvasBox.Image;
            bmap.RotateFlip(rotateFlipType);
            SaveSLoadEffect((Bitmap)bmap);
        }
        private void фільтриToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        private async void на180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>      
            {
                zoom = 1;
                FullRedraw();
                var inner =  Task.Factory.StartNew(() =>
                {
                    RotateFlip(RotateFlipType.Rotate180FlipX);
                }, TaskCreationOptions.AttachedToParent);
            });
           outer.Wait();
           await Task.Delay(1);
        }

        private async void на90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    RotateFlip(RotateFlipType.Rotate90FlipX);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
        }

        private async void на270ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    RotateFlip(RotateFlipType.Rotate270FlipX);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
        }
        public void InsertText(string text, int xPosition,
    int yPosition, Font font, Color colorName1, Color colorName2)
        {
           
            Bitmap bmap = (Bitmap)canvasBox.Image;
            Graphics gr = Graphics.FromImage(bmap);

            Color color1 = colorName1;
            Color color2 = colorName2;
            int gW = (int)(text.Length * font.Size);
            gW = gW == 0 ? 10 : gW;
            LinearGradientBrush LGBrush =
           new LinearGradientBrush(new Rectangle(0, 0, gW, (int)font.Size), color1,
           color2, LinearGradientMode.Vertical);
            gr.DrawString(text, font, LGBrush, xPosition, yPosition);
            SaveSLoadEffect((Bitmap)bmap);
        }

        private async void добавитьГрадиентнийТекстЗЗбереженнямToolStripMenuItem_Click(object sender, EventArgs e)
        {
            byte color=0,color2 = 0;
            if (colorSelector.SelectedItems.Count > 0)
                color = Byte.Parse(colorSelector.SelectedItems[0].Text);
            Color color1=colorDialog1.Color, color22;
            var result = colorDialog1.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            if (colorSelector.SelectedItems.Count > 0)
                color2 = Byte.Parse(colorSelector.SelectedItems[0].Text);
            color22 = colorDialog1.Color;
            var result2 = colorDialog1.ShowDialog();
            if (result2 != DialogResult.OK)
            {
                return;
            }

           
            var result3= fontDialog1.ShowDialog();
            if (result2 != DialogResult.OK)
            {

                return;
            }
            Font font =fontDialog1.Font;


            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    InsertText("max gornitskiy test", 40,40,font, color1, color22);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
        }

        private async void інвертуватиКольориToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    SetInvert();
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
            
        }
        void SetInvert()
        {
            
            Bitmap bmap = (Bitmap)canvasBox.Image;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j,
            Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            SaveSLoadEffect((Bitmap)bmap);
        }


        public void Resize(int newWidth, int newHeight)
        {
            if (newWidth != 0 && newHeight != 0)
            {
                Bitmap temp = (Bitmap)canvasBox.Image;
                Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

                double nWidthFactor = (double)temp.Width / (double)newWidth;
                double nHeightFactor = (double)temp.Height / (double)newHeight;

                double fx, fy, nx, ny;
                int cx, cy, fr_x, fr_y;
                Color color1 = new Color();
                Color color2 = new Color();
                Color color3 = new Color();
                Color color4 = new Color();
                byte nRed, nGreen, nBlue;

                byte bp1, bp2;

                for (int x = 0; x < bmap.Width; ++x)
                {
                    for (int y = 0; y < bmap.Height; ++y)
                    {

                        fr_x = (int)Math.Floor(x * nWidthFactor);
                        fr_y = (int)Math.Floor(y * nHeightFactor);
                        cx = fr_x + 1;
                        if (cx >= temp.Width) cx = fr_x;
                        cy = fr_y + 1;
                        if (cy >= temp.Height) cy = fr_y;
                        fx = x * nWidthFactor - fr_x;
                        fy = y * nHeightFactor - fr_y;
                        nx = 1.0 - fx;
                        ny = 1.0 - fy;

                        color1 = temp.GetPixel(fr_x, fr_y);
                        color2 = temp.GetPixel(cx, fr_y);
                        color3 = temp.GetPixel(fr_x, cy);
                        color4 = temp.GetPixel(cx, cy);

                        // Blue
                        bp1 = (byte)(nx * color1.B + fx * color2.B);

                        bp2 = (byte)(nx * color3.B + fx * color4.B);

                        nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);

                        bp2 = (byte)(nx * color3.G + fx * color4.G);

                        nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);

                        bp2 = (byte)(nx * color3.R + fx * color4.R);

                        nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        bmap.SetPixel(x, y, System.Drawing.Color.FromArgb
                (255, nRed, nGreen, nBlue));
                    }
                }
                
            }
        }

        private async void обрізкаФотоToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            var outer = Task.Factory.StartNew(() =>
            {
                Crop gamma = new Crop(canvasBox.Width, canvasBox.Height);
                if (gamma.ShowDialog() == DialogResult.OK)
                {
                    zoom = 1;
                    FullRedraw();
                    var inner = Task.Factory.StartNew(() =>
                    {
                        DrawOutCropArea(gamma.value1,gamma.value2, gamma.value3,gamma.value4);

                        Task.Delay(10);
                        if (MessageBox.Show("Crop?", "Notifications", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Crop(gamma.value1, gamma.value2, gamma.value3, gamma.value4);

                        }
                        else
                        {
                            FullRedraw();
                        }


                    }, TaskCreationOptions.LongRunning);
                }
            });
            outer.Wait();
            await Task.Delay(1);
        }
        void Crop(int xPosition, int yPosition, int width, int height)
        {
            
            Bitmap bmap = (Bitmap)canvasBox.Image.Clone();
            if (xPosition + width > bmap.Width)
                width = bmap.Width - xPosition;
            if (yPosition + height > canvasBox.Height)
                height = bmap.Height - yPosition;
            Rectangle rect = new Rectangle(xPosition, yPosition, width, height);
            SaveSLoadEffect(bmap.Clone(rect, bmap.PixelFormat));
          
        }

        private async void на90YToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    RotateFlip(RotateFlipType.Rotate90FlipXY);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
        }

        private async void на180XYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    RotateFlip(RotateFlipType.Rotate180FlipXY);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
        }

        private async void на270XYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var outer = Task.Factory.StartNew(() =>
            {
                zoom = 1;
                FullRedraw();
                var inner = Task.Factory.StartNew(() =>
                {
                    RotateFlip(RotateFlipType.Rotate270FlipXY);
                }, TaskCreationOptions.AttachedToParent);
            });
            outer.Wait();
            await Task.Delay(1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           

            if (undoStack.Count > 0)
            {
                var result = MessageBox.Show("Ви хочете зберегти свою роботу перед тим, як вийти?", "Зберегти та вийти", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Cancel) e.Cancel = true;
                if (result == DialogResult.Yes)
                {
                    SaveImage_Click(sender, e);
                }
            }

            
            importer.CancelAsync();
      
       
        }

       
    }



    class ImporterStatus
    {
        public int Progress { get; private set; }
        public int Total { get; private set; }
        public bool BitmapRender { get; private set; }

        public ImporterStatus(bool bitmap, int total)
        {
            BitmapRender = bitmap;
            Total = total;
            Progress = total;
        }

        public ImporterStatus(int progress, int total)
        {
            BitmapRender = false;
            Progress = progress;
            Total = total;
        }

        public override string ToString()
        {
            if (BitmapRender) return "Рендеринг зображення...";
            return String.Format("{0} / {1} pixels processed", Progress, Total);
        }
    }
}
