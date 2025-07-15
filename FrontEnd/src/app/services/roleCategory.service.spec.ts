import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';


import { RoleCategory, CreateRoleCategoryRequest } from '../models/roleCategoryModel';
import { environment } from '../../environment/environment';
import { RoleCategoryService } from './roleCategory.service';
import { provideHttpClient } from '@angular/common/http';

describe('RoleCategoryService', () => {
  let service: RoleCategoryService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/RoleCategoryAccess`;

  const mockRoleCategory: RoleCategory = {
    id: '1',
    roleName: 'Admin',
    categoryName: 'Invoices',
    canUpload: true,
    canDownload: true
  };

  const createRequest: CreateRoleCategoryRequest = {
    role: 'Employee',
    category: 'Reports',
    canUpload: false,
    canDownload: true
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RoleCategoryService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(RoleCategoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all role-category mappings', () => {
    service.getAll().subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockRoleCategory);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockRoleCategory], totalCount: 1 });
  });

  it('should get a role-category mapping by ID', () => {
    service.getById('1').subscribe(res => {
      expect(res).toEqual(mockRoleCategory);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockRoleCategory);
  });

  it('should create a new role-category mapping', () => {
    const created: RoleCategory = {
      id: '2',
      roleName: createRequest.role,
      categoryName: createRequest.category,
      canUpload: createRequest.canUpload,
      canDownload: createRequest.canDownload
    };

    service.create(createRequest).subscribe(res => {
      expect(res).toEqual(created);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(created);
  });

  it('should update an existing role-category mapping', () => {
    const updated: any = {
      ...mockRoleCategory,
      canUpload: false
    };

    service.update('1', updated).subscribe(res => {
      expect(res).toEqual(updated);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updated);
    req.flush(updated);
  });

  it('should delete a role-category mapping', () => {
    service.delete('1').subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });
});
