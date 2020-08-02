namespace FC


open SourceTypes
open System.Text.RegularExpressions
open System.IO

module DirectoryContents =
    // type IncludeStatement = IncludeStatement of string

    // let splitIncludeName (line:string) =
    //     let lst = line.Split("/") |> Array.toList
    //     match lst with
    //     | [_; xs] -> sourceCode xs
    //     |  _ -> None

    // let replaceString (oldValue:string) (newValue:string) (actualString:string) =
    //     actualString.Replace(oldValue, newValue)

    // let splitBrackets (IncludeStatement line) =
    //     let regex = """\<.*?\>"""
    //     let found = Regex.Match(line, regex).Value
    //     found |> replaceString "<" "" |> replaceString ">" ""

    // let getSourceCode (lst: list<IncludeStatement>) =
    //     List.map (splitBrackets >> splitIncludeName) lst

    // let getIncludeStatements (source:SourceCode) =
    //     let filename =
    //         match source with
    //         | HeaderFile n | SourceFile n -> n
    //     let regex = """#include{1}.*<{1}.*>{1}"""
    //     let lines = File.ReadAllText(filename)
    //     Regex.Matches(lines, regex)
    //     |> Seq.map (fun x -> IncludeStatement x.Value)
    //     |> Seq.toList
    let hasMainFunc lines =
        let regex = """int main\s*\(.+\)"""
        match lines with
        | _ when Regex.Match(lines, regex).Success -> true
        | _ -> false

    let getBaseName (path: string) = path.Split("/") |> Array.last

    let sourceCode (path: string) =
        let line = getBaseName path

        let source =
            match line with
            | x when x.EndsWith(".h") -> Some(Header line)
            | x when x.EndsWith(".cpp") -> Some(Source line)
            | x when x.EndsWith(".c") -> Some(Source line)
            | _ -> None

        match source with
        | Some (Source n) ->
            if hasMainFunc (File.ReadAllText(path)) then
                Some
                    { FileType = Executable
                      Name = Source n
                      FullPath = path }
            else
                Some
                    { FileType = Library
                      Name = Source n
                      FullPath = path }
        | Some (Header n) ->
            Some
                { FileType = Library
                  Name = Header n
                  FullPath = path }
        | _ -> None

    let files path =
        Directory.GetFiles(path)
        |> Array.map sourceCode
        |> Array.choose id
        |> Array.toList
