module OpenCoverConverter

open System
open System.IO
open System.Text
open System.Xml
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open FSharp.Data

type OpenCoverXml = XmlProvider<"""
<CoverageSession>
  <Summary numSequencePoints="377871" visitedSequencePoints="28379" numBranchPoints="181526" visitedBranchPoints="13353" sequenceCoverage="7.51" branchCoverage="7.36" maxCyclomaticComplexity="698" minCyclomaticComplexity="1" visitedClasses="1285" numClasses="6446" visitedMethods="7447" numMethods="50288" />
  <Modules>
    <Module skippedDueTo="Filter" hash="1A-0D-72-44-61-32-2D-54-98-83-81-C0-BD-43-07-DA-2F-35-5E-35">
      <FullName>C:\Windows\Microsoft.Net\assembly\GAC_MSIL\System.Xml\v4.0_4.0.0.0__b77a5c561934e089\System.Xml.dll</FullName>
      <ModuleName>System.Xml</ModuleName>
      <Files>
        <File uid="1" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\MessageHub.cs" />
        <File uid="5" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ScreenHandlerMessageHub.cs" />
        <File uid="6" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\UndoRedoMessageHub.cs" />
        <File uid="9" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\DisableEnableActionMessageHub.cs" />
        <File uid="10" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\HelpRequestedMessage.cs" />
        <File uid="11" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\MainLayout.xaml.cs" />
        <File uid="12" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\ObjDrop\Work\Release\x64\ProdWPF\Main\Main\MainLayout.xaml" />
        <File uid="13" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ModelingLayout.xaml.cs" />
        <File uid="14" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\ObjDrop\Work\Release\x64\ProdWPF\Main\Main\ModelingLayout.xaml" />
        <File uid="16" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ModelingUnitsHandler.cs" />
        <File uid="17" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ProdApplication.cs" />
        <File uid="19" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ProdMainApp.cs" />
        <File uid="37" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ValueFormatterProvider.cs" />
      </Files>
      <Classes>
        <Class>
          <Summary numSequencePoints="0" visitedSequencePoints="0" numBranchPoints="0" visitedBranchPoints="0" sequenceCoverage="0" branchCoverage="0" maxCyclomaticComplexity="0" minCyclomaticComplexity="0" visitedClasses="0" numClasses="0" visitedMethods="0" numMethods="0" />
          <FullName>&lt;Module&gt;</FullName>
          <Methods />
        </Class>
       </Classes>
    </Module>
    <Module skippedDueTo="Filter" hash="1A-0D-72-44-61-32-2D-54-98-83-81-C0-BD-43-07-DA-2F-35-5E-35">
      <FullName>C:\Windows\Microsoft.Net\assembly\GAC_MSIL\System.Xml\v4.0_4.0.0.0__b77a5c561934e089\System.Xml.dll</FullName>
      <ModuleName>System.Xml</ModuleName>
      <Files>
        <File uid="1" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\MessageHub.cs" />
        <File uid="5" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ScreenHandlerMessageHub.cs" />
        <File uid="6" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\UndoRedoMessageHub.cs" />
        <File uid="9" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\DisableEnableActionMessageHub.cs" />
        <File uid="10" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\HelpRequestedMessage.cs" />
        <File uid="11" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\MainLayout.xaml.cs" />
        <File uid="12" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\ObjDrop\Work\Release\x64\ProdWPF\Main\Main\MainLayout.xaml" />
        <File uid="13" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ModelingLayout.xaml.cs" />
        <File uid="14" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\ObjDrop\Work\Release\x64\ProdWPF\Main\Main\ModelingLayout.xaml" />
        <File uid="16" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ModelingUnitsHandler.cs" />
        <File uid="17" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ProdApplication.cs" />
        <File uid="19" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ProdMainApp.cs" />
        <File uid="37" fullPath="E:\BuildAgent\work\ae1633cf320ebaec\Core\ProdWPF\ProdWPF\Main\ValueFormatterProvider.cs" />
      </Files>
      <Classes>
        <Class>
          <Summary numSequencePoints="0" visitedSequencePoints="0" numBranchPoints="0" visitedBranchPoints="0" sequenceCoverage="0" branchCoverage="0" maxCyclomaticComplexity="0" minCyclomaticComplexity="0" visitedClasses="0" numClasses="0" visitedMethods="0" numMethods="0" />
          <FullName>&lt;Module&gt;</FullName>
          <Methods />
        </Class>
       </Classes>
    </Module>
    <Module skippedDueTo="Filter" hash="1A-0D-72-44-61-32-2D-54-98-83-81-C0-BD-43-07-DA-2F-35-5E-35">
      <FullName>C:\Windows\Microsoft.Net\assembly\GAC_MSIL\System.Xml\v4.0_4.0.0.0__b77a5c561934e089\System.Xml.dll</FullName>
      <ModuleName>System.Xml</ModuleName>
      <Files />
      <Classes>
        <Class>
          <Summary numSequencePoints="0" visitedSequencePoints="0" numBranchPoints="0" visitedBranchPoints="0" sequenceCoverage="0" branchCoverage="0" maxCyclomaticComplexity="0" minCyclomaticComplexity="0" visitedClasses="0" numClasses="0" visitedMethods="0" numMethods="0" />
          <FullName>&lt;Module&gt;</FullName>

          <Methods />
        </Class>
        <Class>
          <Summary numSequencePoints="0" visitedSequencePoints="0" numBranchPoints="0" visitedBranchPoints="0" sequenceCoverage="0" branchCoverage="0" maxCyclomaticComplexity="0" minCyclomaticComplexity="0" visitedClasses="0" numClasses="0" visitedMethods="0" numMethods="0" />
          <FullName>ProdWPF.MessageHub/FrameTitleChangedDelegate</FullName>
          <Methods>
            <Method skippedDueTo="Unknown" visited="false" cyclomaticComplexity="0" nPathComplexity="0" sequenceCoverage="0" branchCoverage="0" isConstructor="true" isStatic="false" isGetter="false" isSetter="false">
              <MetadataToken>100663535</MetadataToken>
              <Name>System.Void ProdWPF.MessageHub/FrameTitleChangedDelegate::.ctor(System.Object,System.IntPtr)</Name>
              <BranchPoints />
              <SequencePoints />
            </Method>
            <Method skippedDueTo="Unknown" visited="false" cyclomaticComplexity="0" nPathComplexity="0" sequenceCoverage="0" branchCoverage="0" isConstructor="false" isStatic="false" isGetter="false" isSetter="false">
              <MetadataToken>100663536</MetadataToken>
              <Name>System.Void ProdWPF.MessageHub/FrameTitleChangedDelegate::Invoke(System.String,System.String)</Name>
              <BranchPoints />
              <SequencePoints />
            </Method>
            <Method skippedDueTo="Unknown" visited="false" cyclomaticComplexity="0" nPathComplexity="0" sequenceCoverage="0" branchCoverage="0" isConstructor="false" isStatic="false" isGetter="false" isSetter="false">
              <MetadataToken>100663537</MetadataToken>
              <Name>System.IAsyncResult ProdWPF.MessageHub/FrameTitleChangedDelegate::BeginInvoke(System.String,System.String,System.AsyncCallback,System.Object)</Name>
                              <BranchPoints />
                              <SequencePoints />
            </Method>
            <Method skippedDueTo="Unknown" visited="false" cyclomaticComplexity="0" nPathComplexity="0" sequenceCoverage="0" branchCoverage="0" isConstructor="false" isStatic="false" isGetter="false" isSetter="false">
              <MetadataToken>100663538</MetadataToken>
              <Name>System.Void ProdWPF.MessageHub/FrameTitleChangedDelegate::EndInvoke(System.IAsyncResult)</Name>
              <SequencePoints>
                <SequencePoint vc="14" uspid="43" ordinal="0" offset="0" sl="242" sc="13" el="243" ec="59" bec="0" bev="0" fileid="1" />
                <SequencePoint vc="14" uspid="44" ordinal="1" offset="17" sl="244" sc="9" el="244" ec="10" bec="0" bev="0" fileid="1" />
              </SequencePoints>
                <BranchPoints>
                <BranchPoint vc="0" uspid="87" ordinal="0" offset="18" sl="264" path="0" offsetend="20" fileid="1" />
                <BranchPoint vc="0" uspid="88" ordinal="1" offset="18" sl="264" path="1" offsetend="26" fileid="1" />
                <BranchPoint vc="0" uspid="89" ordinal="2" offset="28" sl="268" path="3" offsetend="33" fileid="1" />
                <BranchPoint vc="0" uspid="90" ordinal="3" offset="28" sl="268" path="1" offsetend="360" fileid="1" />
                <BranchPoint vc="0" uspid="91" ordinal="4" offset="59" sl="271" path="0" offsetend="64" fileid="1" />
                <BranchPoint vc="0" uspid="92" ordinal="5" offset="59" sl="271" path="1" offsetend="250" fileid="1" />
                <BranchPoint vc="0" uspid="93" ordinal="6" offset="111" sl="271" path="0" offsetend="113" fileid="1" />
                <BranchPoint vc="0" uspid="94" ordinal="7" offset="111" sl="271" path="1" offsetend="116" fileid="1" />
                <BranchPoint vc="0" uspid="95" ordinal="8" offset="130" sl="271" path="0" offsetend="132" fileid="1" />
                <BranchPoint vc="0" uspid="96" ordinal="9" offset="130" sl="271" path="1" offsetend="250" fileid="1" />
                <BranchPoint vc="0" uspid="101" ordinal="14" offset="439" sl="295" path="0" offsetend="441" fileid="1" />
                <BranchPoint vc="0" uspid="102" ordinal="15" offset="439" sl="295" path="1" offsetend="447" fileid="1" />
                <BranchPoint vc="0" uspid="103" ordinal="16" offset="462" sl="297" path="0" offsetend="464" fileid="1" />
                <BranchPoint vc="0" uspid="104" ordinal="17" offset="462" sl="297" path="1" offsetend="469" fileid="1" />
                <BranchPoint vc="0" uspid="105" ordinal="18" offset="492" sl="305" path="0" offsetend="494" fileid="1" />
                <BranchPoint vc="0" uspid="106" ordinal="19" offset="492" sl="305" path="1" offsetend="500" fileid="1" />
              </BranchPoints>
            </Method>
          </Methods>
        </Class>
       </Classes>
    </Module>
  </Modules>
</CoverageSession>
""">

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
    member val Path : String = path
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
let mutable idResolver : Map<int, string> = Map.empty

