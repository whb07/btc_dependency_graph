module Tests

open Xunit
open System.IO
open System
open SourceTypes
open CMake
open FC

// let HelloWorldProject = "helloworld/src"

type HelloWorldProject = {
    RootDir: string;
    SrcDir: string;
}

let rand = Random()
let genRandomNumbers count = List.init count (fun _ -> rand.Next())
let randomNumString n = genRandomNumbers n |> List.map string |> List.reduce (+)


let getTestProject n = 
    let tmpPath = Path.GetTempPath() + "projects_" + randomNumString n + "/"
    let rootDir = tmpPath + "helloworld"
    let srcDir = rootDir + "/src"
    Directory.CreateDirectory(srcDir) |> ignore
    Assert.True(Directory.Exists(srcDir))
    {RootDir=rootDir; SrcDir=srcDir}

let cleanUpTestProject project =
    Directory.Delete(project.RootDir, true)
    Assert.False(Directory.Exists(project.RootDir))


let writeExecutable path filename =
    let code = ["""#include "header.h" """; "int main(void){"; "    return 0;"; "}"]
    File.WriteAllLines(path + "/" + filename, code)

let generateMainAndHeader path =
    writeExecutable path "main.c"
    let header = ["int one(void);";]
    File.WriteAllLines(path + "/header.h", header)
    let source = ["int one(void){return 1;};";]
    File.WriteAllLines(path + "/header.c", source)

    
let equalStringList (left:list<string>) (right:list<string>) = 
        Assert.Equal<Collections.Generic.IEnumerable<string>>(left, right)



[<Fact>]
let ``Test randomNumString generates new string every time`` () =
    let a = randomNumString 5
    let b = randomNumString 5
    Assert.True(a<>b)

[<Fact>]
let ``Test files returns a list of files for dir`` () =
    let project = getTestProject 2
    generateMainAndHeader project.SrcDir
    let lst = DirectoryContents.files project.SrcDir
    let libFiles = lst |> List.filter (fun f -> f.FileType = Library)
    let executables = lst |> List.filter (fun f -> f.FileType = Executable)
    let value (Source n | Header n) = n
    Assert.True(lst.Length = 3)
    Assert.True(libFiles.Length = 2)
    Assert.True(executables.Length = 1)
    Assert.Equal(value executables.Head.Name ,  "main.c")
    cleanUpTestProject project

[<Fact>]
let ``Test addExecutable returns a single command`` () =
    let project = getTestProject 1
    generateMainAndHeader project.SrcDir
    let lst = DirectoryContents.files project.SrcDir
    
    match CMakeCommands.addExecutables lst with
    | [x] -> Assert.Equal("add_executable(Main_exe main.c)", x)
    | _ -> Assert.False(true)

    Assert.Empty(CMakeCommands.addExecutables [])
    cleanUpTestProject project

[<Fact>]
let ``Test addLibrary returns a single command`` () =
    let project = getTestProject 4
    generateMainAndHeader project.SrcDir
    let lst = DirectoryContents.files project.SrcDir
    
    match CMakeCommands.addLibrary lst with
    | Some n -> Assert.Equal("add_library(Src header.h header.c)", n)
    | _ -> Assert.False(true)

    Assert.Null(CMakeCommands.addLibrary [])
    cleanUpTestProject project

[<Fact>]
let ``Test hasMainFunc classifies proper file contents`` () =
    let project = getTestProject 5
    generateMainAndHeader project.SrcDir
    let lines = File.ReadAllText(project.SrcDir + "/main.c")

    // let path = 
    let header = File.ReadAllText(project.SrcDir + "/header.c")
    let code = ["""#include "vanilla.h" """; "int main (int argc, char *argv[]){"; "    return 0;"; "}"]
    
    File.WriteAllLines(project.SrcDir + "/other.c" , code)
    let other = File.ReadAllText(project.SrcDir + "/other.c")

    Assert.True(DirectoryContents.hasMainFunc lines)
    Assert.True(DirectoryContents.hasMainFunc other)
    Assert.False(DirectoryContents.hasMainFunc header)
    cleanUpTestProject project

// [<Fact>]
// let ``Given sample project returns 1 rootdir and 1 child dir`` () =
//     let project = getTestProject 2
//     let dirs = getDirs project.RootDir
//     match List.head dirs with
//     | ProjectRoot value -> Assert.Equal(project.RootDir, value)
//     | _ -> Assert.Equal("not the right answer", "not real")

