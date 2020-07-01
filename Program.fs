// Learn more about F# at http://fsharp.org

open System.IO
open Microsoft.FSharp.Core
// File.ReadAllLines

[<EntryPoint>]
let main argv =
    let x = 10
    printfn "Hello World from F#! %d" x
    0 // return an integer exit code
