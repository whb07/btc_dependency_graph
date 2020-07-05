module Tests

open Xunit
open System.IO
open System
open SourceTypes
open CMake

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
let ``Test 1 + 1 = 2`` () =
    let project = getTestProject 5
    generateMainAndHeader project.SrcDir
    let files = getAllSourceCode (project.SrcDir)
    let compilationdir = List.map compilationDirectory files
    Assert.Equal(files.Length, 1)
    Assert.Equal(compilationdir.Length, 1)
    
    let cmakecommand = addLibrary (List.head compilationdir)
    let contents =
        match cmakecommand with
        | AddLibrary value -> value.Contents
        | _ -> []
    equalStringList contents ["add_library(Helloworld";"    header.h"; "    header.c)"]
    
    cleanUpTestProject project


[<Fact>]
let ``CMake command valid contents`` () =
    let command = cmakeCommand "add_executable" "myhelloworld" ["main.cpp"]
    let contents =
        match command with
        | AddExecutable value -> value.Contents
        | _ -> []
    equalStringList  ["add_executable(Myhelloworld";"main.cpp)"] contents


[<Fact>]
let ``addExecutable returns proper list`` () =
    let source = SourceFile "main.cpp"
    let executable = {Name=source; DependsOn=[]}
    let execommand = 
        match addExecutable executable with
        | AddExecutable value -> value.Contents
        | _ -> []
    equalStringList  ["add_executable(Main";"    main.cpp)"] execommand



[<Fact>]
let ``generates multiple add_executable from project`` () =
    let project = getTestProject 9
    generateMainAndHeader project.SrcDir
    Assert.True(Directory.Exists project.SrcDir)
    writeExecutable project.SrcDir "other.cpp"
    let files = getAllSourceCode (project.SrcDir)
    let compilationdirs = List.map compilationDirectory files
    let compilationdir = List.head compilationdirs
    let lstExecutables = List.collect (fun x -> List.map addExecutable x.Executables ) compilationdirs
    Assert.Equal(compilationdirs.Length, 1)
    Assert.Equal(compilationdir.Executables.Length, 2)
    Assert.Equal(lstExecutables.Length, 2)
    Assert.Equal<Collections.Generic.IEnumerable<CMakeCommand>>(lstExecutables, listAddExecutables compilationdir)
    cleanUpTestProject project

[<Fact>]
let ``Capitalizes properly`` () =
    // let ans = 1 + 1
    Assert.Equal(upper "hello", "Hello")
    Assert.Equal(upper "William", "William")
    Assert.Equal(upper "1234", "1234")