//     // Assert.Equal(project.RootDir, value)
//     Assert.Equal(dirs.Length, 2)
//     Directory.Delete(project.SrcDir, true)
//     Assert.False(Directory.Exists(project.SrcDir))
//     Assert.Equal((getDirs project.RootDir).Length, 1)
//     cleanUpTestProject project


// [<Fact>]
// let ``addLibrary returns proper list with contents`` () =
//     let project = getTestProject 5
//     generateMainAndHeader project.SrcDir
//     let files = getAllSourceCode (project.SrcDir)
//     let compilationdir = List.map compilationDirectory files
//     Assert.Equal(files.Length, 1)
//     Assert.Equal(compilationdir.Length, 1)
    
//     let cmakecommand = addLibrary (List.head compilationdir)
//     let contents =
//         match cmakecommand with
//         | AddLibrary value -> value.Contents
//         | _ -> []
//     equalStringList contents ["add_library(HelloworldLib";"    header.h"; "    header.c)"]
    
//     cleanUpTestProject project


// [<Fact>]
// let ``CMake command valid contents`` () =
//     let command = cmakeCommand "add_executable" "myhelloworld_exe" ["main.cpp"]
//     let contents =
//         match command with
//         | AddExecutable value -> value.Contents
//         | _ -> []
//     equalStringList  ["add_executable(myhelloworld_exe";"main.cpp)"] contents


// [<Fact>]
// let ``addExecutable returns proper list`` () =
//     let source = SourceFile "main.cpp"
//     let executable = {Name=source; DependsOn=[]}
//     let execommand = 
//         match addExecutable executable with
//         | AddExecutable value -> value.Contents
//         | _ -> []
//     equalStringList  ["add_executable(main_exe";"    main.cpp)"] execommand



// [<Fact>]
// let ``generates multiple add_executable from project`` () =
//     let project = getTestProject 9
//     generateMainAndHeader project.SrcDir
//     Assert.True(Directory.Exists project.SrcDir)
//     writeExecutable project.SrcDir "other.cpp"
//     let files = getAllSourceCode (project.SrcDir)
//     let compilationdirs = List.map compilationDirectory files
//     let compilationdir = List.head compilationdirs
//     let lstExecutables = List.collect (fun x -> List.map addExecutable x.Executables ) compilationdirs
//     Assert.Equal(compilationdirs.Length, 1)
//     Assert.Equal(compilationdir.Executables.Length, 2)
//     Assert.Equal(lstExecutables.Length, 2)
//     Assert.Equal<Collections.Generic.IEnumerable<CMakeCommand>>(lstExecutables, listAddExecutables compilationdir)
//     cleanUpTestProject project

// [<Fact>]
// let ``Capitalizes properly`` () =
//     Assert.Equal(upper "hello", "Hello")
//     Assert.Equal(upper "William", "William")
//     Assert.Equal(upper "1234", "1234")

// [<Fact>]
// let ``CMake Min Version required inserts proper version num`` () =
//     Assert.Equal(cmakeMinReq 3.2,  "cmake_minimum_required(VERSION 3.2 FATAL_ERROR)")
//     Assert.Equal(cmakeMinReq 2.8,  "cmake_minimum_required(VERSION 2.8 FATAL_ERROR)")


// [<Fact>]
// let ``Writes lines to file `` () =
//     let path = Path.GetTempPath() + "/testy.txt"
//     let lines = ["hello"; "world!"] |> List.toArray
//     writeFile path lines
//     let contents = File.ReadAllLines(path) |> Array.toList
//     lines |> Array.toList |>  equalStringList contents
    
// [<Fact>]
// let ``Creates a CMakeLists.txt file`` () =
//     let project = getTestProject 2
// //    let outpath = project.RootDir + "/CMakeLists.txt"

//     let d = writeCMake project.SrcDir
//     Assert.Equal(d.Length, 1)
//     cleanUpTestProject project
//    writeCMake project.SrcDir "/tmp/CMakeLists.txt"
//    let lines = File.ReadAllLines ("/tmp/CMakeLists.txt") |> Array.toList
//    let sample = File.ReadAllLines ("./CMakeLists.txt") |> Array.toList
//    equalStringList lines sample