import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';


import { MediaType, MediaTypeCreateRequest } from '../models/mediaType.model';
import { environment } from '../../environment/environment';
import { MediaTypeService } from './mediaType.service';
import { provideHttpClient } from '@angular/common/http';

describe('MediaTypeService', () => {
  let service: MediaTypeService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/MediaType`;

  const mockMediaType: MediaType = {
    id: '1',
    TypeName: 'PDF',
    Extension: '.pdf'
  };

  const createRequest: MediaTypeCreateRequest = {
    typeName: 'DOCX',
    extension: '.docx'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MediaTypeService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(MediaTypeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all media types', () => {
    service.getAll().subscribe((res) => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockMediaType);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockMediaType], totalCount: 1 });
  });

  it('should get media type by ID', () => {
    service.getById('1').subscribe((res) => {
      expect(res).toEqual(mockMediaType);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMediaType);
  });

  it('should create a new media type', () => {
    const created: MediaType = {
      id: '2',
      TypeName: createRequest.typeName,
      Extension: createRequest.extension
    };

    service.create(createRequest).subscribe((res) => {
      expect(res).toEqual(created);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(createRequest);
    req.flush(created);
  });

  it('should update a media type', () => {
    const updated: any = { ...mockMediaType, TypeName: 'DOC' };

    service.update(updated.id, updated).subscribe((res) => {
      expect(res).toEqual(updated);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updated);
    req.flush(updated);
  });

  it('should delete a media type', () => {
    service.delete('1').subscribe((res) => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });
});
