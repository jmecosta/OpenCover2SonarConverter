module OpenCoverCoverage

type MethodRef() = 
    member val Uid : int = 0 with get, set
    member val Name : string = "" with get, set

[<AllowNullLiteral>]
type MethodTrackedRef() = 
    member val FileRef : string = "" with get, set
    member val MethodName : string = "" with get, set
    member val TrackedTestMethodRefs : Set<int> = Set.empty

type Path(path:int) = 
    let mutable hits : int = 0
    member val Path : int = path
    member this.Hits() = hits
    member this.SetHits(hitdata:int) = 
        hits <- hits + hitdata

type OffsetBranch(offset:int) =
    let mutable paths : Path List = List.Empty
    member val Offset : int = offset

    member this.GetPaths() = paths
    member this.AddPath(path:Path) =
        paths <- paths @ [path]

type Branch() = 
    let mutable offsets : OffsetBranch List = List.Empty

    member this.GetOffSets() = offsets
    member this.GetOffSet(offset:int) =
        offsets |> Seq.tryFind (fun c -> c.Offset = offset)

    member this.AddOffset(offset:OffsetBranch) =
        offsets <- offsets @ [offset]

type Line(line:int) = 
    let mutable hits : int = 0
    let mutable branch : Branch = new Branch()

    member val Line : int = line

    member this.GetOffsetsCover() = 
        branch.GetOffSets()

    member this.GetBranchsToCover() = 
        let mutable branchesToCover = 0
        branch.GetOffSets() |> Seq.iter (fun p -> p.GetPaths() |> Seq.iter (fun p -> branchesToCover <- branchesToCover + 1) )
        branchesToCover

    member this.GetCoveredBranchs() = 
        let mutable coveredBranches = 0
        branch.GetOffSets() |> Seq.iter (fun p -> p.GetPaths() |> Seq.iter (fun p -> if p.Hits() > 0 then coveredBranches <- coveredBranches + 1) )
        coveredBranches

    member this.GetSequenceHits() = hits
    member this.SetLineHit(covered:int) =
        hits <- hits + covered

    member this.SetBranchToCover(path:int, offset:int, hits:int) =
        let offsetpoint = branch.GetOffSet(offset)
        match offsetpoint with
        | Some data ->
            let pathPoint = data.GetPaths() |> Seq.tryFind (fun elem -> elem.Path = path)
            match pathPoint with
            | Some path -> path.SetHits(hits)
            | _ -> 
                let newPath = Path(path)
                newPath.SetHits(hits)
                data.AddPath(newPath)
        | _ ->
            let offset = new OffsetBranch(offset)
            let newPath = Path(path)
            newPath.SetHits(hits)
            offset.AddPath(newPath)
            branch.AddOffset(offset)

type Coverage(path) =
    let mutable coverageData : Line list = List.Empty
    member val Path : string = path
    member this.GetCoverageData() = 
        coverageData
    member this.AddSequenceLineInfo(line:int, hits:int) = 
        let covpoint = coverageData |> Seq.tryFind (fun elem -> elem.Line = line)
        match covpoint with
        | Some data ->
            data.SetLineHit(hits)
        | _ ->
            let newLine = new Line(line)
            newLine.SetLineHit(hits)
            coverageData <- coverageData @ [newLine]

    member this.AddBranchCoverageData(line:int, path:int, offset:int, hits:int) = 
        let covpoint = coverageData |> Seq.tryFind (fun elem -> elem.Line = line)
        match covpoint with
        | Some data ->
            data.SetBranchToCover(path, offset, hits)
        | _ ->
            let newLine = new Line(line)
            newLine.SetBranchToCover(path, offset, hits)
            coverageData <- coverageData @ [newLine]

let mutable cacheData : Map<string, Coverage> =  Map.empty 
let mutable idResolver : Map<string, string> = Map.empty
let mutable testFiles : Set<string>  = Set.empty
let mutable trackedMethodsData : Map<string, MethodRef>  = Map.empty
let mutable trackedReferenceData : Map<string, MethodTrackedRef>  = Map.empty

