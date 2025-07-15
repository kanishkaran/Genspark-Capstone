import { Injectable } from "@angular/core";
import { BaseApiService } from "./basiApi.service";
import { MediaType, MediaTypeCreateRequest } from "../models/mediaType.model";


@Injectable()
export class MediaTypeService extends BaseApiService<MediaType, MediaTypeCreateRequest>{
    protected override endpoint: string = 'MediaType';
    
}