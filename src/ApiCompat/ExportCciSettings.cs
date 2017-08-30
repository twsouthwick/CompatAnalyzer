using Microsoft.Cci;
using System.Collections.Generic;
using System.Composition;

namespace ApiCompat
{
    internal class ExportCciSettings
    {
        public static IEqualityComparer<ITypeReference> StaticSettings { get; set; }

        public ExportCciSettings()
        {
            Settings = StaticSettings;
        }

        [Export(typeof(IEqualityComparer<ITypeReference>))]
        public IEqualityComparer<ITypeReference> Settings { get; set; }
    }
}
