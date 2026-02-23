using System;

namespace SymbX.Shared
{ 
    [Serializable]
    public class SymbXItem
    {
        public string SourcePath {  get; set; }
        public string Link { get; set; }
    }

    public static class SymbXParams
    {
        public const string SYMBX_CORE = "SymbX.Core";
        public const string SYMBX_PIPE = "Global\\SlgFilePipe";
    }
}