let AddSequencePoint(branchpointdata:OpenCoverXml.SequencePoint) = 
    let fileId = branchpointdata.Fileid
    let hits = branchpointdata.Vc
    let line = branchpointdata.Sl
    let fileIdPath = idResolver.[fileId]
    let covpoint = cacheData.[fileIdPath]
    covpoint.AddSequenceLineInfo(line, hits)
    ()

let AddBranchPoint(branchpointdata:OpenCoverXml.BranchPoint) =
    let fileId = branchpointdata.Fileid
    let hits = branchpointdata.Vc
    let line = branchpointdata.Sl
    let offset = branchpointdata.Offset
    let path = branchpointdata.Path
    let fileIdPath = idResolver.[fileId]
    let covpoint = cacheData.[fileIdPath]
    covpoint.AddBranchCoverageData(line, path, offset, hits)

let ParseMethod(methoddata:OpenCoverXml.Method) =
    methoddata.BranchPoints
    |> Seq.iter (fun elem -> AddBranchPoint(elem))

    methoddata.SequencePoints
    |> Seq.iter (fun elem -> AddSequencePoint(elem))

let ParseClass(classdata:OpenCoverXml.Class)= 
    classdata.Methods
    |> Seq.iter (fun elem -> ParseMethod(elem))

let ParseFiles(filedata:OpenCoverXml.File, contertPath:string, endPath:string)= 
    if not(idResolver.ContainsKey(filedata.Uid)) then
        if not(cacheData.ContainsKey(filedata.FullPath)) then
            let cov = new Coverage(filedata.FullPath.Replace(contertPath, endPath))
            cacheData <- cacheData.Add(filedata.FullPath, cov)
        idResolver <- idResolver.Add(filedata.Uid, filedata.FullPath)

