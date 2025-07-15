import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environment/environment';
import { PaginatedResponse, QueryParams } from '../models/apiResponse';

@Injectable()
export abstract class BaseApiService<T, TCreate = T, TUpdate = TCreate> {
  protected abstract endpoint: string;
  protected apiUrl = environment.apiUrl;

  constructor(protected http: HttpClient) {}

  getAll(queryParams?: QueryParams): Observable<PaginatedResponse<T>> {
    let params = new HttpParams();
    
    if (queryParams) {
      if (queryParams.page) params = params.set('page', queryParams.page.toString());
      if (queryParams.pageSize) params = params.set('pageSize', queryParams.pageSize.toString());
      if (queryParams.search) params = params.set('search', queryParams.search);
      if (queryParams.sortBy) params = params.set('sortBy', queryParams.sortBy);
      if (queryParams.Desc) params = params.set('Desc', queryParams.Desc);
      if (queryParams.IncludeInactive) params = params.set('IncludeInactive', queryParams.IncludeInactive);
    }

    return this.http.get<PaginatedResponse<T>>(`${this.apiUrl}/${this.endpoint}`, { params });
  }

  getById(id: string): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}/${this.endpoint}/${id}`);
  }

  create(item: TCreate): Observable<T> {
    return this.http.post<T>(`${this.apiUrl}/${this.endpoint}`, item);
  }

  update(id: string, item: TUpdate): Observable<T> {
    return this.http.put<T>(`${this.apiUrl}/${this.endpoint}/${id}`, item);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${this.endpoint}/${id}`);
  }
}

