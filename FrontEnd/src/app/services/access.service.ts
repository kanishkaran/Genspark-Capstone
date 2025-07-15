import { Injectable } from "@angular/core";
import { AccessCreateRequest, AccessLevel } from "../models/access.model";
import { BaseApiService } from "./basiApi.service";


@Injectable()
export class AccessLevelService extends BaseApiService<AccessLevel, AccessCreateRequest>{
    protected override endpoint: string = 'AccessLevel';
}