let ParseModule(moduledata:OpenCoverXml.Module, contertPath:string, endPath:string)= 
    moduledata.Files 
    |> Seq.iter (fun elem -> ParseFiles(elem, contertPath, endPath))

    moduledata.Classes
    |> Seq.iter (fun elem -> ParseClass(elem))

let CollectMergeData(data:OpenCoverXml.CoverageSession, contertPath:string, endPath:string) =
    data.Modules
    |> Seq.iter (fun elem -> ParseModule(elem, contertPath, endPath))


let ProcessFile(file:string, searchString:string, fail : bool, endPath:string) = 
    let content = File.ReadAllText(file)
    GC.Collect()
    let xmldata = OpenCoverXml.Parse(content)
    GC.Collect()

    let searchString = "\\" + searchString + "\\"

    let ValidateFullPath(path:string) = 
        path.Contains(searchString)

    let ValidateModule(modul:OpenCoverXml.Module) = 
        (modul.Files |> Seq.tryFind (fun file -> ValidateFullPath(file.FullPath))).IsSome

    let mutable convertPath = ""
    let moduledata = xmldata.Modules |> Seq.tryFind (fun modul -> ValidateModule(modul))
    match moduledata with
    | Some data -> 
        let fileName = (data.Files |> Seq.find (fun file -> ValidateFullPath(file.FullPath)))
        convertPath <- fileName.FullPath.Split([|searchString|], StringSplitOptions.RemoveEmptyEntries).[0]
    | _ -> ()

    idResolver <- Map.empty
    CollectMergeData(xmldata, convertPath, endPath)


