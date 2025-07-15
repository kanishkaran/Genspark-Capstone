import { Injectable } from "@angular/core";
import { User, UserCreateRequest, UserPasswordChangeRequest, UserRoleUpdateRequest } from "../models/user.model";
import { BaseApiService } from "./basiApi.service";

@Injectable()
export class UserService extends BaseApiService<User, UserCreateRequest>{
    protected override endpoint: string = 'User';

    updateUserRole(user : UserRoleUpdateRequest){
       return this.http.put(`${this.apiUrl}/${this.endpoint}/role`, user)
    }

    changePassword(request: UserPasswordChangeRequest, username: any){
        return this.http.put(`${this.apiUrl}/${this.endpoint}/${username}/password`, request)
    }

}