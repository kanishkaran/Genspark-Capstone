import { Injectable } from "@angular/core";

import { HttpParams } from "@angular/common/http";
import { Employee, EmployeeQueryParams, EmployeeUpdateRequest } from "../models/employee.model";
import { PaginatedResponse } from "../models/apiResponse";
import { catchError, Observable, of, throwError } from "rxjs";
import { BaseApiService } from "./basiApi.service";
import { RegisterRequest } from "../models/requestModel";

@Injectable()
export class EmployeeService extends BaseApiService<Employee, RegisterRequest, EmployeeUpdateRequest> {
    protected override endpoint: string = "Employee";


    getAllEmployee(queryParams?: EmployeeQueryParams): Observable<PaginatedResponse<Employee>> {
        let params = new HttpParams();

        if (queryParams) {
            if (queryParams.firstName) params = params.set('firstName', queryParams.firstName);
            if (queryParams.lastName) params = params.set('lastName', queryParams.lastName);
            if (queryParams.email) params = params.set('email', queryParams.email);
            if (queryParams.contactNumber) params = params.set('contactNumber', queryParams.contactNumber);
            if (queryParams.page) params = params.set('page', queryParams.page.toString());
            if (queryParams.pageSize) params = params.set('pageSize', queryParams.pageSize.toString());
            if (queryParams.sortBy) params = params.set('sortBy', queryParams.sortBy);
            if (queryParams.Desc) params = params.set('Desc', queryParams.Desc);
            if (queryParams.IncludeInactive) params = params.set('IncludeInactive', queryParams.IncludeInactive);
        }

        return this.http.get<PaginatedResponse<Employee>>(`${this.apiUrl}/${this.endpoint}`, { params });
    }

    getEmployeeByEmail(email: string) {
        let params = new HttpParams();

        if (email) {
            params = params.set('email', email);
        }

        return this.http.get(`${this.apiUrl}/${this.endpoint}/email`, { params });
    }


}