namespace csmacnz.Coveralls.Parsers;

internal interface IXmlCoverageParser
{
    List<FileCoverageData> GenerateSourceFiles(XDocument document);
}
