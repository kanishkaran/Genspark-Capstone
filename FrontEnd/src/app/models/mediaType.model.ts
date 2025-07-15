

export interface MediaType{
    id: string
    TypeName: string
    Extension: string
}

export interface MediaTypeCreateRequest{
    typeName: string
    extension: string
}