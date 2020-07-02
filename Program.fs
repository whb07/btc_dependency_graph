open System.IO
open System.Text.RegularExpressions
open SourceTypes


let filterLib header =
    header.ToString().Contains("/")

let splitLibName header =
    let sheader = header.ToString()
    let head = sheader.Substring(sheader.IndexOf("<") + 1).Split("/") |> Array.toList
    match head with
    | [x; xs] -> x
    |  _ -> ""

let getHeaders filename =
    let regex = """#include{1}.*<{1}.*>{1}"""
    let lines = File.ReadAllText(filename)
    Regex.Matches(lines, regex)
    |> Seq.map (fun x -> x.Value)


[<EntryPoint>]
let main argv =
    let x = 10
    printfn "Hello World from F#! %d" x
    0 // return an integer exit code
