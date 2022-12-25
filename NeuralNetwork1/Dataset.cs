using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace AForge.WindowsForms
{
    class Dataset
    {
        public string path;
        public Dictionary<string, Bitmap[]> data = new Dictionary<string, Bitmap[]>();
        public Dictionary<int, string> ind2class = new Dictionary<int, string>();
        public Dictionary<string, int> class2ind = new Dictionary<string, int>();
        public int num_classes;

        public Dataset(string p)
        {
            path = p;
        }
        public void LoadDataset()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] dires = dirInfo.GetDirectories();
            int j = 0;
            foreach (DirectoryInfo dir in dires)
            {
                string classes = dir.Name;
                Bitmap[] files = new Bitmap[dir.GetFiles().Length];
                int i = 0;
                foreach (FileInfo file in dir.GetFiles())
                {
                    Console.WriteLine(file.FullName);
                    Bitmap bmp = new Bitmap(file.FullName);
                    files[i] = bmp;
                    i++;
                }
                data[dir.Name] = files;
                ind2class[j] = dir.Name;
                class2ind[dir.Name] = j;
                j++;
            }
            num_classes = j;
        }

        public SamplesSet toSampleSet(bool shuffle=false)
        {
            SamplesSet set = new SamplesSet();
            foreach (KeyValuePair<string, Bitmap[]> kvp in data)
            {
                int class_ind = class2ind[kvp.Key];
                foreach(Bitmap img in kvp.Value)
                {
                    Sample sample = new Sample(img, num_classes, class_ind);
                    set.AddSample(sample);
                }
            }
            if (shuffle)
            {
                set.Shuffle();
            }
            return set;
        }
    }
}
