﻿using AssemblyResolver;
using Common;
using Exporter.Workers;
using Platform;
using System.Collections.Generic;

namespace Exporter.Pipeline
{
    public static class ExportStandaloneVmPipeline
    {
        public static void Run(ExportCommand command)
        {
            string vmTargetDir = new GetTargetVmExportDirectoryWorker().DoWorkImpl(command);
            AbstractPlatform platform = command.PlatformProvider.GetPlatform(command.VmPlatform);
            AssemblyMetadata[] assemblyMetadataList = new AssemblyFinder().AssemblyFlatList;
            Dictionary<string, FileOutput> fileOutputContext = new Dictionary<string, FileOutput>();
            new ExportStandaloneVmSourceCodeForPlatformWorker().DoWorkImpl(fileOutputContext, platform, assemblyMetadataList, vmTargetDir, command);
            new EmitFilesToDiskWorker().DoWorkImpl(fileOutputContext, vmTargetDir);
        }
    }
}
