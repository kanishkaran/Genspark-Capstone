import {  Injectable } from "@angular/core";
import { CreateRoleCategoryRequest, RoleCategory } from "../models/roleCategoryModel";
import { BaseApiService } from "./basiApi.service";

@Injectable()
export class RoleCategoryService extends BaseApiService<RoleCategory, CreateRoleCategoryRequest>{
    protected override endpoint: string = "RoleCategoryAccess";
    
}