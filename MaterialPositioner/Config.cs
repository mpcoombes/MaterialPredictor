using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Encog.Util.File;

namespace MaterialPositioner
{
    public static class Config
    {
       

        public static FileInfo BasePath = new FileInfo(@"F:\GIT\ENCOG\encog-dotnet-core-3.1.0\Data\\");

        public static int NumberOfInputs = 1;
        #region "Step 1"

        public static FileInfo BaseFile = FileUtil.CombinePath(BasePath, "Input.csv");
//        public static FileInfo BaseFile = FileUtil.CombinePath(BasePath, "YieldStrengthELModulusBase.csv");

        public static FileInfo ShuffledBaseFile = FileUtil.CombinePath(BasePath, "ShuffledMaterials.csv");

        #endregion

        #region "Step 2"

        public static FileInfo TrainingFile = FileUtil.CombinePath(BasePath, "TrainingMaterials.csv");
        public static FileInfo EvaluationFile = FileUtil.CombinePath(BasePath, "EvaluationMaterials.csv");

        #endregion

        #region "Step 3"

        public static FileInfo NormalizedTrainingFile = FileUtil.CombinePath(BasePath, "NormTrainingMaterials.csv");
        public static FileInfo NormalizedEvaluationFile = FileUtil.CombinePath(BasePath, "NormEvaluationMaterials.csv");
        public static FileInfo AnalysisFile = FileUtil.CombinePath(BasePath, "AnalysisMaterials.ega");

        #endregion

        #region "Step 4"

        public static FileInfo TrainedNetworkFile = FileUtil.CombinePath(BasePath, "Density_Train.eg");

        #endregion

        #region "Step 5"

        #endregion

        #region "Step 6"

        public static FileInfo ValidationResult = FileUtil.CombinePath(BasePath, "Density_ValidationResult.csv");

        #endregion


        #region "Step 7"

        public static FileInfo RealRunnerFile = FileUtil.CombinePath(BasePath, "RealRunnerFile.csv");
        public static FileInfo NormalizedRealRunnerFile = FileUtil.CombinePath(BasePath, "NormalizedRealRunnerFile.csv");

        #endregion

        #region "Step 8"

        public static FileInfo OutputResult = FileUtil.CombinePath(BasePath, "OutputResult.csv");
        public static FileInfo ShuffledFile = FileUtil.CombinePath(BasePath, "ShuffledReal.csv");
        public static FileInfo RealEvaluationFile = FileUtil.CombinePath(BasePath, "RealEvaluationFile.csv");
        public static FileInfo DummyEvaluationFile = FileUtil.CombinePath(BasePath, "DummyEvaluationFile.csv");

        #endregion
    }
}
