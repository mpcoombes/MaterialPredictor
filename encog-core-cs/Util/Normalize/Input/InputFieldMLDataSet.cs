//
// Encog(tm) Core v3.1 - .Net Version
// http://www.heatonresearch.com/encog/
//
// Copyright 2008-2012 Heaton Research, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//   
// For more information on Heaton Research copyrights, licenses 
// and trademarks visit:
// http://www.heatonresearch.com/copyright
//
using System;
using Encog.ML.Data;
using Encog.Neural.NeuralData;

namespace Encog.Util.Normalize.Input
{
    /// <summary>
    /// An input field based on an Encog NeuralDataSet.
    /// </summary>
    [Serializable]
    public class InputFieldMLDataSet : BasicInputField
    {
        /// <summary>
        /// The data set.
        /// </summary>
        private readonly IMLDataSet _data;

        /// <summary>
        /// The input or ideal index.  This treats the input and ideal as one
        /// long array, concatenated together.
        /// </summary>
        private readonly int _offset;

        /// <summary>
        /// Construct a input field based on a NeuralDataSet.
        /// </summary>
        /// <param name="usedForNetworkInput">Is this field used for neural input.</param>
        /// <param name="data">The data set to use.</param>
        /// <param name="offset">The input or ideal index to use. This treats the input 
        /// and ideal as one long array, concatenated together.</param>
        public InputFieldMLDataSet(bool usedForNetworkInput,
                                       IMLDataSet data, int offset)
        {
            _data = data;
            _offset = offset;
            UsedForNetworkInput = usedForNetworkInput;
        }

        /// <summary>
        /// The neural data set to read.
        /// </summary>
        public IMLDataSet NeuralDataSet
        {
            get { return _data; }
        }

        /// <summary>
        /// The field to be accessed. This treats the input and 
        /// ideal as one long array, concatenated together.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
        }
    }
}
