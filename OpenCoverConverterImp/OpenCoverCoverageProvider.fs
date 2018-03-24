﻿namespace OpenCoverConverterImp

open TestImpactRunnerApi
open System.IO
open System
open TestImpactRunnerApi.Json

type CollectAllCoveredMethodsProvider(openCoverPath:string, workingPath:string) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.EnableCoverageBeforeTest(): unit = 
            ()

        member this.GenerateImpactMapForSession(rootPath:string): TiaMapData = 
            TiaMapData(rootPath, System.Collections.Generic.List<string>())

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForTest(test, exclusions, rootPath, outputReportXml)

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string): string * string = 
            let executable = openCoverPath
            let startupArguments = "-register:user -target:" + executableIn + " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml
            (executable, startupArguments)

        member this.DisableCoverageAfterTest() =
            ()

type CollectAllTrackedTestMethodsProvider(openCoverPath:string, workingPath:string) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.EnableCoverageBeforeTest(): unit = 
            ()

        member this.GenerateImpactMapForSession(rootPath:string): TiaMapData = 
            TiaMapData(rootPath, System.Collections.Generic.List<string>())

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForAllTrackedTestMethods(test, exclusions, rootPath, outputReportXml)

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string): string * string = 
            let executable = openCoverPath
            let startupArguments = "-register:user -coverbytest:* -target:" + executableIn + " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml
            (executable, startupArguments)

        member this.DisableCoverageAfterTest() =
            ()

type CollectCoveredMethodsForTestProvider(openCoverPath:string, workingPath:string) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.EnableCoverageBeforeTest(): unit = 
            ()

        member this.GenerateImpactMapForSession(rootPath:string): TiaMapData = 
            TiaMapData(rootPath, System.Collections.Generic.List<string>())

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string): TiaMapData = 
            OpenCoverConverterImpHelpers.GenerateImpactMapForTrackedMethodsByTest(test, exclusions, rootPath, outputReportXml)

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string): string * string = 
            let executable = openCoverPath
            let startupArguments = "-register:user -coverbytest:* -target:" + executableIn + " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml
            (executable, startupArguments)

        member this.DisableCoverageAfterTest() =
            ()