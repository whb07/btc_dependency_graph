module SourceTypes
open System.Text.RegularExpressions
open System.IO


// type Directories = 
//     | ProjectRoot of string
//     | ChildDirectory of string

type ParentDir = | ProjectRoot | ChildDirectory of string

type ProjectDirectory = {
    Parent: ParentDir;
    ChildrenDirectories: List<ProjectDirectory>
}

// let getDirs rootPath =

    // let appendArr lst arr = 
    //     arr |> Array.toList |> List.append lst
    
    // let childDirs arr = arr |> Array.toList |> List.map ChildDirectory
    // let rec allDirs node =
    //     match node.ChildrenDirectories with 
    //     | [] -> node
    //     | _ -> 


    // let projectDir parent children = 
    //     {Parent=parent; ChildrenDirectories=children}
    
    // let rec allDirs node lst =

        // match node.ChildrenDirectories with
        // | x::xs -> let dirs = childDirs (Directory.GetDirectories x) in

        // match paths with
        // | x :: xs -> let dirs = childDirs (Directory.GetDirectories x) in 
        //                 let 
        //                 allDirs (appendArr xs dirs) (List.append lst [x;])
        // | [] -> lst
    
    // let rec 
    // let dirs = allDirs [root] []

    
    // List.foldBack 
    // let rootDir = {Parent=ProjectRoot; }




type SourceCode = 
    | Header of string
    | Source of string

let sourceCodeValue (Header n | Source n ) = n
type FileType = | Executable | Library

type SourceFile = {
    FileType: FileType;
    Name: SourceCode;
    FullPath: string;
}

// type FileName = FileName of string

// type ProjectFile = {
//     Name: FileName;
//     Files: List<SourceCode>;
//     FullPath: string
// }

// type Executable = {
//     Name: SourceCode;
//     DependsOn: List<ProjectFile>
// }

// type CompilationDirectory = {
//     Executables: List<Executable>;
//     Library: ProjectFile;
// }

// let compilationDirectory projectFile = 
//     let hasMainFunc src projectFile =
//         let lines = File.ReadAllText(projectFile.FullPath + "/" + src)
//         let regex = """int main\(.+\)"""
//         match lines with 
//         | _ when Regex.Match(lines, regex).Success -> true
//         | _ -> false
//     let isExe n =
//         match n with
//         | SourceFile name -> hasMainFunc name projectFile
//         | _ -> false
//     let executables = List.filter isExe projectFile.Files
//     let updatedFiles = List.filter (fun x -> not (List.contains x executables)) projectFile.Files
//     let updatedLib = { projectFile with Files=updatedFiles}
//     let updatedExecutables =  executables |> List.map (fun x -> {Name=x; DependsOn=[updatedLib]})
//     {Executables=updatedExecutables; Library=updatedLib}



// let getBaseName (path:string) = 
//     path.Split("/") |> Array.last




// let buildProjectFile path =
//     let files = 
//         Directory.GetFiles(path)
//         |> Array.map (getBaseName >> sourceCode)
//         |> Array.choose id
//         |> Array.toList
//     {Name=FileName(getBaseName path); Files=files; FullPath=path}

// let getAllSourceCode (start:string) =
//     let rec recurseLibraries paths libs =
//         match paths with 
//         | x :: xs -> recurseLibraries xs (List.concat [libs; [buildProjectFile x]])
//         | [] -> libs
    
//     let directories = Directory.GetDirectories start |> Array.toList
//     let x = [buildProjectFile start]
//     recurseLibraries directories x






