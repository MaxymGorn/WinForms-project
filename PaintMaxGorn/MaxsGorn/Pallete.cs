﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Maxs_Gorn
{

    public class Palette
    {
        public const string FileHeader = "TGRP";

        Color[] colors = new Color[256];

        public Color this[int key]
        {
            get
            {
                return this.colors[key];
            }

            set
            {
                this.colors[key] = Color.FromArgb(value.R, value.G, value.B);
            }
        }

        public const int Length = 256;

        #region File load/save

        public static Palette Load(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open);
            var palette = Palette.Load(stream);
            stream.Close();

            return palette;
        }

        public static Palette Load(Stream inStream)
        {
            //
            var br = new BinaryReader(inStream, Encoding.ASCII);

            // check the file magic number
            char[] header = br.ReadChars(FileHeader.Length);
            if (new string(header) != FileHeader) throw new Exception("Invalid format");

            // read in the palette
            int index = 0;
            var palette = new Palette();

            while (index < 256)
            {
                palette[index++] = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
            }

            return palette;
        }

        public void Save(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            this.Save(stream);
            stream.Close();
        }

        public void Save(Stream outStream)
        {
            var bw = new BinaryWriter(outStream, Encoding.ASCII);
            bw.Write(FileHeader.ToCharArray());

            foreach (var color in colors)
            {
                byte[] colorInfo = { color.R, color.G, color.B };
                bw.Write(colorInfo);
            }
        }

        #endregion

        #region Predefined palettes

        public static Palette VGA
        {
            get
            {
                var vgaPalette = new Palette();
                int index = 0;  

                // CGA color palette
                vgaPalette[index++] = Color.FromArgb(0, 0, 0);          // 0 - black
                vgaPalette[index++] = Color.FromArgb(0, 0, 170);        // 1 - blue
                vgaPalette[index++] = Color.FromArgb(0, 170, 0);        // 2 - green 
                vgaPalette[index++] = Color.FromArgb(0, 170, 170);      // 3 - cyan
                vgaPalette[index++] = Color.FromArgb(170, 0, 0);        // 4 - red 
                vgaPalette[index++] = Color.FromArgb(170, 0, 170);      // 5 - magenta
                vgaPalette[index++] = Color.FromArgb(170, 85, 170);     // 6 - brown
                vgaPalette[index++] = Color.FromArgb(170, 170, 170);    // 7 - gray
                vgaPalette[index++] = Color.FromArgb(85, 85, 85);       // 8 - lighter black
                vgaPalette[index++] = Color.FromArgb(85, 85, 255);      // 9 - lighter blue
                vgaPalette[index++] = Color.FromArgb(85, 255, 85);      // A - lighter green 
                vgaPalette[index++] = Color.FromArgb(85, 255, 255);     // B - lighter cyan
                vgaPalette[index++] = Color.FromArgb(255, 85, 85);      // C - lighter red 
                vgaPalette[index++] = Color.FromArgb(255, 85, 255);     // D - lighter magenta
                vgaPalette[index++] = Color.FromArgb(255, 255, 85);     // E - lighter brown
                vgaPalette[index++] = Color.FromArgb(255, 255, 255);    // F - lighter gray

                {
                    byte grey = 0x00;
                    for (int i = 0; i < 16; i++)
                    {
                        vgaPalette[index++] = Color.FromArgb(grey, grey, grey);
                        grey += 0x10;
                    }
                }

        
                double[] satLum = { 1.0, 0.5, 0.25 };

  
                foreach (double lum in satLum)
                    foreach (double sat in satLum)
                        for (int i = 240; i < 240 + 360; i += 15)
                        {
                            vgaPalette[index++] = FromHsv(i, sat, lum);
                        }

                while (index < Palette.Length)
                    vgaPalette[index++] = Color.FromArgb(0, 0, 0);

                return vgaPalette;
            }
        }

        public static Palette Grayscale
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(i, i, i);
                return palette;
            }
        }
        public static Palette PurpleScale
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(i, 0, i);
                return palette;
            }
        }
        public static Palette GreenScale
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(0, i, 0);
                return palette;
            }
        }
        public static Palette RedScale 
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(i, 0, 0);
                return palette;
            }
        }
        public static Palette LightBlueScale
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(0, i, i);
                return palette;
            }
        }
        public static Palette YellowScale
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(i, i, 0);
                return palette;
            }
        }
        public static Palette BlueScale
        {
            get
            {
                var palette = new Palette();
                for (int i = 0; i < 256; i++) palette[i] = Color.FromArgb(0, 0, i);
                return palette;
            }
        }

        #endregion

        #region Misc functions

        /// <summary>
        /// Converts an HSL color into a .NET Color object
        /// </summary>
        private static Color FromHsv(double h, double s, double v)
        {
            h %= 360;    // limit to 360 degrees

            var c = v * s;
            var x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            var m = v - c;

            double r, g, b;

            if (h >= 0 && h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h >= 180 && h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else if (h >= 300 && h < 360)
            {
                r = c;
                g = 0;
                b = x;
            }
            else throw new Exception("Значення не в межах діапазону 0..360");

            // add m to r, g, b
            r = Clip(r + m);
            g = Clip(g + m);
            b = Clip(b + m);

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)((b * 255)));
        }


        private static double Clip(double input, double min = 0, double max = 1) => input < min ? min : input > max ? max : input;

        #endregion
    }
}
