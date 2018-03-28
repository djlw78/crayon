﻿using Common;

namespace Crayon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (new PerformanceSection("Crayon"))
            {
                commandLineArgs = args;
#if DEBUG
                // First chance exceptions should crash in debug builds.
                ExecuteProgramUnchecked();
#else
                try
                {
                    ExecuteProgramUnchecked();
                }
                catch (InvalidOperationException e)
                {
                    System.Console.WriteLine(e.Message);
                }
                catch (MultiParserException e)
                {
                    System.Console.WriteLine(e.Message);
                }
                catch (ParserException e)
                {
                    System.Console.WriteLine(e.Message);
                }
#endif

#if DEBUG
                // Crash if there were any graphics contexts that weren't cleaned up.
                // This is okay on Windows, but on OSX this is a problem, so ensure that a
                // regressions are quickly noticed.
                SystemBitmap.Graphics.EnsureCleanedUp();
#endif
            }
        }

        private static string[] commandLineArgs;

        public static string[] GetCommandLineArgs()
        {
            return GetEffectiveArgs(commandLineArgs);
        }

        private static void ExecuteProgramUnchecked()
        {
            CrayonPipelineInterpreter pipeline = new CrayonPipelineInterpreter()
                .RegisterPipeline(
                    "Crayon::Main", typeof(Program).Assembly, "Pipeline.txt")
                .RegisterPipeline(
                    "Exporter::ExportCbxVmBundle", typeof(ExportCommand).Assembly, "Pipeline/ExportCbxVmBundle.txt")

                // TODO: register workers via reflection

                // Crayon
                .RegisterWorker(new GetBuildContextWorker())
                .RegisterWorker(new RunCbxWorker())
                .RegisterWorker(new TopLevelCheckWorker())
                .RegisterWorker(new UsageDisplayWorker())

                // Exporter
                .RegisterWorker(new ExportCbxVmBundleWorker())

                // TODO: these temporary workers need to be pipelines in other assemblies
                .RegisterWorker(new TemporaryWorkers.ExportStandaloneCbxWorker())
                .RegisterWorker(new TemporaryWorkers.ExportStandaloneVmWorker());

            pipeline.Interpret("Crayon::Main");
        }

        private static string[] GetEffectiveArgs(string[] actualArgs)
        {
#if DEBUG
            if (actualArgs.Length == 0)
            {
                string crayonHome = System.Environment.GetEnvironmentVariable("CRAYON_HOME");
                if (crayonHome != null)
                {
                    string debugArgsFile = FileUtil.JoinPath(crayonHome, "DEBUG_ARGS.txt");
                    if (FileUtil.FileExists(debugArgsFile))
                    {
                        string[] debugArgs = FileUtil.ReadFileText(debugArgsFile).Trim().Split('\n');
                        string lastArgSet = debugArgs[debugArgs.Length - 1].Trim();
                        if (lastArgSet.Length > 0)
                        {
                            return lastArgSet.Split(' ');
                        }
                    }
                }
            }
#endif
            return actualArgs;
        }
    }
}
