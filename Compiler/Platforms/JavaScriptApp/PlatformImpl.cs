﻿using Common;
using CommonUtil;
using Platform;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaScriptApp
{
    public class PlatformImpl : AbstractPlatform
    {
        public override string Name { get { return "javascript-app"; } }
        public override string InheritsFrom { get { return "lang-javascript"; } }
        public override string NL { get { return "\n"; } }

        public PlatformImpl()
            : base("JAVASCRIPT")
        { }

        public override IDictionary<string, object> GetConstantFlags()
        {
            return new Dictionary<string, object>();
        }

        public override void ExportStandaloneVm(
            Dictionary<string, FileOutput> output,
            IList<LibraryForExport> everyLibrary)
        {
            throw new NotImplementedException();
        }

        public override void ExportProject(
            Dictionary<string, FileOutput> output,
            IList<LibraryForExport> libraries,
            ResourceDatabase resourceDatabase,
            Options options)
        {
            List<string> jsExtraHead = new List<string>() { options.GetStringOrEmpty(ExportOptionKey.JS_HEAD_EXTRAS) };

            if (options.GetBool(ExportOptionKey.JS_FULL_PAGE))
            {
                jsExtraHead.Add(
                    "<script type=\"text/javascript\">"
                    + "C$common$globalOptions['fullscreen'] = true;"
                    + "</script>");
            }
            options.SetOption(ExportOptionKey.JS_HEAD_EXTRAS, string.Join("\n", jsExtraHead));

            Dictionary<string, string> replacements = this.GenerateReplacementDictionary(options, resourceDatabase);

            TemplateReader templateReader = new TemplateReader(new PkgAwareFileUtil(), this);
            TemplateSet vmTemplates = templateReader.GetVmTemplates();

            output["vm.js"] = new FileOutput()
            {
                Type = FileOutputType.Text,
                TextContent = vmTemplates.GetText("vm.js"),
            };

            List<LibraryForExport> librariesWithCode = new List<LibraryForExport>();
            foreach (LibraryForExport library in libraries.Where(lib => lib.HasNativeCode))
            {
                string libraryName = library.Name;
                TemplateSet libTemplates = templateReader.GetLibraryTemplates(library);

                List<string> libraryLines = new List<string>();
                libraryLines.Add(libTemplates.GetText("gen/lib_" + libraryName.ToLowerInvariant() + ".js"));
                libraryLines.Add("");
                libraryLines.Add("C$common$scrapeLibFuncNames('" + libraryName.ToLowerInvariant() + "');");
                libraryLines.Add("");

                // add helper functions after the scrape.

                foreach (string jsHelperFile in libTemplates.GetPaths("source/", ".js"))
                {
                    libraryLines.Add(libTemplates.GetText(jsHelperFile));
                    libraryLines.Add("");
                }

                output["libs/lib_" + libraryName.ToLowerInvariant() + ".js"] = new FileOutput()
                {
                    Type = FileOutputType.Text,
                    TextContent = string.Join(this.NL, libraryLines),
                };
                librariesWithCode.Add(library);
            }

            Dictionary<string, string> htmlReplacements = new Dictionary<string, string>(replacements);
            replacements["JS_LIB_INCLUSIONS"] = GenerateJsLibInclusionHtml(output.Keys);

            this.CopyResourceAsText(output, "index.html", "Resources/HostHtml.txt", replacements);
            this.CopyResourceAsText(output, "test_server.py", "Resources/TestServerPy.txt", replacements);

            this.CopyResourceAsText(output, "common.js", "Resources/Common.txt", replacements);

            System.Text.StringBuilder resourcesJs = new System.Text.StringBuilder();

            foreach (FileOutput textResource in resourceDatabase.TextResources)
            {
                resourcesJs.Append("C$common$addTextRes(");
                resourcesJs.Append(StringTokenUtil.ConvertStringValueToCode(textResource.CanonicalFileName));
                resourcesJs.Append(", ");
                resourcesJs.Append(StringTokenUtil.ConvertStringValueToCode(textResource.TextContent));
                resourcesJs.Append(");\n");
            }

            foreach (FileOutput fontResource in resourceDatabase.FontResources)
            {
                resourcesJs.Append("C$common$addBinaryRes(");
                resourcesJs.Append(StringTokenUtil.ConvertStringValueToCode(fontResource.CanonicalFileName));
                resourcesJs.Append(", '");
                resourcesJs.Append(Base64.ToBase64(fontResource.GetFinalBinaryContent()));
                resourcesJs.Append("');\n");
            }

            FileOutput imageSheetManifest = resourceDatabase.ImageSheetManifestFile;
            resourcesJs.Append("C$common$addTextRes('image_sheets.txt', ");
            resourcesJs.Append(imageSheetManifest == null ? "''" : StringTokenUtil.ConvertStringValueToCode(imageSheetManifest.TextContent));
            resourcesJs.Append(");\n");

            resourcesJs.Append("C$common$resourceManifest = ");
            resourcesJs.Append(StringTokenUtil.ConvertStringValueToCode(resourceDatabase.ResourceManifestFile.TextContent));
            resourcesJs.Append(";\n");

            string filePrefix = options.GetStringOrNull(ExportOptionKey.JS_FILE_PREFIX);
            if (filePrefix != null)
            {
                resourcesJs.Append("C$common$jsFilePrefix = ");
                resourcesJs.Append(StringTokenUtil.ConvertStringValueToCode(filePrefix));
                resourcesJs.Append(";\n");
            }

            output["resources.js"] = new FileOutput()
            {
                Type = FileOutputType.Text,
                TextContent = resourcesJs.ToString(),
            };

            output["bytecode.js"] = new FileOutput()
            {
                Type = FileOutputType.Text,
                TextContent = "C$bytecode = " + StringTokenUtil.ConvertStringValueToCode(resourceDatabase.ByteCodeFile.TextContent) + ";",
            };

            foreach (string imageResourceFile in resourceDatabase.ImageSheetFiles.Keys)
            {
                FileOutput file = resourceDatabase.ImageSheetFiles[imageResourceFile];
                output["resources/images/" + imageResourceFile] = file;
            }

            foreach (FileOutput audioResourceFile in resourceDatabase.AudioResources)
            {
                output["resources/audio/" + audioResourceFile.CanonicalFileName] = audioResourceFile;
            }

            // TODO: minify JavaScript across all of output dictionary
        }

        public static string GenerateJsLibInclusionHtml(ICollection<string> filesIncluded)
        {
            string[] libraryPathsIncluded = filesIncluded.Where(s => s.StartsWith("libs/lib_")).OrderBy(s => s).ToArray();
            if (libraryPathsIncluded.Length > 0)
            {
                return
                    "<script type=\"text/javascript\" src=\"" +
                    string.Join(
                        "\"></script>\n<script type=\"text/javascript\" src=\"",
                        libraryPathsIncluded) +
                    "\"></script>";
            }
            return "";
        }

        public override Dictionary<string, string> GenerateReplacementDictionary(
            Options options,
            ResourceDatabase resDb)
        {
            return CommonUtil.Collections.DictionaryUtil.MergeDictionaries(
                this.ParentPlatform.GenerateReplacementDictionary(options, resDb),
                new Dictionary<string, string>()
                {
                    { "PROJECT_TITLE", options.GetString(ExportOptionKey.PROJECT_TITLE, "Untitled") },
                    {
                        "FAVICON",
                        options.GetBool(ExportOptionKey.HAS_ICON)
                            ? "<link rel=\"shortcut icon\" href=\"" + options.GetStringOrEmpty(ExportOptionKey.JS_FILE_PREFIX) + "favicon.ico\">"
                            : ""
                    },
                    {
                        "CSS_EXTRA",
                        options.GetBool(ExportOptionKey.JS_FULL_PAGE)
                            ? ("#crayon_host { background-color:#000; text-align:left; width: 100%; height: 100%; }\n"
                                + "body { overflow:hidden; }")
                            : ""
                    },
                    { "JS_EXTRA_HEAD", options.GetStringOrEmpty(ExportOptionKey.JS_HEAD_EXTRAS) },
                });
        }
    }
}
