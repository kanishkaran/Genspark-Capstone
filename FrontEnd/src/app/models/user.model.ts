
export interface User{
    username: string
    roleName: string
    isDeleted: boolean
}

export interface UserCreateRequest{
    username: string
    password: string
    roleName: string
    isAdmin?: boolean
    secretKey?: boolean
}

export interface UserRoleUpdateRequest{
    username: string
    role: string
}

export interface UserPasswordChangeRequest{
    oldPassword: string
    newPassword: string
}