let GetMethodToTrack(method:OpenCoverXmlHelpers.OpenCoverXml.Method, fileXmlId:string) = 

    if method.FileRef.IsSome then
        let keyMethod = sprintf "%i_%s" method.FileRef.Value.Uid method.Name
        if not(trackedReferenceData.ContainsKey(method.Name)) then
            let methodData = MethodTrackedRef()
            methodData.FileRef <- sprintf "%s-%i" fileXmlId method.FileRef.Value.Uid
            methodData.MethodName <- method.Name
            trackedReferenceData <- trackedReferenceData.Add(keyMethod, methodData)

        trackedReferenceData.[keyMethod]
    else
        null

let ParseSeqTrackedMethodRefs(seqPoint:OpenCoverXmlHelpers.OpenCoverXml.SequencePoint, method:OpenCoverXmlHelpers.OpenCoverXml.Method, methodTrack:MethodTrackedRef) = 
    if seqPoint.TrackedMethodRefs.IsSome then
        seqPoint.TrackedMethodRefs.Value.TrackedMethodRefs
        |> Seq.iter (fun elem -> if not(methodTrack.TrackedTestMethodRefs.Contains(elem.Uid)) then methodTrack.TrackedTestMethodRefs.Add(elem.Uid) |> ignore)

let ParseBranchTrackedMethodRefs(branchPoint:OpenCoverXmlHelpers.OpenCoverXml.BranchPoint, method:OpenCoverXmlHelpers.OpenCoverXml.Method, methodTrack:MethodTrackedRef) = 
    if branchPoint.TrackedMethodRefs.IsSome then
        branchPoint.TrackedMethodRefs.Value.TrackedMethodRefs
        |> Seq.iter (fun elem -> if not(methodTrack.TrackedTestMethodRefs.Contains(elem.Uid)) then methodTrack.TrackedTestMethodRefs.Add(elem.Uid) |> ignore)

let AddSequencePoint(
    seqpointdata:OpenCoverXmlHelpers.OpenCoverXml.SequencePoint,
    method:OpenCoverXmlHelpers.OpenCoverXml.Method,
    ignoreUnTrackedCov:bool,
    methodTrack:MethodTrackedRef,
    xmlReportProvidingCoverage:string,
    lineAndFileToPrint:string) = 

    let fileId = sprintf "%s-%i" xmlReportProvidingCoverage seqpointdata.Fileid
    let hits = seqpointdata.Vc
    let line = seqpointdata.Sl
    if idResolver.ContainsKey(fileId) then
        let fileIdPath = idResolver.[fileId]
        let covpoint = cacheData.[fileIdPath]

        if lineAndFileToPrint <> ""  then
            let lineToPrint = sprintf "%s:%i" fileIdPath line
            if lineToPrint.Contains(lineAndFileToPrint) then
                printf "[TRACKING FILE AND LINE] %s provides %s wiht %i hits\r\n" xmlReportProvidingCoverage lineToPrint hits

        if ignoreUnTrackedCov && seqpointdata.TrackedMethodRefs.IsNone then
            covpoint.AddSequenceLineInfo(line, 0)
        else
            covpoint.AddSequenceLineInfo(line, hits)

    ParseSeqTrackedMethodRefs(seqpointdata, method, methodTrack)

let AddBranchPoint(
    branchpointdata:OpenCoverXmlHelpers.OpenCoverXml.BranchPoint,
    method:OpenCoverXmlHelpers.OpenCoverXml.Method,
    ignoreUnTrackedCov:bool,
    methodTrack:MethodTrackedRef,
    xmlReportProvidingCoverage:string,
    lineAndFileToPrint:string) =
    
    let hits = branchpointdata.Vc
    let line = branchpointdata.Sl
    let offset = branchpointdata.Offset
    let path = branchpointdata.Path
    let fileId = sprintf "%s-%i" xmlReportProvidingCoverage branchpointdata.Fileid
    if idResolver.ContainsKey(fileId) then
        let fileIdPath = idResolver.[fileId]
        let covpoint = cacheData.[fileIdPath]
        if ignoreUnTrackedCov && branchpointdata.TrackedMethodRefs.IsNone then
            covpoint.AddBranchCoverageData(line, path, offset, 0)
        else
            covpoint.AddBranchCoverageData(line, path, offset, hits)

    ParseBranchTrackedMethodRefs(branchpointdata, method, methodTrack)

