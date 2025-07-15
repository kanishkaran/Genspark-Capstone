import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { EmployeeService } from './employee.service';
import { Employee, EmployeeQueryParams, EmployeeUpdateRequest } from '../models/employee.model';
import { environment } from '../../environment/environment';
import { RegisterRequest } from '../models/requestModel';
import { provideHttpClient } from '@angular/common/http';

describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/Employee`;

  const mockEmployee: Employee = {
    id: '1',
    firstName: 'John',
    lastName: 'Doe',
    email: 'john@example.com',
    contactNumber: '1234567890',
    isActive: true,
    fileArchives: []
  };

    const createPayload: RegisterRequest = {
    firstname: 'John',
    lastname: 'Doe',
    email: 'john@example.com',
    contactnumber: '1234567890',
    password: 'secret123'
  };

  const updatePayload: EmployeeUpdateRequest = {
    firstName: 'Johnny',
    lastName: 'Doe',
    email: 'john@example.com',
    contactNumber: '9876543210'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        EmployeeService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(EmployeeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all employees with query params', () => {
    const queryParams: EmployeeQueryParams = {
      firstName: 'John',
      page: 1,
      pageSize: 10,
      Desc: true
    };

    service.getAllEmployee(queryParams).subscribe((res) => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockEmployee);
    });

    const req = httpMock.expectOne((r) =>
      r.url === apiUrl &&
      r.params.get('firstName') === 'John' &&
      r.params.get('page') === '1' &&
      r.params.get('pageSize') === '10' &&
      r.params.get('Desc') === 'true'
    );

    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockEmployee], totalCount: 1 });
  });

  it('should fetch employee by email', () => {
    const email = 'john@example.com';

    service.getEmployeeByEmail(email).subscribe((res) => {
      expect(res).toEqual(mockEmployee);
    });

    const req = httpMock.expectOne((r) =>
      r.url === `${apiUrl}/email` && r.params.get('email') === email
    );

    expect(req.request.method).toBe('GET');
    req.flush(mockEmployee);
  });

  it('should create a new employee', () => {
    const created: Employee = {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      email: 'john@example.com',
      contactNumber: '1234567890',
      isActive: true
    };

    service.create(createPayload).subscribe((res) => {
      expect(res).toEqual(created);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createPayload);
    req.flush(created);
  });

  it('should update an employee', () => {
    const updated: Employee = {
      id: '1',
      ...updatePayload,
      isActive: true
    };

    service.update('1', updatePayload).subscribe((res) => {
      expect(res).toEqual(updated);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatePayload);
    req.flush(updated);
  });
});
