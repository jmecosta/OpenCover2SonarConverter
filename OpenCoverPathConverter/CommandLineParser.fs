module CommandLineParser

open System
open System.IO
open System.Xml
open System.Text.RegularExpressions

// parse command using regex
// if matched, return (command name, command value) as a tuple
let (|Command|_|) (s:string) =
    let r = new Regex(@"^(?:-{1,2}|\/)(?<command>\w+)[=:]*(?<value>.*)$",RegexOptions.IgnoreCase)
    let m = r.Match(s)
    if m.Success then 
        Some(m.Groups.["command"].Value.ToLower(), m.Groups.["value"].Value)
    else
        None

// take a sequence of argument values
// map them into a (name,value) tuple
// scan the tuple sequence and put command name into all subsequent tuples without name
// discard the initial ("","") tuple
// group tuples by name 
// convert the tuple sequence into a map of (name,value seq)
let parseArgs (args:string seq) =
    args 
    |> Seq.map (fun i -> 
                        match i with
                        | Command (n,v) -> (n,v) // command
                        | _ -> ("",i)            // data
                       )
    |> Seq.scan (fun (sn,_) (n,v) -> if n.Length>0 then (n,v) else (sn,v)) ("","")
    |> Seq.skip 1
    |> Seq.groupBy (fun (n,_) -> n)
    
    
    
    |> Seq.map (fun (n,s) -> (n, s |> Seq.map (fun (_,v) -> v) |> Seq.filter (fun i -> i.Length>0)))
    |> Map.ofSeq


let ShowHelp () =
        Console.WriteLine ("Usage: OpenCoverPathConverter [OPTIONS]")
        Console.WriteLine ("Converts build path in xml files into another path")
        Console.WriteLine ()
        Console.WriteLine ("Options:")
        Console.WriteLine ("    /W|/w:<base work dir for tia>")
        Console.WriteLine ("    /D|/d:<directory to look for files>")
        Console.WriteLine ("    /P|/p:<Pattern of files to search>")
        Console.WriteLine ("    /E|/e:<End Path to Use for path replacement in Report>")
        Console.WriteLine ("    /S|/s:<Search string for path replace in report>")
        Console.WriteLine ("    /R|/r:<covert reports : search string>")
        Console.WriteLine ("    /O|/o:<out file>")
        Console.WriteLine ("    /X|/x:<output xml>")
        Console.WriteLine ("    /I|/i Ignores coverage without tracked tests")
        Console.WriteLine ("    /G|/g:<Generate Test Impact Analisys : outFile>")

