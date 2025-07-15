

export interface Employee {
    id?: string
    firstName?: string;
    lastName?: string;
    email?: string;
    contactNumber?: string;
    isActive?: Boolean;
    fileArchives?: any
}

export interface EmployeeQueryParams {
    firstName?: string;
    lastName?: string;
    email?: string;
    contactNumber?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    Desc?: boolean;
    IncludeInactive?: boolean;
}

export interface EmployeeUpdateRequest {
    firstName: string;
    lastName: string;
    email: string;
    contactNumber: string;
}


