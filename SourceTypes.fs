module SourceTypes
open System.Text.RegularExpressions
open System.IO

type SourceCode = 
    | HeaderFile of string
    | SourceFile of string

type Lib = Lib of string

type Library = {
    Name: Lib;
    Files: List<SourceCode>;
    FullPath: string
}

type Executable = {
    Name: SourceCode;
    DependsOn: List<Library>
}

type IncludeStatement = IncludeStatement of string

let sourceCode (line:string) =
    match line with
    | x when x.EndsWith(".h") -> Some(HeaderFile line)
    | x when x.EndsWith(".cpp") -> Some(SourceFile line)
    | x when x.EndsWith(".c") -> Some(SourceFile line)
    | _ -> None

let splitIncludeName (line:string) =
    let lst = line.Split("/") |> Array.toList
    match lst with
    | [_; xs] -> sourceCode xs
    |  _ -> None

let replaceString (oldValue:string) (newValue:string) (actualString:string) =
    actualString.Replace(oldValue, newValue)
 
let splitBrackets (IncludeStatement line) =
    let regex = """\<.*?\>"""
    let found = Regex.Match(line, regex).Value
    found |> replaceString "<" "" |> replaceString ">" ""

let getSourceCode (lst: list<IncludeStatement>) =
    List.map (splitBrackets >> splitIncludeName) lst

let getIncludeStatements (source:SourceCode) =
    let filename = 
        match source with
        | HeaderFile n | SourceFile n -> n
    let regex = """#include{1}.*<{1}.*>{1}"""
    let lines = File.ReadAllText(filename)
    Regex.Matches(lines, regex)
    |> Seq.map (fun x -> IncludeStatement x.Value)
    |> Seq.toList



let filterForExecutables library = 
    let hasMainFunc src library =
        let lines = File.ReadAllText(library.FullPath + "/" + src)
        let regex = """int main\(.+\)"""
        match lines with 
        | _ when Regex.Match(lines, regex).Success -> true
        | _ -> false
    let isExe n =
        match n with
        | SourceFile name -> hasMainFunc name library
        | _ -> false
    List.filter isExe library.Files
    |> List.map (fun x -> {Name=x; DependsOn=[library]})



let getBaseName (path:string) = 
    path.Split("/") |> Array.last

let buildLibrary path =
    let files = 
        Directory.GetFiles(path)
        |> Array.map (getBaseName >> sourceCode)
        |> Array.choose id
        |> Array.toList
    {Name=(Lib (getBaseName path)); Files=files; FullPath=path}

let getAllSourceCode (start:string) =
    let rec recurseLibraries paths libs =
        match paths with 
        | x :: xs -> recurseLibraries xs (List.concat [libs; [buildLibrary x]])
        | [] -> libs
    
    let directories = Directory.GetDirectories start |> Array.toList
    let x = [buildLibrary start]
    recurseLibraries directories x




