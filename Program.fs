open SourceTypes





let printit n =
    match n with
    | Some(x) -> printfn "%A" x
    | _ -> ()

[<EntryPoint>]
let main argv =
    let x = 10
    let src = SourceFile "/tmp/wallet.cpp"
    let lst = getIncludeStatements src
    let xlst = getSourceCode lst
    let zz = 
        getAllSourceCode "."
    let yy = List.collect filterForExecutables zz
    // printfn "The total count is %d" zz
    0