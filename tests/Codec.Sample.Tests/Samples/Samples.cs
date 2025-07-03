namespace Codec.Sample.Tests.Samples;

public static class Samples
{
    public static Sample GetSample(string projectName)
    {
        var path = Path.Combine("Samples", projectName);

        return new Sample(path);
    }
}
