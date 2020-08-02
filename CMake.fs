namespace CMake

open SourceTypes
open System.IO
open System
open FC

// type AddContents = {
//     Contents: List<string>
// }

module CMakeCommands =

    let upper (s: string) =
        s |> Seq.mapi (fun i c -> match i with | 0 -> (Char.ToUpper(c)) | _ -> c)  |> String.Concat
    
    let cmakeMinReq (n: float) =
        let x = n.ToString()
        sprintf "cmake_minimum_required(VERSION %s FATAL_ERROR)" x

    // type Command =
    //     | AddLibrary of List<SourceFile>
    //     | AddExecutable of SourceFile

    // let cmakeCommand (cmd:string) (name:string) (body:list<string>) =
    //     let command = cmd + sprintf "(%s" name
    //     let lst = [command; ]
    //     let last = List.last body
    //     let contents = List.concat [ lst; List.filter (fun x-> x <> last) body; [last + ")"]]
    //                    |> List.filter (fun s -> s <> "")
    //     match cmd with
    //     | "add_executable" -> AddExecutable {Contents=contents}
    //     | _ -> AddLibrary {Contents=contents}
     
    // let addExecutable (executable:Executable) =
    //     let name =
    //         match executable.Name with
    //         | Header n | Source n -> n
    //     let exeName = name.Split(".") |> Array.head |> sprintf "%s_exe"
    //     let body = List.map commandLine [executable.Name]
    //     cmakeCommand "add_executable" exeName body
    
    let addExecutables files = 
        let executables = files |> List.filter (fun f -> f.FileType = Executable)
        // let exe = List.tryHead executables
        let exeName (name:string) = name.Split(".") |> Array.head |> upper |> sprintf "%s_exe"
        // let name = 
        let command name = sprintf "add_executable(%s %s)" (exeName name) name
        executables |> List.map (fun n -> command (sourceCodeValue n.Name))
        // match exe with 
        // | Some(n) -> Some ()
        // | _ -> None

    let addLibrary files = 
        let libFiles = files |> List.filter (fun f -> f.FileType = Library)
        let first = List.tryHead libFiles
        let libName (path:string) = Path.GetDirectoryName path |> DirectoryContents.getBaseName |> upper
        // let name = 
        let command path name = sprintf "add_library(%s %s" (libName path) name
        let rec addOtherFiles lst cmd =
            match lst with
            | x::xs -> addOtherFiles xs (cmd + " " + (sourceCodeValue x.Name))
            | [] -> cmd + ")"
        
        match first with 
        | Some(n) -> Some (addOtherFiles (List.filter (fun x -> x <> n) libFiles) (command n.FullPath (sourceCodeValue n.Name)))
        | _ -> None

    


// let commandLine file =
//     match file with
//     | Source n | Header n -> sprintf "    %s" n




// let addLibrary (compilation:CompilationDirectory) =
//     let libraryString (libname:string) (Sources:List<SourceCode>) =
//         let body = List.map commandLine Sources
//         cmakeCommand "add_library" (upper libname) body
//     let (FileName name) = compilation.Library.Name
//     let libName = if name = "src" then getBaseName(Directory.GetParent(compilation.Library.FullPath).ToString()) else name
//     let appendLibStr = sprintf "%sLib" libName
//     libraryString appendLibStr compilation.Library.Files




// let listAddExecutables (compilation:CompilationDirectory) =
//     List.map addExecutable compilation.Executables


// let commandListToWrite command =
//     match command with
//     | AddLibrary value | AddExecutable value -> value.Contents

// let writeFile path lines =
//     File.WriteAllLines(path, lines)

// let writeCMake (path:string)  =
//    let x = List.collect (compilationDirectory >> listAddExecutables) (getAllSourceCode path)
//    x
//    let compilationDirs = List.map compilationDirectory (getAllSourceCode path) |> List.map listAddExecutables
//    compilationDirs
//    let execs = List.map listAddExecutables compilationDirs
//    List.collect execs
//    let compilationDir = compilationDirectory ( List. head ())
//    let execs = List.collect commandListToWrite (listAddExecutables compilationDir)
//    let lib = commandListToWrite ( addLibrary compilationDir )
//    let contents = List.concat([ [(cmakeMinReq  3.0);]; lib; execs]) |> List.toArray
//    writeFile output contents
