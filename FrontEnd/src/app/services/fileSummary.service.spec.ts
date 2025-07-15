import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { environment } from '../../environment/environment';
import { FileSummaryService } from './fileSummary.service';
import { provideHttpClient } from '@angular/common/http';

describe('FileSummaryService', () => {
  let service: FileSummaryService;
  let httpMock: HttpTestingController;

  const apiUrl = `${environment.apiUrl}/FileSummary/summarize/`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        FileSummaryService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(FileSummaryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return a summary string', () => {
    const mockSummary = 'This is a summary of the file.';
    const fileName = 'document.pdf';

    service.getFileSummary(fileName).subscribe((res: any) => {
      expect(typeof res).toBe('string');
      expect(res).toBe(mockSummary);
    });

    const req = httpMock.expectOne(
      r => r.url === apiUrl && r.params.get('fileName') === fileName
    );

    expect(req.request.method).toBe('GET');
    req.flush(mockSummary);
  });
});
