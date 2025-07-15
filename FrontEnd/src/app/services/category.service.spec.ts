import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { CategoryService } from './category.service';
import { Category, CreateCategoryRequest } from '../models/categoryModel';
import { environment } from '../../environment/environment';
import { provideHttpClient } from '@angular/common/http';

describe('CategoryService', () => {
  let service: CategoryService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/Category`;

  const mockCategory: Category = {
    id: '1',
    categoryName: 'Invoices',
    access: 'Read-Only'
  };

  const createCategoryReq: CreateCategoryRequest = {
    categoryName: 'Invoices',
    accessLevel: 'Read-Only'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CategoryService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(CategoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all categories', () => {
    service.getAll().subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockCategory);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockCategory], totalCount: 1 });
  });

  it('should get category by id', () => {
    service.getById('1').subscribe(res => {
      expect(res).toEqual(mockCategory);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockCategory);
  });

  it('should create category', () => {
    service.create(createCategoryReq).subscribe(res => {
      expect(res).toEqual(mockCategory);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createCategoryReq);
    req.flush(mockCategory);
  });

  it('should update category', () => {
    const updated: any = { ...mockCategory, categoryName: 'Reports' };

    service.update(updated.id, updated).subscribe(res => {
      expect(res).toEqual(updated);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updated);
    req.flush(updated);
  });

  it('should delete category', () => {
    service.delete('1').subscribe(res => {
      expect(res).toBeTruthy();
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });
});
