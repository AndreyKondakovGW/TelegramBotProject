using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork1
{
    class ModelLoader
    {
        bool trainNetworks = true;
        int input_size = 196;
        string train_dataset_path = @"../../dataset";
        public Dataset traindata;
        SamplesSet train_set;
        public ModelLoader(bool train = true)
        {
            trainNetworks = train; 
            traindata = new Dataset(train_dataset_path);
            traindata.LoadDataset();
            Console.WriteLine("Загружаем данные...");
            train_set = traindata.toSampleSet(true);
        }

        public BaseNetwork loadAccordNetwork()
        {
            int[] net_arch = { input_size , 100, traindata.num_classes };
            AccordNet accordNet = new AccordNet(net_arch);

            if (trainNetworks)
            {
                Console.WriteLine("Обучаем AccordNet...");
                accordNet.TrainOnDataSet(train_set, 100, 0.01, true);
                double accuracy = train_set.TestNeuralNetwork(accordNet);
                Console.WriteLine($"Accuracy Aforge {accuracy} ");
            }
            return accordNet;
        }

        public BaseNetwork loadStudentNetwork()
        {
            int[] net_arch = { input_size, 200, traindata.num_classes };
            StudentNetwork studentNet = new StudentNetwork(net_arch);

            if (trainNetworks)
            {
                Console.WriteLine("Обучаем studentNet...");
                studentNet.TrainOnDataSet(train_set, 200, 0.01, true);
                double accuracy_student = train_set.TestNeuralNetwork(studentNet);
                Console.WriteLine($"Accuracy Student{accuracy_student}");
            }
            return studentNet;
        }
    }
}
