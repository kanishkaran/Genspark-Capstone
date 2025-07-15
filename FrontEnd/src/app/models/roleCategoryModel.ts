

export interface RoleCategory{
    id: string
    roleName: string
    categoryName: string
    canUpload: boolean
    canDownload: boolean
}

export interface CreateRoleCategoryRequest{
    role: string
    category: string
    canUpload: boolean
    canDownload: boolean
}