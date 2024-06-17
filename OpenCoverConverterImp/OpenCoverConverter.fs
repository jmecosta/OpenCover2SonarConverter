module OpenCoverConverter

open System
open System.IO
open System.Text
open FSharp.Data
open TestImpactRunnerApi
open OpenCoverCoverage
open TestImpactRunnerApi.Json
open System.Text.RegularExpressions

let CollectMergeData(data:OpenCoverXmlHelpers.OpenCoverXml.CoverageSession, contertPath:string, endPath:string, ignoreUnTrackedCov:bool, xmlReportProvidingCoverage:string, lineAndFileToPrint:string) =
    data.Modules
    |> Seq.iter (fun elem -> OpenCoverCoverage.ParseModule(elem, contertPath, endPath, ignoreUnTrackedCov, xmlReportProvidingCoverage, lineAndFileToPrint))

let ReplaceCharacterSequences (input: string) =
    let pattern = "&#x([0-9A-Fa-f]+);"

    let replaced = Regex.Replace(input, pattern, fun (m : Match) ->
        let hexNumber = m.Groups.[1].Value
        let intValue = Convert.ToInt32(hexNumber, 16)
        let replacement = sprintf "ObfuscatedFunctionMethod%i" intValue
        replacement)

    replaced

let UpdateTestImpactData(trackedReferenceData : Map<string, MethodTrackedRef>,
                         trackedTestMethodsData : Map<string, MethodRef>,
                         idResolver : Map<string, string>,
                         testImpactMapTests:TiaMapData,
                         workDir:string,
                         exclusions: List<string>) = 

    let pathWithoutDrive = workDir.Substring(1)
    
    // lets populate the test impact map with the cached data from this file
    let createMapForTestMethod(testMethod:MethodRef) =
        let collectUidRef(ref:int, refData:MethodTrackedRef) =
            if ref = testMethod.Uid then

                let Excluded(fileName:string) = 
                    if fileName.Substring(1).Contains(pathWithoutDrive) then
                        let isInExclusion = exclusions |> Seq.tryFind (fun x -> fileName.Contains(x))
                        match isInExclusion with
                        | Some _ -> true
                        | _ -> false
                    else
                        true

                let fileName = idResolver.[refData.FileRef]
                if not(Excluded(fileName)) then
                    let newFileDependency =
                        let data = testImpactMapTests.Files
                                   |> Seq.tryFind (fun elem -> elem.FileName.Equals(fileName))

                        match data with
                        | Some value -> value
                        | _ -> let newFileDep = TiaFileData(fileName)
                               testImpactMapTests.Files.Add(newFileDep) |> ignore
                               newFileDep

                    let newMethodDependency = 
                        let data = newFileDependency.Methods
                                   |> Seq.tryFind (fun elem -> elem.Name.Equals(refData.MethodName))

                        match data with
                        | Some value -> value
                        | _ -> let newMethodDep = TiaMethodData(refData.MethodName)
                               newFileDependency.Methods.Add(newMethodDep) |> ignore
                               newMethodDep

                    let data = newMethodDependency.Tests
                                |> Seq.tryFind (fun elem -> elem.Equals(testMethod.Name))

                    match data with
                    | Some value -> ()
                    | _ -> newMethodDependency.Tests.Add(TiaTestData(testMethod.Name, "openCover")) |> ignore

        let processTrackedMethodsRef(refData:MethodTrackedRef) =
            refData.TrackedTestMethodRefs
            |> Seq.iter (fun element ->  collectUidRef(element, refData))

        // collect all methods that are called by this method
        //let keyMethod = sprintf "%i_%s" data.FileRef.Value.Uid method.Name
        trackedReferenceData
        |> Map.iter (fun _ dataCached -> processTrackedMethodsRef(dataCached))

    trackedTestMethodsData
    |> Map.iter (fun _ data -> createMapForTestMethod(data))

let ProcessFileForExternalTest(file:string,
                               searchStringIn:string,
                               endPath:string,
                               testImpactMapTests:TiaMapData,
                               testImpact:ImpactedTest,
                               exclusions:string List,
                               workPath:string) = 

    let content = ReplaceCharacterSequences(File.ReadAllText(file))
    GC.Collect()
    let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
    GC.Collect()

    let searchString = "\\" + searchStringIn + "\\"

    let validateFullPath(path:string) = 
        path.Contains(searchString)

    let validateModule(modul:OpenCoverXmlHelpers.OpenCoverXml.Module) = 
        (modul.Files |> Seq.tryFind (fun file -> validateFullPath(file.FullPath))).IsSome

    let mutable convertPath = ""
    let moduledata = xmldata.Modules |> Seq.tryFind (fun modul -> validateModule(modul))
    match moduledata with
    | Some data -> 
        let fileName = (data.Files |> Seq.find (fun file -> validateFullPath(file.FullPath)))
        convertPath <- fileName.FullPath.Split([|searchString|], StringSplitOptions.RemoveEmptyEntries).[0]
    | _ -> ()

    OpenCoverCoverage.idResolver <- Map.empty
    OpenCoverCoverage.trackedMethodsData <- Map.empty

    let methodData = MethodTrackedRef()
    methodData.FileRef <- ""
    methodData.MethodName <- "asda"

    OpenCoverCoverage.trackedReferenceData <- Map.empty.Add("asasd", methodData)
    CollectMergeData(xmldata, convertPath, endPath, true, file, "")
    UpdateTestImpactData(OpenCoverCoverage.trackedReferenceData,
                         OpenCoverCoverage.trackedMethodsData,
                         OpenCoverCoverage.idResolver,
                         testImpactMapTests,
                         workPath,
                         exclusions)

