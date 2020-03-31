open System.IO
open System
open System.Threading
open TestImpactRunnerApi
open TestImpactRunnerApi.Json
open System.Diagnostics

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
        let files = Directory.GetFiles(searchPath, pattern)

        let ignoreCoverageWithoutTracked = arguments.ContainsKey("i")

        let outFile = arguments.["o"] |> Seq.head
        let currentPath = Environment.CurrentDirectory
        let endFile = Path.Combine(currentPath, outFile)

        let endPath = arguments.["e"] |> Seq.head
        let searchString = arguments.["s"] |> Seq.head

        let working = arguments.["w"] |> Seq.head
        let tiamapFile = Path.Combine(working, "tia.json")
        let tiaMap = if arguments.ContainsKey("g") && File.Exists(tiamapFile) then JsonUtilities.ReadMap(working, tiamapFile) else TiaMapData()

        let drives = DriveInfo.GetDrives()
        let gbConv = 1024 * 1024 * 1024

        let stopwatch = Stopwatch()
        stopwatch.Start()
        
        files |> Seq.iter (fun file-> 
                                try
                                    printf "Convert %s\r\n" file 
                                    OpenCoverConverter.ProcessFile(file, searchString, endPath, ignoreCoverageWithoutTracked, tiaMap, List.empty, searchPath)
                                    printf "Free memory and Delete file from disk\r\n"
                                    GC.Collect()
                                    File.Delete(file)
                                    for drive in drives do
                                        if file.Contains(drive.Name) then
                                            let gbSpaceFree = drive.AvailableFreeSpace / int64(gbConv)
                                            printf "Available Space: %i GB\r\n" gbSpaceFree
                                with
                                | ex -> printf "Failed to convert %s %s\r\n" file ex.Message
                                )
        stopwatch.Stop()
        printf "Duration %i:%i:%i \r\n" stopwatch.Elapsed.Hours stopwatch.Elapsed.Minutes stopwatch.Elapsed.Seconds

        if File.Exists(endFile) then
            File.Delete(endFile)

        let fileToPublish = 
            if arguments.ContainsKey("x") then
                OpenCoverConverter.CreateMergeCoverageFile(endFile)
            else
                OpenCoverConverter.CreateMergeCoverageFileJson(endFile)

        printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" fileToPublish

        if arguments.ContainsKey("g") then 
            let endPathForTia =  arguments.["g"] |> Seq.head
            let handles = System.Collections.Generic.List<ICoverageHandle>()
            let coverageService = TiaCoverageService(tiaMap, working, handles, "json", endPathForTia)
            coverageService.EnableService <- true
            let tiaFileToPublish = coverageService.WriteTiaMapToFile()
            printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" tiaFileToPublish
    0 // return an integer exit code
