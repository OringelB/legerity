namespace Legerity.PageObjectGenerator.Infrastructure.Configuration
{
    using System;
    using CommandLine;

    public class GeneratorOptions
    {
        [Option('p', HelpText = "The path to the folder where platform pages exist that will be generating page objects for. Default to current folder.")]
        public string Path { get; set; } = Environment.CurrentDirectory;

        [Option('t', Required = true, HelpText = "The type of platform that will be generating page objects for.")]
        public PlatformType PlatformType { get; set; }
    }
}