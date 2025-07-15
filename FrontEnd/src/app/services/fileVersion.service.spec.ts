import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { FileVersion } from '../models/fileVersion.model';
import { environment } from '../../environment/environment';
import { FileVersionService } from './fileVersion.service';
import { provideHttpClient } from '@angular/common/http';

describe('FileVersionService', () => {
  let service: FileVersionService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/FileVersion`;

  const mockFileVersions: FileVersion[] = [
    {
      id: 'v1',
      archiveId: 'a1',
      fileName: 'report_v1.pdf',
      versionNumber: 1,
      contentType: 'application/pdf',
      createdAt: new Date(),
      createdBy: 'Alice'
    },
    {
      id: 'v2',
      archiveId: 'a1',
      fileName: 'report_v2.pdf',
      versionNumber: 2,
      contentType: 'application/pdf',
      createdAt: new Date(),
      createdBy: 'Bob'
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [FileVersionService, provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(FileVersionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all file versions', () => {
    service.getAll().subscribe((res) => {
      expect(res.length).toBe(2);
      expect(res).toEqual(mockFileVersions);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockFileVersions);
  });

  it('should get versions by archiveId', () => {
    const archiveId = 'a1';

    service.getByArchiveId(archiveId).subscribe((res) => {
      expect(res.length).toBe(2);
      expect(res.every(v => v.archiveId === archiveId)).toBeTrue();
    });

    const req = httpMock.expectOne(`${apiUrl}/${archiveId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockFileVersions);
  });

});
