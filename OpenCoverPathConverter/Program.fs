open System
open System.IO
open System.Threading

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

        let outFile = arguments.["o"] |> Seq.head
        let currentPath = Environment.CurrentDirectory
        let endFile = Path.Combine(currentPath, outFile)

        let endPath = arguments.["e"] |> Seq.head
        let searchString = arguments.["s"] |> Seq.head
        let fail = arguments.ContainsKey("b")
        files |> Seq.iter (fun c-> 
                                printf "Convert %s\r\n" c 
                                OpenCoverConverter.ProcessFile(c, searchString, fail, endPath)
                                printf "Free memory\r\n"
                                GC.Collect()
                                Thread.Sleep(1000))

        if File.Exists(endFile) then
            File.Delete(endFile)

        if arguments.ContainsKey("x") then
            OpenCoverConverter.CreateMergeCoverageFile(endFile)
        else
            OpenCoverConverter.CreateMergeCoverageFileJson(endFile)
    0 // return an integer exit code
