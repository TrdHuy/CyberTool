using System;
using cyber_base.extension;

namespace TestImportLib
{
    public class Class1 : ICyberExtension
    {
        public string ExtensionName => "Test extension";

        public void printf()
        {
            Console.WriteLine("some data");
        }
    }
}