let ProcessFile(file:string,
                searchStringIn:string,
                endPath:string,
                ignoreUnTrackedCov:bool,
                testImpactMapTests:TiaMapData,
                exclusions:string List,
                workPath:string,
                logMethodOrVaribleToStdout:string) = 

    let content = ReplaceCharacterSequences(File.ReadAllText(file))
    GC.Collect()
    let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
    GC.Collect()

    let searchString = "\\" + searchStringIn + "\\"

    let validateFullPath(path:string) = 
        path.Contains(searchString)

    let validateModule(modul:OpenCoverXmlHelpers.OpenCoverXml.Module) = 
        (modul.Files |> Seq.tryFind (fun file -> validateFullPath(file.FullPath))).IsSome

    let mutable convertPath = ""
    let moduledata = xmldata.Modules |> Seq.tryFind (fun modul -> validateModule(modul))
    match moduledata with
    | Some data -> 
        let fileName = (data.Files |> Seq.find (fun file -> validateFullPath(file.FullPath)))
        convertPath <- fileName.FullPath.Split([|searchString|], StringSplitOptions.RemoveEmptyEntries).[0]
    | _ -> ()

    OpenCoverCoverage.idResolver <- Map.empty
    OpenCoverCoverage.trackedMethodsData <- Map.empty
    OpenCoverCoverage.trackedReferenceData <- Map.empty
    CollectMergeData(xmldata, convertPath, endPath, ignoreUnTrackedCov, Path.GetFileNameWithoutExtension(file), logMethodOrVaribleToStdout)
        
    UpdateTestImpactData(OpenCoverCoverage.trackedReferenceData,
                         OpenCoverCoverage.trackedMethodsData,
                         OpenCoverCoverage.idResolver,
                         testImpactMapTests,
                         workPath,
                         exclusions)


let CreateMergeCoverageFileJson(file:string) = 
    let stringBuilder = StringBuilder()
    stringBuilder.AppendLine("[") |> ignore
    let PrintSeqPoint(cov:OpenCoverCoverage.Line) = 
        let brachesToCover = cov.GetOffsetsCover()
        let isCovered = 
            if cov.GetSequenceHits() > 0 then
                "true"
            else
                "false"

        stringBuilder.AppendLine("      {") |> ignore
        stringBuilder.AppendLine((sprintf "        \"line\": \"%i\"," cov.Line)) |> ignore
        stringBuilder.AppendLine((sprintf "        \"covered\": \"%s\"," isCovered)) |> ignore
        stringBuilder.AppendLine("        \"branches\": [") |> ignore

        let GetBranchData =
            let retData = new StringBuilder()
            let mutable branchCnt = 0
            let ProceedPath(path:OpenCoverCoverage.Path) = 
                branchCnt <- branchCnt + 1
                retData.AppendLine("          {") |> ignore
                retData.AppendLine((sprintf "            \"branch\": \"%i\"," branchCnt)) |> ignore
                let isCovered = 
                    if path.Hits() > 0 then
                        "true"
                    else
                        "false"
                retData.AppendLine((sprintf "            \"covered\": \"%s\"," isCovered)) |> ignore
                retData.AppendLine("          },") |> ignore

            brachesToCover |> Seq.iter (fun offset -> offset.GetPaths() |> Seq.iter (fun path -> ProceedPath(path)))
            retData.AppendLine("        ]") |> ignore
            retData.ToString()

        stringBuilder.Append(GetBranchData) |> ignore
        stringBuilder.AppendLine("      },") |> ignore


    let PrintCovPoint(cov:OpenCoverCoverage.Coverage) = 
        stringBuilder.AppendLine("  {") |> ignore
        let path = sprintf "    \"file\": \"%s\"," (cov.Path.Replace("\\", "/"))
        stringBuilder.AppendLine(path) |> ignore
        stringBuilder.AppendLine("    \"lines\": [") |> ignore
        cov.GetCoverageData()
         |> Seq.sortBy (fun line -> line.Line)
         |> Seq.iter (fun point -> PrintSeqPoint(point))
        stringBuilder.AppendLine("     ]") |> ignore
        stringBuilder.AppendLine("  },") |> ignore

    OpenCoverCoverage.cacheData
    |> Map.toSeq
    |> Seq.iter (fun (uid,cov) -> PrintCovPoint(cov))
    stringBuilder.AppendLine("]") |> ignore

    File.WriteAllText(file, stringBuilder.ToString())
    file


let CreateMergeCoverageFile(file:string) = 
    use streamWriter = new StreamWriter(file, false)
    streamWriter.WriteLine("<coverage version=\"1\">")
    let PrintSeqPoint(cov:OpenCoverCoverage.Line) = 
        let brachesToCover = cov.GetBranchsToCover()
        let coveredBranchs = cov.GetCoveredBranchs()
        let isCovered = 
            if cov.GetSequenceHits() > 0 then
                "true"
            else
                "false"

        let seqpoint = 
            sprintf "  <lineToCover lineNumber=\"%i\" covered=\"%s\" branchesToCover=\"%i\" coveredBranches=\"%i\"/>" cov.Line isCovered brachesToCover coveredBranchs
        streamWriter.WriteLine(seqpoint)


    let PrintCovPoint(cov:OpenCoverCoverage.Coverage) = 
        let path = sprintf "<file path=\"%s\">" cov.Path
        streamWriter.WriteLine(path)
        cov.GetCoverageData() |> Seq.iter (fun point -> PrintSeqPoint(point))
        streamWriter.WriteLine("</file>")

    OpenCoverCoverage.cacheData
    |> Map.toSeq
    |> Seq.iter (fun (uid,cov) -> PrintCovPoint(cov))

    streamWriter.WriteLine("</coverage>")
    file


