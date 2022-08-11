using cyber_extension.dll_base.extension;
using System;

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
