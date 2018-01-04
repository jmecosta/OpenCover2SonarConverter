module OpenCoverConverter

open System
open System.IO
open System.Text
open FSharp.Data
open TestImpactRunnerApi.Tia

let CollectMergeData(data:OpenCoverXmlHelpers.OpenCoverXml.CoverageSession, contertPath:string, endPath:string, ignoreUnTrackedCov:bool) =
    data.Modules
    |> Seq.iter (fun elem -> CoveragePointData.ParseModule(elem, contertPath, endPath, ignoreUnTrackedCov))

let UpdateTestImpactData(trackedReferenceData : Map<string, MethodTrackedRef>,
                         trackedTestMethodsData : Map<string, MethodRef>,
                         idResolver : Map<int, string>,
                         testImpactMapTests:TiaMap) = 

    // lets populate the test impact map with the cached data from this file
    let createMapForTestMethod(data:MethodRef) =
        if not(testImpactMapTests.ImpactTestMap.ContainsKey(data.Name)) then
            let newImpactedTest = TestImpactMapTest()
            testImpactMapTests.ImpactTestMap.Add(data.Name, newImpactedTest)

        let impactTest = testImpactMapTests.ImpactTestMap.[data.Name]
        
        let collectUidRef(ref:int, refData:MethodTrackedRef) =
            if ref = data.Uid then

                let fileName = idResolver.[refData.FileRef]
                let newDependency = 
                    let data = impactTest.Dependencies
                               |> Seq.tryFind (fun elem -> elem.FileName.Equals(fileName))

                    match data with
                    | Some value -> value
                    | _ -> let newTestDep = TestImpactDependency()
                           newTestDep.FileName <- fileName
                           newTestDep

                if not(newDependency.Methods.Contains(refData.MethodName)) then
                    newDependency.Methods.Add(refData.MethodName) |> ignore
                    impactTest.Dependencies.Add(newDependency) |> ignore

        let processTrackedMethodsRef(refData:MethodTrackedRef) =

                       
            refData.TrackedTestMethodRefs
            |> Seq.iter (fun element ->  collectUidRef(element, refData))

        // collect all methods that are called by this method
        //let keyMethod = sprintf "%i_%s" data.FileRef.Value.Uid method.Name
        trackedReferenceData
        |> Map.iter (fun _ dataCached -> processTrackedMethodsRef(dataCached))

    trackedTestMethodsData
    |> Map.iter (fun _ data -> createMapForTestMethod(data))

let ProcessFile(file:string, searchString:string, fail : bool, endPath:string, ignoreUnTrackedCov:bool, testImpactMapTests:TiaMap) = 

    CoveragePointData.trackedMethodsData <- Map.empty
    CoveragePointData.trackedReferenceData <- Map.empty

    let content = File.ReadAllText(file)
    GC.Collect()
    let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
    GC.Collect()

    let searchString = "\\" + searchString + "\\"

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

    CoveragePointData.idResolver <- Map.empty
    CollectMergeData(xmldata, convertPath, endPath, ignoreUnTrackedCov)
    UpdateTestImpactData(CoveragePointData.trackedReferenceData, CoveragePointData.trackedMethodsData, CoveragePointData.idResolver, testImpactMapTests)


let CreateMergeCoverageFileJson(file:string) = 
    let stringBuilder = StringBuilder()
    stringBuilder.AppendLine("[") |> ignore
    let PrintSeqPoint(cov:CoveragePointData.Line) = 
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
            let ProceedPath(path:CoveragePointData.Path) = 
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


    let PrintCovPoint(cov:CoveragePointData.Coverage) = 
        stringBuilder.AppendLine("  {") |> ignore
        let path = sprintf "    \"file\": \"%s\"," (cov.Path.Replace("\\", "/"))
        stringBuilder.AppendLine(path) |> ignore
        stringBuilder.AppendLine("    \"lines\": [") |> ignore
        cov.GetCoverageData() |> Seq.iter (fun point -> PrintSeqPoint(point))
        stringBuilder.AppendLine("     ]") |> ignore
        stringBuilder.AppendLine("  },") |> ignore

    CoveragePointData.cacheData
    |> Map.toSeq
    |> Seq.iter (fun (uid,cov) -> PrintCovPoint(cov))
    stringBuilder.AppendLine("]") |> ignore

    File.WriteAllText(file, stringBuilder.ToString())
    Path.GetFileName(file)


let CreateMergeCoverageFile(file:string) = 
    use streamWriter = new StreamWriter(file, false)
    streamWriter.WriteLine("<coverage version=\"1\">")
    let PrintSeqPoint(cov:CoveragePointData.Line) = 
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


    let PrintCovPoint(cov:CoveragePointData.Coverage) = 
        let path = sprintf "<file path=\"%s\">" cov.Path
        streamWriter.WriteLine(path)
        cov.GetCoverageData() |> Seq.iter (fun point -> PrintSeqPoint(point))
        streamWriter.WriteLine("</file>")

    CoveragePointData.cacheData
    |> Map.toSeq
    |> Seq.iter (fun (uid,cov) -> PrintCovPoint(cov))

    streamWriter.WriteLine("</coverage>")
    Path.GetFileName(file)


