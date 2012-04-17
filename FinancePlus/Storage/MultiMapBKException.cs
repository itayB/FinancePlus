using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinancePlus.PersistentLayer
{
    /// <summary>
    /// MultiMap collection's exception class;
    /// </summary>
    public class MultiMapBKException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public MultiMapBKException()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExceptionParam"></param>
        /// <param name="ExMessage"></param>
        public MultiMapBKException(Exception ExceptionParam, string ExMessage)
            : base(ExMessage, ExceptionParam)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        public MultiMapBKException(string Message)
            : base(Message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public MultiMapBKException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }


    }
}
