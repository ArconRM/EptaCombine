namespace LatexCompiler.Options;

public class CompilerSettings
{
    public string TempSubdirectory { get; set; }

    public string FullTempPath => Path.Combine(Path.GetTempPath(), TempSubdirectory);
}