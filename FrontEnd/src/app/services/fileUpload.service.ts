import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { BaseApiService } from './basiApi.service';
import { FileArchive, FileUploadRequest } from '../models/fileArchive.model';

@Injectable()
export class FileArchiveService extends BaseApiService<FileArchive> {
  protected endpoint = 'FileArchive';

  uploadFile(uploadRequest: FileUploadRequest): Observable<any> {
    const formData = new FormData();
    formData.append('file', uploadRequest.file);
    formData.append('category', uploadRequest.category);

    return this.http.post(`${this.apiUrl}/${this.endpoint}/upload`, formData);
  }

  downloadFile(fileName: string, versionNumber?: number): Observable<any> {
    let params = new HttpParams().set('fileName', fileName);
    if (versionNumber) {
      params = params.set('versionNumber', versionNumber.toString());
    }

    console.log(params)
    return this.http.get(`${this.apiUrl}/${this.endpoint}/download`, { 
      params, 
      responseType: 'blob' 
    });
  }
}