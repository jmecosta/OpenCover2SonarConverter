namespace OpenCoverConverterImp

open TestImpactRunnerApi
open System.IO
open System
open TestImpactRunnerApi.Json

type OpenCoverCoverageProvider(openCoverPath:string, workingPath:string) =
    let outputReportXml = Path.Combine(workingPath, "openCover.xml")
    interface ICoverageHandle with
        member this.EnableCoverageBeforeTest(): unit = 
            ()

        member this.GenerateImpactMapForSession(): TiaMapData = 
            TiaMapData()

        member this.GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string): TiaMapData = 
            let tiaMap = TiaMapData()
            let exclusionsData = (List.ofSeq exclusions)

            let IsExcluded(fileName:string) = 
                if fileName.StartsWith(rootPath) then
                    let relativePath = fileName.Replace(rootPath + "\\", "")
                    (exclusionsData |> Seq.tryFind (fun x -> relativePath.StartsWith(x))).IsSome
                else
                    true

            let content = File.ReadAllText(outputReportXml)
            GC.Collect()
            let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
            GC.Collect()

            let mutable methods = Map.empty
            let mutable indexData = Map.empty

            let ParseFiles(filedata:OpenCoverXmlHelpers.OpenCoverXml.File) = 
                if not(indexData.ContainsKey(filedata.Uid)) then
                    indexData <- indexData.Add(filedata.Uid, filedata.FullPath)

            let AddVisitedMethodToList(method:OpenCoverXmlHelpers.OpenCoverXml.Method) =
                if method.FileRef.IsSome then
                    let key = sprintf "%i:%s" method.FileRef.Value.Uid method.Name
                    if not(methods.ContainsKey(key)) then
                        methods <- methods.Add(key, method.Name)

            let ParseClass(classdata:OpenCoverXmlHelpers.OpenCoverXml.Class) = 
                classdata.Methods |> Seq.iter (fun method -> if method.Visited then AddVisitedMethodToList(method))

            let ParseModule(moduledata:OpenCoverXmlHelpers.OpenCoverXml.Module) =  
                moduledata.Files |> Seq.iter (fun file -> ParseFiles(file))
                moduledata.Classes |> Seq.iter (fun classdata -> ParseClass(classdata))

            xmldata.Modules 
            |> Seq.iter (fun modulo -> if modulo.SkippedDueTo.IsNone then ParseModule(modulo))

            for entry in methods do
                let keyForFile = entry.Key.Substring(0, entry.Key.IndexOf(':'))
                let keyForMethod = entry.Value
                let fileName = indexData.[keyForFile |> int]

                if not(IsExcluded(fileName)) then
                    let relevantFileDep = 
                        let depFile = tiaMap.Files |> Seq.tryFind (fun x -> x.FileName.Equals(fileName))
                        match depFile with
                        | Some devalue -> devalue
                        | _ ->  let impact = TiaFileData(fileName)
                                tiaMap.Files.Add(impact)
                                impact

                    let relevatMethodDep = 
                        let depMethodOption = relevantFileDep.Methods |> Seq.tryFind (fun x -> x.Name.Equals(keyForMethod))
                        match depMethodOption with
                        | Some depMethod -> depMethod
                        | _ ->  let impactMethod = TiaMethodData(keyForMethod)
                                relevantFileDep.Methods.Add(impactMethod)
                                impactMethod

                    let depTestOption = relevatMethodDep.Tests |> Seq.tryFind (fun x -> x.Equals(test.TestName))
                    match depTestOption with
                    | Some _ -> ()
                    | _ ->  relevatMethodDep.Tests.Add(test.TestName)

            tiaMap

        member this.TransformRunArgumentsForRun(executableIn: string, startupArgumentsIn: string): string * string = 
            let executable = openCoverPath
            let startupArguments = "-register:user -target:" + executableIn + " -targetargs:\"" + startupArgumentsIn + "\" -mergebyhash -output:" +  outputReportXml
            (executable, startupArguments)

        member this.DisableCoverageAfterTest() =
            ()