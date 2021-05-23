open System.IO
open System
open TestImpactRunnerApi
open TestImpactRunnerApi.Json
open System.Diagnostics
open System.IO.Compression
open Microsoft.FSharp.Collections
open FSharp.Collections.ParallelSeq

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let arguments = CommandLineParser.parseArgs(argv)

    if arguments.ContainsKey("h") then
        CommandLineParser.ShowHelp ()
    elif not(arguments.ContainsKey("d")) || not(arguments.ContainsKey("p")) then
        CommandLineParser.ShowHelp ()
        printf "Define both /d and /p"
    else
        let searchPath = arguments.["d"] |> Seq.head
        let pattern = arguments.["p"] |> Seq.head
        let files = Directory.GetFiles(searchPath, pattern, SearchOption.AllDirectories)

        let ignoreCoverageWithoutTracked = arguments.ContainsKey("i")
        let working = try arguments.["w"] |> Seq.head with | _ -> Environment.CurrentDirectory

        let outFile = if Path.IsPathRooted(arguments.["o"] |> Seq.head) then arguments.["o"] |> Seq.head else Path.Combine(working, arguments.["o"] |> Seq.head)
                
        let endPath = try arguments.["e"] |> Seq.head with | _ -> ""
        let searchString = try arguments.["s"] |> Seq.head with | _ -> ""

        
        let tiamapFile = Path.Combine(working, "tia.json")
        let tiaMap = if arguments.ContainsKey("g") && File.Exists(tiamapFile) then JsonUtilities.ReadMap(working, tiamapFile) else TiaMapData()

        let drives = DriveInfo.GetDrives()
        let gbConv = 1024 * 1024 * 1024

        let stopwatch = Stopwatch()
        stopwatch.Start()
        
        files |> Seq.iter (fun file-> 
                                try
                                    printf "Convert %s\r\n" file

                                    let ConverteFile(fileData:string) =
                                        OpenCoverConverter.ProcessFile(fileData, searchString, endPath, ignoreCoverageWithoutTracked, tiaMap, List.empty, searchPath)
                                        printf "Free memory and Delete file from disk\r\n"
                                        GC.Collect()
                                        File.Delete(fileData)
                                        for drive in drives do
                                            if file.Contains(drive.Name) then
                                                let gbSpaceFree = drive.AvailableFreeSpace / int64(gbConv)
                                                printf "Available Space: %i GB\r\n" gbSpaceFree

                                    if file.EndsWith(".zip") then
                                        let parentFolder = Path.GetDirectoryName(file)
                                        let ExtractFile(file:string) = 
                                            use archive = ZipFile.Open(file, ZipArchiveMode.Read)                                                
                                            archive.ExtractToDirectory(parentFolder)

                                        ExtractFile(file)
                                        File.Delete(file)
                                        for fileData in Directory.GetFiles(parentFolder, "*.xml") do
                                            ConverteFile(fileData)
                                    else
                                        ConverteFile(file)
                                with
                                | ex -> printf "Failed to convert %s %s %s\r\n" file ex.Message ex.StackTrace   
                                )
        stopwatch.Stop()
        printf "Duration %i:%i:%i \r\n" stopwatch.Elapsed.Hours stopwatch.Elapsed.Minutes stopwatch.Elapsed.Seconds

        if File.Exists(outFile) then
            File.Delete(outFile)

        let fileToPublish = 
            if arguments.ContainsKey("x") then
                OpenCoverConverter.CreateMergeCoverageFile(outFile)
            else
                OpenCoverConverter.CreateMergeCoverageFileJson(outFile)
        printf "Created: %s\r\n" fileToPublish
        printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" fileToPublish

        if arguments.ContainsKey("g") then 
            let endPathForTia =  arguments.["g"] |> Seq.head
            let handles = System.Collections.Generic.List<ICoverageHandle>()
            let coverageService = TiaCoverageService(tiaMap, working, handles, "json", endPathForTia)
            coverageService.EnableService <- true
            let tiaFileToPublish = coverageService.WriteTiaMapToFile()
            printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" tiaFileToPublish
    0 // return an integer exit code
