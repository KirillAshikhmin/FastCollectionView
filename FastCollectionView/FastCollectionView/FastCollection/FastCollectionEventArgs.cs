//  Based on https://github.com/twintechs/TwinTechsFormsLib
//  Special thanks to Twin Technologies from Binwell Ltd. 

//  Distributed under Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0

using System;

namespace Binwell.Controls.FastCollectionView.FastCollection
{
    public class FastCollectionEventArgs<T> : EventArgs
    {
        public FastCollectionEventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
