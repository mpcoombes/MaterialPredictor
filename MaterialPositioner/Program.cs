using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.App.Analyst;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.CSV.Segregate;
using Encog.App.Analyst.CSV.Shuffle;
using Encog.App.Analyst.Script;
using Encog.App.Analyst.Wizard;
using Encog.Engine.Network.Activation;
using Encog.ML;
using Encog.ML.Bayesian;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Persist;
using Encog.Util.Arrayutil;
using Encog.Util.CSV;
using Encog.Util.Simple;

namespace MaterialPositioner
{
    class Program
    {
        private const bool Classification = false;

        private static void Main(string[] args)
        {

            Parameters = new Dictionary<string, int>();

            int hiddenLayers = 6;

          
            int i = 0;

            System.Threading.Thread.Sleep(100);
            i++;
            // Step 1
            Shuffle(Config.BaseFile, Config.ShuffledBaseFile);

            // Step 2
            Segregate(Config.ShuffledBaseFile, Config.TrainingFile, Config.EvaluationFile, 99, 1);
            Segregate(Config.RealRunnerFile, Config.RealEvaluationFile, Config.DummyEvaluationFile, 100, 0);

            // Step 3
            Normalize();
            // Step 4

            CreateNetwork(Config.TrainedNetworkFile, hiddenLayers);
            // Step 5
            TrainNetwork();
            // Step 6

            Evaluate();

            EvaluateNewInput();


            Console.WriteLine("press any key to exit...");
            Console.ReadLine();
        }

        static void Shuffle(FileInfo source, FileInfo output)
        {
            var shuffle = new ShuffleCSV();
            shuffle.Analyze(source, true, CSVFormat.English);
            shuffle.ProduceOutputHeaders = true;
            shuffle.Process(output);
        }

        static void Segregate(FileInfo source, FileInfo file1, FileInfo file2, int percentage1, int percentage2)
        {
            var segrate = new SegregateCSV();
            segrate.Targets.Add(new SegregateTargetPercent(file1, percentage1));
            segrate.Targets.Add(new SegregateTargetPercent(file2, percentage2));
            segrate.ProduceOutputHeaders = true;
            segrate.Analyze(source, true, CSVFormat.English);
            segrate.Process();
        }



        static void Normalize()
        {
            var analyst = new EncogAnalyst();

            var wizard = new AnalystWizard(analyst);
            wizard.Goal = AnalystGoal.Regression;

            wizard.Wizard(Config.BaseFile, true, AnalystFileFormat.DecpntComma);

            int num = 0;
            foreach (var field in analyst.Script.Normalize.NormalizedFields)
            {
                Parameters[field.Name] = num;

                field.Action = Encog.Util.Arrayutil.NormalizationAction.Normalize;
                Console.WriteLine("Name {0} and number {1}", field.Name, num);

                num++;
            }


            var norm = new AnalystNormalizeCSV();
            norm.ProduceOutputHeaders = true;
            norm.Analyze(Config.TrainingFile, true, CSVFormat.English, analyst);
            norm.Normalize(Config.NormalizedTrainingFile);
            norm.Analyze(Config.EvaluationFile, true, CSVFormat.English, analyst);
            norm.Normalize(Config.NormalizedEvaluationFile);
            norm.Analyze(Config.RealEvaluationFile, true, CSVFormat.English, analyst);
            norm.Normalize(Config.NormalizedRealRunnerFile);
            analyst.Save(Config.AnalysisFile);
        }

        static void CreateNetwork(FileInfo networkFile, int HiddenLayers)
        {
            var network = new BasicNetwork();
            //network.AddLayer(new BasicLayer(null, false, 2));
            int numberOfInput = Config.NumberOfInputs;
            network.AddLayer(new BasicLayer(null, false, numberOfInput));
            network.AddLayer(new BasicLayer(new ActivationTANH(), true, 6));
            network.AddLayer(new BasicLayer(new ActivationTANH(), false, 16));



            network.Structure.FinalizeStructure();
            network.Reset();
            EncogDirectoryPersistence.SaveObject(networkFile, (BasicNetwork)network);
        }