let CreateMergeCoverageFileJson(file:string) = 
    let stringBuilder = new StringBuilder()
    stringBuilder.AppendLine("[") |> ignore
    let PrintSeqPoint(cov:Line) = 
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
            let ProceedPath(path:Path) = 
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


    let PrintCovPoint(cov:Coverage) = 
        stringBuilder.AppendLine("  {") |> ignore
        let path = sprintf "    \"file\": \"%s\"," (cov.Path.Replace("\\", "/"))
        stringBuilder.AppendLine(path) |> ignore
        stringBuilder.AppendLine("    \"lines\": [") |> ignore
        cov.GetCoverageData() |> Seq.iter (fun point -> PrintSeqPoint(point))
        stringBuilder.AppendLine("     ]") |> ignore
        stringBuilder.AppendLine("  },") |> ignore

    cacheData
    |> Map.toSeq
    |> Seq.iter (fun (uid,cov) -> PrintCovPoint(cov))
    stringBuilder.AppendLine("]") |> ignore

    File.WriteAllText(file, stringBuilder.ToString())
    let fileToPublish = Path.GetFileName(file)
    printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" fileToPublish

let CreateMergeCoverageFile(file:string) = 
    use streamWriter = new StreamWriter(file, false)
    streamWriter.WriteLine("<coverage version=\"1\">")
    let PrintSeqPoint(cov:Line) = 
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


    let PrintCovPoint(cov:Coverage) = 
        let path = sprintf "<file path=\"%s\">" cov.Path
        streamWriter.WriteLine(path)
        cov.GetCoverageData() |> Seq.iter (fun point -> PrintSeqPoint(point))
        streamWriter.WriteLine("</file>")

    cacheData
    |> Map.toSeq
    |> Seq.iter (fun (uid,cov) -> PrintCovPoint(cov))

    streamWriter.WriteLine("</coverage>")
    let fileToPublish = Path.GetFileName(file)
    printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" fileToPublish
