module SourceTypes

type SourceCode = 
    | HeaderFile of string
    | SourceFile of string

type Library = 
    | External of string
    | Internal of string