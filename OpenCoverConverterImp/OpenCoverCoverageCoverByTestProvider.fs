module OpenCoverConverterImpHelpers

open TestImpactRunnerApi
open System.IO
open System
open System.Diagnostics
open TestImpactRunnerApi.Json
open System.Threading

let WaitUntilOpenCoverCompletes() =    
    while Process.GetProcessesByName("OpenCover.Console").Length > 0 do
        Thread.Sleep(1000)

let GenerateImpactMapForTest(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string, reportFile:string) = 
    WaitUntilOpenCoverCompletes()
    let tiaMap = TiaMapData(rootPath, exclusions)
    let exclusionsData = (List.ofSeq exclusions)

    let content = File.ReadAllText(reportFile)
    GC.Collect()
    let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
    GC.Collect()

    let mutable methods = Map.empty
    let mutable indexData = Map.empty

    let ParseFiles(filedata:OpenCoverXmlHelpers.OpenCoverXml.File) = 
        if not(indexData.ContainsKey(filedata.Uid)) then
            indexData <- indexData.Add(filedata.Uid, filedata.FullPath)

    let AddVisitedMethodToList(method:OpenCoverXmlHelpers.OpenCoverXml.Method) =
        if method.FileRef.IsSome && method.Visited then
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
        let file = indexData.[keyForFile |> int]
        let fileName = tiaMap.ConvertFilePathToTiaMap(file)

        if not(tiaMap.IsExcluded(file)) then
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
            | _ ->  relevatMethodDep.Tests.Add(TiaTestData(test.TestName, "opencover"))

    tiaMap

let GenerateImpactMapForAllTrackedTestMethods(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string, reportFile:string) = 
    WaitUntilOpenCoverCompletes()
    let tiaMap = TiaMapData(rootPath, exclusions)
    let exclusionsData = (List.ofSeq exclusions)

    let content = File.ReadAllText(reportFile)
    GC.Collect()
    let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
    GC.Collect()

    let mutable methods = Map.empty
    let mutable indexData = Map.empty
    let mutable trackedMethodData = Map.empty

    let ParseFiles(filedata:OpenCoverXmlHelpers.OpenCoverXml.File) = 
        if not(indexData.ContainsKey(filedata.Uid)) then
            indexData <- indexData.Add(filedata.Uid, filedata.FullPath)

    let AddVisitedMethodToList(method:OpenCoverXmlHelpers.OpenCoverXml.Method) =
        if method.FileRef.IsSome && method.Visited then
            let key = sprintf "%i:%s" method.FileRef.Value.Uid method.Name
            if not(methods.ContainsKey(key)) then
                if method.MethodPoint.IsSome && method.MethodPoint.Value.TrackedMethodRefs.IsSome then
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
        let file = indexData.[keyForFile |> int]
        let fileName = tiaMap.ConvertFilePathToTiaMap(file)

        if not(tiaMap.IsExcluded(file)) then
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
            | _ ->  relevatMethodDep.Tests.Add(TiaTestData(test.TestName, "opencover"))

    tiaMap


let GenerateImpactMapForTrackedMethodsByTest(test: ImpactedTest, exclusions:System.Collections.Generic.List<string>, rootPath:string, reportFile:string) = 
    WaitUntilOpenCoverCompletes()
    let tiaMap = TiaMapData(rootPath, exclusions)
    let exclusionsData = (List.ofSeq exclusions)

    let content = File.ReadAllText(reportFile)
    GC.Collect()
    let xmldata = OpenCoverXmlHelpers.OpenCoverXml.Parse(content)
    GC.Collect()

    let mutable coveredMethods = Map.empty
    let mutable trackedMethodRefs = Map.empty
    let mutable indexData = Map.empty

    let CollectTrackedMethod(filedata:OpenCoverXmlHelpers.OpenCoverXml.TrackedMethod) = 
        if not(trackedMethodRefs.ContainsKey(filedata.Uid)) then
            if filedata.Name.Equals(test.TestMethod) then
                trackedMethodRefs <- trackedMethodRefs.Add(filedata.Uid, filedata.Name)

    let ParseFiles(filedata:OpenCoverXmlHelpers.OpenCoverXml.File) = 
        if not(indexData.ContainsKey(filedata.Uid)) then
            indexData <- indexData.Add(filedata.Uid, filedata.FullPath)

    let AddVisitedMethodToList(method:OpenCoverXmlHelpers.OpenCoverXml.Method) =
        if method.FileRef.IsSome && method.Visited then
            let key = sprintf "%i:%s" method.FileRef.Value.Uid method.Name
            if not(coveredMethods.ContainsKey(key)) then
                if method.MethodPoint.IsSome && method.MethodPoint.Value.TrackedMethodRefs.IsSome then
                    coveredMethods <- coveredMethods.Add(key, (method.Name, method.MethodPoint.Value.TrackedMethodRefs))

    let ParseClass(classdata:OpenCoverXmlHelpers.OpenCoverXml.Class) = 
        classdata.Methods |> Seq.iter (fun method -> if method.Visited then AddVisitedMethodToList(method))

    let ParseModule(moduledata:OpenCoverXmlHelpers.OpenCoverXml.Module) =  
        if moduledata.TrackedMethods.IsSome then
            moduledata.TrackedMethods.Value.TrackedMethods |> Seq.iter (fun trackedMethod -> CollectTrackedMethod(trackedMethod))
        moduledata.Files |> Seq.iter (fun file -> ParseFiles(file))
        moduledata.Classes |> Seq.iter (fun classdata -> ParseClass(classdata))

    xmldata.Modules 
    |> Seq.iter (fun modulo -> if modulo.SkippedDueTo.IsNone then ParseModule(modulo))

    if not(trackedMethodRefs.IsEmpty) then
        for entry in coveredMethods do
            let keyForFile = entry.Key.Substring(0, entry.Key.IndexOf(':'))
            let keyForMethod, trackedRefs = entry.Value
            let file = indexData.[keyForFile |> int]
            let fileName = tiaMap.ConvertFilePathToTiaMap(file)

            if not(tiaMap.IsExcluded(file)) then                
                let relevantFileDep, addToTiaMap = 
                    let depFile = tiaMap.Files |> Seq.tryFind (fun x -> x.FileName.Equals(fileName))
                    match depFile with
                    | Some devalue -> devalue, false
                    | _ ->  let impact = TiaFileData(fileName)
                            impact, true

                let relevatMethodDep, addTestToDep = 
                    let depMethodOption = relevantFileDep.Methods |> Seq.tryFind (fun x -> x.Name.Equals(keyForMethod))
                    match depMethodOption with
                    | Some depMethod -> depMethod, true
                    | _ ->  let impactMethod = TiaMethodData(keyForMethod)
                            if trackedRefs.IsSome && (trackedRefs.Value.TrackedMethodRefs |> Seq.tryFind (fun x -> trackedMethodRefs.ContainsKey(x.Uid))).IsSome then
                                relevantFileDep.Methods.Add(impactMethod)
                                impactMethod,true
                            else
                                impactMethod,false

                if addTestToDep then
                    if addToTiaMap then
                        tiaMap.Files.Add(relevantFileDep)                    

                    let depTestOption = relevatMethodDep.Tests |> Seq.tryFind (fun x -> x.Equals(test.TestName))
                    match depTestOption with
                    | Some _ -> ()
                    | _ ->  relevatMethodDep.Tests.Add(TiaTestData(test.TestName, "opencover"))

    tiaMap

    