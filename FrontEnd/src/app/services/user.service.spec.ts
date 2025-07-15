import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { UserService } from './user.service';
import {
  User,
  UserCreateRequest,
  UserPasswordChangeRequest,
  UserRoleUpdateRequest
} from '../models/user.model';
import { environment } from '../../environment/environment';
import { provideHttpClient } from '@angular/common/http';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/User`;

  const mockUser: User = {
    username: 'john.doe',
    roleName: 'Employee',
    isDeleted: false
  };

  const createRequest: UserCreateRequest = {
    username: 'alice',
    password: 'secure123',
    roleName: 'Admin'
  };

  const roleUpdateRequest: UserRoleUpdateRequest = {
    username: 'john.doe',
    role: 'Manager'
  };

  const passwordChangeRequest: UserPasswordChangeRequest = {
    oldPassword: 'oldPass',
    newPassword: 'newPass123'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UserService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all users', () => {
    service.getAll().subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockUser);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockUser], totalCount: 1 });
  });

  it('should get user by username', () => {
    service.getById('john.doe').subscribe(res => {
      expect(res).toEqual(mockUser);
    });

    const req = httpMock.expectOne(`${apiUrl}/john.doe`);
    expect(req.request.method).toBe('GET');
    req.flush(mockUser);
  });

  it('should create a user', () => {
    const createdUser: User = {
      username: createRequest.username,
      roleName: createRequest.roleName,
      isDeleted: false
    };

    service.create(createRequest).subscribe(res => {
      expect(res).toEqual(createdUser);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(createdUser);
  });

  it('should update a user role', () => {
    service.updateUserRole(roleUpdateRequest).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/role`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(roleUpdateRequest);
    req.flush({ success: true });
  });

  it('should change a user password', () => {
    const username = 'john.doe';

    service.changePassword(passwordChangeRequest, username).subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/${username}/password`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(passwordChangeRequest);
    req.flush({ success: true });
  });

  it('should delete a user', () => {
    service.delete('john.doe').subscribe(res => {
      expect(res).toEqual({ deleted: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/john.doe`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ deleted: true });
  });
});
