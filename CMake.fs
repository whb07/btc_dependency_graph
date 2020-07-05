module CMake
open SourceTypes
open System.IO
open System

type AddContents = {
    Contents: List<string>
}

type CMakeCommand = 
    | AddLibrary of AddContents
    | AddExecutable of AddContents


let upper (s: string) =
    s |> Seq.mapi (fun i c -> match i with | 0 -> (Char.ToUpper(c)) | _ -> c)  |> String.Concat

let commandLine file =
    match file with
    | SourceFile n | HeaderFile n -> sprintf "    %s" n

let cmakeCommand (cmd:string) (name:string) (body:list<string>) =
    // let command = sprintf "%s (%s" (cmd; upper name)
    let command = cmd + sprintf "(%s" (upper name)
    let lst = [command; ]
    let last = List.last body
    let contents = List.concat [ lst; List.filter (fun x-> x <> last) body; [last + ")"]]
                   |> List.filter (fun s -> s <> "")
    match cmd with
    | "add_executable" -> AddExecutable {Contents=contents}
    | _ -> AddLibrary {Contents=contents}


let addLibrary (compilation:CompilationDirectory) =
    let libraryString (libname:string) (sourcefiles:List<SourceCode>) =
        let body = List.map commandLine sourcefiles
        cmakeCommand "add_library" (upper libname) body
    let (Lib name) = compilation.Library.Name
    let libName = if name = "src" then getBaseName(Directory.GetParent(compilation.Library.FullPath).ToString()) else name
    libraryString libName compilation.Library.Files


let addExecutable (executable:Executable) =
    let name =
        match executable.Name with
        | HeaderFile n | SourceFile n -> n
    let exeName = name.Split(".") |> Array.head |> upper
    let body = List.map commandLine [executable.Name]
    cmakeCommand "add_executable" exeName body

let listAddExecutables (compilation:CompilationDirectory) =
    List.map addExecutable compilation.Executables


    // let libraryString (libname:string) (sourcefiles:List<SourceCode>) =
    //     let body = List.map commandLine sourcefiles
    //     cmakeCommand "add_library" (upper libname) body
    // let (Lib name) = compilation.Library.Name
    // let libName = if name = "src" then getBaseName(Directory.GetParent(compilation.Library.FullPath).ToString()) else name
    // libraryString libName compilation.Library.Files