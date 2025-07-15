import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';


import { AccessLevel, AccessCreateRequest } from '../models/access.model';
import { environment } from '../../environment/environment';
import { AccessLevelService } from './access.service';
import { provideHttpClient } from '@angular/common/http';

describe('AccessLevelService', () => {
  let service: AccessLevelService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/AccessLevel`;

  const mockAccess: AccessLevel = {
    id: '1',
    access: 'Read',
    isActive: true
  };

  const mockCreate: AccessCreateRequest = {
    access: 'Read-Only'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AccessLevelService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AccessLevelService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); 
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all access levels', () => {
    service.getAll().subscribe((res) => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockAccess);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockAccess], totalCount: 1 });
  });

  it('should fetch access level by ID', () => {
    service.getById('1').subscribe((res) => {
      expect(res).toEqual(mockAccess);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockAccess);
  });

  it('should create a new access level', () => {
    service.create(mockCreate).subscribe((res) => {
      expect(res).toEqual(mockAccess);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockCreate);
    req.flush(mockAccess);
  });

  it('should update an access level', () => {
    const updated: AccessLevel = { id: '1', access: 'Admin', isActive: true };

    service.update('1', updated).subscribe((res) => {
      expect(res).toEqual(updated);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updated);
    req.flush(updated);
  });

  it('should delete an access level', () => {
    service.delete('1').subscribe((res) => {
      expect(res).toBeTruthy();
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });
});
