using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestANN
{
    class ImageHelper
    {
        byte[] myImageData;
        Bitmap myImage;
        public int Width{ get{ return myImage.Width; } }
        public int Height { get { return myImage.Height; } }
        public void loadImage(string fileName) {
            myImage = new Bitmap(fileName);
            BitmapData bmpData1 = myImage.LockBits(new Rectangle(0, 0, myImage.Width, myImage.Height),
                             System.Drawing.Imaging.ImageLockMode.ReadWrite, myImage.PixelFormat);
            myImageData = new byte[bmpData1.Stride * bmpData1.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData1.Scan0, myImageData, 0
                                   , myImageData.Length);
            myImage.UnlockBits(bmpData1);

            //Image img = Bitmap.FromFile(fileName);
            //ImageConverter converter = new ImageConverter();
            //byte[] data = (byte[])converter.ConvertTo(img, typeof(byte[]));
            return;
        }
        public void getData(byte[] data, int x, int y, int w, int h)
        {
            for(int i=0;i<w;i++) //w
                for(int j=0;j<h;j++) //h
                {
                    data[i+j*w] = (byte)
                        ((myImageData[((j + y) * myImage.Width + x + i) * 3+0]
                        + myImageData[((j + y) * myImage.Width + x + i) * 3+1]
                        + myImageData[((j + y) * myImage.Width + x + i) * 3+2])/3.0);
                }
        }
        public void setData(byte[] data, int x, int y, int w, int h)
        {
            for (int i = 0; i < w; i++) //w
                for (int j = 0; j < h; j++) //h
                {
                    myImage.SetPixel(x + i, j + y, Color.FromArgb(data[i+j*w], data[i + j * w], data[i + j * w]));
                }
        }
        public void saveImage(string fileName)
        {
            myImage.Save(fileName);
        //    private unsafe Bitmap ToBitmap(double[,] rawImage)
        //{
        //    int width = rawImage.GetLength(1);
        //    int height = rawImage.GetLength(0);

        //    Bitmap Image = new Bitmap(width, height);
        //    BitmapData bitmapData = Image.LockBits(
        //        new Rectangle(0, 0, width, height),
        //        ImageLockMode.ReadWrite,
        //        PixelFormat.Format32bppArgb
        //    );
        //    ColorARGB* startingPosition = (ColorARGB*)bitmapData.Scan0;


        //    for (int i = 0; i < height; i++)
        //        for (int j = 0; j < width; j++)
        //        {
        //            double color = rawImage[i, j];
        //            byte rgb = (byte)(color * 255);

        //            ColorARGB* position = startingPosition + j + i * width;
        //            position->A = 255;
        //            position->R = rgb;
        //            position->G = rgb;
        //            position->B = rgb;
        //        }

        //    Image.UnlockBits(bitmapData);
        //    return Image;
        //}

    }
}
}
