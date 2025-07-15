import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';


import { FileArchive, FileUploadRequest } from '../models/fileArchive.model';
import { environment } from '../../environment/environment';
import { FileArchiveService } from './fileUpload.service';
import { provideHttpClient } from '@angular/common/http';

describe('FileArchiveService', () => {
  let service: FileArchiveService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/FileArchive`;

  const mockFileArchive: FileArchive = {
    id: '1',
    fileName: 'report.pdf',
    categoryName: 'Invoices',
    uploadedByName: 'Alice',
    status: true,
    canSummarise: true
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FileArchiveService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(FileArchiveService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all file archives', () => {
    service.getAll().subscribe(res => {
      expect(res.data.length).toBe(1);
      expect(res.data[0]).toEqual(mockFileArchive);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [mockFileArchive], totalCount: 1 });
  });

  it('should get file archive by id', () => {
    service.getById('1').subscribe(res => {
      expect(res).toEqual(mockFileArchive);
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockFileArchive);
  });

  it('should delete a file archive', () => {
    service.delete('1').subscribe(res => {
      expect(res).toEqual({ success: true });
    });

    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush({ success: true });
  });

  it('should upload a file', () => {
    const file = new File(['dummy content'], 'test.txt', { type: 'text/plain' });
    const uploadRequest: FileUploadRequest = {
      file,
      category: 'Reports'
    };

    service.uploadFile(uploadRequest).subscribe(res => {
      expect(res).toEqual({ message: 'Upload successful' });
    });

    const req = httpMock.expectOne(`${apiUrl}/upload`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body instanceof FormData).toBeTrue();
    expect(req.request.body.has('file')).toBeTrue();
    expect(req.request.body.has('category')).toBeTrue();
    expect(req.request.body.get('category')).toBe('Reports');
    req.flush({ message: 'Upload successful' });
  });

  it('should download a file (no version)', () => {
    const fileName = 'report.pdf';
    const mockBlob = new Blob(['File content'], { type: 'application/pdf' });

    service.downloadFile(fileName).subscribe(res => {
      expect(res instanceof Blob).toBeTrue();
      expect(res.type).toBe('application/pdf');
    });

    const req = httpMock.expectOne(r =>
      r.url === `${apiUrl}/download` &&
      r.params.get('fileName') === fileName
    );

    expect(req.request.method).toBe('GET');
    expect(req.request.responseType).toBe('blob');
    req.flush(mockBlob);
  });

  it('should download a specific version of a file', () => {
    const fileName = 'report.pdf';
    const version = 2;
    const mockBlob = new Blob(['v2 content'], { type: 'application/pdf' });

    service.downloadFile(fileName, version).subscribe(res => {
      expect(res instanceof Blob).toBeTrue();
      expect(res.type).toBe('application/pdf');
    });

    const req = httpMock.expectOne(r =>
      r.url === `${apiUrl}/download` &&
      r.params.get('fileName') === fileName &&
      r.params.get('versionNumber') === version.toString()
    );

    expect(req.request.method).toBe('GET');
    req.flush(mockBlob);
  });
});
