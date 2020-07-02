module SourceTypes

type SourceCode = 
    | HeaderFile of string
    | SourceFile of string

type Library = {
    Name: string;
    Files: List<SourceCode>
}