        static void TrainNetwork()
        {
            var network = (BasicNetwork)EncogDirectoryPersistence.LoadObject((Config.TrainedNetworkFile));

            var trainingSet = EncogUtility.LoadCSV2Memory(Config.NormalizedTrainingFile.ToString(),
                network.InputCount, network.OutputCount, true, CSVFormat.English, false);
            var train = new ResilientPropagation(network, trainingSet);

            int epoch = 1;
            var previousError = 100.00;
            var change = 100.0;
            do
            {
                train.Iteration();
                Console.WriteLine("Epoch: {0} Error : {1}", epoch, train.Error);
                epoch++;
                change = (previousError - train.Error) / previousError;
                previousError = train.Error;

            } while (change > 0.001);    // Tensile Strength Elastic Limit

            EncogDirectoryPersistence.SaveObject(Config.TrainedNetworkFile, network);
        }

        private static double totalWinningBets { get; set; }
        private static double totalBets { get; set; }


        private static void Evaluate()
        {
            var network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(Config.TrainedNetworkFile);
            var analyst = new EncogAnalyst();
            analyst.Load(Config.AnalysisFile.ToString());
            var evaluationSet = EncogUtility.LoadCSV2Memory(Config.NormalizedEvaluationFile.ToString(),
                network.InputCount, network.OutputCount, true, CSVFormat.English, false);

            int n = 0;
            using (var file = new System.IO.StreamWriter(Config.ValidationResult.ToString()))
            {
                foreach (var item in evaluationSet)
                {
                    var actualLine = "Actual,";
                    var resultLine = "Predicted,";
                    int p = 0;


                    for (int inputIndex = 0; inputIndex < item.Input.Count; ++inputIndex)
                    {
                        p++;
                    }


                    for (var i = 0; i < item.Ideal.Count; ++i)
                    {
                        var NormalizedActualoutput = (BasicMLData)network.Compute(item.Input);

                        var NetworkOutput =
                            analyst.Script.Normalize.NormalizedFields[p].DeNormalize(NormalizedActualoutput.Data[i]);
                        var Actualoutput = analyst.Script.Normalize.NormalizedFields[p].DeNormalize(item.Ideal[i]);

                        resultLine = resultLine + NetworkOutput.ToString() + ",";
                        actualLine = actualLine + Actualoutput.ToString() + ",";

                        p++;
                    }

                    file.WriteLine(resultLine);
                    file.WriteLine(actualLine);
                }
            }
        }



        private static void EvaluateNewInput()
        {

            var network = (BasicNetwork)EncogDirectoryPersistence.LoadObject(Config.TrainedNetworkFile);
            var analyst = new EncogAnalyst();
            analyst.Load(Config.AnalysisFile.ToString());
            var newSet = EncogUtility.LoadCSV2Memory(Config.NormalizedRealRunnerFile.ToString(),
                  network.InputCount, network.OutputCount, true, CSVFormat.English, false);
            using (var file = new System.IO.StreamWriter(Config.OutputResult.ToString()))
            {

                foreach (var item in newSet)
                {
                    var resultLine = "";
                    int p = 0;

                    for (int inputIndex = 0; inputIndex < item.Input.Count; ++inputIndex)
                    {
                        p++;
                    }


                    var NormalizedActualoutput = (BasicMLData)network.Compute(item.Input);

                    for (var i = 0; i < NormalizedActualoutput.Data.Length; ++i)
                    {

                        var NetworkOutput =
                            analyst.Script.Normalize.NormalizedFields[p].DeNormalize(NormalizedActualoutput.Data[i]);

                        p++;
                        resultLine = resultLine + NetworkOutput.ToString() + ",";
                    }

                    file.WriteLine(resultLine);

                }
            }

        }

        public static Dictionary<string, int> Parameters { get; set; }
    }
}
