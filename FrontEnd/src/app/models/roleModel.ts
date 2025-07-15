

export interface Role{
    id: string
    roleName: string
    access: string
}

export interface CreateRoleRequest{
    role: string
    accessLevel: string
}