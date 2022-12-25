using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;



namespace NeuralNetwork1
{
    internal class Processor
    {
        public int targetResolution = 14;

        public int border = 20;

        public int top = 40;
        public int left = 40;
        public Bitmap original;

        public Processor(){}
        public Bitmap ProcessImage(Bitmap bitmap)
        {
            // На вход поступает необработанное изображение с веб-камеры

            //  Минимальная сторона изображения (обычно это высота)
            if (bitmap.Height > bitmap.Width)
                throw new Exception("К такой забавной камере меня жизнь не готовила!");
            //  Можно было, конечено, и не кидаться эксепшенами в истерике, но идите и купите себе нормальную камеру!
            int side = bitmap.Height;

            //  GrayScale
            AForge.Imaging.Filters.Grayscale grayFilter = new AForge.Imaging.Filters.Grayscale(0.2125, 0.7154, 0.0721);
            var uProcessed = grayFilter.Apply(AForge.Imaging.UnmanagedImage.FromManagedImage(bitmap));

            //  Пороговый фильтр применяем. Величина порога берётся из настроек, и меняется на форме
            AForge.Imaging.Filters.BradleyLocalThresholding threshldFilter = new AForge.Imaging.Filters.BradleyLocalThresholding();
            threshldFilter.PixelBrightnessDifferenceLimit = 0.05313726f;
            threshldFilter.ApplyInPlace(uProcessed);

            AForge.Imaging.Filters.Invert InvertFilter = new AForge.Imaging.Filters.Invert();
            InvertFilter.ApplyInPlace(uProcessed);

            ///Создаём BlobCounter, выдёргиваем самый большой кусок, масштабируем, пересечение и сохраняем
            ///изображение в эксклюзивном использовании
            AForge.Imaging.BlobCounterBase bc = new AForge.Imaging.BlobCounter();


            bc.FilterBlobs = true;
            bc.MinWidth = 10;
            bc.MinHeight = 10;
            // Упорядочиваем по размеру
            bc.ObjectsOrder = AForge.Imaging.ObjectsOrder.Size;
            // Обрабатываем картинку
            
            bc.ProcessImage(uProcessed);

            Rectangle[] rects = bc.GetObjectsRectangles();
            if (rects.Length > 0)
            {
                //Чучуть границы
                int step = 100;
                Rectangle biggest_Rect = new Rectangle(rects[0].X - step / 2, rects[0].Y - step / 2, rects[0].Width + step, rects[0].Height + step);

                // Обрезаем края, оставляя только центральные блобчики
                AForge.Imaging.Filters.Crop cropFilter = new AForge.Imaging.Filters.Crop(biggest_Rect);
                uProcessed = cropFilter.Apply(uProcessed);


                //  Масштабируем до 14x14
                AForge.Imaging.Filters.ResizeBicubic scaleFilter = new AForge.Imaging.Filters.ResizeBicubic(targetResolution, targetResolution);
                uProcessed = scaleFilter.Apply(uProcessed);
                InvertFilter.ApplyInPlace(uProcessed);
            }
            else
            {
                return null;
            }
            return uProcessed.ToManagedImage();
        }
    }
}