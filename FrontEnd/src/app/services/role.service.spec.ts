import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { RoleService } from './role.service';
import { Role, CreateRoleRequest } from '../models/roleModel';
import { environment } from '../../environment/environment';
import { provideHttpClient } from '@angular/common/http';

describe('RoleService', () => {
  let service: RoleService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/Role`;

  const mockRole: Role = {
    id: '1',
    roleName: 'Manager',
    access: 'Read-Only'
  };

  const createRequest: CreateRoleRequest = {
    role: 'Admin',
    accessLevel: 'Admin'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RoleService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(RoleService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all roles', () => {
    service.getAll().subscribe((res) => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockRole);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockRole], totalCount: 1 });
  });

  it('should get role by ID', () => {
    service.getById('1').subscribe((res) => {
      expect(res).toEqual(mockRole);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockRole);
  });

  it('should create a role', () => {
    const created: Role = {
      id: '2',
      roleName: createRequest.role,
      access: createRequest.accessLevel
    };

    service.create(createRequest).subscribe((res) => {
      expect(res).toEqual(created);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(created);
  });

  it('should update a role', () => {
    const updated: any = { ...mockRole, roleName: 'Supervisor' };

    service.update(updated.id, updated).subscribe((res) => {
      expect(res).toEqual(updated);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updated);
    req.flush(updated);
  });

  it('should delete a role', () => {
    service.delete('1').subscribe((res) => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });
});
