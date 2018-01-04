open System
open System.IO
open System.Threading
open TestImpactRunnerApi.Tia

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
        let fail = arguments.ContainsKey("b")
        let tiaMap = new TiaMap()
        files |> Seq.iter (fun c-> 
                                try
                                    printf "Convert %s\r\n" c 
                                    OpenCoverConverter.ProcessFile(c, searchString, fail, endPath, ignoreCoverageWithoutTracked, tiaMap)
                                    printf "Free memory\r\n"
                                    GC.Collect()
                                    Thread.Sleep(200)
                                with
                                | ex -> printf "Failed to convert %s %s" c ex.Message
                                )

        if File.Exists(endFile) then
            File.Delete(endFile)

        let fileToPublish = 
            if arguments.ContainsKey("x") then
                OpenCoverConverter.CreateMergeCoverageFile(endFile)
            else
                OpenCoverConverter.CreateMergeCoverageFileJson(endFile)

        printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" fileToPublish

        if arguments.ContainsKey("g") then
            let working = arguments.["w"] |> Seq.head
            let endPath = arguments.["g"] |> Seq.head
            let fileToPublish = TestImpactAnalysisData.GenerateTestAnalysisImpactFile(endPath, CoveragePointData.testFiles, working, tiaMap)
            printf "##teamcity[publishArtifacts '%s => Coverage.zip']\r\n" fileToPublish
    0 // return an integer exit code
