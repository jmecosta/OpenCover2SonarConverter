namespace OpenCoverConverterImp

open TestImpactRunnerApi
open System.IO
open System
open TestImpactRunnerApi.Json

type CollectAllCoveredMethodsProvider(openCoverPath:string, workingPath:string) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.GetName() = "OpenCover"
        member this.EnableCoverageBeforeTest(logger:ITestImpactLogger): unit = 
            ()

        member this.GenerateImpactMapForSession(rootPath:string, logger:ITestImpactLogger): TiaMapData = 
            TiaMapData(rootPath, System.Collections.Generic.SortedSet<string>())

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.SortedSet<string>, rootPath:string, logger:ITestImpactLogger): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForTest(test, exclusions, rootPath, outputReportXml)

        member this.GenerateImpactMapForTestGivenAReportFile(test: ImpactedTest, exclusions:System.Collections.Generic.SortedSet<string>, reportFile:string, logger:ITestImpactLogger): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForAllTrackedTestMethods(test, exclusions, workingPath, reportFile)

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string, logger:ITestImpactLogger): string * string = 
            let executable = openCoverPath
            let startupArguments = "-register:user -target:" + executableIn + " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml
            (executable, startupArguments)

        member this.DisableCoverageAfterTest(logger:ITestImpactLogger) =
            ()

type CollectAllTrackedTestMethodsProvider(openCoverPath:string, workingPath:string, filter:string, excludebyFile:string, excludedFolders:string, otherArgsForOpenCover:string, logger:ITestImpactLogger) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.GetName() = "OpenCover"
        member this.EnableCoverageBeforeTest(logger:ITestImpactLogger): unit = 
            ()

        member this.GenerateImpactMapForSession(rootPath:string, logger:ITestImpactLogger): TiaMapData = 
            TiaMapData(rootPath, System.Collections.Generic.SortedSet<string>())

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.SortedSet<string>, rootPath:string, logger:ITestImpactLogger): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForAllTrackedTestMethods(test, exclusions, rootPath, outputReportXml)

        member this.GenerateImpactMapForTestGivenAReportFile(test: ImpactedTest, exclusions:System.Collections.Generic.SortedSet<string>, reportFile:string, logger:ITestImpactLogger): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForAllTrackedTestMethods(test, exclusions, workingPath, reportFile)

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string, logger:ITestImpactLogger): string * string = 
            let executable = openCoverPath
            let startupArguments = "-hideskipped:File;Filter;Attribute;MissingPdb;All -register:user -coverbytest:* -target:" + executableIn +
                                   " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml

            let excludeByFilter =
                if filter <> "" then
                    " -excludebyfile:\"" + filter + "\""
                else
                    ""

            let excludeByFile =
                if excludebyFile <> "" then
                    " -excludebyfile:\"" + excludebyFile + "\""
                else
                    ""

            let excludedByDir = 
                if excludedFolders <> "" then
                    " -excludedirs:\"" + excludedFolders + "\""
                else
                    ""

            (executable, startupArguments + excludeByFilter + excludeByFile + excludedByDir + " " + otherArgsForOpenCover)

        member this.DisableCoverageAfterTest(logger:ITestImpactLogger) =
            ()

type CollectCoveredMethodsForTestProvider(openCoverPath:string, workingPath:string) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.GetName() = "OpenCover"
        member this.EnableCoverageBeforeTest(logger:ITestImpactLogger): unit = 
            ()

        member this.GenerateImpactMapForSession(rootPath:string, logger:ITestImpactLogger): TiaMapData = 
            TiaMapData(rootPath, System.Collections.Generic.SortedSet<string>())

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.SortedSet<string>, rootPath:string, logger:ITestImpactLogger): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForTrackedMethodsByTest(test, exclusions, rootPath, outputReportXml)

        member this.GenerateImpactMapForTestGivenAReportFile(test: ImpactedTest, exclusions:System.Collections.Generic.SortedSet<string>, reportFile:string, logger:ITestImpactLogger): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForAllTrackedTestMethods(test, exclusions, workingPath, reportFile)

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string, logger:ITestImpactLogger): string * string = 
            let executable = openCoverPath
            let startupArguments = "-register:user -coverbytest:* -target:" + executableIn + " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml
            (executable, startupArguments)

        member this.DisableCoverageAfterTest(logger:ITestImpactLogger) =
            ()