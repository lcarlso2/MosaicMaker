﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupEMosaicMaker.Model
{
    public class ImageManipulator
    {
        #region Methods

        public static void DrawGrid(byte[] sourcePixels, uint imageWidth, uint imageHeight, int blockSize)
        {
            for (var i = 0; i < imageHeight; i++)
            {
                for (var j = 0; j < imageWidth; j += blockSize)
                {
                    var pixelColor = Color.FromArgb(255, 255, 255, 255);
                    setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    setPixelBgra8(sourcePixels, i, j + 1, pixelColor, imageWidth, imageHeight);
                }
            }

            for (var i = 0; i < imageHeight; i += blockSize)
            {
                for (var j = 0; j < imageWidth; j++)
                {
                    var pixelColor = Color.FromArgb(255, 255, 255, 255);
                    setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    setPixelBgra8(sourcePixels, i + 1, j, pixelColor, imageWidth, imageHeight);
                }
            }
        }

        public static void CreateMosaic(byte[] sourcePixels, uint imageWidth, uint imageHeight, int blockSize)
        {
            var currentPixelHeight = 0;
            var currentPixelMaxHeight = blockSize;
            var currentPixelWidth = 0;
            var currentPixelMaxWidth = blockSize;
            var maxForBlockHeight = Convert.ToInt32(Math.Round(Convert.ToDecimal(imageHeight / blockSize))) + 1;
            var maxForBlockWidth = Convert.ToInt32(Math.Round(Convert.ToDecimal(imageWidth / blockSize))) + 1;
            for (var blockHeight = 0;
                blockHeight < maxForBlockHeight;
                blockHeight++)
            {
                for (var blockWidth = 0;
                    blockWidth < maxForBlockWidth;
                    blockWidth++)
                {
                    var byteCollection = new Collection<byte>();
                    var totalRed = 0;
                    var totalBlue = 0;
                    var totalGreen = 0;
                    var pixelCounter = 0;

                    for (var i = currentPixelHeight; i < currentPixelMaxHeight; i++)
                    {
                        for (var j = currentPixelWidth; j < currentPixelMaxWidth; j++)
                        {
                            //var pixelColor = getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                            //totalRed += pixelColor.R;
                            //totalBlue += pixelColor.B;
                            //totalGreen += pixelColor.G;
                            //pixelCounter++;
                            var myPixels = getPixelAt(sourcePixels, i, j, imageWidth);
                            foreach(var pixel in myPixels)
                            {
                                byteCollection.Add(pixel);
                            }
                            

                        }
                    }

                    Panel.FillPanelWithAverageColor(byteCollection.ToArray());
                    //for (var i = currentPixelHeight; i < currentPixelMaxHeight; i++)
                    //{
                    //    for (var j = currentPixelWidth; j < currentPixelMaxWidth; j++)
                    //    {
                    //        var pixelColor = getPixelBgra8(sourcePixels, i, j, imageWidth, imageHeight);
                    //       // pixelColor.R = BitConverter.GetBytes(totalRed / pixelCounter)[0];
                    //       // pixelColor.B = BitConverter.GetBytes(totalBlue / pixelCounter)[0];
                    //       // pixelColor.G = BitConverter.GetBytes(totalGreen / pixelCounter)[0];
                    //        pixelColor.R = (byte) (totalRed / pixelCounter);
                    //        pixelColor.B = (byte) (totalBlue / pixelCounter);
                    //        pixelColor.G = (byte) (totalGreen / pixelCounter);
                    //        setPixelBgra8(sourcePixels, i, j, pixelColor, imageWidth, imageHeight);
                    //    }
                    //}

                    currentPixelWidth += blockSize;
                    if (currentPixelMaxWidth + blockSize > imageWidth)
                    {
                        currentPixelMaxWidth += (Convert.ToInt32(imageWidth) - currentPixelMaxWidth);
                    }
                    else
                    {
                        currentPixelMaxWidth += blockSize;
                    }

                }
                currentPixelHeight += blockSize;
                if (currentPixelMaxHeight + blockSize > imageHeight)
                {
                    currentPixelMaxHeight += (Convert.ToInt32(imageHeight) - currentPixelMaxHeight);
                }
                else
                {
                    currentPixelMaxHeight += blockSize;
                }
                currentPixelWidth = 0;
                currentPixelMaxWidth = blockSize;
            }
        }

        private static byte[] getPixelAt(byte[] pixels, int x, int y, uint width)
        {
            var offset = (x * (int)width + y) * 4;
            return new[] { pixels[offset], pixels[offset + 1], pixels[offset + 2], pixels[offset + 3] };
        }

        private async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        private static Color getPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int) width + y) * 4;
            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        private static void setPixelBgra8(byte[] pixels, int x, int y, Color color, uint width, uint height)
        {
            var offset = (x * (int) width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        #endregion
    }
}