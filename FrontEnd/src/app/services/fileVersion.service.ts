import { inject, Injectable } from "@angular/core";
import { environment } from "../../environment/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { FileVersion } from "../models/fileVersion.model";

@Injectable()
export class FileVersionService {
    private readonly apiUrl = environment.apiUrl;
    endpoint: string = 'FileVersion'
    private http = inject(HttpClient);

    getAll(){
        return this.http.get<FileVersion[]>(`${this.apiUrl}/${this.endpoint}`);
    }

    getByArchiveId(id: string): Observable<FileVersion[]> {
        return this.http.get<FileVersion[]>(`${this.apiUrl}/${this.endpoint}/${id}`);
    }
}