using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSDynamoDBS3.Helpers
{
    public class ToolKit
    {
        public static String Normalize(String nombre)
        {
            String nombenormalizado = nombre.Replace(' ', '_');
            nombenormalizado = nombenormalizado.ToLower();
            return nombenormalizado;
        }
    }
}
