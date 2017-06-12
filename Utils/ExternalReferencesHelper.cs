using Antlr.Runtime;
using Castle.Core;
using CGLib;
using Common.Logging;
using Nito.KitchenSink.CRC;

namespace ZF.BL.Nesper.Utils
{
    /// <summary>
    ///     Dummy file to help resolve build problem. Do not use anywhere in code.
    /// </summary>
    internal class ExternalReferencesHelper
    {
        private ExternalReferencesHelper(){}
        //--------------------------
        // Important! Do not remove the following private members
        // This declarations make sure that the libraries are copied to the output folder
        // Otherwise they may not be deployed causing runtime error

#pragma warning disable 0169 // disables compiler warning Field ABC is never used
        // ReSharper disable UnusedMember.Local
        private FastBase _fb;
        private ANTLRFileStream _s;
        private IServiceProviderEx _ex;
        private IConfigurationReader _r;
        private CRC16 _crc16;

        // ReSharper restore UnusedMember.Local
#pragma warning restore 0169
        //-------------------------- 
    }
}