let ParseMethod(
    methoddata:OpenCoverXmlHelpers.OpenCoverXml.Method,
    ignoreUnTrackedCov:bool,
    xmlReportProvidingCoverage:string,
    lineAndFileToPrint:string) =
    let methodTracked = GetMethodToTrack(methoddata, xmlReportProvidingCoverage)
    methoddata.BranchPoints
    |> Seq.iter (fun elem -> AddBranchPoint(elem, methoddata, ignoreUnTrackedCov, methodTracked, xmlReportProvidingCoverage, lineAndFileToPrint))

    methoddata.SequencePoints
    |> Seq.iter (fun elem -> AddSequencePoint(elem, methoddata, ignoreUnTrackedCov, methodTracked, xmlReportProvidingCoverage, lineAndFileToPrint))

let ParseTrackedMethod(trackedMethod:OpenCoverXmlHelpers.OpenCoverXml.TrackedMethod, xmlReportProvidingCoverage:string, lineAndFileToPrint:string)= 
    let key = sprintf "%i_%s" trackedMethod.Uid trackedMethod.Name
    if not(trackedMethodsData.ContainsKey(key)) then
        let methodData = MethodRef()
        methodData.Name <- trackedMethod.Name
        methodData.Uid <- trackedMethod.Uid
        trackedMethodsData <- trackedMethodsData.Add(key, methodData)
    
    if not(testFiles.Contains(key)) then
        testFiles <- testFiles.Add(key)

let ParseClass(classdata:OpenCoverXmlHelpers.OpenCoverXml.Class, ignoreUnTrackedCov:bool, xmlReportProvidingCoverage:string, lineAndFileToPrint:string)= 
    classdata.Methods
    |> Seq.iter (fun elem -> ParseMethod(elem, ignoreUnTrackedCov, xmlReportProvidingCoverage, lineAndFileToPrint))

let ParseFiles(filedata:OpenCoverXmlHelpers.OpenCoverXml.File, contertPath:string, endPath:string, xmlReportProvidingCoverage:string, lineAndFileToPrint:string) = 
    let lowerPath = filedata.FullPath.ToLower()
    if lowerPath.StartsWith(endPath.ToLower()) && not(lowerPath.Contains("objdrop")) then
        let fileId = sprintf "%s-%i" xmlReportProvidingCoverage filedata.Uid
        if not(idResolver.ContainsKey(fileId)) then
            if not(cacheData.ContainsKey(filedata.FullPath)) then
                let convertedPath =
                    if contertPath <> "" then
                        filedata.FullPath.Replace(contertPath, endPath)
                    else
                        filedata.FullPath

                let cov = Coverage(convertedPath)
                cacheData <- cacheData.Add(filedata.FullPath, cov)
            idResolver <- idResolver.Add(fileId, filedata.FullPath)

let ParseModule(moduledata:OpenCoverXmlHelpers.OpenCoverXml.Module, contertPath:string, endPath:string, ignoreUnTrackedCov:bool, xmlReportProvidingCoverage:string, lineAndFileToPrint:string)= 
    if moduledata.TrackedMethods.IsSome then
        moduledata.TrackedMethods.Value.TrackedMethods
        |> Seq.iter (fun elem -> ParseTrackedMethod(elem, xmlReportProvidingCoverage, lineAndFileToPrint))

    moduledata.Files 
    |> Seq.iter (fun elem -> ParseFiles(elem, contertPath, endPath, xmlReportProvidingCoverage, lineAndFileToPrint))

    moduledata.Classes
    |> Seq.iter (fun elem -> ParseClass(elem, ignoreUnTrackedCov, xmlReportProvidingCoverage, lineAndFileToPrint))


