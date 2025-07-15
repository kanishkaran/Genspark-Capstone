import { Injectable } from "@angular/core";
import { CreateRoleRequest, Role } from "../models/roleModel";
import { BaseApiService } from "./basiApi.service";

@Injectable()
export class RoleService extends BaseApiService<Role, CreateRoleRequest>{
    protected override endpoint: string = "Role";
}